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
	/// Represents the colgroup tag.
	/// </summary>
	
	[Dom.TagName("colgroup")]
	public class HtmlColgroupElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InColumnGroup;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Most common case:
			if(mode==HtmlTreeMode.InTable){
				
				lexer.CloseToTableContext();
				
				lexer.Push(this,true);
				lexer.CurrentMode = HtmlTreeMode.InColumnGroup;
				
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
				
				// [Table component] - Step the template:
				lexer.TemplateStep(this,null,HtmlTreeMode.InTable);
				
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
				
			}else if(mode==HtmlTreeMode.InColumnGroup){
				
				if(lexer.CurrentElement.Tag=="colgroup"){
					// Ignore otherwise
					
					lexer.CloseCurrentNode();
					
					// Change mode:
					lexer.CurrentMode=HtmlTreeMode.InTable;
					
				}
				
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