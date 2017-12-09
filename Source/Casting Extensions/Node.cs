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

using PowerUI;


namespace Dom{
	
	public partial class Node{

		/// <summary>The parent as a HtmlElement (convenience method).</summary>
		public HtmlElement htmlParentNode{
			get{
				return parentNode as HtmlElement;
			}
		}
		
		/// <summary>The ownerDocument as a Html document.</summary>
		public HtmlDocument htmlDocument{
			get{
				return document_ as HtmlDocument;
			}
		}
		
		/// <summary>Casts getElementByTagName to a HtmlElement for you (exists because of SVG and MathML).</summary>
		public HtmlElement getByTagName(string tag){
			
			return getElementByTagName(tag) as HtmlElement;
			
		}
		
		/// <summary>Casts getElementById to a HtmlElement for you (exists because of SVG and MathML).</summary>
		public HtmlElement getById(string id){
			
			return getElementById(id) as HtmlElement;
			
		}
		
		/// <summary>Casts getElementByAttribute to a HtmlElement for you (exists because of SVG and MathML).</summary>
		public HtmlElement getByAttribute(string property,string value){
			
			return getElementByAttribute(property,value) as HtmlElement;
			
		}
		
	}
	
}