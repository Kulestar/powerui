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
	/// Represents a HTML base element.
	/// </summary>
	
	[Dom.TagName("base")]
	public class HtmlBaseElement:HtmlElement{
		
		/// <summary>The href attribute.</summary>
		public string href{
			get{
				return getAttribute("href");
			}
			set{
				setAttribute("href", value);
			}
		}
		
		/// <summary>The target attribute.</summary>
		public string target{
			get{
				return getAttribute("target");
			}
			set{
				setAttribute("target", value);
			}
		}
		
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
			
			if((mode & (HtmlTreeMode.InHead | HtmlTreeMode.InTemplate | HtmlTreeMode.InBody))!=0){
				
				// Append it. DO NOT push to the stack:
				lexer.Push(this,false);
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="href"){
				string href=getAttribute("href");
				
				// Change the documents base path:
				document.basepath=new Location(href,document.location);
				
				return true;
			}else if(property=="target"){
				string target=getAttribute("target");
				
				// Change the document's target override:
				htmlDocument.baseTarget=target;
			}
			
			
			return false;
		}
		
	}
	
}