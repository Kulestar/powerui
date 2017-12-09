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
	/// Represents a HTML basefont element. Don't use this!
	/// </summary>
	
	[Dom.TagName("basefont")]
	public class HtmlBaseFontElement:HtmlElement{
		
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
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & (HtmlTreeMode.InHead | HtmlTreeMode.InBody | HtmlTreeMode.InTemplate | HtmlTreeMode.InHeadNoScript))!=0){
				
				// Append it. DO NOT push to the stack:
				lexer.Push(this,false);
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
	}
	
}