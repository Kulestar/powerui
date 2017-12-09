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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles textarea tags.
	/// </summary>
	
	[Dom.TagName("textarea")]
	public class HtmlTextareaElement:HtmlElement{
		
		/// <summary>This is the caret element.</summary>
		public HtmlCaretElement Caret;
		/// <summary>The original value.</summary>
		public string defaultValue;
		/// <summary>The maximum length of text in this box.</summary>
		public int MaxLength=int.MaxValue;
		/// <summary>A placeholder value.</summary>
		private string Placeholder_="";
		
		
		public HtmlTextareaElement(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		/// <summary>Gets or sets the value of this element. Input/Select/Textarea elements only.</summary>
		public override string value{
			get{
				return textContent;
			}
			set{
				SetValue(value);
			}
		}
		
		/// <summary>The placeholder text, if any.</summary>
		public string placeholder{
			get{
				return getAttribute("placeholder");
			}
			set{
				setAttribute("placeholder", value);
			}
		}
		
		/// <summary>True if this element can be autofocused.</summary>
		public bool autofocus{
			get{
				return GetBoolAttribute("autofocus");
			}
			set{
				SetBoolAttribute("autofocus",value);
			}
		}
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		/// <summary>True if this element is disabled.</summary>
		public bool disabled{
			get{
				return GetBoolAttribute("disabled");
			}
			set{
				SetBoolAttribute("disabled",value);
			}
		}
		
		/// <summary>The max length.</summary>
		public long maxLength{
			get{
				
				if(MaxLength>=int.MaxValue){
					return -1;
				}
				
				return MaxLength;
			}
			set{
				setAttribute("maxlength", value.ToString());
			}
		}
		
		/// <summary>True if this element is required.</summary>
		public bool required{
			get{
				return GetBoolAttribute("required");
			}
			set{
				SetBoolAttribute("required",value);
			}
		}
		
		/// <summary>True if this element is readonly.</summary>
		public bool readOnly{
			get{
				return GetBoolAttribute("readonly");
			}
			set{
				SetBoolAttribute("readonly",value);
			}
		}
		
		/// <summary>The start of the selection, or the caret index.</summary>
		public ulong selectionStart{
			get{
				if(Caret==null){
					return 0;
				}
				
				return Caret.selectionStart;
			}
		}
		
		/// <summary>The end of the selection, or the caret index+1.</summary>
		public ulong selectionEnd{
			get{
				if(Caret==null){
					return 0;
				}
				
				return Caret.selectionEnd;
			}
		}
		
		/// <summary>Selection direction.</summary>
		public string selectionDirection{
			get{
				return (selectionEnd>selectionStart) ? "forward" : "backward";
			}
		}
		
		/// <summary>The wrap attribute.</summary>
		public string wrap{
			get{
				return getAttribute("wrap");
			}
			set{
				setAttribute("wrap", value);
			}
		}
		
		public override void OnFormReset(){
			SetValue(defaultValue);
		}
		
		/// <summary>Does this element get reset with the form?</summary>
		internal override bool IsFormResettable{
			get{
				return true;
			}
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
		
		/// <summary>Looks out for paste events.</summary>
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(base.HandleLocalEvent(e,bubblePhase)){
				// It was blocked. Don't run the default.
				return true;
			}
			
			if(e is ClipboardEvent && bubblePhase && e.type=="paste"){
				
				// Paste the data at the caret index (must be text only).
				string textToPaste=(e as ClipboardEvent).text;
				
				if(textToPaste!=null){
					
					string value=this.value;
					
					if(value==null){
						value=""+textToPaste;
					}else{
						value=value.Substring(0,CaretIndex)+textToPaste+value.Substring(CaretIndex,value.Length-CaretIndex);
					}
					
					SetValue(value);
					MoveCaret(CaretIndex+textToPaste.Length,true);
					
				}
				
			}
			
			return false;
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.SkipNewline();
				
				lexer.FramesetOk=false;
				
				lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.RCData);
				
			}else if(mode==HtmlTreeMode.InSelect){
				
				lexer.InputOrTextareaInSelect(this);
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>The current location of the caret.</summary>
		public int CaretIndex{
			get{
				if(Caret==null){
					return 0;
				}
				
				return Caret.Index;
			}
		}
		
		/// <summary>The container holding the text.</summary>
		public RenderableTextNode TextHolder{
			get{
				return firstChild as RenderableTextNode;
			}
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
			
			if(property=="placeholder"){
				
				Placeholder_=getAttribute("placeholder");
				innerHTML=Placeholder_;
				
				return true;
				
			}else if(property=="maxlength"){
				
				string value=getAttribute("maxlength");
				
				if(string.IsNullOrEmpty(value)){
					// It's blank - set it to the default.
					MaxLength=int.MaxValue;
				}else{
					
					// Parse the maximum length from the string:
					if(!int.TryParse(value,out MaxLength)){
						
						// Not a number!
						MaxLength=int.MaxValue;
						
					}
					
				}
				
				return true;
			}
			
			return false;
		}
		
		public override KeyboardMode OnShowMobileKeyboard(){
			KeyboardMode result=new KeyboardMode();
			return result;
		}
		
		/// <summary>Sets the value of this textarea.</summary>
		/// <param name="value">The value to set.</param>
		private void SetValue(string newValue){
			
			Dom.Event e=new Dom.Event("change");
			e.SetTrusted(false);
			if(!dispatchEvent(e)){
				return;
			}
			
			if(MaxLength!=int.MaxValue){
				// Do we need to clip it?
				if(newValue.Length>MaxLength){
					// Yep!
					newValue=newValue.Substring(0,MaxLength);
				}
			}
			
			if(CaretIndex>newValue.Length){
				MoveCaret(0);
			}
			
			if(newValue=="" && Placeholder_!=""){
				newValue=Placeholder_;
			}
			
			// Write text content:
			textContent=newValue;
			
			// Redraw:
			RequestLayout();
			
		}
		
		public override void OnChildrenLoaded(){
			
			// Save default value:
			defaultValue=textContent;
			
		}
		
		public override void OnKeyPress(KeyboardEvent pressEvent){
			
			if(readOnly){
				return;
			}
			
			if(pressEvent.heldDown){
				// Add to value unless it's backspace:
				string value=this.value;
				
				if(!char.IsControl(pressEvent.character) && pressEvent.character!='\0'){
					
					// Drop the character in the string at caretIndex
					if(value==null){
						value=""+pressEvent.character;
					}else{
						value=value.Substring(0,CaretIndex)+pressEvent.character+value.Substring(CaretIndex,value.Length-CaretIndex);
					}
					
					SetValue(value);
					MoveCaret(CaretIndex+1);
					
					return;
				}
				
				// It's a command character:
				
				KeyCode key=pressEvent.unityKeyCode;
				
				if(key==KeyCode.LeftArrow){
					MoveCaret(CaretIndex-1,true);
				}else if(key==KeyCode.RightArrow){
					MoveCaret(CaretIndex+1,true);
				}else if(key==KeyCode.UpArrow){
					MoveCaret(FindNewline(-1),true);
				}else if(key==KeyCode.DownArrow){
					MoveCaret(FindNewline(1),true);
				}else if(key==KeyCode.Backspace){
					
					// Delete the character before the caret (or the selection, if we have one).
					
					// Got a selection?
					if(Caret!=null && Caret.TryDeleteSelection()){
						return;
					}
					
					if(string.IsNullOrEmpty(value) || CaretIndex==0){
						return;
					}
					
					int index=CaretIndex;
					value=value.Substring(0,index-1)+value.Substring(index,value.Length-index);
					SetValue(value);
					MoveCaret(index-1);
				}else if(key==KeyCode.Delete){
					// Delete the character after the caret.
					
					// Got a selection?
					if(Caret!=null && Caret.TryDeleteSelection()){
						return;
					}
					
					if(string.IsNullOrEmpty(value)||CaretIndex==value.Length){
						return;
					}
					
					int index=CaretIndex;
					value=value.Substring(0,index)+value.Substring(index+1,value.Length-index-1);
					SetValue(value);
				}else if(key==KeyCode.Home){
					// Hop to the start:
					
					MoveCaret(0,true);
				
				}else if(key==KeyCode.End){
					// Hop to the end:
					
					int maxCaret=0;
					
					if(value!=null){
						maxCaret=value.Length;
					}
					
					MoveCaret(maxCaret,true);
				
				}else if(key==KeyCode.Return || key==KeyCode.KeypadEnter){
					
					// Add a newline
					if(value==null){
						value=""+pressEvent.character;
					}else{
						value=value.Substring(0,CaretIndex)+'\n'+value.Substring(CaretIndex,value.Length-CaretIndex);
					}
					SetValue(value);
					MoveCaret(CaretIndex+1);
				
				}
				
			}
			
			base.OnKeyPress(pressEvent);
			
		}
		
		/// <summary>Gets the next/previous suitable newline index.</summary>
		public int FindNewline(int direction){
			
			// Get the text content:
			RenderableTextNode htn=TextHolder;
			
			if(htn==null || htn.RenderData.Text==null || htn.RenderData.Text.Characters==null){
				return CaretIndex;
			}
			
			// Get the renderer so we can access the chars:
			TextRenderingProperty trp=htn.RenderData.Text;
			
			// First index to check is..
			int index=CaretIndex+direction;
			
			// Safety check:
			int max=trp.Characters.Length;
			
			if(index>=max){
				index=max-1;
			}else if(index<0){
				index=0;
			}
			
			while(index>0 && index<max){
				
				// Check if char[index] is a newline:
				InfiniText.Glyph glyph=trp.Characters[index];
				
				if(glyph!=null && glyph.Charcode==(int)'\n'){
					
					// Got it!
					if(direction==1){
						index++;
					}
					break;
					
				}
				
				// Next one:
				index+=direction;
				
			}
			
			return index;
			
		}
		
		/// <summary>Called when the element is focused.</summary>
		internal override void OnFocusEvent(FocusEvent fe){
			
			if(Caret!=null){
				return;
			}
			
			if(innerHTML==Placeholder_ && Placeholder_!=""){
				innerHTML="";
			}
			
			// Add a caret.
			Caret=Style.Computed.GetOrCreateVirtual(HtmlCaretElement.Priority,"caret") as HtmlCaretElement;
			
		}
		
		/// <summary>Called when the element is unfocused/blurred.</summary>
		internal override void OnBlurEvent(FocusEvent fe){
			if(Caret==null){
				return;
			}
			
			// Remove the caret:
			Style.Computed.RemoveVirtual(HtmlCaretElement.Priority);
			Caret=null;
			
			if(innerHTML=="" && Placeholder_!=""){
				innerHTML=Placeholder_;
			}
			
		}
		
		/// <summary>For text and password inputs, this relocates the caret to the given index.</summary>
		/// <param name="index">The character index to move the caret to, starting at 0.</param>
		public void MoveCaret(int index){
			MoveCaret(index,false);
		}
		
		/// <summary>For text and password inputs, this relocates the caret to the given index.</summary>
		/// <param name="index">The character index to move the caret to, starting at 0.</param>
		/// <param name="immediate">True if the caret should be moved right now.</param>
		public void MoveCaret(int index,bool immediate){
			
			if(Caret==null){
				return;
			}
			
			Caret.Move(index,immediate);
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Ensure it's focused:
			focus();
			
			// Get the text content:
			RenderableTextNode htn=TextHolder;
			
			if(htn==null){
				// Index is just 0.
				return;
			}
			
			// Get the letter index:
			int index=htn.LetterIndex(clickEvent.clientX,clickEvent.clientY);
			
			// Move the caret there (requesting a redraw):
			MoveCaret(index,true);
			
		}
		
	}
	
}