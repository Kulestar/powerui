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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a html button. This is the same as &lt;input type="button"&gt;, but a little shorter.
	/// </summary>
	
	[Dom.TagName("button")]
	public class HtmlButtonElement:HtmlElement{
		
		/// <summary>The value text for this button.</summary>
		public string Value;
		
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		public HtmlButtonElement(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		/// <summary>Does this element get submitted with the form?</summary>
		internal override bool IsFormSubmittable{
			get{
				return true;
			}
		}
		
		/// <summary>Does this element list in form.elements?</summary>
		internal override bool IsFormListed{
			get{
				return true;
			}
		}
		
		/// <summary>Can this element have a label?</summary>
		internal override bool IsFormLabelable{
			get{
				return true;
			}
		}
		
		/// <summary>All labels targeting this select element.</summary>
		public NodeList labels{
			get{
				return HtmlLabelElement.FindAll(this);
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				if(lexer.IsInScope("button")){
					
					// (Parse error)
					lexer.GenerateImpliedEndTags();
					
					lexer.CloseInclusive("button");
					
				}
				
				lexer.ReconstructFormatting();
				
				lexer.Push(this,true);
				
				lexer.FramesetOk=false;
				
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
				lexer.BlockClose("button");
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="value"){
				SetValue(getAttribute("value"));
				return true;
			}else if(property=="content"){
				SetValue(getAttribute("content"),true);
				return true;
			}
			
			return false;
		}
		
		/// <summary>Sets the value of this button.</summary>
		/// <param name="value">The value to set. Note that HTML is stripped here.</param>
		public void SetValue(string value){
			SetValue(value,false);
		}
		
		/// <summary>Sets the value of this button, optionally as a html string.</summary>
		/// <param name="value">The value to set.</param>
		/// <param name="html">True if the value can safely contain html.</param>
		public void SetValue(string value,bool html){
			
			setAttribute("value", Value=value);
			
			if(html){
				innerHTML=value;
			}else{
				textContent=value;
			}
			
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Focus it:
			focus();
			
		}
		
	}
	
}