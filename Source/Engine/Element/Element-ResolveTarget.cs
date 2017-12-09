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
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	
	public partial class HtmlElement{
		
		/// <summary>Resolves the target attribute to a document.</summary>
		public HtmlDocument ResolveTarget(){
			return ResolveTarget(getAttribute("target"));
		}
		
		/// <summary>Resolves the given target to a document.</summary>
		/// <returns>The targeted document. Null if there is no document at all and the target is essentially outside of Unity.</returns>
		public HtmlDocument ResolveTarget(string target){
			
			// Grab the document the element is in:
			HtmlDocument document=htmlDocument;
			
			if(target==null){
				// No target - does the document define a default one?
				// Note that this is set with the "base" html tag.
				
				if(document.location!=null){
					target=document.baseTarget;
				}
				
			}
			
			// Null target is the same as _self.
			if(string.IsNullOrEmpty(target)){
				target="_self";
			}
			
			// Grab the window:
			Window window=document.window;
			
			switch(target){
				case "_blank":
					
					// Open the given url outside Unity.
					return null;
				
				case "_top":
					// Open the given URL at the top window.
					
					return window.top.document;
					
				case "_parent":
					
					// Open it there:
					return window.parent.document;
					
				case "_self":
					
					// Open it in this document:
					return document;
					
				case "_main":
					
					// Open into the main UI:
					return UI.document;
					
				default:
					// Anything else and it's the name of an iframe (preferred) or a WorldUI.
					
					// Get the element by name:
					HtmlElement iframeElement=document.getElementByAttribute("name",target) as HtmlElement ;
					
					if(iframeElement==null){
						
						// WorldUI with this name?
						WorldUI ui=WorldUI.Find(target);
						
						if(ui==null){
							
							// Not found - same as self:
							return document;
							
						}
						
						// Load into the WorldUI:
						return ui.document;
						
					}
					
					// Great, we have an iframe - grab the content document:
					return iframeElement.contentDocument;
					
			}
			
		}
		
	}
	
}