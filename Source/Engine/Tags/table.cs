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
	/// Handles a table.
	/// </summary>
	
	[Dom.TagName("table")]
	public class HtmlTableElement:HtmlElement{
		
		/// <summary>The caption element in the table.</summary>
		public HtmlTableCaptionElement caption{
			get{
				return getElementByTagName("caption") as HtmlTableCaptionElement;
			}
		}
		
		/// <summary>The thead element in the table.</summary>
		public HtmlTableHeaderElement tHead{
			get{
				return getElementByTagName("thead") as HtmlTableHeaderElement;
			}
		}
		
		/// <summary>The tfoot element in the table.</summary>
		public HtmlTableFooterElement tFoot{
			get{
				return getElementByTagName("tfoot") as HtmlTableFooterElement;
			}
		}
		
		/// <summary>All the rows in the table.</summary>
		public HTMLCollection rows{
			get{
				return getElementsByTagName("tr");
			}
		}
		
		/// <summary>The tbody elements in the table.</summary>
		public HTMLCollection tBodies{
			get{
				return getElementsByTagName("tbody");
			}
		}
		
		/// <summary>The sortable attribute.</summary>
		public bool sortable{
			get{
				return GetBoolAttribute("sortable");
			}
			set{
				SetBoolAttribute("sortable",value);
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
		
		/// <summary>The border attribute.</summary>
		public string border{
			get{
				return getAttribute("border");
			}
			set{
				setAttribute("border", value);
			}
		}
		
		/// <summary>The cellpadding attribute.</summary>
		public string cellPadding{
			get{
				return getAttribute("cellpadding");
			}
			set{
				setAttribute("cellpadding", value);
			}
		}
		
		/// <summary>The cellspacing attribute.</summary>
		public string cellSpacing{
			get{
				return getAttribute("cellspacing");
			}
			set{
				setAttribute("cellspacing", value);
			}
		}
		
		/// <summary>The frame attribute.</summary>
		public string frame{
			get{
				return getAttribute("frame");
			}
			set{
				setAttribute("frame", value);
			}
		}
		
		/// <summary>The rules attribute.</summary>
		public string rules{
			get{
				return getAttribute("rules");
			}
			set{
				setAttribute("rules", value);
			}
		}
		
		/// <summary>The summary attribute.</summary>
		public string summary{
			get{
				return getAttribute("summary");
			}
			set{
				setAttribute("summary", value);
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
		
		/// <summary>Gets or creates a thead.</summary>
		public HtmlElement createTHead(){
			
			HtmlElement e=tHead;
			
			if(e!=null){
				return e;
			}
			
			// Create it:
			e=document.createElement("thead") as HtmlElement;
			
			if(childNodes_!=null){
				
				// insert after caption and colgroup.
				for(int i=0;i<childNodes_.length;i++){
					
					Element current=childNodes_[i] as Element;
					
					if(current==null || current.Tag=="colgroup" || current.Tag=="caption"){
						// Not an element - skip.
						continue;
					}
					
					// Insert before current.
					childNodes_.insert(i,e);
					return e;
					
				}
			
			}
			
			// Append:
			appendChild(e);
			return e;
		}
		
		/// <summary>Deletes the first thead.</summary>
		public void deleteTHead(){
			HtmlElement e=tHead;
			if(e!=null){
				e.remove();
			}
		}
		
		/// <summary>Gets or creates a tfoot.</summary>
		public HtmlElement createTFoot(){
			
			HtmlElement e=tFoot;
			
			if(e!=null){
				return e;
			}
			
			// Create it:
			e=document.createElement("tfoot") as HtmlElement;
			
			if(childNodes_!=null){
				
				// insert after caption and colgroup.
				for(int i=0;i<childNodes_.length;i++){
					
					Element current=childNodes_[i] as Element;
					
					if(current==null || current.Tag=="colgroup" || current.Tag=="caption" || current.Tag=="thead"){
						// Not an element - skip.
						continue;
					}
					
					// Insert before current.
					childNodes_.insert(i,e);
					return e;
					
				}
			
			}
			
			// Append:
			appendChild(e);
			return e;
		}
		
		/// <summary>Deletes the first tfoot.</summary>
		public void deleteTFoot(){
			HtmlElement e=tFoot;
			if(e!=null){
				e.remove();
			}
		}
		
		/// <summary>Gets or creates a caption.</summary>
		public HtmlElement createCaption(){
			HtmlElement e=caption;
			
			if(e!=null){
				return e;
			}
			
			// Create it:
			e=document.createElement("caption") as HtmlElement;
			prependChild(e);
			return e;
		}
		
		/// <summary>Creates and inserts a new row.</summary>
		public HtmlElement insertRow(){
			return insertRow(-1);
		}
		
		/// <summary>Creates and inserts a new row.</summary>
		public HtmlElement insertRow(int index){
			
			HtmlElement e=document.createElement("tr") as HtmlElement;
			
			if(index!=-1){
				// Get the row at that index:
				Element r=getRow(index);
				
				if(r!=null){
					// Insert before it:
					r.parentNode.insertBefore(e,r);
					return e;
				}
				
			}
			
			// Insert as last tbody:
			HTMLCollection bodies=tBodies;
			Node body=(bodies.length==0) ? appendChild(document.createElement("tbody")) : bodies[bodies.length-1];
			
			// append to that:
			body.appendChild(e);
			
			return e;
		}
		
		/// <summary>Removes the sortable attribute from all th nodes.</summary>
		public void stopSorting(){
			foreach(Element e in getElementsByTagName("th")){
				e.removeAttribute("sortable");
			}
		}
		
		/// <summary>Gets a row from its index.</summary>
		public Element getRow(int index){
			HTMLCollection allRows=rows;
			return allRows.item(index==-1 ? allRows.length-1 : index);
		}
		
		/// <summary>Deletes a row at a particular index.</summary>
		public void deleteRow(int index){
			Element e=getRow(index);
			if(e!=null){
				e.remove();
			}
		}
		
		/// <summary>Deletes the first caption.</summary>
		public void deleteCaption(){
			HtmlElement e=caption;
			if(e!=null){
				e.remove();
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
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphButtonScope();
				lexer.Push(this,true);
				lexer.FramesetOk=false;
				lexer.CurrentMode=HtmlTreeMode.InTable;
				
			}else if(mode==HtmlTreeMode.InTable){
				
				
				if(lexer.IsInTableScope("table")){
					// Ignore otherwise
					
					lexer.CloseInclusive("table");
					
					// Reset mode:
					lexer.Reset();
					
					// Reprocess:
					lexer.Process(this,null);
					
				}
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				// [Table component] - Close a select (and reprocess) when it appears:
				lexer.CloseSelect(true,this,null);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InTable){
				
				// Close it
				
				if(lexer.IsInTableScope("table")){
					// Ignore otherwise
					
					lexer.CloseInclusive("table");
					
					// Reset mode:
					lexer.Reset();
					
				}
				
			}else if(mode==HtmlTreeMode.InTableBody){
				
				// Close to table if in a table body context and reprocess:
				lexer.CloseToTableBodyIfBody(null,"table");
				
			}else if(mode==HtmlTreeMode.InRow){
				
				lexer.TableBodyIfTrInScope(null,"table");
				
			}else if(mode==HtmlTreeMode.InCell){
				
				lexer.CloseTableZoneInCell("table");
				
			}else if(mode==HtmlTreeMode.InCaption){
				
				lexer.CloseCaption(null,"table");
				
			}else if(mode==HtmlTreeMode.InSelectInTable){
				
				lexer.CloseSelect(false,null,"table");
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>True if this element is part of table structure, except for td.</summary>
		public override bool IsTableStructure{
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
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InTable;
			
		}
		
	}
	
}