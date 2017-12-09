using System;
using Css;
using Dom;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// Used when an asset bundle is ready.
	/// </summary>
	public delegate void BundleReadyEvent(AssetBundle bundle);
	
	/// <summary>
	/// This protocol is used to access the internals of an asset bundle. If you want direct access to the bundle itself
	/// See the Bundles class.
	/// bundle://http://yoursite.com/bundle.assetbundle#assetName
	/// </summary>
	
	public class BundleProtocol:FileProtocol{
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
		public override string[] GetNames(){
			return new string[]{"bundle"};		
		}
		
		private IEnumerator Loader(AssetBundleCreateRequest request){
			yield return request;
		}
		
		/// <summary>Gets a bundle using the given bundle URI.</summary>
		private void LoadBundle(Location uri,BundleReadyEvent bre){
			
			// Main thread only. (Even things like bund!=null can fail).
			Callback.MainThread(delegate(){
				
				// The underlying uri is uri.Path.
				string path=uri.Path;
				
				// Loading or loaded?
				AssetBundle bund=Bundles.Get(path);
				
				if(bund!=null){
					
					bre(bund);
					return;
					
				}
				
				DataPackage package;
				
				if(Bundles.Loading.TryGetValue(path,out package)){
					
					// Loading - just add a listener (always runs after the bundle loading callback):
					package.addEventListener("onload",delegate(UIEvent e){
						
						// Callback:
						bre(Bundles.Get(path));
						
					});
					
				}else{
					
					// Make a request:
					package=new DataPackage(path,null);
					
					package.addEventListener("onload",delegate(UIEvent e){
						
						// 5.4.1 onwards
						#if !UNITY_5_4_0 && UNITY_5_4_OR_NEWER
						AssetBundleCreateRequest request=AssetBundle.LoadFromMemoryAsync(package.responseBytes);
						#else
						AssetBundleCreateRequest request=AssetBundle.CreateFromMemory(package.responseBytes);
						#endif
						
						// Get the enumerator:
						IEnumerator enumerator=Loader(request);
						
						// Add updater:
						OnUpdateCallback cb=null;
						
						cb=OnUpdate.Add(delegate(){
							
							// Move enumerator:
							enumerator.MoveNext();
							
							// Request done?
							if(request.isDone){
								
								// Great! Stop:
								cb.Stop();
								
								// Set now:
								AssetBundle bundle=request.assetBundle;
								
								Bundles.Add(path,bundle);
								
								// Callback:
								bre(bundle);
								
							}
							
						});
						
					});
					
					// Send now:
					package.send();
					
				}
				
			});
			
		}
		
		/// <summary>Attempts to get a graphic from the given location using this protocol.</summary>
		/// <param name="package">The audio request. GotGraphic must be called on this when the protocol is done.</param>
		/// <param name="path">The location of the file to retrieve using this protocol.</param>
		public override void OnGetAudio(AudioPackage package){
			
			LoadBundle(package.location,delegate(AssetBundle bundle){
				
				// Pull audio from the bundle using the hash as our hierarchy:
				UnityEngine.Object asset=bundle.LoadAsset(package.location.hash);
				
				// Try loading from the asset:
				if(package.Contents.LoadFromAsset(asset,package)){
					
					// Ok!
					package.Done();
					
				}
				
			});
			
		}
		
		/// <summary>Attempts to get a graphic from the given location using this protocol.</summary>
		/// <param name="package">The image request. GotGraphic must be called on this when the protocol is done.</param>
		/// <param name="path">The location of the file to retrieve using this protocol.</param>
		public override void OnGetGraphic(ImagePackage package){
			
			LoadBundle(package.location,delegate(AssetBundle bundle){
				
				// Pull an image from the bundle using the hash as our hierarchy:
				UnityEngine.Object asset=bundle.LoadAsset(package.location.hash);
				
				// Try loading from the asset:
				if(package.Contents.LoadFromAsset(asset,package)){
					
					// Ok!
					package.Done();
					
				}
				
			});
			
		}
		
	}
	
}