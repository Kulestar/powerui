//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Blaze;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// A mesh made up of a dynamic number of "blocks".
	/// Each block always consists of two triangles and 4 vertices.
	/// This is used for displaying the content in 3D with a single mesh.
	/// </summary>
	
	public class DynamicMesh{
		
		/// <summary>The batch this mesh belongs to.</summary>
		public UIBatch Batch;
		/// <summary>The number of blocks that have been allocated.</summary>
		public int BlockCount;
		/// <summary>The unity mesh that this will be flushed into.</summary>
		public Mesh OutputMesh;
		/// <summary>The material to use when rendering.</summary>
		public Material Material;
		/// <summary>The number of blocks that were allocated last UI update.</summary>
		private int LastBlockCount;
		/// <summary>The renderer being used to layout the blocks.</summary>
		private MeshRenderer Renderer;
		/// <summary>The material used by this mesh when it is using an atlas.</summary>
		private Material GlobalMaterial;
		/// <summary>The transform of this mesh.</summary>
		public Transform OutputTransform;
		/// <summary>The gameobjec that holds this mesh.</summary>
		public GameObject OutputGameObject;
		/// <summary>An array of uv coordinates.</summary>
		public FixedSizeBuffer<Vector2> UV;
		/// <summary>An array of uv coordinates.</summary>
		public FixedSizeBuffer<Vector2> UV2;
		/// <summary>An array of uv coordinates.</summary>
		public FixedSizeBuffer<Vector2> UV3;
		/// <summary>An array of triangles.</summary>
		public FixedSizeBuffer<int> Triangles;
		/// <summary>An array of vertex colours.</summary>
		public FixedSizeBuffer<Color> Colours;
		/// <summary>An array of normals.</summary>
		public FixedSizeBuffer<Vector3> Normals;
		/// <summary>An array of vertex coordinates.</summary>
		public FixedSizeBuffer<Vector3> Vertices;
		
		/// <summary>The raw vertex buffer linked list. Data is written into these.
		/// If we create too many verts, it simply creates another buffer.
		/// Flatten is used to convert this flexible structure into the actual vector3[] etc.</summary>
		public BlockBuffer LastBuffer;
		/// <summary>The raw vertex buffer linked list. Data is written into these.
		/// If we create too many verts, it simply creates another buffer.
		/// Flatten is used to convert this flexible structure into the actual vector3[] etc.</summary>
		public BlockBuffer FirstBuffer;
		/// <summary># of blocks in the current buffer.</summary>
		public int CurrentBufferBlocks;
		/// <summary>Total number of full buffers.</summary>
		public int FullBufferCount;
		
		
		/// <summary>Creates a new dynamic mesh which uses the hybrid shader and a texture atlas.</summary>
		public DynamicMesh(UIBatch batch){
			Batch=batch;
			Setup();
		}
		
		/// <summary>Gets the next vertex buffer.</summary>
		public void NextBuffer(){
			
			// Get it:
			BlockBuffer buffer=MeshDataBufferPool.GetBuffer();
			buffer.Mesh=this;
			
			if(Normals!=null){
				buffer.RequireNormals();
			}
			
			if(UV3!=null){
				buffer.RequireUV3();
			}
			
			// Clear count:
			CurrentBufferBlocks=0;
			
			if(FirstBuffer==null){
				// Only one:
				buffer.Previous=null;
				buffer.Offset=0;
				buffer.BlocksBefore=0;
				FirstBuffer=LastBuffer=buffer;
			}else{
				
				// Filled one:
				FullBufferCount++;
				buffer.Offset=FullBufferCount * MeshDataBufferPool.VertexBufferSize;
				buffer.BlocksBefore=FullBufferCount * MeshDataBufferPool.BlockCount;
				
				// Add to end:
				buffer.Previous=LastBuffer;
				LastBuffer.Next=buffer;
				LastBuffer=buffer;
			}
			
		}
		
		/// <summary>Total number of allocated blocks.</summary>
		public int TotalBlockCount{
			get{
				return FullBufferCount*MeshDataBufferPool.BlockCount + CurrentBufferBlocks;
			}
		}
		
		/// <summary>Changes the font atlas used by the default material.</summary>
		public void SetFontAtlas(TextureAtlas atlas){
			Texture2D texture;
			
			if(atlas==null){
				texture=null;
			}else{
				texture=atlas.Texture;
			}
			
			Material.SetTexture("_Font",texture);
		}
		
		/// <summary>Sets a default material to this mesh.</summary>
		public void SetGraphicsAtlas(TextureAtlas atlas){
			Texture2D texture;
			
			if(atlas==null){
				texture=null;
			}else{
				texture=atlas.Texture;
			}
			
			Material.SetTexture("_MainTex",texture);
		}
		
		public void SetGlobalMaterial(Shader shader){
			
			if(GlobalMaterial==null){
				// Create it now:
				GlobalMaterial=new Material(shader);
			}else{
				// Update the shader:
				GlobalMaterial.shader=shader;
			}
			
			SetMaterial(GlobalMaterial);
		}
		
		/// <summary>Applies the given material to this mesh.</summary>
		/// <param name="material">The material to apply.</param>
		public void SetMaterial(Material material){
			if(Material==material){
				return;
			}
			
			Material=material;
			
			if(Renderer==null){
				return;
			}
			
			Renderer.sharedMaterial=Material;
		}
		
		/// <summary>Called only by constructors. This creates the actual mesh and the buffers for verts/tris etc.</summary>
		private void Setup(){
			UV=new FixedSizeBuffer<Vector2>(4,false);
			Triangles=new FixedSizeBuffer<int>(6,true);
			Colours=new FixedSizeBuffer<Color>(4,false);
			Vertices=new FixedSizeBuffer<Vector3>(4,false);
			UV2=new FixedSizeBuffer<Vector2>(4,false);
			
			OutputMesh=new Mesh();
			OutputGameObject=new GameObject();
			OutputTransform=OutputGameObject.GetComponent<Transform>();	
			
			Renderer=OutputGameObject.AddComponent<MeshRenderer>();
			Renderer.sharedMaterial=Material;
			
			MeshFilter filter=OutputGameObject.AddComponent<MeshFilter>();
			filter.mesh=OutputMesh;
		}
		
		/// <summary>Let the mesh know that normals are required.</summary>
		public void RequireNormals(){
			
			if(Normals!=null){
				return;
			}
			
			Normals=new FixedSizeBuffer<Vector3>(4,false);
			
			BlockBuffer buff=FirstBuffer;
			
			while(buff!=null){
				buff.RequireNormals();
				buff=buff.Next;
			}
			
		}
		
		/// <summary>Let the mesh know that UV3 is required.</summary>
		public void RequireUV3(){
			
			if(UV3!=null){
				return;
			}
			
			UV3=new FixedSizeBuffer<Vector2>(4,false);
			
			BlockBuffer buff=FirstBuffer;
			
			while(buff!=null){
				buff.RequireUV3();
				buff=buff.Next;
			}
			
		}
		
		/// <summary>Called to update the parenting of this mesh.</summary>
		public void ChangeParent(){
		
			if(Batch!=null && Batch.Renderer!=null && Batch.Renderer.Node!=null){
				OutputTransform.parent=Batch.Renderer.Node.transform;
			}else if(UI.GUINode!=null){
				OutputTransform.parent=UI.GUINode.transform;
			}
			
			// Make sure the object is correctly transformed:
			OutputTransform.localScale=Vector3.one;
			OutputTransform.localPosition=Vector3.zero;
			OutputTransform.localRotation=Quaternion.identity;
			
		}
		
		/// <summary>Let the mesh know it's about to undergo a layout routine.
		/// <see cref="PowerUI.Renderman.Layout"/>.</summary>
		public void PrepareForLayout(){
			
			// Release the buffers back to the pool:
			MeshDataBufferPool.Return(FirstBuffer,LastBuffer);
			
			// Clear values:
			FirstBuffer=null;
			LastBuffer=null;
			FullBufferCount=0;
			CurrentBufferBlocks=0;
			BlockCount=0;
			
		}
		
		/// <summary>After the draw pass, builds flat buffers.</summary>
		public void CompletedLayout(){
			int total=TotalBlockCount;
			BlockCount=total;
			
			if(total!=LastBlockCount){
				// We gained or lost some blocks - resize our buffers:
				UV.Resize(total);
				UV2.Resize(total);
				Colours.Resize(total);
				Vertices.Resize(total);
				Triangles.Resize(total);
			}
			
			// Always make sure UV3 and Normals are valid:
			if(Normals!=null){
				Normals.ResizeMatch(total,Vertices.Buffer.Length);
			}
			
			if(UV3!=null){
				UV3.ResizeMatch(total,Vertices.Buffer.Length);
			}
			
			LastBlockCount=total;
			
			// Flatten now:
			Flatten(FirstBuffer,total);
			
			// Output the new mesh data:
			Flush();
			
		}
		
		/// <summary>Called with the first buffer in a set. 
		/// Flattens the set of verts into the provided mesh.</summary>
		public void Flatten(BlockBuffer current,int blockCount){
			
			// Current progress:
			int vertIndex=0;
			int triIndex=0;
			
			// For each requested buffer..
			while(blockCount>0){
				
				// How many blocks can be provided by the current buffer?
				int bufferCount;
				
				if(blockCount>MeshDataBufferPool.BlockCount){
					
					// Full array:
					bufferCount=MeshDataBufferPool.BlockCount;
					blockCount-=MeshDataBufferPool.BlockCount;
					
				}else{
					
					// Last one - count to go:
					bufferCount=blockCount;
					blockCount=0;
					
				}
				
				int vertCount=bufferCount * 4;
				int triCount=bufferCount * 6;
				
				// Copy from the current page:
				Array.Copy(current.Triangles,0,Triangles.Buffer,triIndex,triCount);
				
				// Copy from the current page:
				Array.Copy(current.Vertices,0,Vertices.Buffer,vertIndex,vertCount);
				
				// Copy from the current page:
				Array.Copy(current.Colours,0,Colours.Buffer,vertIndex,vertCount);
				
				// Copy from the current page:
				Array.Copy(current.UV1,0,UV.Buffer,vertIndex,vertCount);
				
				// Copy from the current page:
				Array.Copy(current.UV2,0,UV2.Buffer,vertIndex,vertCount);
				
				if(Normals!=null){
					// Copy from the current page:
					Array.Copy(current.Normals,0,Normals.Buffer,vertIndex,vertCount);
				}
				
				if(UV3!=null){
					// Copy from the current page:
					Array.Copy(current.UV3,0,UV3.Buffer,vertIndex,vertCount);
				}
				
				// Move indices:
				vertIndex+=vertCount;
				triIndex+=triCount;
				
				// Seek to next buffer:
				current=current.Next;
				
			}
			
		}
		
		/// <summary>Allocates a block from this mesh. Note that the block object is actually shared. The block can then have
		/// its vertices/triangles edited. Changes will be outputted visually when MeshBlock.Done is called.</summary>
		public MeshBlock Allocate(Renderman renderer){
			MeshBlock block=renderer.Block;
			block.Colour=Color.white;
			block.TextUV=null;
			block.ImageUV=null;
			
			if(FirstBuffer==null || CurrentBufferBlocks==MeshDataBufferPool.BlockCount){
				NextBuffer();
			}
			
			// Apply buffer and block index:
			block.Buffer=LastBuffer;
			block.BlockIndex=CurrentBufferBlocks;
			
			// Bump up the index:
			CurrentBufferBlocks++;
			
			return block;
		}
		
		/// <summary>Outputs all the verts/triangles etc to the underlying unity mesh.</summary>
		public void Flush(){
			// Strip old triangles:
			OutputMesh.triangles=null;
			
			// Apply the vertices:
			OutputMesh.vertices=Vertices.Buffer;
			OutputMesh.colors=Colours.Buffer;
			
			if(UV3==null){
				OutputMesh.uv3=null;
			}else{
				OutputMesh.uv3=UV3.Buffer;
			}
			
			OutputMesh.uv2=UV2.Buffer;
			OutputMesh.uv=UV.Buffer;
			
			if(Normals!=null){
				OutputMesh.normals=Normals.Buffer;
			}
			
			//And apply the triangles:
			OutputMesh.triangles=Triangles.Buffer;
			OutputMesh.RecalculateBounds();
		}
		
		/// <summary>Permanently destroys this mesh.</summary>
		public void Destroy(){
			OutputMesh=null;
			OutputTransform=null;
			
			if(OutputGameObject!=null){
				GameObject.Destroy(OutputGameObject);
				OutputGameObject=null;
			}
			
			UV=null;
			UV2=null;
			UV3=null;
			Colours=null;
			Normals=null;
			Vertices=null;
			Triangles=null;
			
		}
		
	}
	
}