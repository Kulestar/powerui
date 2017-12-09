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

namespace PowerUI{
	
	/// <summary>
	/// A scrollbar element. Used internally but you can hook them up to other things if you wish using the target='id' attribute
	/// or alternatively use target='_blank' and listen for the onchange event.
	/// </summary>
	
	[Dom.TagName("scrollbar")]
	public class HtmlScrollbarElement:HtmlElement{
		
		/// <summary>True if this bar is hidden.</summary>
		internal bool Hidden;
		/// <summary>0 means unset, 1 is horizontal, 2 is vertical.</summary>
		private int OrientState=0;
		/// <summary>A custom targeted ID.</summary>
		private string TargetID;
		/// <summary>Is this a vertical scrollbar?</summary>
		public bool IsVertical{
			get{
				return OrientState==2;
			}
		}
		/// <summary>A custom target.</summary>
		public HtmlElement Target;
		/// <summary>The tag handler for the scrollbars thumb.</summary>
		public HtmlScrollThumbElement Thumb;
		
		
		public override void OnChildrenLoaded(){
			
			if(childNodes_!=null){
				// Setting innerHTML calls OnChildrenLoaded again.
				// Block that!
				return;
			}
			
			if(OrientState<=0){
				// Not set orient yet.
				OrientState=-1;
				return;
			}
			
			// Set innerHTML:
			innerHTML="<scrollbutton/><scrollthumb/><scrollbutton/>";
			
			// Recalc thumb:
			RecalculateThumb();
			
		}
		
		/// <summary>Scrolls a scrollbar by the given number of pixels.
		/// This may fail if it's already at an end of the bar and can't move any further.</summary>
		/// <param name="pixels">The number of pixels the scrollbar tab will try to move by.</param>
		public void ScrollBy(int pixels){
			if(Thumb!=null){
				Thumb.ScrollBy(pixels);
			}
		}
		
		/// <summary>Scrolls a scrollbar to the given location in pixels.</summary>
		/// <param name="location">The number of pixels the scrollbar tab will try to locate at.</param>
		public void ScrollTo(int location){
			if(Thumb!=null){
				Thumb.ScrollTo(location,true);
			}
		}
		
		/// <summary>Used only by scrollbars. Gets the target element to be scrolled.</summary>
		/// <returns>The target element.</returns>
		public HtmlElement scrollTarget{
			get{
				if(Target==null){
					
					if(TargetID==null){
						Target=parentElement as HtmlElement;
					}else if(TargetID!="_blank"){
						Target=document.getElementById(TargetID) as HtmlElement;
					}
					
				}
				
				return Target;
			}
		}
		
		/// <summary>Scrolls a scrollbar to the given 0-1 location along the bar.</summary>
		/// <param name="position">The 0-1 location along the bar that the tab will locate at.</param>
		public void ScrollTo(float position){
			if(Thumb!=null){
				Thumb.ScrollToPoint(position,true);
			}
		}
		
		/// <summary>Called when ScrollTop or ScrollLeft has changed.</summary>
		public void ElementScrolled(){
			
			if(Thumb!=null){
				
				HtmlElement target=scrollTarget;
				
				if(target==null){
					return;
				}
				
				ComputedStyle cs=target.style.Computed;
				
				float barProgress=0f;
				
				if(IsVertical){
					barProgress=(float)cs.ScrollTop / (float)cs.ContentHeight;
				}else{
					barProgress=(float)cs.ScrollLeft / (float)cs.ContentWidth;
				}
				
				if(barProgress<0f){
					barProgress=0f;
				}else if(barProgress>1f){
					barProgress=1f;
				}
				
				Thumb.ElementScrolled(barProgress);
			}
			
		}
		
		public override bool OnAttributeChange(string property){
			
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="orient"){
				
				// Sometimes this happens after OnChildrenLoaded.
				// We use OrientState to check for that
				// (it'll be -1 if OnChildrenLoaded ran before this did).
				
				int os=OrientState;
				
				if(getAttribute("orient")=="vertical"){
					OrientState=2;
				}else{
					OrientState=1;
				}
				
				if(os==-1){
					// Try on children loaded again:
					OnChildrenLoaded();
				}
				
				return true;
			
			}else if(property=="target"){
				
				TargetID=getAttribute("target");
				Target=null;
				
			}
			
			return false;
		}
		
		/// <summary>Recalculates the thumb size of a scroll bar.</summary>
		public void RecalculateThumb(){
			
			if(Thumb==null){
				if(childNodes_.length>1){
					Node thumb=childNodes_[1];
					
					if(thumb!=null){
						Thumb=((HtmlScrollThumbElement)thumb);
					}
				}
			}
			
			if(Thumb!=null){
				Thumb.RecalculateNext=true;
			}
			
		}
		
	}
	
}