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
using Css;
using Dom;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// The HTML namespace attribute as used by all HTML nodes.
	/// </summary>
	public class HtmlNamespace : XmlNamespace{
		
		public HtmlNamespace()
			:base("http://www.w3.org/1999/xhtml","xhtml","text/html",typeof(PowerUI.HtmlDocument),"svg:svg,mml:math")
		{
		
		}
		
	}
	
}
