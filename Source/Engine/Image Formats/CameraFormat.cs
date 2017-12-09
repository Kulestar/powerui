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
using Css;
using UnityEngine;
using Blaze;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a view of a camera when displayed using the camera:// protocol.
	/// </summary>
	
	public class CameraFormat:PictureFormat{
		
		/// <summary>The underlying camera.</summary>
		public Camera Camera;
		
		
		public CameraFormat(){}
		
		/// <summary>Sets the URI to the camera.</summary>
		public void SetPath(string path){
			
			// Get it:
			GameObject gameObject=GameObject.Find(path);
			
			if(gameObject!=null){
				// Grab the camera:
				Camera=gameObject.GetComponent<Camera>();
			}
			
			if(Camera!=null){
				
				// Apply initial image:
				Image=Camera.targetTexture;
				
			}
			
		}
		
		/// <summary>True if this camera tag created a render texture.</summary>
		private bool CreatedTexture;
		
		/// <summary>Resizes the render texture.</summary>
		private void Resize(int w,int h){
			
			if(Camera.targetTexture!=null){
				Camera.targetTexture.Release();
			}
			
			CreatedTexture=true;
			
			RenderTexture rt=new RenderTexture(w,h,24);
			
			// Create it now:
			Camera.targetTexture=rt;
			
			// Apply cached one:
			Image=rt;
			
			// Setup the clear flags:
			// Camera.clearFlags=CameraClearFlags.Depth;
			
		}
		
		/// <summary>Releases this format now.</summary>
		public void Release(){
			
			if(Camera!=null && CreatedTexture){
				
				// Release the RT:
				(Image as RenderTexture).Release();
				
			}
			
		}
		
		/// <summary>Called just before the image is about to be drawn (only ever when it's actually visible).
		/// Note that everything else - e.g. ImageMaterial or Width/Height - is always called after this.</summary>
		public override void OnLayout(Css.RenderableData context,LayoutBox box,out float width,out float height){
			
			// Get the shape of the element:
			width=box.PaddedWidth;
			height=box.PaddedHeight;
			
			if(width!=Image.width || height!=Image.height){
				Resize((int)width,(int)height);
			}
			
		}
		
		/// <summary>Should this image be isolated - i.e. off atlas.</summary>
		public override bool Isolate{
			get{
				return true;
			}
		}
		
		public override string[] GetNames(){
			return new string[]{"camera"};
		}
		
		public override ImageFormat Instance(){
			return new CameraFormat();
		}
		
	}
	
}