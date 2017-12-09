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
using System.Text;


namespace PowerUI{
	
	/// <summary>
	/// Represents a pre element.
	/// </summary>
	
	[Dom.TagName("pre")]
	public class HtmlPreElement:HtmlElement{
		
		/// <summary>The width attribute (a long).</summary>
		public long width{
			get{
				long v;
				long.TryParse(getAttribute("width"),out v);
				return v;
			}
			set{
				setAttribute("width", value.ToString());
			}
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				PreOrListingAdd(this,lexer);
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		internal static void PreOrListingAdd(Element el,HtmlLexer lexer){
			
			// p element in button scope; close it.
			lexer.CloseParagraphButtonScope();
			
			// Insert:
			lexer.Push(el,true);
			
			lexer.SkipNewline();
			
			// Clear frames ok flag:
			lexer.FramesetOk=false;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				lexer.BlockClose("pre");
			}else{
				return false;
			}
			
			return true;
			
		}
		
	}
	
}