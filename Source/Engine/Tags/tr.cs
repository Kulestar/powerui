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

using System;
using System.Collections;
using System.Collections.Generic;
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles a table row.
	/// </summary>
	
	[Dom.TagName("tr")]
	public class HtmlTableRowElement:HtmlElement{
		
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
		
		/// <summary>The section row index.</summary>
		public long sectionRowIndex{
			get{
				return childIndex;
			}
		}
		
		/// <summary>An index in the entire table.</summary>
		public long rowIndex{
			get{
				// Get the table itself:
				HtmlTableElement table=GetParentByTagName("table") as HtmlTableElement;
				
				// Must align with table.deleteRow(index).
				if(table==null){
					return -1;
				}
				
				return table.rows.indexOf(this);
			}
		}
		
		/// <summary>The cells on the row (td and th).</summary>
		public HTMLCollection cells{
			get{
				HTMLCollection hc=new HTMLCollection();
				getElementsByTagName("td",false,hc);
				getElementsByTagName("th",false,hc);
				return hc;
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
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InRow;
			
		}
		
		/// <summary>True if this element is a table row context.</summary>
		public override bool IsTableRowContext{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element is part of table structure, except for td.</summary>
		public override bool IsTableStructure{
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
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Most common (Assumption here; most people don't bother with tbody):
			if(mode==HtmlTreeMode.InTable){
				
				lexer.CloseToTableContext();
				
				// Create a tbody:
				Element el=lexer.CreateTag("tbody",true);
				lexer.Push(el,true);
				lexer.CurrentMode = HtmlTreeMode.InTableBody;
				
				// Reproc:
				lexer.Process(this,null);
				
			}else if(mode==HtmlTreeMode.InTableBody){
				
				lexer.CloseToTableBodyContext();
				
				lexer.Push(this,true);
				
				lexer.CurrentMode=HtmlTreeMode.InRow;
				
			}else if(mode==HtmlTreeMode.InRow){
				
				// [Table component] - Close to table body if <tr> is in scope and reprocess:
				lexer.TableBodyIfTrInScope(this,null);
				
			}else if(mode==HtmlTreeMode.InCell){
				
				// [Table component] - Close to table cell if <th> or <td> is in scope and reprocess:
				lexer.CloseIfThOrTr(this,null);
				
			}else if(mode==HtmlTreeMode.InTemplate){
				
				// Step:
				lexer.TemplateStep(this,null,HtmlTreeMode.InTableBody);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// [Table component] - Parse error if this appears in the body.
				// Just ignore it.
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				// [Table component] - Close it and reprocess:
				lexer.CloseCaption(this,null);
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// [Table component] - Close a select (and reprocess) when it appears:
				lexer.CloseSelect(true,this,null);
				
			}else{
			
				return false;
			
			}
			
			return true;
			
			
		}
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InTable
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				lexer.CloseSelect(false,null,"tr");
				
			}else if(mode==HtmlTreeMode.InRow){
				
				if(lexer.IsInTableScope("tr")){
					// Ignore otherwise.
					
					lexer.CloseToTableRowContext();
					
					lexer.CloseCurrentNode();
					lexer.CurrentMode=HtmlTreeMode.InTableBody;
					
				}
				
			}else if(mode==HtmlTreeMode.InCell){
				
				lexer.CloseTableZoneInCell("tr");
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}