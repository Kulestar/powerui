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
	/// Clones an element.
	/// </summary>

	public partial class HtmlElement{
		
		/// <summary>Clones this element.</summary>
		public override Node cloneNode(bool deep){
			
			Node nd=base.cloneNode(deep);
			
			HtmlElement ele=(nd as HtmlElement );
			
			// Apply styles:
			ele.Style.Computed.RefreshStructure();
			
			return ele;
			
		}
		
	}
	
}