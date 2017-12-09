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
	/// Represents a standard paragraph (p) block element.
	/// </summary>
	
	[Dom.TagName("p")]
	public class HtmlParagraphElement:HtmlElement{
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return getAttribute("align");
			}
			set{
				setAttribute("align", value);
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphThenAdd(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				if(lexer.IsInButtonScope("p")){
					
					lexer.CloseParagraph();
					
				}else{
					
					// (Parse error)
					// Create a p node, but don't push it onto the stack:
					Element el=lexer.CreateTag("p",true);
					lexer.Push(el,false);
					
				}
				
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
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Normal;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
	}
	
}