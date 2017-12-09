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
	/// Handles the quote tag.
	/// </summary>

	[Dom.TagName("q")]
	public class HtmlQuoteElement:HtmlElement{
		
		/// <summary>The cite attribute.</summary>
		public string cite{
			get{
				return getAttribute("cite");
			}
			set{
				setAttribute("cite", value);
			}
		}
		
	}
	
}