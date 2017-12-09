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
	/// Represents a HTML5 details element.
	/// </summary>
	
	[Dom.TagName("details")]
	public class HtmlDetailsElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphThenAdd(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				lexer.BlockClose("details");
			}else{
				return false;
			}
			
			return true;
			
		}
		
		public override void OnChildrenLoaded(){
			// Grab the summary element:
			HtmlSummaryElement summaryTag=getElementByTagName("summary") as HtmlSummaryElement;
			
			if(summaryTag!=null){
				// Pop it out:
				summaryTag.parentNode.removeChild(summaryTag);
				
				// Set this details element to it:
				summaryTag.Details=this;
				
				// Insert it as a child alongside this details tag:
				parentNode.appendChild(summaryTag);
				
				// We know for sure that summary is the last element, and this details
				// tag is immediately after it. Their the wrong way around, so simply flip them over:
				NodeList children=parentNode.childNodes_;
				
				// Whats the index of the last child?
				int last=children.length-1;
				
				// The last one is now Element:
				children[last]=this;
				
				// And the one before it is summary:
				children[last-1]=summaryTag;
				
			}
			
			// Base:
			base.OnChildrenLoaded();
			
		}
		
	}
	
}