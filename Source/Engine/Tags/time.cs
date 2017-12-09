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
	/// Handles the standard inline time element.
	/// </summary>
	
	[Dom.TagName("time")]
	public class HtmlTimeElement:HtmlElement{
		
		/// <summary>The datetime text, if any.</summary>
		public string datetime{
			get{
				return getAttribute("datetime");
			}
			set{
				setAttribute("datetime", value);
			}
		}
		
	}
	
}