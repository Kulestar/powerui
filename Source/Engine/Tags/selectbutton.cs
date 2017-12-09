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


namespace PowerUI{
	
	/// <summary>
	/// Select button. Used by select internally - when clicked, it displays the dropdown menu.
	/// </summary>
	
	[Dom.TagName("selectbutton")]
	public class HtmlSelectButtonElement:HtmlElement{
		
		public const int Priority=Css.VirtualElements.DURING_ZONE+95;
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>The parent select element.</summary>
		public HtmlSelectElement Select{
			get{
				return parentNode as HtmlSelectElement;
			}
		}
		
		/*
		This button actually does nothing at all - it's the parent select element that actually handles clicks.
		(Because people click on any part of it)
		*/
		
	}
	
}