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

namespace PowerUI{
	
	/// <summary>
	/// Handles the scroll right/down up/left buttons on scrollbars.
	/// </summary>
	
	[Dom.TagName("scrollbutton")]
	public class HtmlScrollButtonElement:HtmlElement{
		
		/// <summary>True if this is the starting end (up/left).</summary>
		public bool IsStart;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override void OnChildrenLoaded(){
			
			IsStart=(this==parentNode.firstChild);
			
			// Set attribs for CSS:
			setAttribute("part", IsStart ? "start" : "end");
			
			HtmlScrollbarElement bar=parentElement as HtmlScrollbarElement;
			
			string type;
			
			if(bar.IsVertical){
				
				type=IsStart?"up":"down";
				
			}else{
				
				type=IsStart?"left":"right";
				
			}
			
			setAttribute("orient",type);
			
		}
		
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(base.HandleLocalEvent(e,bubblePhase)){
				// It was blocked. Don't run the default.
				return true;
			}
			
			if(e.type=="mousedown"){
				
				// Get the scroll bar:
				HtmlScrollbarElement scroll=parentElement as HtmlScrollbarElement;
				
				// And scroll it:
				scroll.ScrollBy(IsStart?-1:1);
				
			}
			
			return false;
		}
		
	}
	
}