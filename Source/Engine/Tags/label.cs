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
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles the standard inline label element.
	/// Clicking on them acts just like clicking on the input they target.
	/// </summary>
	
	[Dom.TagName("label")]
	public class HtmlLabelElement:HtmlElement{
		
		/// <summary>Finds all labels targeting the given labelable element.</summary>
		public static NodeList FindAll(HtmlElement forParent){
			
			NodeList hc=new NodeList();
			
			// For each label in the document..
			foreach(Dom.Element e in forParent.document.getElementsByTagName("label")){
				
				// Get as label element:
				HtmlLabelElement hle=(e as HtmlLabelElement);
				
				// control matches?
				if(hle.control==forParent){
					// Yep!
					hc.push(hle);
				}
				
			}
			
			return hc;
			
		}
		
		/// <summary>The ID of the element the clicks of this get 'directed' at.
		/// If blank/null, the first child of this element that is labelable is used.</summary>
		public string ForElement;
		
		
		/// <summary>The ID of a labelable form-related element.</summary>
		public string htmlFor{
			get{
				return getAttribute("for");
			}
			set{
				setAttribute("for", value);
			}
		}
		
		/// <summary>The element that this label is for.</summary>
		public HtmlElement control{
			get{
				return GetFor();
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			if(property=="for"){
				ForElement=getAttribute("for");
				return true;
			}
			return false;
		}
		
		/// <summary>Finds a labelable child node.</summary>
		private HtmlElement FindLabelable(Node parent){
			
			if(parent.childNodes_==null){
				return null;
			}
			
			// For each child node..
			for(int i=0;i<parent.childNodes_.length;i++){
				
				// Get it:
				HtmlElement child=parent.childNodes_[i] as HtmlElement;
				
				// Might be an optgroup containing it. Check if it is:
				if(child!=null && child.IsFormLabelable){
					return child;
				}
				
				if(child.childNodes_!=null){
					
					// Go recursive:
					HtmlElement inChild=FindLabelable(child);
					
					if(inChild!=null){
						return inChild;
					}
					
				}
				
			}
			
			// Not found here.
			return null;
		}
		
		/// <summary>Gets the element this label is for. If found, it should always be a labelable element.</summary>
		private HtmlElement GetFor(){
			
			if(ForElement==null){
				return FindLabelable(this);
			}
			
			// ForElement is an ID - lets go find the element in the document with that ID.
			return document.getElementById(ForElement) as HtmlElement;
			
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Who wants the click? That's the for element:
			HtmlElement forElement=control;
			
			if(forElement!=null && clickEvent.isTrusted){
				
				// Click it (note that this click is *not trusted* which blocks it from going recursive):
				forElement.click();
				
			}
			
		}
		
	}
	
}