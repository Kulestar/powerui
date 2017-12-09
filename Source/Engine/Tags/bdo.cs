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

namespace PowerUI{
	
	/// <summary>
	/// Represents a HTML5 Bi-direction override element.
	/// </summary>
	
	[Dom.TagName("bdo")]
	public class HtmlBdoElement:HtmlElement{
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="dir"){
				// Grab the direction:
				string direction=getAttribute("dir");
				
				// Grab the computed style:
				ComputedStyle computed=Style.Computed;
				
				// Apply it to CSS - it's exactly the same value for the direction CSS property:
				computed.ChangeTagProperty("direction",direction);
				
				return true;
			}
			
			
			return false;
		}
	}
	
}