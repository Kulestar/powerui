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
	/// Represents a html form which lets you collect information from the player.
	/// For those new to html, see input and select tags.
	/// Supports onsubmit="nitroMethodName" and the action attributes.
	/// </summary>
	
	[Dom.TagName("form")]
	public class HtmlFormElement:HtmlElement{
		
		/// <summary>The url to post the form to.</summary>
		public string Action;
		
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
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
		
		/// <summary>The method attribute.</summary>
		public string method{
			get{
				return getAttribute("method");
			}
			set{
				setAttribute("method", value);
			}
		}
		
		/// <summary>The target attribute.</summary>
		public string target{
			get{
				return getAttribute("target");
			}
			set{
				setAttribute("target", value);
			}
		}
		
		/// <summary>The action attribute.</summary>
		public string action{
			get{
				return getAttribute("action");
			}
			set{
				setAttribute("action", value);
			}
		}
		
		/// <summary>The encoding attribute.</summary>
		public string encoding{
			get{
				return getAttribute("encoding");
			}
			set{
				setAttribute("encoding", value);
			}
		}
		
		/// <summary>The enctype attribute.</summary>
		public string enctype{
			get{
				return getAttribute("enctype");
			}
			set{
				setAttribute("enctype", value);
			}
		}
		
		/// <summary>The accept-charset attribute.</summary>
		public string acceptCharset{
			get{
				return getAttribute("accept-charset");
			}
			set{
				setAttribute("accept-charset", value);
			}
		}
		
		/// <summary>The autocomplete attribute.</summary>
		public string autocomplete{
			get{
				return getAttribute("autocomplete");
			}
			set{
				setAttribute("autocomplete", value);
			}
		}
		
		/// <summary>The novalidate attribute.</summary>
		public bool noValidate{
			get{
				return GetBoolAttribute("novalidate");
			}
			set{
				SetBoolAttribute("novalidate",value);
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InTable){
				
				if(lexer.TagCurrentlyOpen("template") || lexer.form!=null){
					
					// Ignore it.
					
				}else{
					
					// Add but don't push:
					lexer.Push(this,false);
					lexer.form=this;
					
				}
				
			}else if(mode==HtmlTreeMode.InBody){
				
				bool openTemplate=lexer.TagCurrentlyOpen("template");
				
				if(lexer.form!=null && !openTemplate){
					
					// Parse error - ignore the token.
					
				}else{
					
					lexer.CloseParagraphButtonScope();
					
					// Add and set form:
					lexer.Push(this,true);
					
					if(!openTemplate){
						lexer.form=this;
					}
					
				}
				
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
				
				if(lexer.TagCurrentlyOpen("template")){
					
					// Template in scope.
					lexer.GenerateImpliedEndTags();
					
					lexer.CloseInclusive("form");
					
				}else if(lexer.IsInScope("form")){
					
					// No template - ordinary form.
					Element node=lexer.form;
					lexer.form=null;
					
					if(node!=null && lexer.IsInScope("form")){
						
						lexer.GenerateImpliedEndTags();
						
						if(node==lexer.CurrentElement){
							
							lexer.CloseCurrentNode();
							
						}else{
							
							// Fatal parse error.
							throw new DOMException(DOMException.SYNTAX_ERR, (ushort)HtmlParseError.FormClosedWrong);
							
						}
						
					}
					
				}
				
				// Ignore otherwise
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="onsubmit"){
			}else if(property=="action"){
				Action=getAttribute("action");
			}else{
				return false;
			}
			
			return true;
		}
		
		/// <summary>Gets all input elements contained within this form.</summary>
		/// <param name='mode'>The form search options.</summary>
		/// <returns>A list of all input elements.</returns>
		public HTMLFormControlsCollection GetAllInputs(InputSearchMode mode){
			HTMLFormControlsCollection results=new HTMLFormControlsCollection();
			GetAllInputs(results,this,mode);
			return results;
		}
		
		/// <summary>Gets all inputs from the given element, adding the results to the given list.</summary>
		/// <param name="results">The list that all results are added to.</param>
		/// <param name="element">The element to check.</param>
		public static void GetAllInputs(INodeList results,HtmlElement element,InputSearchMode mode){
			
			NodeList kids=element.childNodes_;
			
			if(kids==null){
				return;
			}
			
			for(int i=0;i<kids.length;i++){
				HtmlElement child=kids[i] as HtmlElement;
				
				if(child==null || child.Tag=="form"){
					// Don't go into child forms.
					continue;
				}
				
				if(
					(mode==InputSearchMode.Submittable && child.IsFormSubmittable) || 
					(mode==InputSearchMode.Listed && child.IsFormListed) ||
					(mode==InputSearchMode.Resettable && child.IsFormResettable)
				){
					results.push(child);
				}else{
					GetAllInputs(results,child,mode);
				}
				
			}
		}
		
		/// <summary>Number of controls in the form.</summary>
		public long length{
			get{
				return elements.length;
			}
		}
		
		/// <summary>Gets all input elements contained within this form.</summary>
		public HTMLFormControlsCollection elements{
			get{
				return GetAllInputs(InputSearchMode.Listed);
			}
		}
		
		/// <summary>Gets the selected input by the given name attribute.
		/// E.g. there may be more than one input element (such as with radios); this is the active one.</summary>
		public HtmlElement getField(string name){
			
			NodeList allWithName=getElementsByAttribute("name",name);
			if(allWithName.length==0){
				return null;
			}
			
			HtmlInputElement tag=allWithName[0] as HtmlInputElement;
			
			if(allWithName.length==1){
				return tag;
			}
			
			// We have more than one. If it's a radio, return the one which is selected.
			// Otherwise, return the last one.
			
			if(tag==null){
				return null;
			}
			
			if(tag.Type==InputType.Radio){
				
				// Which is selected?
				foreach(HtmlElement radio in allWithName){
					if(((HtmlInputElement)radio).Checked){
						return radio;
					}
				}
			}
			
			return allWithName[allWithName.length-1] as HtmlElement ;
		}
		
		/// <summary>True if this form has a submit button within it.</summary>
		public bool HasSubmitButton{
			get{
				return GetSubmitButton()!=null;
			}
		}
		
		/// <summary>Gets a submit button. Null if none were found.</summary>
		public HtmlElement GetSubmitButton(){
		
			// Get all inputs:
			HTMLCollection allInputs=getElementsByTagName("input");
			
			// Are any a submit?
			foreach(Element element in allInputs){
				if(element.getAttribute("type")=="submit"){
					return element as HtmlElement;
				}
			}
			
			return null;
		}
		
		/// <summary>Resets this form.</summary>
		public void reset(){
			
			// Dispatch the reset event:
			FormEvent de=new FormEvent("reset");
			de.SetTrusted(true);
			// Hook up the form element:
			de.form=this;
			
			if(dispatchEvent(de)){
				
				// Get resettable elements:
				HTMLFormControlsCollection allReset=GetAllInputs(InputSearchMode.Submittable);
				
				foreach(Element e in allReset){
					HtmlElement he=(e as HtmlElement);
					
					if(he!=null){
						he.OnFormReset();
					}
				}
				
			}
			
		}
		
		/// <summary>Submits this form.</summary>
		public override void submit(){
			submit(null);
		}
		
		/// <summary>Gets an attribute value which may be overriden in the given element.</summary>
		private string GetOverriden(string name,HtmlElement button){
			
			// Get from this element:
			string current=getAttribute(name);
			
			// Got an override?
			if(button!=null){
				string overriden=button.getAttribute("form"+name);
				
				if(overriden!=null){
					return overriden;
				}
			}
			
			return current;
			
		}
		
		/// <summary>Submits this form using the given button. It may override the action etc.</summary>
		public void submit(HtmlElement clickedButton){
			
			// Generate a nice dictionary of the form contents.
			
			// Step 1: find the unique names of the elements:
			Dictionary<string,string> uniqueValues=new Dictionary<string,string>();
			
			// Get submittable elements:
			HTMLFormControlsCollection allInputs=GetAllInputs(InputSearchMode.Submittable);
			
			foreach(Element element in allInputs){
				
				if(element is HtmlButtonElement){
					// No buttons.
					continue;
				}
				
				string type=element.getAttribute("type");
				if(type=="submit"){
					// No submit either.
					continue;
				}
				
				string name=element.getAttribute("name");
				if(name==null){
					name="";
				}
				
				// Step 2: For each one, get their value.
				// We might have a name repeated, in which case we check if they are radio boxes.
				
				if(uniqueValues.ContainsKey(name)){
					// Ok the element is already added - we have two with the same name.
					// If they are radio, then only overwrite value if the new addition is checked.
					// Otherwise, overwrite - furthest down takes priority.
					if(element.Tag=="input"){
						
						HtmlInputElement tag=(HtmlInputElement)element;
						
						if(tag.Type==InputType.Radio&&!tag.Checked){
							// Don't overwrite.
							continue;
						}
					}
				}
				string value=(element as HtmlElement ).value;
				if(value==null){
					value="";
				}
				uniqueValues[name]=value;
			}
			
			// Get the action:
			string action=GetOverriden("action",clickedButton);
			
			FormEvent formData=new FormEvent(uniqueValues);
			formData.SetTrusted(true);
			formData.EventType="submit";
			// Hook up the form element:
			formData.form=this;
			
			if( dispatchEvent(formData) ){
				
				// Get ready to post now!
				DataPackage package=new DataPackage(action,document.basepath);
				package.AttachForm(formData.ToUnityForm());
				
				// Apply request to the data:
				formData.request=package;
				
				// Apply load event:
				package.onload=package.onerror=delegate(UIEvent e){
					
					// Attempt to run ondone (doesn't bubble):
					formData.Reset();
					formData.EventType="done";
					
					if(dispatchEvent(formData)){
						// Otherwise the ondone function quit the event.
						
						// Load the result into target now.
						string target=GetOverriden("target",clickedButton);
						
						HtmlDocument targetDocument=ResolveTarget(target);
						
						if(targetDocument==null){
							// Posting a form to an external target.
							
							Log.Add("Warning: Unity cannot post forms to external targets. It will be loaded a second time.");
							
							// Open the URL outside of Unity:
							UnityEngine.Application.OpenURL(package.location.absoluteNoHash);
							
						}else{
							
							// Change the location:
							targetDocument.SetRawLocation(package.location);
							
							// History entry required:
							targetDocument.window.history.DocumentNavigated();
							
							// Apply document content:
							targetDocument.GotDocumentContent(package.responseText,package.statusCode,true);
							
						}
						
					}
					
				};
				
				// Send now!
				package.send();
				
			}
		}
		
	}
	
	/// <summary>
	/// Options when searching for input elements.
	/// </summary>
	public enum InputSearchMode{
		Submittable,
		Listed,
		Resettable
	}
	
	public partial class HtmlElement{
		
		/// <summary>Called when the parent form is reset.</summary>
		public virtual void OnFormReset(){}
		
		/// <summary>Submits the form this element is in.</summary>
		public virtual void submit(){
		}
		
		/// <summary>Scans up the DOM to find the parent form element.
		/// Note: <see cref="PowerUI.HtmlElement.form"/> may be more useful than the element iself.</summary>
		public HtmlFormElement formElement{
			get{
				return form;
			}
		}
		
		/// <summary>Scans up the DOM to find the parent form element's handler.
		/// The object returned provides useful methods such as <see cref="PowerUI.HtmlFormElement.submit"/>. </summary>
		public HtmlFormElement form{
			get{
				return GetParentByTagName("form") as HtmlFormElement;
			}
		}
		
	}
	
}