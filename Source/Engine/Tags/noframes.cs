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
	/// Represents a noframes element.
	/// </summary>
	
	[TagName("noframes")]
	public class HtmlNoFramesElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>All the modes where noframes is added as if it's in "in head" state.</summary>
		private const int InHeadMode=HtmlTreeMode.AfterAfterFrameset
		| HtmlTreeMode.InHead
		| HtmlTreeMode.AfterFrameset
		| HtmlTreeMode.InHeadNoScript
		| HtmlTreeMode.InFrameset
		| HtmlTreeMode.InBody
		| HtmlTreeMode.InTemplate;
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & InHeadMode)!=0){
				
				lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.Rawtext);
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
	}
	
}