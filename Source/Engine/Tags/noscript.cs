//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright � 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a noscript element.
	/// </summary>
	
	[TagName("noscript")]
	public class HtmlNoScriptElement:HtmlElement{
		
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
			
			if(mode==HtmlTreeMode.InHeadNoScript){
				
				// Ignore it.
				
			}else if(mode==HtmlTreeMode.InBody){
				
				if(Document.AllowScripts){
					
					lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.Rawtext);
					
				}
				
			}else if(mode==HtmlTreeMode.InHead){
				
				// Allowed?
				if(Document.AllowScripts){
					
					lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.Rawtext);
					
				}else{
					
					// Append it:
					lexer.Push(this,true);
					
					// Switch to HeadNoScript:
					lexer.CurrentMode = HtmlTreeMode.InHeadNoScript;
					
				}
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InHeadNoScript){
				
				// Close the tag.
				lexer.CloseCurrentNode();
				
				// Switch mode:
				lexer.CurrentMode = HtmlTreeMode.InHead;
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}