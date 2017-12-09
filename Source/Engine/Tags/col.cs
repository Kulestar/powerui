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
	/// Represents the col tag.
	/// </summary>
	
	[Dom.TagName("col")]
	public class HtmlTableColumnElement:HtmlElement{
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return getAttribute("align");
			}
			set{
				setAttribute("align", value);
			}
		}
		
		/// <summary>The span attribute.</summary>
		public ulong span{
			get{
				ulong v;
				ulong.TryParse(getAttribute("span"),out v);
				return v;
			}
			set{
				setAttribute("span", value.ToString());
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
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Most common case:
			if(mode==HtmlTreeMode.InColumnGroup){
				
				// Push but not onto the stack:
				lexer.Push(this,false);
				
			}else if(mode==HtmlTreeMode.InTable){
				
				lexer.CloseToTableContext();
				
				Element el=lexer.CreateTag("colgroup",true);
				lexer.Push(el,true);
				
				lexer.CurrentMode = HtmlTreeMode.InColumnGroup;
				lexer.Process(this,null);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// [Table component] - Parse error if this appears in the body.
				// Just ignore it.
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				// [Table component] - Close it and reprocess:
				lexer.CloseCaption(this,null);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				// [Table component] - Close to table cell if <th> or <td> is in scope and reprocess:
				lexer.CloseIfThOrTr(this,null);
				
			}else if(mode==HtmlTreeMode.InTableBody){
				
				// [Table component] - Close to table if in a table body context and reprocess:
				lexer.CloseToTableBodyIfBody(this,null);
				
			}else if(mode==HtmlTreeMode.InRow){
				
				// [Table component] - Close to table body if <tr> is in scope and reprocess:
				lexer.TableBodyIfTrInScope(this,null);
				
			}else if(mode==HtmlTreeMode.InTemplate){
				
				lexer.TemplateStep(this,null,HtmlTreeMode.InColumnGroup);
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InColumnGroup
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow
		| HtmlTreeMode.InCell;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}