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

using Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Represents a slot element.
	/// </summary>
	
	[Dom.TagName("slot")]
	public class HtmlSlotElement:HtmlElement{
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		/// <summary>The assigned nodes in this slot.</summary>
		public IEnumerable<Node> assignedNodes(){
			return assignedNodes(null);
		}
		
		/// <summary>The assigned nodes in this slot.</summary>
		public IEnumerable<Node> assignedNodes(object options){
			
			if(parentNode!=null){
				
				// Iterate the virtuals of the parent:
				RenderableData cs=(parentNode as IRenderableNode).RenderData;
				
				if(cs.Virtuals!=null){
					
					// Return each one:
					foreach(KeyValuePair<int,Node> kvp in cs.Virtuals.Elements){
						
						yield return kvp.Value;
						
					}
					
				}
			
			}
			
		}
		
	}
	
}