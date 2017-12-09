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
	
	/// <summary>Helps display more friendly error results.</summary>
	/// <param name="error">Contains information about the error itself.</param>
	/// <param name="document">The document which should display the error. Note that this can be null if you use target="_blank" (outside Unity).</param>
	public delegate void ErrorHandler(ErrorInfo error,HtmlDocument document);
	
	/// <summary>
	/// Manages events such as 404 pages.
	/// </summary>
	
	public static class ErrorHandlers{
		
		/// <summary>True if all errors should be handled including ones where a site has returned a custom error.</summary>
		public static bool CatchAll;
		/// <summary>Used when a http:// or resources file link errors. This may be due to a 404/ file not found or network down.</summary>
		public static ErrorHandler PageNotFound;
		/// <summary>Redirection loop error.</summary>
		public const int TooManyRedirects=508;
		
		/// <summary>Converts the Unity WWW.error messages into an easier to use status code.</summary>
		public static int GetUnityErrorCode(string unityError){
			
			Dom.Log.Add("Unity Network Error: "+unityError);
			return 419;
			
		}
		
		/// <summary>Displays an error in the given document.</summary>
		public static void Display(ErrorInfo error){
			
			// Create a package:
			DataPackage package=new DataPackage("resources://standardErrors.html");
			
			// Load:
			package.onload=delegate(UIEvent e){
				
				// Apply the innerHTML:
				error.htmlDocument.innerHTML=package.responseText;
				
				// Get the status code:
				int code=error.StatusCode;
				
				// Get the status element:
				HtmlElement element=error.htmlDocument.getElementById("error-code") as HtmlElement;
				
				// Apply:
				element.innerHTML=code+"";
				
				// Get the URL element:
				element=error.htmlDocument.getElementById("error-url") as HtmlElement;
				
				// Apply:
				element.innerHTML=error.Url.absolute;
				
			};
			
			// Get it now:
			package.send();
			
		}
		
	}
	
}