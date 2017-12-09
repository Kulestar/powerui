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


namespace PowerUI{
	
	/// <summary>
	/// Manages the currently focused element for this document.
	/// </summary>
	
	public partial class HtmlDocument{
		
		/// <summary>Current focused element casted to a HtmlElement (generally you should use activeElement instead).</summary>
		public HtmlElement htmlActiveElement{
			get{
				return activeElement as HtmlElement ;
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element above.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-up="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusUp(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element above:
			HtmlElement element=htmlActiveElement.GetFocusableAbove();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element below.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-down="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusDown(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element below:
			HtmlElement element=htmlActiveElement.GetFocusableBelow();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element to the left.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-left="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusLeft(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element to the left:
			HtmlElement element=htmlActiveElement.GetFocusableLeft();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will move focus to the nearest focusable element to the right.
		/// You can define 'focusable' on any element, or use a tag that is focusable anyway (input, textarea, a etc).
		/// You can also define focus-right="anElementID" to override which element will be focused next.</summary>
		public void MoveFocusRight(){
			if(htmlActiveElement==null){
				return;
			}
			
			// Grab the element to the right:
			HtmlElement element=htmlActiveElement.GetFocusableRight();
			
			if(element!=null){
				// Focus it:
				element.focus();
			}
		}
		
		/// <summary>If there is an element focused, this will click it (mouse down and up are triggered).</summary>
		public void ClickFocused(){
			
			HtmlElement focused=htmlActiveElement;
			
			if(focused==null){
				return;
			}
			
			// Click it:
			focused.click();
			
		}
		
		/// <summary>Moves the focus to the previous element as defined by tabindex.
		/// All elements with an explicit tabindex are defined as being before all
		/// elements which don't have an explicit tabindex.</summary>
		/// <returns>True if anything happened.</returns>
		public bool TabPrevious(){
			
			// These track the current best found element.
			int bestSoFar=int.MaxValue;
			HtmlElement best=null;
			
			// Get the current focused element:
			HtmlElement focused=htmlActiveElement;
			
			if(focused==null){
				
				// Haven't got one - hunt for a node with *no* tabindex first:
				body.SearchChildFocusable(null,false,-1,ref bestSoFar,ref best);
				
				if(best==null){
					
					// Find the last node with a tabIndex:
					body.SearchChildFocusable(null,false,int.MaxValue,ref bestSoFar,ref best);
					
				}
				
			}else{
				
				best=focused.GetFocusedPrevious();
				
			}
			
			if(best!=null){
				// Focus it now:
				best.focus();
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Moves the focus to the next element as defined by tabindex. 
		/// All elements with an explicit tabindex are defined as being before all
		/// elements which don't have an explicit tabindex.</summary>
		/// <returns>True if anything happened.</returns>
		public bool TabNext(){
			
			// - From the current focused element, we check all elements after it to see which
			//   has the 'closest' tabIndex to currentIndex.
			// - If there are no immediate matches, we wrap around the DOM and continue searching
			// - If there is no current focused element, we start at very top and don't wrap at all.
			
			// These track the current best found element.
			int bestSoFar=int.MaxValue;
			HtmlElement best=null;
			
			// Get the current focused element:
			HtmlElement focused=htmlActiveElement;
			
			if(focused==null){
				
				// Haven't got one - hunt for a node with a tabindex first:
				body.SearchChildFocusable(null,true,0,ref bestSoFar,ref best);
				
				if(best==null){
					
					// Find the first focusable node:
					body.SearchChildFocusable(null,true,-1,ref bestSoFar,ref best);
					
				}
				
			}else{
				
				best=focused.GetFocusedNext();
				
			}
			
			if(best!=null){
				// Focus it now:
				best.focus();
				
				return true;
			}
			
			return false;
		}
		
	}
	
}