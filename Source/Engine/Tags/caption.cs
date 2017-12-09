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
	/// Represents the caption tag.
	/// </summary>
	
	[Dom.TagName("caption")]
	public class HtmlTableCaptionElement:HtmlElement{
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return getAttribute("align");
			}
			set{
				setAttribute("align", value);
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InCaption;
			
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
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow
		| HtmlTreeMode.InCell
		| HtmlTreeMode.InTable;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// Close down to select:
				lexer.CloseSelect(false,null,"caption");
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Most common case:
			if(mode==HtmlTreeMode.InTable){
				
				// Caption in table:
				lexer.CloseToTableContext();
				
				lexer.AddScopeMarker();
				lexer.Push(this,true);
				lexer.CurrentMode = HtmlTreeMode.InCaption;
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// [Table component] - Parse error if this appears in the body.
				// Just ignore it.
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// [Table component] - Close a select (and reprocess) when it appears:
				lexer.CloseSelect(true,this,null);
				
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
				
				// [Table component] - Step the template:
				lexer.TemplateStep(this,null,HtmlTreeMode.InTable);
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Thorough;
			}
		}
		
	}
	
}