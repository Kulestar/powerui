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
	/// Represents a HTML5 description element.
	/// </summary>
	
	[Dom.TagName("dd")]
	public class HtmlDescriptionDElement:HtmlElement{
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				HtmlDescriptionDElement.DdAndDtBodyAdd(this,lexer);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				HtmlDescriptionDElement.DdAndDtBodyClose("dd",lexer);
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>Used when closing 'dd' or 'dt' with the given parser in 'in body' mode.</summary>
		public static void DdAndDtBodyClose(string close,HtmlLexer lexer){
			
			// In scope?
			if(lexer.IsInScope(close)){
				
				// Implicit close
				lexer.GenerateImpliedEndTagsExceptFor(close);
				
				lexer.CloseInclusive(close);
				
			}
			
		}
		
		/// <summary>Used when adding 'dd' or 'dt' with the given parser in 'in body' mode.</summary>
		public static void DdAndDtBodyAdd(Element el,HtmlLexer lexer){
			
			lexer.FramesetOk=false;
			
			int index=lexer.OpenElements.Count-1;
			Element node=lexer.OpenElements[index];
			
			while(true){
				
				if(node is HtmlDescriptionDElement){
				
					lexer.GenerateImpliedEndTagsExceptFor("dd");
					
					lexer.CloseInclusive("dd");
					
					break;
					
				}else if(node is HtmlDescriptionTElement){
				
					lexer.GenerateImpliedEndTagsExceptFor("dt");
					
					lexer.CloseInclusive("dt");
					
					break;
					
				}else if(node.IsSpecial){
					
					string tag=node.Tag;
					
					if(tag!="p" && tag!="address" && tag!="div"){
						break;
					}
					
				}
				
				// Next one:
				index--;
				
				if(index<0){
					break;
				}
				
				node=lexer.OpenElements[index];
			}
			
			// Close paragraphs and append:
			lexer.CloseParagraphButtonScope();
			lexer.Push(el,true);
			
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