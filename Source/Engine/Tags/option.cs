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
	/// Handles option tags for dropdowns. Supports the selected and value="" attributes.
	/// </summary>

	[Dom.TagName("option")]
	public class HtmlOptionElement:HtmlElement{
		
		/// <summary>True if this is the selected option.</summary>
		public bool Selected;
		/// <summary>The select dropdown that this option belongs to.</summary>
		public HtmlSelectElement Dropdown;
		
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Normal;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InSelect){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
				lexer.Push(this,true);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
				lexer.ReconstructFormatting();
				
				lexer.Push(this,true);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InSelect){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){	
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="selected"){
				string isSelected=getAttribute("selected");
				Selected=(string.IsNullOrEmpty(isSelected) || isSelected=="1" || isSelected=="true" || isSelected=="yes");
				
				if(Dropdown!=null){
					// Tell the select:
					Dropdown.SetSelected(this);
				}
				
				return true;
			}else if(property=="text"){
				
				// Write the innerHTML:
				innerHTML=getAttribute("text");
				
				return true;
			}
			
			return false;
		}
		
		public override void OnChildrenLoaded(){
			
			// Get the dropdown:
			Dropdown=GetParentByTagName("select") as HtmlSelectElement;
			
			if(Selected && Dropdown!=null){
				// Tell the select:
				Dropdown.SetSelected(this);
			}
			
		}
		
		/// <summary>True if this element is disabled.</summary>
		public bool disabled{
			get{
				if(GetBoolAttribute("disabled")){
					return true;
				}
				
				HtmlOptGroupElement gp=GetParentByTagName("optgroup") as HtmlOptGroupElement;
				
				if(gp==null){
					return false;
				}
				
				return gp.disabled;
			}
			set{
				SetBoolAttribute("disabled",value);
			}
		}
		
		/// <summary>A label for the group.</summary>
		public string label{
			get{
				string l=getAttribute("label");
				
				if(l==null){
					return text;
				}
				
				return l;
			}
			set{
				setAttribute("label", value);
			}
		}
		
		/// <summary>True if this option is currently selected.</summary>
		public bool selected{
			get{
				
				// Get the options menu:
				if(Dropdown==null){
					return false;
				}
				
				return Dropdown.SelectedOption==this;
			}
			set{
				if(value){
					Dropdown.SetSelected(this);
				}else{
					// Return to index 0:
					Dropdown.SetSelected(0);
				}
			}
		}
		
		/// <summary>the index of this option element.</summary>
		public int index{
			get{
				if(Dropdown==null){
					// Datalist
					return 0;
				}
				return Dropdown.GetOptionIndex(this);
			}
		}
		
		/// <summary>The text of an option element.</summary>
		public string text{
			get{
				return getAttribute("text");
			}
			set{
				setAttribute("text", value);
			}
		}
		
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(bubblePhase && Dropdown!=null && e.type=="mouseup"){
				
				Dropdown.SetSelected(this);
				Dropdown.Hide();
				
			}
			
			// Handle locally:
			return base.HandleLocalEvent(e,bubblePhase);
			
		}
		
	}
	
}