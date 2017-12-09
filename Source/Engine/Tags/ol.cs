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
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Represents a standard ordered list element.
	/// </summary>
	
	[Dom.TagName("ol")]
	public class HtmlOListElement:HtmlElement{
		
		/// <summary>The starting index.</summray.
		internal int Start_=1;
		/// <summary>True if the numbering is reversed.</summary>
		internal bool Reversed_;
		
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return getAttribute("type");
			}
			set{
				setAttribute("type", value);
			}
		}
		
		/// <summary>The start attribute.</summary>
		public long start{
			get{
				return Start_;
			}
			set{
				setAttribute("start", value.ToString());
			}
		}
		
		/// <summary>True if the numbering is reversed.</summary>
		public bool reversed{
			get{
				return Reversed_;
			}
			set{
				SetBoolAttribute("reversed",value);
			}
		}
		
		/// <summary>Resolves the marker for the given computed style.</summary>
		public static string GetOrdinal(ComputedStyle style,bool prefixed){
			
			Node ele=style.Element;
			HtmlOListElement ol=ele.parentNode as HtmlOListElement;
			
			int start;
			bool reversed;
			
			if(ol==null){
				start=1;
				reversed=false;
			}else{
				start=ol.Start_;
				reversed=ol.Reversed_;
			}
			
			// Get the list style type for this element:
			Css.Value value=style[Css.Properties.ListStyleType.GlobalProperty];
			int index;
			
			if(value==null){
				
				index=ele.childElementIndex;
				
				if(reversed){
					index=start-index;
				}else{
					index+=start;
				}
				
				// Disc is the default:
				return style.reflowDocument.GetOrdinal(index,"disc",prefixed)+" ";
				
			}else if(value.IsType(typeof(Css.Keywords.None))){
				return "";
			}
			
			index=ele.childElementIndex;
			
			if(reversed){
				index=start-index;
			}else{
				index+=start;
			}
			
			// Get textual name:
			string name=value.Text;
			
			// Is it a named set?
			string result=style.reflowDocument.GetOrdinal(index,name,prefixed);
			
			if(result==null){
				// It's possibly the symbols() function or alternatively it's just some text.
				// Assume it's some text for now!
				result=value.Text;
			}
			
			return result;
		}
		
		/// <summary>Figures out the ordinal type for the given type string. 1, a, A, i, I.</summary>
		public static string GetListStyleMode(string type){
			
			switch(type){
				case "a":
					return "lower-alpha";
				case "A":
					return "upper-alpha";
				case "i":
					return "lower-roman";
				case "I":
					return "upper-roman";
				default:
				case "1":
					return "decimal";
			}
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphThenAdd(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				lexer.BlockClose("ol");
			}else{
				return false;
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="type"){
				
				Style.Computed.ChangeTagProperty(
					"list-style-type",
					GetListStyleMode(getAttribute("type"))
				);
				
			}else if(property=="start"){
				int.TryParse(getAttribute("start"),out Start_);
			}else if(property=="reversed"){
				Reversed_=GetBoolAttribute("reversed");
			}else{
				return false;
			}
			
			return true;
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
	}
	
}