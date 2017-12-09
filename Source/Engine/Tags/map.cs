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
	/// Represents a map element.
	/// </summary>
	
	[Dom.TagName("map")]
	public class HtmlMapElement:HtmlElement{
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		/// <summary>The areas associated with this map.</summary>
		public HTMLCollection areas{
			get{
				return getElementsByTagName("area");
			}
		}
		
		/// <summary>The images associated with this map.</summary>
		public NodeList images{
			get{
				// Get all elements with usemap="#name":
				return document.getElementsByAttribute("usemap","#"+name);
			}
		}
		
	}
	
}