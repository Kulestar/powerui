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
	/// Represents a optgroup element.
	/// </summary>
	
	[TagName("optgroup")]
	public class HtmlOptGroupElement:HtmlElement{
		
		/// <summary>A label for the group.</summary>
		public string label{
			get{
				return getAttribute("label");
			}
			set{
				setAttribute("label", value);
			}
		}
		
		/// <summary>True if this element is disabled.</summary>
		public bool disabled{
			get{
				return GetBoolAttribute("disabled");
			}
			set{
				SetBoolAttribute("disabled",value);
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
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InSelect){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
				if(lexer.CurrentElement.Tag=="optgroup"){
					lexer.CloseCurrentNode();
				}
				
				lexer.Push(this,true);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				if(lexer.CurrentElement.Tag=="option"){
					lexer.CloseCurrentNode();
				}
				
				lexer.ReconstructFormatting();
				
				lexer.Push(this,true);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InSelect){
				
				if(lexer.CurrentElement.Tag=="option" && lexer.OpenElements[lexer.OpenElements.Count-2].Tag=="optgroup"){
					lexer.CloseCurrentNode();
				}
				
				if(lexer.CurrentElement.Tag=="optgroup"){
					lexer.CloseCurrentNode();
				}
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}