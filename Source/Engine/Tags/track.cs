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
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{

	/// <summary>
	/// Represents HTML5 video tracks.
	/// </summary>

	[Dom.TagName("track")]
	public partial class HtmlTrackElement:HtmlElement{
		
		/// <summary>The kind attribute.</summary>
		public string kind{
			get{
				return getAttribute("kind");
			}
			set{
				setAttribute("kind", value);
			}
		}
		
		/// <summary>The src attribute.</summary>
		public string src{
			get{
				return getAttribute("src");
			}
			set{
				setAttribute("src", value);
			}
		}
		
		/// <summary>The srclang attribute.</summary>
		public string srclang{
			get{
				return getAttribute("srclang");
			}
			set{
				setAttribute("srclang", value);
			}
		}
		
		/// <summary>The label attribute.</summary>
		public string label{
			get{
				return getAttribute("label");
			}
			set{
				setAttribute("label", value);
			}
		}
		
		/// <summary>The default attribute.</summary>
		public bool Default{
			get{
				return GetBoolAttribute("default");
			}
			set{
				SetBoolAttribute("default",value);
			}
		}
		
		// NB: Fine for OnLexerAddNode to use the default.
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
	}
	
}