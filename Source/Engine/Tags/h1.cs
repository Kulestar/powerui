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
	/// Represents a standard header block element.
	/// </summary>
	
	[TagName("h1")]
	public class HtmlH1Element:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			return HtmlH1Element.HeaderAdd(this,lexer,mode);
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			return HtmlH1Element.HeaderClose("h1",lexer,mode);
			
		}
		
		public static bool HeaderAdd(Element el,HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				// p element in button scope; close it.
				lexer.CloseParagraphButtonScope();
				
				// Insert:
				lexer.Push(el,true);
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		public static bool HeaderClose(string close,HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				// Implicit close
				lexer.GenerateImpliedEndTags();
				
				
				// Pop up to and incl (any of h1-h6):
				string tag=lexer.CurrentElement.Tag;
				
				// Special case for 'hr'; it's the only other 2 char hX tag.
				while(tag.Length!=2 || tag[0]!='h' || tag=="hr"){
					lexer.CloseCurrentNode();
					tag=lexer.CurrentElement.Tag;
				}
				
				// Inclusive pop:
				lexer.CloseCurrentNode();
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}