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
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	
	public partial class HtmlElement{
		
		
		/// <summary>Called when a 'click' (down and up on the same element) occurs.</summary>
		/// <param name="clickEvent">The event that represents the click.</param>
		/// <returns>True if it accepted the click.</returns>
		public virtual void OnClickEvent(MouseEvent clickEvent){
		}
		
		/// <summary>True if the mouse is over this element.</summary>
		public bool IsMousedOver(){
			
			// The element must be one of the active pointed ones:
			for(int i=0;i<InputPointer.PointerCount;i++){
				
				if(InputPointer.AllRaw[i].ActiveOverTarget==this){
					// Great, got it!
					return true;
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>Was the mouse clicked on this element during the last mouse down?</summary>
		public bool WasPressed(){
			
			// The element must be one of the active pointed ones:
			for(int i=0;i<InputPointer.PointerCount;i++){
				
				if(InputPointer.AllRaw[i].ActivePressedTarget==this){
					// Great, got it!
					return true;
				}
				
			}
			
			return false;
		}
		
		/// <summary>Clicks this element (mousedown and a mouseup).</summary>
		public void click(){
			
			// Coords:
			int midX=(int)style.Computed.GetMidpointX();
			int midY=(int)style.Computed.GetMidpointY();
			
			// Left click (which is not trusted):
			MouseEvent me=new MouseEvent(midX,midY,0,false);
			me.bubbles=true;
			me.EventType="click";
			
			// Trigger:
			dispatchEvent(me);
			
		}
		
	}
	
}