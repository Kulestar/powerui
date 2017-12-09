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


namespace PowerUI{
	
	/// <summary>
	/// Flat WorldUI's render to textures. Normal worldUI's are better for performance but flat worldUI's are more flexible. 
	/// Free/Indie users should use normal WorldUI's instead for performance purposes, however this can still be used.
	/// The difference here is the texture can be e.g. applied to a curved surface.
	/// </summary>
	
	public partial class FlatWorldUI:WorldUI{
		
		/// <summary>Global position offset on z.</summary>
		private static float GlobalOffset=-1000f;
		
		/// <summary>The default layer to use for rendering FlatWorldUI's. If not set the PowerUI layer is used, Change by using yourWorldUI.Layer.</summary>
		private int DefaultLayer=-1;
		
		/// <summary>A delegate called when the Update event is fired, right before the WorldUI redraws.</summary>
		public UpdateMethod OnUpdate;
		/// <summary>The raw texture. If this changes because you resized your FlatWorldUI, 
		/// an imagechange event will be fired on the window (type of Dom.Event).</summary>
		public RenderTexture Texture;
		/// <summary>The internal camera being used to render it flat.</summary>
		public Camera SourceCamera;
		/// <summary>The internal camera's gameobject being used.</summary>
		public GameObject CameraObject;
		/// <summary>A filter to apply.</summary>
		private Loonim.SurfaceTexture Filter_;
		/// <summary>The draw info which draws the filter.</summary>
		private Loonim.DrawInfo FilterDrawInfo;
		
		
		/// <summary>Creates a new Flat World UI with 100x100 pixels of space and a name of "new World UI".
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		public FlatWorldUI():this("new World UI",0,0){}
		
		/// <summary>Creates a new Flat World UI with 100x100 pixels of space and the given name.
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		/// <param name="name">The name for the UI's gameobject.</param>
		public FlatWorldUI(string name):this(name,0,0){}
		
		/// <summary>Creates a new Flat World UI with the given pixels of space and a name of "new World UI".
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		/// <param name="widthPX">The width in pixels of this UI.</param>
		/// <param name="heightPX">The height in pixels of this UI.</param>
		public FlatWorldUI(int widthPX,int heightPX):this("new World UI",widthPX,heightPX){}
		
		/// <summary>Creates a new Flat World UI with the given pixels of space and a given name.
		/// The gameobjects origin sits at the middle of the UI by default. See <see cref="PowerUI.WorldUI.SetOrigin"/>. 
		/// By default, 100 pixels are 1 world unit. See <see cref="PowerUI.WorldUI.SetResolution"/>.</summary>
		/// <param name="name">The name for the UI's gameobject.</param>
		/// <param name="widthPX">The width in pixels of this UI.</param>
		/// <param name="heightPX">The height in pixels of this UI.</param>
		public FlatWorldUI(string name,int widthPX,int heightPX):base(name,widthPX,heightPX){
			
			// It's a flat UI:
			Flat=true;
			
			// Create camera gameobject:
			CameraObject=new GameObject();
			
			CameraObject.name=name+"-#camera";
			
			// Parent the camera to the root:
			CameraObject.transform.parent=gameObject.transform;
			CameraObject.transform.localPosition=Vector3.zero;
			
			// Add a camera:
			SourceCamera=CameraObject.AddComponent<Camera>();
			
			// Put it right at the back:
			SourceCamera.depth=-9999;
			
			// Set the clear flags:
			SourceCamera.clearFlags=CameraClearFlags.Color;
			SourceCamera.backgroundColor=new Color(1f,1f,1f,0f);
			
			// Make it forward rendered (it deals with transparency):
			SourceCamera.renderingPath=RenderingPath.Forward;
			
			// Disable the camera object so we 
			// can manually redraw at the UI rate:
			CameraObject.SetActive(false);
			
			float zSpace=UI.GetCameraDistance();
			
			// Setup the cameras distance:
			SetCameraDistance(zSpace);
			
			// Call the camera creation method:
			UI.CameraGotCreated(SourceCamera);
			
			// Make it orthographic:
			SourceCamera.orthographic=true;
			
			// Set the orthographic size:
			SetOrthographicSize();
			
			// Next it's time for the texture itself!
			// We now always have a RenderTexture.
			
			// Update the render texture:
			ChangeImage(widthPX,heightPX);
			
			// Change the layer of the gameobject and also the camera.
			
			// Set the culling mask:
			if(DefaultLayer==-1){
			
				Layer=UI.Layer;
				
			}else{
				
				Layer=DefaultLayer;
				
			}
			
			gameObject.transform.position=new Vector3(0f,-150f,GlobalOffset);
			GlobalOffset+=zSpace+1f;
			
		}
		
		/// <summary>Updates the render texture.</summary>
		private void ChangeImage(int width,int height){
			
			if(Texture!=null){
				
				// Destroy existing one:
				GameObject.Destroy(Texture);
			
			}
			
			// Create tex:
			Texture=new RenderTexture(width,height,16,RenderTextureFormat.ARGB32);
			
			// Hook it up:
			SourceCamera.targetTexture=Texture;
			
			// If we have a filter, update its input:
			if(Filter_==null){
				
				// Fire an imagechange event into the window:
				Dom.Event e=new Dom.Event("imagechange");
				e.SetTrusted(false);
				document.window.dispatchEvent(e);
				
			}else{
				// Connect up the filter
				// (which will fire an imagechange for us):
				ConnectFilter();
			}
			
		}
		
		/// <summary>Connects a filter to the camera's output.
		/// Fires an imagechange event too as this updates Texture to the filtered version.</summary>
		private void ConnectFilter(){
			
			if(Filter_!=null){
				
				if(FilterDrawInfo==null){
					FilterDrawInfo=new Loonim.DrawInfo();
				}
				
				// Update draw info:
				FilterDrawInfo.SetSize(pixelWidth,pixelHeight);
				
				// Update source:
				Filter_.Set("source0",SourceCamera.targetTexture);
				
				// Reallocate the filter:
				Filter_.PreAllocate(FilterDrawInfo);
				
				// Grab the main output (always a RT):
				Texture=Filter_.Texture as RenderTexture;
				
				if(Texture==null){
					
					// This isn't a valid filter!
					// It either had no nodes or e.g. e.g. a solid colour.
					Debug.Log("Invalid filter was set to a FlatWorldUI - removed it.");
					Filter_=null;
					
				}
				
			}
			
			if(Filter_==null){
				
				// Clear draw info:
				FilterDrawInfo=null;
				
				// Revert to the camera's output:
				Texture=SourceCamera.targetTexture;
				
			}
			
			// Fire an imagechange event into the window:
			Dom.Event e=new Dom.Event("imagechange");
			e.SetTrusted(false);
			document.window.dispatchEvent(e);
			
		}
		
		/// <summary>Alias for Texture.</summary>
		public RenderTexture texture{
			get{
				return Texture;
			}
		}
		
		/// <summary>This FlatWorldUI will accept input from the given collider. Note that it will use the textureCoord's of the hit point
		/// (this will work with mesh colliders too).</summary>
		public void AcceptInputFrom(Collider collider){
			AcceptInputFrom(collider.transform);
		}
		
		/// <summary>This FlatWorldUI will accept input from a gameobject with one or more colliders.
		/// Note that it will use the textureCoord's of the hit point
		/// (this will work with mesh colliders too).</summary>
		public void AcceptInputFrom(GameObject colliderObject){
			AcceptInputFrom(colliderObject.transform);
		}
		
		/// <summary>This FlatWorldUI will accept input from a gameobject with one or more colliders.
		/// Note that it will use the textureCoord's of the hit point
		/// (this will work with mesh colliders too).</summary>
		public void AcceptInputFrom(Transform colliderTransform){
			
			if(transform!=null){
				// Remove from the physics lookup if already in there:
				if(PhysicsLookup!=null){
					PhysicsLookup.Remove(transform);
				}
			}
			
			// Set transform:
			transform=colliderTransform;
			
			// Accept input (which will add it to the physics lookup):
			AcceptInput=true;
			
		}
		
		/// <summary>Applies Texture to the sharedMaterial on the gameobject.</summary>
		public void ApplyTo(GameObject gameObject){
			
			MeshRenderer renderer=gameObject.GetComponent<MeshRenderer>();
			Material material;
			
			if(renderer==null){
				
				// Try skinned:
				SkinnedMeshRenderer sme=gameObject.GetComponent<SkinnedMeshRenderer>();
				
				if(sme==null){
					throw new Exception("That gameObject doesn't have a mesh renderer or a skinned mesh renderer.");
				}
				
				// Get the material:
				material=sme.sharedMaterial;
				
			}else{
				
				// Get the material:
				material=renderer.sharedMaterial;
				
			}
			
			// Apply it:
			material.mainTexture=Texture;
			
		}
		
		/// <summary>Applies Texture to material.mainTexture.</summary>
		public void ApplyTo(Material material){
			
			// Apply it:
			material.mainTexture=Texture;
			
		}
		
		/// <summary>Creates a material with the given shader and applies texture to its mainTexture property.</summary>
		public Material CreateMaterial(Shader shader){
			
			Material material=new Material(shader);
			
			material.mainTexture=Texture;
			
			return material;
			
		}
		
		/// <summary>Creates a Standard Shader material and applies texture to its mainTexture property.</summary>
		public Material CreateMaterial(){
			
			Material material=new Material(Shader.Find("Standard"));
			
			material.mainTexture=Texture;
			
			return material;
			
		}
		
		public override void RenderWithCamera(int layer){
			base.RenderWithCamera(layer);
			SourceCamera.cullingMask=(1<<layer);
		}
		
		/// <summary>Sets the distance of the camera, essentially defining the amount of z-index available.</summary>
		/// <param name="distance">The distance of the camera from the origin along the z axis in world units.</param>
		public void SetCameraDistance(float distance){
			SourceCamera.farClipPlane=distance*2f;
			CameraObject.transform.localPosition=new Vector3(0f,0f,-distance);
		}
		
		public override void SetOrigin(float x,float y){
			x=0.5f;
			y=0.5f;
			base.SetOrigin(x,y);
		}
		
		public override void SetResolution(float pp){
		}
		
		public override void SetResolution(int x,int y){
		}
		
		public override bool SetDimensions(int x,int y){
			
			// Check for changes:
			if(pixelWidth==x && pixelHeight==y){
				return false;
			}
			
			// Set the base dimensions:
			base.SetDimensions(x,y);
			
			if(Texture!=null){
				
				// Recreate:
				ChangeImage(x,y);
				
			}
			
			// Set the orthographic size:
			SetOrthographicSize();
			
			return true;
		}
		
		private void SetOrthographicSize(){
			
			if(SourceCamera==null){
				return;
			}
			
			// Grab the world per pixel size:
			SourceCamera.orthographicSize=pixelHeight / 2f * WorldPerPixel.y;
			
		}
		
		/// <summary>A filter to apply to this FWUI's output.</summary>
		public Loonim.SurfaceTexture Filter{
			get{
				return Filter_;
			}
			set{
				Filter_=value;
				
				if(SourceCamera.targetTexture!=null){
					
					// Already got a texture - hook up the filter now:
					ConnectFilter();
					
				}
			}
		}
		
		public override void Update(){
			
			if(OnUpdate!=null){
				OnUpdate();
			}
			
			base.Update();
			
			if(Renderer==null){
				return;
			}
			
			// Apply aspect ratio - we do this in update as 
			// changing the screen size affects our FWUI cams too:
			SourceCamera.aspect=Ratio;
			
			// Render now!
			SourceCamera.Render();
			
			// If we have Loonim filters, redraw those too.
			if(Filter_!=null){
				Filter_.ForceDraw(FilterDrawInfo);
			}
			
		}
		
		public override void Destroy(){
			
			base.Destroy();
			
			if(Texture!=null){
				
				// Destroy it:
				GameObject.Destroy(Texture);
				Texture=null;
				
			}
			
		}
		
	}
	
}