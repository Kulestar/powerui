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
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles elements going fullscreen.
	/// </summary>

	public partial class HtmlElement{
		
		/// <summary>The original FS parent node.</summary>
		internal static Node CachedFullscreenParent;
		/// <summary>The original style of the FS element.</summary>
		internal static string CachedFullscreenStyle;
		
		
		/// <summary>Requests this element (e.g. a video) to go fullscreen.</summary>
		public void requestFullscreen(){
			
			if(htmlDocument.fullscreenElement==this){
				// Already fullscreen:
				return;
			}
			
			// Trigger fullscreen event:
			Dom.Event e=new Dom.Event("fullscreenchange");
			e.SetTrusted(false);
			
			if(!dispatchEvent(e)){
				// Cancelled it.
				return;
			}
			
			if(htmlDocument.fullscreenElement!=null){
				// Cancel it:
				htmlDocument.exitFullscreen();
			}
			
			// Apply element:
			htmlDocument.fullscreenElement=this;
			
			// Cache the current parent:
			CachedFullscreenParent=parentNode;
			
			if(parentNode!=null){
				// Can't actually do anything with it anyway otherwise - it's already filling the screen!
				
				// Remove it from the DOM:
				parentNode.removeChild(this);
				
				// Add it elsewhere:
				htmlDocument.html.appendChild(this);
				
				// Cache ele style:
				CachedFullscreenStyle=style.cssText;
				
				// Set basic style:
				style.cssText="width:100%;height:100%;left:0px;top:0px;right:0px;bottom:0px;position:fixed;";
				
			}
			
			// Set (note that this triggers a CSS event):
			setAttribute("fullscreen", "1");
			
			// Update local style:
			Style.Computed.RefreshLocal();
			
		}
		
	}
	
}