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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using Css;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles the resources (default) protocol.
	/// Files here are loaded from the Unity 'Resources' folder in the project.
	/// Note that animation files must end in .bytes (e.g. animation.spa.bytes)
	/// and all images must be read/write enabled as well as have a "To power 2" of none.
	/// </summary>
	
	public class ResourcesProtocol:FileProtocol{
		
		public override string[] GetNames(){
			return new string[]{"","resources","res"};
		}
		
		public override void OnGetAudio(AudioPackage package){
			
			// Main thread only:
			Callback.MainThread(delegate(){
				
				string resUrl=package.location.Directory+package.location.Filename;
				
				if(resUrl.Length>0 && resUrl[0]=='/'){
					resUrl=resUrl.Substring(1);
				}
				
				// Get the audio:
				UnityEngine.Object resource=Resources.Load(resUrl);
				
				if(resource==null){
					
					// Note: the full file should be called something.bytes for this to work in Unity.
					resUrl=package.location.Path;
					
					if(resUrl.Length>0 && resUrl[0]=='/'){
						resUrl=resUrl.Substring(1);
					}
					
					resource=Resources.Load(resUrl);
					
				}
				
				// Try loading from the asset:
				if(package.Contents.LoadFromAsset(resource,package)){
					
					package.Done();
					
				}
				
			});
			
		}
		
		public override void OnGetGraphic(ImagePackage package){
			
			// Main thread only:
			Callback.MainThread(delegate(){
				
				string resUrl=package.location.Directory+package.location.Filename;
				
				if(resUrl.Length>0 && resUrl[0]=='/'){
					resUrl=resUrl.Substring(1);
				}
				
				// Get the image:
				UnityEngine.Object resource=Resources.Load(resUrl);
				
				if(resource==null){
					
					// Note: the full file should be called something.bytes for this to work in Unity.
					resUrl=package.location.Path;
					
					if(resUrl.Length>0 && resUrl[0]=='/'){
						resUrl=resUrl.Substring(1);
					}
					
					resource=Resources.Load(resUrl);
					
				}
				
				// Try loading from the asset:
				if(package.Contents.LoadFromAsset(resource,package)){
					
					package.Done();
					
				}
				
			});
			
		}
		
		public override void OnGetDataNow(ContentPackage package){
			
			// Main thread only:
			Callback.MainThread(delegate(){
				
				// Getting a files text content from resources.
				byte[] data=null;
				
				string path=GetPath(package.location);
				
				TextAsset asset=Resources.Load(path) as TextAsset;
				
				if(asset==null){
					// Not found
					package.Failed(404);
				}else{
					// Ok
					data=asset.bytes;
					package.ReceivedHeaders(data.Length);
					package.ReceivedData(data,0,data.Length);
				}
				
			});
			
		}
		
		/// <summary>Some file types like .xml and .html have to be chopped off for use with Resources.Load.
		/// This returns the correct path to use.</summary>
		private string GetPath(Location path){
			
			string filetype=path.Filetype;
			string result;
			
			if(filetype=="html" || filetype=="htm" || filetype=="txt" || filetype=="xml" || filetype=="json"){
				result=path.Directory+path.Filename;
			}else{
			
				// The file MUST end in .bytes for this to work.
				result=path.Path;
				
			}
			
			if(result.Length>0 && result[0]=='/'){
				
				// Remove initial foward slash:
				result=result.Substring(1);
				
			}
			
			return result;
			
		}
		
		public override void OnFollowLink(HtmlElement linkElement,Location path){
			string target=linkElement.getAttribute("target");
			
			if(target!=null && target=="_blank"){
				// Open the given url.
				Application.OpenURL(path.absolute);
				return;
			}
			
			// Apply location:
			linkElement.htmlDocument.location=path;
			
		}
		
		public override bool FullAccess(Location path){
			// This is entirely local and controlled by the developer - it's a safe protocol.
			return true;
		}
		
	}
	
}