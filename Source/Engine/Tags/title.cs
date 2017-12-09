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
	/// Handles the title tag.
	/// Note that the title is set to <see cref="PowerUI.Document.title"/> if you wish to use it.
	/// </summary>
	
	[Dom.TagName("title")]
	public class HtmlTitleElement:HtmlElement{
		
		/// <summary>The title text.</summary>
		public string text{
			get{
				return innerHTML;
			}
			set{
				innerHTML=value;
			}
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override void OnChildrenLoaded(){
			htmlDocument.title=innerHTML;
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & (HtmlTreeMode.InHead | HtmlTreeMode.InBody | HtmlTreeMode.InTemplate))!=0){
				
				// Generic RCData algorithm:
				lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.RCData);
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
	}
	
}