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
using Dom;
using PowerUI;
using Css;
using PowerUI.Http;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// Handles the http:// protocol.
	/// Downloads files and text from the web and also handles web links.
	/// Note that this protocol (and many others) have been designed to be removeable - just delete the file.
	/// </summary>
	
	public class HttpProtocol:FileProtocol{
		
		public override string[] GetNames(){
			return new string[]{"http","web","https","ws"};
		}
		
		public override void OnFollowLink(HtmlElement linkElement,Location path){
			
			// Grab the protocol:
			string protocol=path.Protocol;
			
			// Is it actually a web one?
			string[] names=GetNames();
			
			bool web=false;
			
			for(int i=0;i<names.Length;i++){
				
				if(protocol==names[i]){
					
					web=true;
					break;
					
				}
				
			}
			
			HtmlDocument targetDocument=null;
			
			if(web){
				
				// Otherwise it's e.g. an app link. No target there - always external.
				
				// Resolve the link elements target:
				targetDocument=linkElement.ResolveTarget();
				
			}
			
			if(targetDocument==null){
				
				// Open the URL outside of Unity:
				Application.OpenURL(path.absolute);
				
			}else{
				
				// Load into that document:
				targetDocument.location=path;
				
			}
			
		}
		
		/// <summary>Attempts to get cached data for the given location.</summary>
		public override CachedContent GetCached(Dom.Location location){
			
			// Standard cached data:
			return GetCached(location,false);
			
		}
		
		public override void OnGetDataNow(ContentPackage package){
			
			// Pass straight through to a HttpRequest:
			HttpRequest req=new HttpRequest(package);
			req.Send();
			
		}
		
	}
	
}