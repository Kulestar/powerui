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
	/// Represents a template element.
	/// </summary>
	
	[Dom.TagName("template")]
	public class HtmlTemplateElement:HtmlElement{
		
		/// <summary>The fragment.</summary>
		public DocumentFragment content;
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return lexer.TemplateModes.Peek();
			
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
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
		
		/// <summary>The modes which act like 'in head' when a template tag is seen.</summary>
		private const int InHeadOpen=HtmlTreeMode.InHead
		| HtmlTreeMode.InTable
		| HtmlTreeMode.InColumnGroup
		| HtmlTreeMode.InSelectInTable
		| HtmlTreeMode.InSelect
		| HtmlTreeMode.InBody
		| HtmlTreeMode.InTemplate;
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & InHeadOpen)!=0){
				
				// Opening a template
				lexer.Push(this,true);
				
				lexer.AddScopeMarker();
				lexer.FramesetOk = false;
				lexer.CurrentMode = HtmlTreeMode.InTemplate;
				lexer.TemplateModes.Push(HtmlTreeMode.InTemplate);
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>The modes which act like 'in head' when a /template tag is seen.</summary>
		private const int InHeadClose=HtmlTreeMode.InHead
		| HtmlTreeMode.InBody
		| HtmlTreeMode.InColumnGroup
		| HtmlTreeMode.InSelectInTable
		| HtmlTreeMode.InSelect
		| HtmlTreeMode.InTable
		| HtmlTreeMode.AfterHead
		| HtmlTreeMode.InTemplate;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if((mode & InHeadClose)!=0){
				
				// Close a template tag.
				if(lexer.TagCurrentlyOpen("template")){
					
					// Generate implied thoroughly:
					lexer.GenerateImpliedEndTagsThorough();
					
					// Close it:
					lexer.CloseTemplate();
					
				}
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>Called when the kids are all ready.</summary>
		public override void OnChildrenLoaded(){
			
			// Create frag:
			content=new DocumentFragment();
			content.document_=document_;
			
			// Transfer kids into content:
			content.childNodes_=childNodes_;
			
			if(childNodes_==null){
				return;
			}
			
			for(int i=childNodes_.length-1;i>=0;i--){
				
				// Get the node:
				Node node=childNodes_[i];
				
				// Simple parent change:
				node.parentNode_=content;
				
			}
			
			childNodes_=null;
			
		}
		
	}
	
}