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

using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles a table header cell.
	/// </summary>
	
	[Dom.TagName("th")]
	public class HtmlTableHeaderCellElement:HtmlTableCellElement{
		
		/// <summary>The abbr attribute.</summary>
		public string abbr{
			get{
				return getAttribute("abbr");
			}
			set{
				setAttribute("abbr", value);
			}
		}
		
		/// <summary>The sorted attribute.</summary>
		public bool sorted{
			get{
				return GetBoolAttribute("sorted");
			}
			set{
				SetBoolAttribute("sorted",value);
			}
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			return HandleClose("th",lexer,mode);
			
		}
		
	}
	
}