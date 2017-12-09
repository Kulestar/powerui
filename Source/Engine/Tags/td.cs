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

using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles a table data cell.
	/// </summary>
	
	[Dom.TagName("td")]
	public class HtmlTableDataCellElement:HtmlTableCellElement{
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			return HandleClose("td",lexer,mode);
		}
		
	}
	
	/// <summary>
	/// Handles a normal table cell.
	/// </summary>
	public class HtmlTableCellElement:HtmlElement{
		
		/// <summary>The colspan attribute.</summary>
		public ulong colSpan{
			get{
				ulong v;
				ulong.TryParse(getAttribute("colspan"),out v);
				return v;
			}
			set{
				setAttribute("colspan", value.ToString());
			}
		}
		
		/// <summary>The rowspan attribute.</summary>
		public ulong rowSpan{
			get{
				ulong v;
				ulong.TryParse(getAttribute("rowspan"),out v);
				return v;
			}
			set{
				setAttribute("rowspan", value.ToString());
			}
		}
		
		/*
		public DOMSettableTokenList headers{
			get{
				return new DOMSettableTokenList(this,"headers");
			}
		}
		*/
		
		/// <summary>The index of the cell in its parent row.</summary>
		public long cellIndex{
			get{
				if(parentNode is HtmlTableRowElement){
					return childIndex;
				}
				return -1;
			}
		}
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return getAttribute("align");
			}
			set{
				setAttribute("align", value);
			}
		}
		
		/// <summary>The bgcolor attribute.</summary>
		public string bgColor{
			get{
				return getAttribute("bgcolor");
			}
			set{
				setAttribute("bgcolor", value);
			}
		}
		
		/// <summary>The axis attribute.</summary>
		public string axis{
			get{
				return getAttribute("axis");
			}
			set{
				setAttribute("axis", value);
			}
		}
		
		/// <summary>The height attribute.</summary>
		public string height{
			get{
				return getAttribute("height");
			}
			set{
				setAttribute("height", value);
			}
		}
		
		/// <summary>The width attribute.</summary>
		public string width{
			get{
				return getAttribute("width");
			}
			set{
				setAttribute("width", value);
			}
		}
		
		/// <summary>The nowrap attribute.</summary>
		public bool noWrap{
			get{
				return GetBoolAttribute("nowrap");
			}
			set{
				SetBoolAttribute("nowrap",value);
			}
		}
		
		/// <summary>The valign attribute.</summary>
		public string vAlign{
			get{
				return getAttribute("valign");
			}
			set{
				setAttribute("valign", value);
			}
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
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Thorough;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow;
		
		protected bool HandleClose(string close,HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				lexer.CloseSelect(false,null,close);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				if(lexer.IsInTableScope(close)){
					
					// Generate implied:
					lexer.GenerateImpliedEndTags();
					
					// Close including:
					lexer.CloseInclusive(close);
					
					// Clear to marker:
					lexer.ClearFormatting();
					
					// Back in row mode:
					lexer.CurrentMode=HtmlTreeMode.InRow;
					
				}
				
			}else{
			
				return false;
			
			}
			
			return true;
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Most common:
			if(mode==HtmlTreeMode.InRow){
				
				lexer.CloseToTableRowContext();
				
				lexer.Push(this,true);
				
				lexer.CurrentMode=HtmlTreeMode.InCell;
				
				lexer.AddScopeMarker();
			
			}else if(mode==HtmlTreeMode.InTableBody){
				
				lexer.CloseToTableBodyContext();
				
				Element el=lexer.CreateTag("tr",true);
				lexer.Push(el,true);
				lexer.CurrentMode=HtmlTreeMode.InRow;
				
				// Reproc:
				lexer.Process(this,null);
			
			}else if(mode==HtmlTreeMode.InTable){
				
				lexer.CloseToTableContext();
				
				// Create a tbody:
				Element el=lexer.CreateTag("tbody",true);
				lexer.Push(el,true);
				lexer.CurrentMode = HtmlTreeMode.InTableBody;
				
				// Reproc:
				lexer.Process(this,null);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				// [Table component] - Close to table cell if <th> or <td> is in scope and reprocess:
				lexer.CloseIfThOrTr(this,null);
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// [Table component] - Close a select (and reprocess) when it appears:
				lexer.CloseSelect(true,this,null);
				
			}else if(mode==HtmlTreeMode.InTemplate){
				
				// [Table component] - Step the template:
				lexer.TemplateStep(this,null,HtmlTreeMode.InRow);
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				// [Table component] - Close it and reprocess:
				lexer.CloseCaption(this,null);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// [Table component] - Parse error if this appears in the body.
				// Just ignore it.
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return last ? Dom.HtmlTreeMode.InBody : Dom.HtmlTreeMode.InCell;
			
		}
		
	}
	
}