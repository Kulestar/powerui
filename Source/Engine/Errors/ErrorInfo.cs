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
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Contains information about an error, such as a 404 page not found.
	/// </summary>
	
	public class ErrorInfo{
		
		/// <summary>The URL if there was one.</summary>
		public Location Url;
		/// <summary>The custom HTML if there is any.</summary>
		public string Custom;
		/// <summary>The HTTP status code.</summary>
		public int StatusCode;
		/// <summary>The host document.</summary>
		public Document document;
		
		
		/// <summary>The host HTML document.</summary>
		public HtmlDocument htmlDocument{
			get{
				return document as HtmlDocument;
			}
		}
		
	}
	
}