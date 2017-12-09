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
	/// HTML mod element (used by del and ins).
	/// </summary>
	
	public class HtmlModElement:HtmlElement{
		
		/// <summary>The cite attribute.</summary>
		public string cite{
			get{
				return getAttribute("cite");
			}
			set{
				setAttribute("cite", value);
			}
		}
		
		/// <summary>The datetime attribute.</summary>
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