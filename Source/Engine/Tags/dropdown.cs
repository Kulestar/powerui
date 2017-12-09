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


namespace PowerUI{
	
	/// <summary>
	/// Dropdown boxes used by select. It's a virtual element (of the HTML node) and can be targeted with CSS selectors.
	/// Its childNode set is the same object as the childNode set from the select element.
	/// </summary>
	
	[Dom.TagName("dropdown")]
	public class HtmlDropdownElement:HtmlElement{
		
		public const int Priority=Css.VirtualElements.DURING_ZONE+100;
		
	}
	
}