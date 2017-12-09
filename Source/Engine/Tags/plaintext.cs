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
	/// Represents a plaintext element.
	/// </summary>
	
	[Dom.TagName("plaintext")]
	public class HtmlPlaintextElement:HtmlElement{
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				// Close paragraphs and append:
				lexer.CloseParagraphButtonScope();
				lexer.Push(this,true);
				
				// Plaintext state:
				lexer.State=HtmlParseMode.Plaintext;
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
	}
	
}