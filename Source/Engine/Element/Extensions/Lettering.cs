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
using Css;
using Dom;
using PowerUI;


namespace Dom{
	
	/// <summary>
	/// A useful function which splits all letters in an element into their own individual elements.
	/// Great for animating each letter on its own. Similar to lettering.js.
	/// </summary>

	public partial class Element{
		
		public void Lettering(){
			
			if(childNodes_==null){
				return;
			}
			
			// Cache child nodes:
			NodeList kids=childNodes_;
			
			// Empty:
			empty();
			
			int count=kids.length;
			
			for(int i=0;i<count;i++){
				
				// Grab the child node:
				Node child=kids[i];
				
				// Get as text:
				RenderableTextNode text=child as RenderableTextNode;
				
				if(text==null){
					
					// Direct add:
					appendChild(child);
					
					// Invoke lettering on it too:
					Element e=child as Element;
					
					if(e!=null){
						e.Lettering();
					}
					
					continue;
					
				}
				
				// Get the text:
				string characters=text.data;
				
				// How many chars?
				int characterCount=characters==null ? 0 : characters.Length;
				
				// If it's already a 'letter' just re-add it:
				if(child.parentNode!=null && child.parentNode.getAttribute("letter")=="1"){
					appendChild(text);
					continue;
				}
				
				// Add each letter as a new element:
				for(int c=0;c<characterCount;c++){
					
					// The character(s) as a string:
					string charString;
					
					// Grab the character:
					char character=characters[c];
					
					// Surrogate pair?
					if(char.IsHighSurrogate(character) && c!=characters.Length-1){
						
						// Low surrogate follows:
						char lowChar=characters[c+1];
						
						c++;
						
						// Get the charcode:
						int code=char.ConvertToUtf32(character,lowChar);
						
						// Turn it back into a string:
						charString=char.ConvertFromUtf32(code);
						
					}else{
						
						charString=""+character;
						
					}
					
					// Create a new span:
					Element span=document.createElement("span");
					span.setAttribute("letter", "1");
					
					if(charString==" "){
						// NBSP:
						span.textContent="\u00A0";
					}else{
						span.textContent=charString;
					}
					
					// Add it:
					appendChild(span);
					
				}
				
			}
			
		}
		
		/// <summary>True if this element is already suitably split up as letters.</summary>
		private bool IsLetters{
			get{
				// For each text node..
				foreach(Dom.TextNode text in allText){
					
					// Already got a letter?
					if(text.parentNode.getAttribute("letter")!="1"){
						return false;
					}
				}
				
				return true;
			}
		}
		
		/// <summary>Gets the number of letters.</summary>
		public int letterCount{
			get{
				int c=0;
				
				// For each text node..
				foreach(Dom.TextNode text in allText){
					c+=text.data.Length;
				}
				
				return c;
			}
		}
		
		/// <summary>All letters in this element and its child nodes.
		/// Automatically calls element.Lettering() if any text node has more than one letter.
		/// If you don't want that to happen, use allText instead.</summary>
		public IEnumerable<HtmlSpanElement> letters{
			get{
				
				if(!IsLetters){
					// Split it now:
					Lettering();
				}
				
				// This parts almost the same as allText, except we just return the parent:
				foreach(Node child in all){
					if(child is TextNode){
						yield return child.parentNode as HtmlSpanElement;
					}
				}
				
			}
		}
	
	}
	
}