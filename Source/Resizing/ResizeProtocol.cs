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
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Automatically resizes the "actual" image to prevent wasting memory.
	/// With this, you can have one set of high-res images for all your devices and they'll just fit.
	/// Requests from Resources only.
	/// </summary>
	
	public class ResizeProtocol:ResourcesProtocol{
		
		public override string[] GetNames(){
			return new string[]{"resize"};
		}
		
		public override void OnGetGraphic(ImagePackage package){
			
			// Already resized?
			ResizedImage resized=ResizedImages.Get(package.location.Path);
			
			if(resized!=null){
				
				// Sure is!
				package.GotGraphic(resized.Image);
				
				return;
				
			}
			
			// Main thread only:
			Callback.MainThread(delegate(){
				
				// Try loading from resources:
				string resUrl=package.location.Directory+package.location.Filename;
				
				if(resUrl.Length>0 && resUrl[0]=='/'){
					resUrl=resUrl.Substring(1);
				}
				
				// Get the image:
				UnityEngine.Object resource=Resources.Load(resUrl);
				
				if(resource==null){
					
					// Note: the full file should be called something.bytes for this to work in Unity.
					resource=Resources.Load(package.location.Path);
					
				}
				
				if(!package.Contents.LoadFromAsset(resource,package)){
					return;
				}
				
				PictureFormat pict=package.Contents as PictureFormat;
				
				if(pict!=null){
					// Resize the image:
					resized=ResizedImages.Add(package.location.Path,pict.Image as Texture2D);
					
					// Apply:
					pict.Image=resized.Image;
				}
				
				// Great, stop there:
				package.Done();
				
			});
			
		}
		
	}
	
}