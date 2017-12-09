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
using Css;


namespace PowerUI{
	
	/// <summary>
	/// A base class for all html tag types (e.g. script, a, body etc).
	/// These tag handlers tell the UI how to render and work with this type of tag.
	/// Tag handlers are stored globally for lookup and instanced per element.
	/// </summary>
	
	public partial class HtmlElement{
		
		/// <summary>True if this tag is focusable.</summary>
		public bool IsFocusable;
		
		
		/// <summary>Requests the renderer handling this element to layout next update.</summary>
		public void RequestLayout(){
			htmlDocument.RequestLayout();
		}
		
		/// <summary>Tells the parser to not include this element in the DOM.</summary>
		/// <returns>True if this tag should be dumped and not enter the DOM once fully loaded.</returns>
		public virtual bool Junk(){
			return false;
		}
		
		/// <summary>Called when all variable (&Variable;) values must have their content reloaded.</summary>
		public virtual void OnResetAllVariables(){}
		
		/// <summary>Called when the host element is removed from the DOM.</summary>
		public virtual void OnRemovedFromDOM(){}
		
		/// <summary>Called when the named variable variable (&name;) values must have its content reloaded.</summary>
		public virtual void OnResetVariable(string name){}
		
		/// <summary>Called during the box compute process. Useful if your element has clever dimensions, such as the img tag or words.</summary>
		public virtual void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
		}
		
		/// <summary>Called during a global layout event on all elements.</summary>
		public virtual void OnRender(Renderman renderer){}
		
		/// <summary>Called when this element comes into focus.</summary>
		internal virtual void OnFocusEvent(FocusEvent fe){}
		
		/// <summary>Called when this element becomes unfocused.</summary>
		internal virtual void OnBlurEvent(FocusEvent fe){}
		
		/// <summary>Called when PowerUI attempts to display the mobile keyboard (only called on mobile platforms).</summary>
		/// <returns>A KeyboardType if this element wants the keyboard to show up (KeyboardType.None otherwise).</returns>
		public virtual KeyboardMode OnShowMobileKeyboard(){
			return null;
		}
		
		/// <summary>Called when an attribute of the element was changed.
		/// Returns true if the method handled the change to prevent unnecessary checks.</summary>
		public override bool OnAttributeChange(string property){
			
			if(property=="style"){
				Style.cssText=getAttribute("style");
				return true;
			}
			
			// Style refresh:
			if(Style.Computed.FirstMatch!=null){
				// This is a runtime attribute change.
				// We must consider if it's affecting the style or not:
				Style.Computed.AttributeChanged(property);
			}
			
			if(property=="id"){
				return true;
			}else if(property=="class"){
				return true;
			}else if(property=="name"){
				// Nothing happens with this one - ignore it.
				return true;
			}else if(property=="onmousedown"){
				return true;
			}else if(property=="onmouseup"){
				return true;
			}else if(property=="onkeydown"){
				return true;
			}else if(property=="onkeyup"){
				return true;
			}else if(property=="height"){
				string height=getAttribute("height");
				if(height.IndexOf("%")==-1 && height.IndexOf("px")==-1 && height.IndexOf("em")==-1){
					height+="px";
				}
				style.height=height;
				return true;
			}else if(property=="width"){	
				string width=getAttribute("width");
				if(width.IndexOf("%")==-1 && width.IndexOf("px")==-1 && width.IndexOf("em")==-1){
					width+="px";
				}
				style.width=width;
				return true;
			}else if(property=="align"){
				
				string align=getAttribute("align").ToLower();
				
				if(align=="center"){
					align="-moz-center";
				}
				
				style.textAlign=align;
				return true;
			}else if(property=="valign"){
				style.verticalAlign=getAttribute("valign");
				return true;
			}
			
			return false;
		}
		
	}
	
}