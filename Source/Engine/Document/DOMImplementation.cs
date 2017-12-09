using System;
using System.Collections;
using System.Collections.Generic;
using PowerUI;


namespace Dom{

	/// <summary>
	/// Provides a number of methods for performing operations that are
	/// independent of any particular instance of the DOM.
	/// </summary>
	public partial class DOMImplementation{
		
		/// <summary>Creates a HTML document with the given title.</summary>
		public HtmlDocument createHTMLDocument(string title){
			
			if (!string.IsNullOrEmpty(title)){
				title="<title>"+title+"</title>";
			}else{
				title="";
			}
			
			HtmlDocument document = new HtmlDocument();
			document.innerHTML="<!doctype html5><html><head>"+title+"</head><body></body></html>";
			
			document.basepath = _owner.basepath;
			return document;
			
		}
		
	}
	
}
