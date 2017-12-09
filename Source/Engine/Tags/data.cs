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
	/// Handles the data tag.
	/// </summary>

	[Dom.TagName("data")]
	public class HtmlDataElement:HtmlElement{
		
		/// <summary>The value attribute.</summary>
		public string Value{
			get{
				return getAttribute("value");
			}
			set{
				setAttribute("value", value);
			}
		}
		
	}
	
}