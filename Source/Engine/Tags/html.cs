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
	/// Represents the html tag. Note that this is added automatically by PowerUI and isn't required.
	/// </summary>
	
	[Dom.TagName("html")]
	public class HtmlHtmlElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		internal override bool ClearBackground{
			get{
				// See info at Node.ClearBackground for more.
				return false;
			}
		}
		
		public const int SyntaxErrorModes=HtmlTreeMode.InCell
			| HtmlTreeMode.InCaption
			| HtmlTreeMode.InHead
			| HtmlTreeMode.InSelect
			| HtmlTreeMode.InSelectInTable
			| HtmlTreeMode.BeforeHead
			| HtmlTreeMode.InHeadNoScript
			| HtmlTreeMode.InColumnGroup
			| HtmlTreeMode.AfterBody
			| HtmlTreeMode.InFrameset
			| HtmlTreeMode.AfterFrameset
			| HtmlTreeMode.AfterAfterFrameset
			| HtmlTreeMode.AfterAfterBody
			| HtmlTreeMode.InBody;
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.BeforeHtml){
			
				// 'html' start tag. Append:
				lexer.Push(this,true);
				
				// Change to before head:
				lexer.CurrentMode = HtmlTreeMode.BeforeHead;
				
				
			}else if((mode & SyntaxErrorModes)!=0){
				
				// (Syntax error)
				
				if(!lexer.TagCurrentlyOpen("template")){
					
					// Ignore it otherwise
					
					// Combine attribs into TOS:
					lexer.CombineInto(this,lexer.OpenElements[0]);
					
				}
			
			}else{
				
				return false;
				
			}
			
			return true;
		} 
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow
		| HtmlTreeMode.InCell;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
				
			}else if(mode==HtmlTreeMode.AfterBody){
				
				// After after body:
				lexer.CurrentMode=HtmlTreeMode.AfterAfterBody;
				
			}else if(mode==HtmlTreeMode.InHead){
				
				// Use anything else method:
				lexer.InHeadElse(null,"html");
				
			}else if(mode==HtmlTreeMode.BeforeHtml){
				
				// Allowed to fall through the 'anything else' case:
				lexer.BeforeHtmlElse(null,"html");
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				// Use anything else method:
				lexer.AfterHeadElse(null,"html");
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// Check if the stack contains elements that aren't allowed to still be open:
				lexer.CheckAfterBodyStack();
				
				// Ok! (It throws a fatal error otherwise)
				// Change to after body and reprocess:
				lexer.CurrentMode=HtmlTreeMode.AfterBody;
				lexer.Process(null,"html");
				
			}else if(mode==HtmlTreeMode.AfterFrameset){
				
				// After after frameset:
				lexer.CurrentMode=HtmlTreeMode.AfterAfterFrameset;
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
		public override void OnTagLoaded(){
			HtmlDocument doc=htmlDocument;
			
			if(doc==null || doc.html!=null){
				return;
			}
			
			doc.html=this;
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.BeforeHead;
			
		}
		
		/// <summary>True if this element is a table row context.</summary>
		public override bool IsTableRowContext{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element is a table body context.</summary>
		public override bool IsTableBodyContext{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element is a table context.</summary>
		public override bool IsTableContext{
			get{
				return true;
			}
		}
		
	}
	
}