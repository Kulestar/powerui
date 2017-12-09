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
	/// Represents a keygen element.
	/// </summary>
	
	[Dom.TagName("keygen")]
	public class HtmlKeygenElement:HtmlElement{
		
		/// <summary>True if this element can be autofocused.</summary>
		public bool autofocus{
			get{
				return GetBoolAttribute("autofocus");
			}
			set{
				SetBoolAttribute("autofocus",value);
			}
		}
		
		/// <summary>The challenge attribute.</summary>
		public string challenge{
			get{
				return getAttribute("challenge");
			}
			set{
				setAttribute("challenge", value);
			}
		}
		
		/// <summary>True if this element is disabled.</summary>
		public bool disabled{
			get{
				return GetBoolAttribute("disabled");
			}
			set{
				SetBoolAttribute("disabled",value);
			}
		}
		
		/// <summary>The keytype attribute.</summary>
		public string keytype{
			get{
				return getAttribute("keytype");
			}
			set{
				setAttribute("keytype", value);
			}
		}
		
		/// <summary>All labels targeting this select element.</summary>
		public NodeList labels{
			get{
				return HtmlLabelElement.FindAll(this);
			}
		}
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return "keygen";
			}
		}
		
		/// <summary>Always false for keygen elements.</summary>
		public bool willValidate{
			get{
				return false;
			}
			set{}
		}
		
		/// <summary>Does this element get reset with the form?</summary>
		internal override bool IsFormResettable{
			get{
				return true;
			}
		}
		
		/// <summary>Does this element get submitted with the form?</summary>
		internal override bool IsFormSubmittable{
			get{
				return true;
			}
		}
		
		/// <summary>Does this element list in form.elements?</summary>
		internal override bool IsFormListed{
			get{
				return true;
			}
		}
		
		/// <summary>Can this element have a label?</summary>
		internal override bool IsFormLabelable{
			get{
				return true;
			}
		}
		
		/// <summary>Checks if this element is valid.</summary>
		public bool checkValidity(){
			return true;
		}
		
	}
	
}