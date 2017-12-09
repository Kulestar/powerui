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
	
	[Dom.TagName("head")]
	public class HtmlHeadElement:HtmlElement{
		
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
			
			if(mode==HtmlTreeMode.BeforeHead){
				
				// Add:
				lexer.Push(this,true);
				lexer.head=this;
				
				// Switch:
				lexer.CurrentMode = HtmlTreeMode.InHead;
				
			}else if((mode & (HtmlTreeMode.AfterHead |HtmlTreeMode.InHead | HtmlTreeMode.InHeadNoScript | HtmlTreeMode.InBody))!=0){
				
				// Just ignore it.
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InHead){
				
				// Close the head tag.
				lexer.CloseCurrentNode();
				
				// Switch mode:
				lexer.CurrentMode = HtmlTreeMode.AfterHead;
				
			}else if(mode==HtmlTreeMode.BeforeHtml){
				
				// Allowed to fall through the 'anything else' case:
				lexer.BeforeHtmlElse(null,"head");
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return last ? Dom.HtmlTreeMode.InBody : Dom.HtmlTreeMode.InHead;
			
		}
		
	}
	
}