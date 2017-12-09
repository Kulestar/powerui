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
	/// Represents an output element.
	/// </summary>
	
	[Dom.TagName("output")]
	public class HtmlOutputElement:HtmlElement{
		
		/// <summary>The default value.</summary>
		public string defaultValue="";
		
		/// <summary>The string 'output'.</summary>
		public string type{
			get{
				return "output";
			}
			set{}
		}
		
		/// <summary>The value.</summary>
		public string Value{
			get{
				return textContent;
			}
			set{
				textContent=value;
			}
		}
		
		/// <summary>Does this element get reset with the form?</summary>
		internal override bool IsFormResettable{
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
		
		/// <summary>Will this element validate? 
		/// Note: This is conflicted (some browser vendors disagree with the spec).</summary>
		public bool willValidate{
			get{
				return false;
			}
		}
		
		/*
		public DOMSettableTokenList htmlFor{
			get{
				return new DOMSettableTokenList(this,"for");
			}
		}
		*/
		
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
		
		public override void OnFormReset(){
			Value=defaultValue;
		}
		
		public override void OnChildrenLoaded(){
			
			// The content:
			string content=textContent;
			
			// Apply as default:
			if(!string.IsNullOrEmpty(content)){
				defaultValue=content;
			}
			
		}
		
		/// <summary>Note: affected by a specification bug.</summary>
		public bool checkValidity(){
			return true;
		}
		
	}
	
}