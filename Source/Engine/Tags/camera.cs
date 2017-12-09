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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	#define PRE_UNITY4
#endif

using System;
using Css;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Custom tag for an inline camera. You can place your UI before and after this as normal.
	/// You must importantly set the path="" attribute to the path in the hierarchy of the camera itself.
	/// This tag also has a mask="file_path" attribute which can be used to shape the camera
	/// in interesting ways e.g. a circular minimap.
	/// You must also set the height and width of this element using either css or height="" and width="".
	/// </summary>
	
	[Dom.TagName("camera")]
	public class HtmlCameraElement:HtmlElement{
		
		/// <summary>The camera component iself. Set with path="hierarchy_path".</summary>
		public Camera Camera;
		/// <summary>The transform of the mask, if there is one. Set with mask="file_path".</summary>
		public Transform Mask;
		/// <summary>The depth factor of a perspective camera.</summary>
		public float DepthFactor;
		/// <summary>The field of view of a perspective camera.</summary>
		public float FieldOfView;
		/// <summary>True if this element has authority over the RenderTexture (and can release it/ create a new one).</summary>
		public bool CanResize=true;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>True if this tag is non-standard.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="path"){
				// Go get the camera now!
				
				// Clear any existing one:
				Camera=null;
				
				Callback.MainThread(delegate(){
					
					// Grab the path itself:
					string path=getAttribute("path");
					
					// Get it:
					GameObject gameObject=GameObject.Find(path);
					
					if(gameObject!=null){
						// Grab the camera:
						Camera=gameObject.GetComponent<Camera>();
					}
					
					if(Camera!=null){
						
						ComputedStyle computed=Style.Computed;
						
						// Create RT if one is needed:
						RenderTexture rt=Camera.targetTexture;
						
						if(rt==null){
							
							// Apply:
							ApplyNewRenderTexture((int)computed.InnerWidth,(int)computed.InnerHeight);
							
						}else{
							
							// Apply to background:
							image=rt;
							
						}
						
					}
					
					ParentMask();
					
				});
				
				return true;
			}else if(property=="noresize"){
				
				// Can't resize if noresize is not null:
				CanResize=(getAttribute("noresize")==null);
				
			}else if(property=="mask"){
				// We've got a mask!
				
				// Grab the file path:
				string maskFile=getAttribute("mask");
				
				if(maskFile==null){
					
					SetMask(null);
					
				}else{
					
					// Create a package to get the mask:
					ImagePackage package=new ImagePackage(maskFile,document.basepath);
					
					package.onload=delegate(UIEvent e){
						
						// Apply the mask:
						PictureFormat pict=package.Contents as PictureFormat;
						
						if(pict!=null){
							SetMask(pict.Image);
						}
						
					};
					
					// Go get it:
					package.send();
				}
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Called when the host element is removed from the DOM.</summary>
		internal override void RemovedFromDOM(){
			
			base.RemovedFromDOM();
			
			// Remove from cache and release the RT if we made it:
			if(Camera!=null && CreatedTexture){
				
				CreatedTexture=false;
				
				// Clear:
				Camera.targetTexture.Release();
				Camera.targetTexture=null;
				
			}
			
		}
		
		/// <summary>The approximate pixel rect of this camera.</summary>
		public Rect pixelRect{
			get{
				
				LayoutBox box=Style.Computed.FirstBox;
				
				return new Rect(
					box.X,
					box.Y,
					box.InnerWidth,
					box.InnerHeight
				);
				
			}
		}
		
		/// <summary>True if this camera tag created a render texture.</summary>
		private bool CreatedTexture;
		
		internal void ApplyNewRenderTexture(int w,int h){
			
			if(w<=0 || h<=0){
				return;
			}
			
			CreatedTexture=true;
			
			RenderTexture rt=new RenderTexture(w,h,24);
			
			// Create it now:
			Camera.targetTexture=rt;
			
			// Element.image API:
			image=rt;
			
			// Dispatch an event to signal the RT changed:
			Dom.Event e=new Dom.Event("cameraresize");
			e.SetTrusted(false);
			dispatchEvent(e);
			
			// Setup the clear flags:
			// Camera.clearFlags=CameraClearFlags.Depth;
			
		}
		
		/// <summary>Parents the mask to the camera, if there is one.</summary>
		private void ParentMask(){
			if(Mask==null || Camera==null){
				return;
			}
			
			// Parent it:
			Mask.parent=Camera.transform;
			
			// Reset the location:
			Mask.localRotation=Quaternion.identity;
			Mask.localPosition=new Vector3(0f,0f,Camera.nearClipPlane+0.5f);
			Mask.localScale=new Vector3(80f,80f,80f);
		}
		
		/// <summary>Fires all events into the scene.</summary>
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			// Handle locally:
			if(!base.HandleLocalEvent(e,bubblePhase)){
				
				// Fire it in if it's got coords:
				FireIntoScene(e as UIEvent);
				
				return true;
				
			}
			
			return false;
			
		}
		
		/// <summary>Fires an event into the scene of the camera.</summary>
		internal void FireIntoScene(UIEvent e){
			
			if(e==null){
				return;
			}
			
			// Get coords relative to the element. These are non-standard convenience properties:
			float localX=e.localX;
			float localY=e.localY;
			
			// Flip Y because Unity is upside down relative to input coords:
			localY=ScreenInfo.ScreenY-1-localY;
			
			// Fire off an input event at that point:
			RaycastHit worldUIHit;
			if(Physics.Raycast(Camera.ScreenPointToRay(new Vector2(localX,localY)),out worldUIHit)){
				
				// Did it hit a worldUI?
				WorldUI worldUI=WorldUI.Find(worldUIHit);
				
				if(worldUI==null){
					
					// Nope!
					return;
				}
				
				// Resolve the hit into a -0.5 to +0.5 point:
				float x;
				float y;
				worldUI.ResolvePoint(worldUIHit,out x,out y);
				
				// Map it from a relative point to a 'real' one:
				Vector2 point=worldUI.RelativePoint(x,y);
				
				// Apply to x/y:
				x=point.x;
				y=point.y;
				
				// Pull an element from that worldUI:
				Element el=worldUI.document.elementFromPointOnScreen(x,y);
				
				if(el!=null){
					
					// Fire the event for that element:
					el.dispatchEvent(e);
					
				}
				
			}
			
		}
		
		/// <summary>Applies an alpha mask over the camera itself. This can be used to shape the camera
		/// into e.g. a circle.</summary>
		/// <param name="image">The mask image. Originates from the mask="file_path" attribute.</param>
		public void SetMask(Texture image){
			if(htmlDocument.AotDocument){
				return;
			}
			
			// Main thread only (as it checks if Mask is null):
			Callback.MainThread(delegate(){
				
				if(Mask==null){
					// Create the gameobject.
					
					if(image==null){
						return;
					}
					
					// Create the object:
					#if PRE_UNITY4
					GameObject maskObject=GameObject.CreatePrimitive(PrimitiveType.Plane);
					#else
					GameObject maskObject=GameObject.CreatePrimitive(PrimitiveType.Quad);
					#endif
					
					// Remove the MC:
					MeshCollider collider=maskObject.GetComponent<MeshCollider>();
					
					if(collider!=null){
						// Remove it:
						GameObject.Destroy(collider);
					}
					
					// Grab the transform:
					Mask=maskObject.transform;
					
					// Set the name:
					maskObject.name="#PowerUI-Mask";
					
					// If possible, parent it to the camera now:
					ParentMask();
					
					// Grab the renderer:
					MeshRenderer renderer=maskObject.GetComponent<MeshRenderer>();
					
					// Grab the material:
					Material material=renderer.material;
					
					// Apply the shader:
					material.shader=MaskShader;
					
					// Set the offset - it must be the first thing rendered always:
					material.renderQueue=1;
					
					// Apply the mask texture:
					material.SetTexture("_Mask",image);
					
				}
				
			});
			
		}
		
		/// <summary>Called during the layout pass.</summary>
		public override void OnRender(Renderman renderer){
			
			if(Camera==null){
				return;
			}
			
			// Grab the computed style:
			LayoutBox box=Style.Computed.FirstBox;
			
			if(box==null){
				return;
			}
			
			// Update render texture size:
			int w=(int)box.InnerWidth;
			int h=(int)box.InnerHeight;
			
			// Get the texture:
			RenderTexture rt=Camera.targetTexture;
			
			if(rt==null){
				
				// Allocate it now (camera is set so one should be required):
				ApplyNewRenderTexture(w,h);
				
			}else if((rt.width!=w || rt.height!=h) && CanResize){
				
				// Release it (can't resize):
				rt.Release();
				
				// Create a new one:
				ApplyNewRenderTexture(w,h);
				
			}
			
			// Figure out the mask size, if there is one:
			if(Mask!=null){
				
				// Figure out the aspect ratio:
				float aspect=((float)box.InnerWidth/(float)box.InnerHeight);
				
				if(Camera.orthographic){
					if(DepthFactor!=0f){
						DepthFactor=0f;
					}
					
					// Grab the orthoSize:
					float size=Camera.orthographicSize*2f;
					
					// Set the scale:
					Mask.localScale=new Vector3(size*aspect,size,1f);
				}else{
					
					if(DepthFactor==0f || Camera.fieldOfView!=FieldOfView){
						ComputeDepth();
					}
					
					// It just needs to be depth factor long:
					
					Mask.localScale=new Vector3(
						DepthFactor*aspect,
						DepthFactor,
						1f
					);
					
				}
			}
			
		}
		
		/// <summary>Computes the depth factor for a perspective camera when it's required for a mask.</summary>
		private void ComputeDepth(){
			
			// The depth that the mask is at (constant):
			float depth=Mask.localPosition.z;
			
			// Update the FOV:
			FieldOfView=Camera.fieldOfView;
			
			// Forming a triangle from the camera, figure out how "long" the mask should be:
			float opp=depth*(float)Math.Tan((FieldOfView/2f)*Mathf.Deg2Rad);
			
			// The depth factor is the target length/1, which is just target length:
			// opp was using a triangle (only one half of the cameras view), so *2:
			DepthFactor=opp*2f;
			
		}
		
		/// <summary>The shader used for the camera mask.</summary>
		public Shader MaskShader{
			get{
				return Shader.Find("PowerUI/Camera Mask");
			}
		}
		
	}
	
}