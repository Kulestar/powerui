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

using Css;
using Dom;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A caret element. Used internally by textarea's/ input elements.
	/// </summary>
	
	[Dom.TagName("caret")]
	public class HtmlCaretElement:HtmlElement{
		
		/// <summary>The virtual caret element draws after everything.</summary>
		public const int Priority=VirtualElements.AFTER_ZONE+10;
		
		/// <summary>Text index of the caret.</summary>
		public int Index;
		/// <summary>True if the caret should be relocated after the next update.</summary>
		public bool Locate=true;
		
		
		/// <summary>The host input element. Either a textarea or input.</summary>
		public HtmlElement Input{
			get{
				return parentElement as HtmlElement;
			}
		}
		
		/// <summary>The start of the selection, or the caret index.</summary>
		public ulong selectionStart{
			get{
				// Got a selection?
				Selection s=GetSelection();
				
				if(s==null){	
					return (ulong)Index;
				}
				
				// Anchor is the start:
				return (ulong)(s.anchorOffset);
			}
		}
		
		/// <summary>The end of the selection, or the caret index+1.</summary>
		public ulong selectionEnd{
			get{
				// Got a selection?
				Selection s=GetSelection();
				
				if(s==null){	
					return (ulong)(Index+1);
				}
				
				// Focus is the end:
				return (ulong)(s.focusOffset);
			}
		}
		
		/// <summary>Scrolls the input box if the given position is out of bounds.</summary>
		private void ScrollIfBeyond(ref Vector2 position){
			
			// Scroll input if the caret is beyond the end of the box:
			ComputedStyle inputCS=Input.Style.Computed;
			
			LayoutBox input=inputCS.FirstBox;
			
			float boxSize=input.InnerWidth;
			float scrollLeft=input.Scroll.Left;
			
			float scrolledPosition=position.x-scrollLeft;
			
			if(scrolledPosition>boxSize){
				
				// Beyond the right edge.
				// Scroll it such that the caret is positioned just on that right edge:
				scrollLeft+=(scrolledPosition-boxSize)+3f;
				
			}else if(scrolledPosition<0f){
				
				// Beyond the left edge.
				// Scroll it such that the caret is positioned just on that left edge:
				scrollLeft+=scrolledPosition;
				
			}
			
			// Clamp the scrolling:
			if(scrollLeft<0f){
				scrollLeft=0f;
			}
			
			// Update scroll left:
			if(scrollLeft!=input.Scroll.Left){
				
				// For this pass:
				input.Scroll.Left=scrollLeft;
				
				// For future passes:
				inputCS.ChangeTagProperty("scroll-left",new Css.Units.DecimalUnit(scrollLeft),false);
			}
			
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			// Locate the caret if we need to:
			if(Locate){
				Locate=false;
				
				RenderableTextNode htn=TextHolder;
				Vector2 position;
				
				if(htn==null){
					
					// Just at 0,0:
					position=Vector2.zero;
					
				}else{
					
					// Clip:
					if(Index>=htn.length){
						Index=htn.length;
					}
					
					// Get the position of the given letter:
					position=htn.GetPosition(Index);
				
				}
				
				// Scroll it if position is out of range:
				ScrollIfBeyond(ref position);
				
				// Set it in for this pass:
				box.Position.Top=position.y;
				box.Position.Left=position.x;
				
				// Write it out:
				Style.Computed.ChangeTagProperty("left",new Css.Units.DecimalUnit(position.x),false);
				Style.Computed.ChangeTagProperty("top",new Css.Units.DecimalUnit(position.y),false);
				
			}
			
		}
		
		/// <summary>Gets the selection of this caret. Null if none.</summary>
		public Selection GetSelection(){
			
			if(TextHolder==null){
				return null;
			}
			
			// Got a selection?
			Selection s=htmlDocument.getSelection();
			
			if(s==null || s.focusNode!=TextHolder){	
				return null;
			}
			
			// Selected the text.
			return s;
			
		}
		
		/// <summary>True if it successfully deleted a selection.</summary>
		public bool TryDeleteSelection(){
			
			// Got a selection?
			Selection s=GetSelection();
			
			if(s==null){	
				return false;
			}
			
			// Selected the text.
			
			// Delete it now:
			string value=Input.value;
			int start=s.anchorOffset;
			int end=s.focusOffset;
			
			if(end<start){
				int t=end;
				end=start;
				start=t;
			}
			
			value=value.Substring(0,start)+value.Substring(end,value.Length-end);
			
			// Apply now:
			Input.value=value;
			Move(start,false);
			
			// Remove all ranges:
			s.removeAllRanges();
			
			return true;
			
		}
		
		/// <summary>For text and password inputs, this relocates the caret to the given index.</summary>
		/// <param name="index">The character index to move the caret to, starting at 0.</param>
		/// <param name="immediate">True if the caret should be moved right now.</param>
		public void Move(int index,bool immediate){
			
			if(index<0){
				index=0;
			}
			
			if(index==Index){
				return;
			}
			
			Index=index;
			
			Locate=true;

			if(immediate){
				// We have enough info to place the caret already.
				// Request a layout.
				RequestLayout();
			}
			
			// Otherwise locating the caret is delayed until after the new value has been rendered.
			// This is used immediately after we set the value.
		}
		
		/// <summary>The container holding the text.</summary>
		public RenderableTextNode TextHolder{
			get{
				return Input.firstChild as RenderableTextNode;
			}
		}
		
	}
	
}