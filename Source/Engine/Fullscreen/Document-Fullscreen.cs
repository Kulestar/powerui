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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Handles functionality for when an element wants to go fullscreen.
	/// </summary>

	public partial class HtmlDocument{
		
		/// <summary>The current fullscreen element, if any.</summary>
		public HtmlElement fullscreenElement;
		
		
		/// <summary>Exits a fullscreen viewing.</summary>
		public void exitFullscreen(){
			
			if(fullscreenElement==null){
				return;
			}
		
			// Trigger fullscreen event:
			Dom.Event e=new Dom.Event("fullscreenchange");
			e.SetTrusted();
			
			if(!dispatchEvent(e)){
				// Something killed the request.
				return;
			}
			
			// Restore style:
			fullscreenElement.style.cssText=HtmlElement.CachedFullscreenStyle;
			
			// Restore original parent:
			if(HtmlElement.CachedFullscreenParent!=null){
				
				fullscreenElement.parentNode.removeChild(fullscreenElement);
				
				HtmlElement.CachedFullscreenParent.appendChild(fullscreenElement);
				
				// Clear:
				HtmlElement.CachedFullscreenParent=null;
				
			}
			
			// Clear it:
			HtmlElement.CachedFullscreenStyle=null;
			
			// Clear attrib:
			fullscreenElement.removeAttribute("fullscreen");
			
			// Update local style:
			fullscreenElement.style.Computed.RefreshLocal();
			
			// Clear fullscreen ele:
			fullscreenElement=null;
			
		}
		
		/// <summary>Is something currently fullscreen?</summary>
		public bool fullscreen{
			get{
				return (fullscreenElement!=null);
			}
		}
		
		/// <summary>Is full screen available for this document?</summary>
		public bool fullscreenEnabled{
			get{
				return true;
			}
		}
		
	}
	
}