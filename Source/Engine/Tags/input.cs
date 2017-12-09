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
using Css;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents the input tag which handles various types of input on forms.
	/// Note that all input tags are expected to be on a form to work correctly.
	/// E.g. radio buttons won't work if they are not on a form.
	/// Supports the type, name, value and checked attributes.
	/// Also supports a 'content' attribute which accepts a value as html; great for buttons.
	/// </summary>
	
	[Dom.TagName("input")]
	public class HtmlInputElement:HtmlElement{
		
		/// <summary>Used by password inputs. True if this input's value is hidden.</summary> 
		public bool Hidden;
		/// <summary>The value text for this input.</summary>
		public string Value;
		/// <summary>The original value.</summary>
		public string defaultValue;
		/// <summary>For boolean (radio or checkbox) inputs, this is true if this one is checked.</summary>
		private bool Checked_;
		/// <summary>The default checked state. 1 is checked, 2 is not checked, 0 is not set yet.</summary>
		private int DefaultChecked_;
		/// <summary>For text or password input boxes, this is the caret.</summary>
		public HtmlCaretElement Caret;
		/// <summary>The type of input that this is; e.g. text/password/radio etc.</summary>
		public InputType Type=InputType.Undefined;
		/// <summary>The maximum length of text in this box.</summary>
		public int MaxLength=int.MaxValue;
		/// <summary>A placeholder value.</summary>
		private string Placeholder_="";
		
		/// <summary>The current location of the caret.</summary>
		public int CaretIndex{
			get{
				if(Caret==null){
					return 0;
				}
				
				return Caret.Index;
			}
		}
		
		/// <summary>The alt attribute.</summary>
		public string alt{
			get{
				return getAttribute("alt");
			}
			set{
				setAttribute("alt", value);
			}
		}
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return getAttribute("type");
			}
			set{
				setAttribute("type", value);
			}
		}
		
		/// <summary>The accept attribute.</summary>
		public string accept{
			get{
				return getAttribute("accept");
			}
			set{
				setAttribute("accept", value);
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
		
		/// <summary>Overrides the action of the host form.</summary>
		public string formAction{
			get{
				return getAttribute("formaction");
			}
			set{
				setAttribute("formaction", value);
			}
		}
		
		/// <summary>Overrides the enctype of the host form.</summary>
		public string formEncType{
			get{
				return getAttribute("formenctype");
			}
			set{
				setAttribute("formenctype", value);
			}
		}
		
		/// <summary>Overrides the method of the host form.</summary>
		public string formMethod{
			get{
				return getAttribute("formmethod");
			}
			set{
				setAttribute("formmethod", value);
			}
		}
		
		/// <summary>Overrides the validate of the host form.</summary>
		public bool formNoValidate{
			get{
				return GetBoolAttribute("formnovalidate");
			}
			set{
				SetBoolAttribute("formnovalidate",value);
			}
		}
		
		/// <summary>Overrides the method of the host form.</summary>
		public string formTarget{
			get{
				return getAttribute("formtarget");
			}
			set{
				setAttribute("formtarget", value);
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
		
		/// <summary>True if this element is disabled.</summary>
		public bool disabled{
			get{
				return GetBoolAttribute("disabled");
			}
			set{
				SetBoolAttribute("disabled",value);
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
		
		/// <summary>All labels targeting this select element.</summary>
		public NodeList labels{
			get{
				return HtmlLabelElement.FindAll(this);
			}
		}
		
		/// <summary>The height attribute.</summary>
		public string height{
			get{
				return getAttribute("height");
			}
			set{
				setAttribute("height", value);
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
		
		/// <summary>The src attribute.</summary>
		public string src{
			get{
				return getAttribute("src");
			}
			set{
				setAttribute("src", value);
			}
		}
		
		/// <summary>The width attribute.</summary>
		public string width{
			get{
				return getAttribute("width");
			}
			set{
				setAttribute("width", value);
			}
		}
		
		public override void OnFormReset(){
			if(Type==InputType.Checkbox || Type==InputType.Radio){
				Checked=defaultChecked;
			}else{
				SetValue(defaultValue);
			}
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
		
		/// <summary>The container holding the text.</summary>
		public RenderableTextNode TextHolder{
			get{
				return firstChild as RenderableTextNode;
			}
		}
		
		
		public HtmlInputElement(){
			// Make sure this tag is focusable:
			IsFocusable=true;
		}
		
		public override void OnTagLoaded(){
			
			if(Type==InputType.Undefined){
				// Specify as text:
				setAttribute("type", "text");
			}
			
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>Returns/sets the default state as specified in the doc.</summary>
		public bool defaultChecked{
			get{
				return DefaultChecked_==1;
			}
			set{
				DefaultChecked_=value?1:2;
			}
		}
		
		/// <summary>Gets or sets the checked state of this radio/checkbox input.
		/// Note that 'checked' is a C# keyword, thus the uppercase.
		/// Nitro is not case-sensitive (and JS lowercases it), so element.checked works fine there.</summary>
		public override bool Checked{
			get{
				return GetBoolAttribute("checked");
			}
			set{
				SetBoolAttribute("checked",value);
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
		
		/// <summary>Looks out for paste events.</summary>
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			// Handle locally:
			if(base.HandleLocalEvent(e,bubblePhase)){
				// It was blocked. Don't run the default.
				return true;
			}
			
			if(e is ClipboardEvent && IsTextInput() && e.type=="paste"){
				
				// Paste the data at the caret index (must be text only).
				string textToPaste=(e as ClipboardEvent).text;
				
				if(textToPaste!=null){
					
					string value=Value;
					
					if(value==null){
						value=""+textToPaste;
					}else{
						value=value.Substring(0,CaretIndex)+textToPaste+value.Substring(CaretIndex,value.Length-CaretIndex);
					}
					
					SetValue(value);
					MoveCaret(CaretIndex+textToPaste.Length);
					
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.ReconstructFormatting();
				
				lexer.Push(this,false);
				
				
				string type=getAttribute("type");
				
				if(type==null || type=="hidden"){
					
					lexer.FramesetOk=false;
					
				}
				
			}else if(mode==HtmlTreeMode.InTable){
				
				string type=getAttribute("type");
				
				if(type==null || type=="hidden"){
					
					// Go the anything else route.
					lexer.InTableElse(this,null);
					
				}else{
					
					// Add but don't push:
					lexer.Push(this,false);
					
				}
				
			}else if(mode==HtmlTreeMode.InSelect){
				
				lexer.InputOrTextareaInSelect(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="type"){
				string type=getAttribute("type");
				if(type==null){
					type="text";
				}
				
				if(type=="radio"){
					Type=InputType.Radio;
				}else if(type=="checkbox"){
					Type=InputType.Checkbox;
				}else if(type=="submit"){
					Type=InputType.Submit;
				}else if(type=="button"){
					Type=InputType.Button;
				}else if(type=="hidden"){
					Type=InputType.Hidden;
				}else if(type=="number"){
					Type=InputType.Number;
				}else if(type=="url"){
					Type=InputType.Url;
				}else if(type=="email"){
					Type=InputType.Email;
				}else{
					Type=InputType.Text;					
					Hidden=(type=="password");
				}
				
				return true;
			
			}else if(property=="placeholder"){
				
				Placeholder_=getAttribute("placeholder");
				innerHTML=Placeholder_;
				
				return true;
			
			}else if(property=="size"){
				
				// A rough font size based width:
				int size;
				if(int.TryParse(getAttribute("size"),out size)){
					
					// Don't apply if it's a button:
					if(Type!=InputType.Submit && Type!=InputType.Button){
						style.width=(size*10)+"px";
					}
					
				}
				
			}else if(property=="maxlength"){
				
				string value=getAttribute("maxlength");
				
				if(string.IsNullOrEmpty(value)){
					// It's blank - set it to the default.
					MaxLength=int.MaxValue;
				}else{
					// Parse the maximum length from the string:
					if(int.TryParse(value,out MaxLength)){
						
						if(MaxLength<0){
							MaxLength=int.MaxValue;
						}
						
						// Clip the value if we need to:
						if(Value!=null && Value.Length>MaxLength){
							SetValue(Value);
						}
					}else{
						// Not a number!
						MaxLength=int.MaxValue;
					}
				}
				
				return true;
			}else if(property=="checked"){
				
				// Get the checked state:
				string state=getAttribute("checked");
				
				// Awkwardly, null/ empty is checked.
				// 0 or false are not checked, anything else is!
				
				if( string.IsNullOrEmpty(state) ){
					
					Select();
					
					if(DefaultChecked_==0){
						// First time. Apply it:
						DefaultChecked_=1;
					}
					
				}else{
					state=state.ToLower().Trim();
					
					if(state=="0" || state=="false"){
						
						Unselect();
						
						if(DefaultChecked_==0){
							// First time. Apply it:
							DefaultChecked_=2;
						}
						
					}else{
						
						Select();
						
						if(DefaultChecked_==0){
							// First time. Apply it:
							DefaultChecked_=1;
						}
						
					}
					
				}
				
				// Refresh local selectors:
				Style.Computed.RefreshLocal(true);
				
				RequestLayout();
				return true;
			}else if(property=="value"){
				SetValue(getAttribute("value"));
				return true;
			}else if(property=="content"){
				SetValue(getAttribute("content"),true);
				return true;
			}
			return false;
		}
		
		public override KeyboardMode OnShowMobileKeyboard(){
			if(!IsTextInput()){
				return null;
			}
			
			KeyboardMode result=new KeyboardMode();
			result.Secret=Hidden;
			
			#if MOBILE
			
			if(Type==InputType.Number){
				
				// Number keyboard:
				result.Type=TouchScreenKeyboardType.NumbersAndPunctuation;
				
			}else if(Type==InputType.Url){
				
				// Url keyboard:
				result.Type=TouchScreenKeyboardType.URL;
			
			}else if(Type==InputType.Email){
				
				// Email keyboard:
				result.Type=TouchScreenKeyboardType.EmailAddress;
				
			}
			
			#endif
			
			return result;
		}
		
		/// <summary>Used by boolean inputs (checkbox/radio). Unselects this from being the active one.</summary>
		public void Unselect(){
			if(!Checked_){
				return;
			}
			
			Checked_=false;
			
			// Clear checked:
			setAttribute("checked", "0");
			
			if(Type==InputType.Checkbox){
				SetValue(null);
			}
			
		}
		
		/// <summary>Used by boolean inputs (checkbox/radio). Selects this as the active one.</summary>
		public void Select(){
			if(Checked_){
				return;
			}
			Checked_=true;
			
			// Set checked:
			setAttribute("checked", "1");
			
			if(Type==InputType.Radio){
				// Firstly, unselect all other radio elements with this same name:
				string name=getAttribute("name");
				
				HTMLCollection allInputs;
				
				if(form==null){
					
					// Find all inputs with the same name:
					allInputs=document.getElementsByTagName("input");
					
				}else{
					
					allInputs=form.elements;
					
				}
				
				if(allInputs!=null){
					
					foreach(Element element in allInputs){
						
						if(element==this){
							// Skip this element
							continue;
						}
						
						if(element.getAttribute("type")=="radio" && element.getAttribute("name")==name){
							// Yep; unselect it.
							((HtmlInputElement)element).Unselect();
						}
						
					}
					
				}
				
				Dom.Event e=new Dom.Event("change");
				e.SetTrusted(true);
				dispatchEvent(e);
				
			}else if(Type==InputType.Checkbox){
				SetValue("1");
			}
			
		}
		
		/// <summary>Sets the value of this input box.</summary>
		/// <param name="value">The value to set.</param>
		public void SetValue(string value){
			SetValue(value,false);
		}
		
		/// <summary>Sets the value of this input box, optionally as a html string.</summary>
		/// <param name="value">The value to set.</param>
		/// <param name="html">True if the value can safely contain html.</param>
		public void SetValue(string value,bool html){
			
			// Trigger onchange:
			Dom.Event e=new Dom.Event("change");
			e.SetTrusted(false);
			if(!dispatchEvent(e)){
				return;
			}
			
			if(MaxLength!=int.MaxValue){
				// Do we need to clip it?
				if(value!=null && value.Length>MaxLength){
					// Yep!
					value=value.Substring(0,MaxLength);
				}
			}
			
			if(value==null || CaretIndex>value.Length){
				MoveCaret(0);
			}
			
			if(Value==null){
				defaultValue=value;
			}
			
			// Update the value:
			setAttribute("value", Value=value);
			
			if(!IsBoolInput()){
				if(Hidden){
					// Unfortunately the new string(char,length); constructor isn't reliable.
					// Build the string manually here.
					StringBuilder sb=new StringBuilder("",value.Length);
					for(int i=0;i<value.Length;i++){
						sb.Append('*');
					}
					
					if(html){
						innerHTML=sb.ToString();
					}else{
						textContent=sb.ToString();
					}
				}else{
					if(html){
						innerHTML=value;
					}else{
						textContent=value;
					}
				}
			}
			
		}
		
		/// <summary>Checks if this is a radio or checkbox input box.</summary>
		/// <returns>True if it is; false otherwise.</returns>
		private bool IsBoolInput(){
			return (Type==InputType.Radio||Type==InputType.Checkbox);
		}
		
		/// <summary>Checks if this is a text, number or password input box.</summary>
		/// <returns>True if it is; false otherwise.</returns>
		private bool IsTextInput(){
			return (Type==InputType.Text || Type==InputType.Number || Type==InputType.Url || Type==InputType.Email);
		}
		
		public override void OnKeyPress(KeyboardEvent pressEvent){
			
			if(readOnly){
				return;
			}
			
			if(Type==InputType.Number){
				
				if( !char.IsNumber(pressEvent.character) && pressEvent.character!='.' && !char.IsControl(pressEvent.character) ){
					
					// Not a number, point or control character. Block it:
					return;
					
				}
				
			}
			
			if(pressEvent.heldDown){
				if(IsTextInput()){
					// Add to value if pwd/text, unless it's backspace:
					string value=Value;
					
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
					
					// Grab the keycode:
					KeyCode key=pressEvent.unityKeyCode;
					
					if(key==KeyCode.LeftArrow){
						MoveCaret(CaretIndex-1,true);
					}else if(key==KeyCode.RightArrow){
						MoveCaret(CaretIndex+1,true);
					}else if(key==KeyCode.Backspace){
						// Delete the character before the caret.
						
						// Got a selection?
						if(Caret!=null && Caret.TryDeleteSelection()){
							return;
						}
						
						if(string.IsNullOrEmpty(value)||CaretIndex==0){
							return;
						}
						value=value.Substring(0,CaretIndex-1)+value.Substring(CaretIndex,value.Length-CaretIndex);
						int index=CaretIndex;
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
						
						value=value.Substring(0,CaretIndex)+value.Substring(CaretIndex+1,value.Length-CaretIndex-1);
						SetValue(value);
					}else if(key==KeyCode.Return || key==KeyCode.KeypadEnter){
						// Does the form have a submit button? If so, submit now.
						// Also call a convenience (non-standard) "onenter" method.
						
						HtmlFormElement f=form;
						if(f!=null){
							
							// Get the first submit button:
							HtmlElement submitButton=f.GetSubmitButton();
							
							if(submitButton!=null){
								// Submit it now:
								f.submit(submitButton);
							}
							
						}
						
						return;
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
						
					}
					
				}else if(Type==InputType.Submit){
					
					// Submit button.
					if(!char.IsControl(pressEvent.character) && pressEvent.character!='\0'){
						return;
					}
					
					// Grab the keycode:
					KeyCode key=pressEvent.unityKeyCode;
					
					if(key==KeyCode.Return || key==KeyCode.KeypadEnter){
						
						// Find the form and then attempt to submit it.
						HtmlFormElement f=form;
						
						if(f!=null){
							f.submit(this);
						}
						
					}
					
				}
				
			}
			
			base.OnKeyPress(pressEvent);
			
		}
		
		/// <summary>Called when the element is focused.</summary>
		internal override void OnFocusEvent(FocusEvent fe){
			if(!IsTextInput()||Caret!=null){
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
			
			// Focus it:
			focus();
			
			if(Type==InputType.Submit){
				// Find the form and then attempt to submit it.
				HtmlFormElement f=form;
				if(f!=null){
					f.submit(this);
				}
				
			}else if(Type==InputType.Radio){
				Select();
			}else if(Type==InputType.Checkbox){
				
				if(Checked_){
					Unselect();
				}else{
					Select();
				}
				
			}else if(IsTextInput()){
				
				// Move the caret to the clicked point.
				
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
	
	
	public partial class HtmlElement{
		
		
		/// <summary>Gets or sets the value of this element. Input/Select/Textarea elements only.</summary>
		public virtual string value{
			get{
				return getAttribute("value");
			}
			set{
				setAttribute("value", value);
			}
		}
		
		/// <summary>Gets or sets the value as html for this element. Input/Select elements only.</summary>
		public virtual string htmlValue{
			get{
				return getAttribute("content");
			}
			set{
				setAttribute("content", value);
			}
		}
		
	}
	
}