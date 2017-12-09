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
using PowerUI;
using Blaze;


namespace Css{
	
	/// <summary>
	/// Helps with the process of rastering elements. Used by the CSS filter property.
	/// </summary>
	
	public partial class RasterDisplayableProperty:DisplayableProperty{
		
		/// <summary>The raster output.</summary>
		public RenderTexture Output{
			get{
				return Renderer.Texture;
			}
		}
		
		/// <summary>A material displaying the output.</summary>
		private Material Material;
		/// <summary>A flatWorldUI which helps with the process of rendering the element.</summary>
		public FlatWorldUI Renderer;
		/// <summary>An AtlasLocation object used to describe where the image is.</summary>
		private AtlasLocation LocatedAt;
		/// <summary>A filter to apply when the renderer is available.</summary>
		private Loonim.SurfaceTexture PendingFilter;
		
		
		/// <summary>Creates a new solid background colour property for the given element.</summary>
		/// <param name="data">The renderable object to give a bg colour to.</param>
		public RasterDisplayableProperty(RenderableData data):base(data){
		}
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				// Must be the very first thing.
				return 1;
			}
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			// Does the given renderer belong to the worldUI?
			if(Renderer!=null && renderer==Renderer.Renderer){
				
				// Yes! We're actually drawing the element. Do nothing.
				return;
				
			}
			
			MeshBlock block=GetFirstBlock(renderer);
			
			if(block==null){
				// This can happen if an animation is requesting that a now offscreen element gets painted only.
				return;
			}
			
			block.PaintColour(renderer.ColorOverlay);
			
		}
		
		/// <summary>Sets a filter to apply. This is what rasterising elements is all for!</summary>
		public void SetFilter(Loonim.SurfaceTexture tex){
			
			if(Renderer==null){
				// We've now got a filter waiting for the renderer:
				PendingFilter=tex;
			}else{
				// Setting the filter will typically trigger an imagechange event.
				// That then updates the material image.
				Renderer.Filter=tex;
			}
			
		}
		
		internal override void NowOffScreen(){
			
			if(Renderer!=null){
				
				FlatWorldUI fwui=Renderer;
				Renderer=null;
				
				// Make sure we're on the main thread:
				Callback.MainThread(delegate(){
					
					// Destroy it:
					fwui.Destroy();
					
				});
				
			}
			
		}
		
		/// <summary>Destroys this RDP.</summary>
		public void Destroy(){
			
			// Destroy it:
			if(Renderer!=null){
				Renderer.Destroy();
				Renderer=null;
			}
			
		}
		
		/// <summary>Updates the FlatWorldUI so it builds the mesh for this element.</summary>
		private void UpdateRenderer(LayoutBox box,float width,float height){
			
			// - Set w/h to width and height:
			int w=(int)width;
			int h=(int)height;
			
			// Resize the renderer (which will emit a changed image event):
			Renderer.SetDimensions(w,h);
			
			// Temporarily set the positioning of 'box' such that it's at the origin:
			int _pos=box.PositionMode;
			BoxStyle _position=box.Position;
			
			// Clear:
			box.Position=new BoxStyle(0f,float.MaxValue,float.MaxValue,0f);
			box.PositionMode=PositionMode.Fixed;
			
			// Put the RenderData in the render only queue of *Renderer* and ask it to layout now:
			RenderableData _next=RenderData.Next;
			UpdateMode _mode=RenderData.NextUpdateMode;
			RenderableData _ancestor=RenderData.Ancestor;
			
			// Clear:
			RenderData.Next=null;
			RenderData.NextUpdateMode=UpdateMode.Render;
			RenderData.Ancestor=null;
			
			// Queue:
			Renderer.Renderer.StylesToUpdate=RenderData;
			Renderer.Renderer.HighestUpdateMode=UpdateMode.Render;
			
			// Draw now!
			Renderer.Renderer.Update();
			
			// Restore (box):
			box.Position=_position;
			box.PositionMode=_pos;
			
			// Restore (queue):
			RenderData.Next=_next;
			RenderData.NextUpdateMode=_mode;
			RenderData.Ancestor=_ancestor;
			
		}
		
		/// <summary>Called when the output changes.
		/// Essentially forwards an imagechange event to this node.</summary>
		private void ChangedOutputEvent(Dom.Event e){
			
			// Dispatch the event here too:
			RenderData.Node.dispatchEvent(e);
			
			if(Material!=null){
				
				// Hook up the output:
				Material.SetTexture("_MainTex",Output);
				
			}
			
		}
		
		/// <summary>A unique identifier.</summary>
		private static int RasterID=1;
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			// Dimensions:
			float width=box.Width;
			float height=box.Height;
			
			if(Renderer==null){
				
				// Create the FWUI now:
				Renderer=new FlatWorldUI("#Internal-PowerUI-Raster-"+RasterID,(int)width,(int)height);
				Renderer.Renderer.AllowLayout = false;
				RasterID++;
				
				// Add the listener:
				Renderer.document.window.addEventListener("imagechange",delegate(Dom.Event ev){
					ChangedOutputEvent(ev);
				});
				
				if(PendingFilter!=null){
					
					// Setting the filter will cause an imagechange event anyway:
					Renderer.Filter=PendingFilter;
					PendingFilter=null;
					
				}else{
					// Invoke our change event now:
					Dom.Event e=new Dom.Event("imagechange");
					e.SetTrusted(false);
					ChangedOutputEvent(e);
				}
				
			}
			
			// Does the given renderer belong to the worldUI?
			if(renderer==Renderer.Renderer){
				
				// Yes! We're actually drawing the element.
				return;
				
			}
			
			// Next we'll draw the rastered image.
			// It's essentially just the output from the renderer.
			
			// Get the top left inner corner (inside margin and border):
			float top=box.Y;
			float left=box.X;
			
			// Update the FlatWorldUI next:
			UpdateRenderer(box,width,height);
			
			// Always isolated:
			Isolate();
			
			// Make sure the renderer stalls and doesn't draw anything else of this element or its kids.
			renderer.StallStatus=2;
			
			// Setup boundary:
			BoxRegion boundary=new BoxRegion(left,top,width,height);
			
			if(!boundary.Overlaps(renderer.ClippingBoundary)){
				
				if(Visible){
					SetVisibility(false);
				}
				
				return;
			}else if(!Visible){
				
				// ImageLocation will allocate here if it's needed.
				SetVisibility(true);
				
			}
			
			// Texture time - get its location on that atlas:
			if(LocatedAt==null){
				LocatedAt=new AtlasLocation(width,height);
			}else{
				
				// Dimensions changed?
				int w=(int)width;
				int h=(int)height;
			
				if(LocatedAt.Width!=w || LocatedAt.Height!=h){
					
					// Update it:
					LocatedAt.UpdateFixed(width,height);
					
				}
				
			}
			
			boundary.ClipBy(renderer.ClippingBoundary);
			
			// Ensure we have a batch:
			renderer.SetupBatch(this,null,null);
			
			if(Material==null){
				
				// Create the material now using the isolated shader:
				Material=new Material(renderer.CurrentShaderSet.Isolated);
				
				// Hook up the output:
				Material.SetTexture("_MainTex",Output);
				
			}
			
			// Allocate the block:
			MeshBlock block=Add(renderer);
			
			// Set current material:
			SetBatchMaterial(renderer,Material);
			
			// Set the (overlay) colour:
			block.SetColour(renderer.ColorOverlay);
			block.TextUV=null;
			
			// Z-index (same as a background-image):
			float zIndex = (RenderData.computedStyle.ZIndex-0.003f);
			
			BoxRegion screenRegion=new BoxRegion();
			screenRegion.Set(left,top,width,height);
			
			// Setup the block:
			block.ImageUV=block.SetClipped(boundary,screenRegion,renderer,zIndex,LocatedAt,block.ImageUV);
			
			// Flush it:
			block.Done(renderer.Transform);
			
		}
		
	}
	
	public partial class RenderableData{
		
		/// <summary>Gets or sets a RasterDisplayableProperty.</summary>
		public RasterDisplayableProperty RasterProperty{
			get{
				return GetProperty(typeof(RasterDisplayableProperty)) as RasterDisplayableProperty;
			}
			set{
				AddOrReplaceProperty(value,typeof(RasterDisplayableProperty));
			}
		}
		
	}
	
}