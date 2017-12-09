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
	/// Handles the object tag.
	/// </summary>
	
	[Dom.TagName("object")]
	public class HtmlObjectElement:HtmlElement{
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return getAttribute("align");
			}
			set{
				setAttribute("align", value);
			}
		}
		
		/// <summary>The archive attribute.</summary>
		public string archive{
			get{
				return getAttribute("archive");
			}
			set{
				setAttribute("archive", value);
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
		
		/// <summary>The codebase attribute.</summary>
		public string codeBase{
			get{
				return getAttribute("codebase");
			}
			set{
				setAttribute("codebase", value);
			}
		}
		
		/// <summary>The codetype attribute.</summary>
		public string codeType{
			get{
				return getAttribute("codetype");
			}
			set{
				setAttribute("codetype", value);
			}
		}
		
		/// <summary>The data attribute.</summary>
		public string data{
			get{
				return getAttribute("data");
			}
			set{
				setAttribute("data", value);
			}
		}
		
		/// <summary>The declare attribute.</summary>
		public string declare{
			get{
				return getAttribute("declare");
			}
			set{
				setAttribute("declare", value);
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
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		/// <summary>The standby attribute.</summary>
		public string standby{
			get{
				return getAttribute("standby");
			}
			set{
				setAttribute("standby", value);
			}
		}
		
		/// <summary>The tabindex of this element.</summary>
		public long tabindex{
			get{
				return tabIndex;
			}
			set{
				tabIndex=(int)value;
			}
		}
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return getAttribute("type");
			}
			set{
				setAttribute("type", value);
			}
		}
		
		/// <summary>The typemustmatch attribute.</summary>
		public bool typeMustMatch{
			get{
				return GetBoolAttribute("typemustmatch");
			}
			set{
				SetBoolAttribute("typemustmatch",value);
			}
		}
		
		/// <summary>The useMap attribute.</summary>
		public string useMap{
			get{
				return getAttribute("usemap");
			}
			set{
				setAttribute("usemap", value);
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
		
		/// <summary>Can the element be validated?</summary>
		public bool willValidate{
			get{
				return false;
			}
		}
		
		/// <summary>Checks if this element is valid.</summary>
		public bool checkValidity(){
			return true;
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.AddMarkedFormattingElement(this);
				
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
				
				lexer.CloseMarkedFormattingElement("object");
				
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
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
		}
		
		/// <summary>Does this element get submitted with the form?</summary>
		internal override bool IsFormSubmittable{
			get{
				return true;
			}
		}
		
		/// <summary>Does this element list in form.elements?</summary>
		internal override bool IsFormListed{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="border"){
				Style.Computed.ChangeTagProperty("border-width",NormalizeSize(getAttribute("border")));
			}else{
				return false;
			}
			
			return true;
		}
		
		/// <summary>Normalises a size to a CSS compatible value.</summary>
		private string NormalizeSize(string size){
			if(size!=null && !size.Contains("%")){
				size+="px";
			}
			
			return size;
		}
		
	}
	
}