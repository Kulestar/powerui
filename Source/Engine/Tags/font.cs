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
	/// Handles the standard inline font element.
	/// </summary>
	
	[Dom.TagName("font")]
	public class HtmlFontElement:HtmlElement{
		
		/// <summary>The color attribute.</summary>
		public string color{
			get{
				return getAttribute("color");
			}
			set{
				setAttribute("color", value);
			}
		}
		
		/// <summary>The size attribute.</summary>
		public string size{
			get{
				return getAttribute("size");
			}
			set{
				setAttribute("size", value);
			}
		}
		
		/// <summary>The face attribute.</summary>
		public string face{
			get{
				return getAttribute("face");
			}
			set{
				setAttribute("face", value);
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.AddFormattingElement(this);
				
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
				
				lexer.AdoptionAgencyAlgorithm("font");
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			ComputedStyle computed=Style.Computed;
			
			string value=getAttribute(property);
			if(value==null){
				value="";
			}
			
			if(property=="color"){
				
				Style.Computed.ChangeTagProperty(
					"color",
					new Css.Units.ColourUnit(
						Css.ColourMap.ToSpecialColour(value)
					)
				);
				
			}else if(property=="size"){
				if(value!=""){
					value+="px";
				}
				computed.ChangeTagProperty("font-size",value);
			}else if(property=="face"){
				computed.ChangeTagProperty("font-family",value);
			}else{
				return false;
			}
			
			return true;
		}
		
	}
	
}