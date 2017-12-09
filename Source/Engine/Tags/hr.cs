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
	/// Represents a standard horizontal rule element.
	/// </summary>
	
	[Dom.TagName("hr")]
	public class HtmlHRElement:HtmlElement{
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return getAttribute("align");
			}
			set{
				setAttribute("align", value);
			}
		}
		
		/// <summary>The color attribute.</summary>
		public string color{
			get{
				return getAttribute("color");
			}
			set{
				setAttribute("color", value);
			}
		}
		
		/// <summary>The noshade attribute.</summary>
		public bool noshade{
			get{
				return GetBoolAttribute("noshade");
			}
			set{
				SetBoolAttribute("noshade",value);
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
		
		/// <summary>The width attribute.</summary>
		public string width{
			get{
				return getAttribute("width");
			}
			set{
				setAttribute("width", value);
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphButtonScope();
				
				lexer.Push(this,false);
				
				lexer.FramesetOk=false;
				
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
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="color"){
				
				Style.Computed.ChangeTagProperty(
					"color",
					new Css.Units.ColourUnit(
						Css.ColourMap.ToSpecialColour(value)
					)
				);
			
			}else if(property=="size"){
				Style.Computed.ChangeTagProperty("height",NormalizeSize(getAttribute("size")));
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