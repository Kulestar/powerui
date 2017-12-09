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

using Dom;


namespace PowerUI{

	/// <summary>
	/// Handles the datalist tag.
	/// </summary>

	[Dom.TagName("datalist")]
	public class HtmlDataListElement:HtmlElement{
		
		/// <summary>The set of options.</summary>
		public HTMLCollection options{
			get{
				HTMLCollection hoc=new HTMLCollection();
				HtmlSelectElement.CollectOptions(this,hoc);
				return hoc;
			}
		}
		
	}
	
}