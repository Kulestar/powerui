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


namespace PowerUI{
	
	/// <summary>
	/// Extends UnityEngine.Color with a convenient ToCss() method.
	/// </summary>
	public static class ColorExtension{
		
		
		/// <summary>
		/// Returns a CSS formatted string of the colour. It's of the form rgba(r,g,b,a).
		/// </summary>
		public static string ToCss(this UnityEngine.Color colour){
			
			// Use RGBA CSS function 
			return "rgba("+colour.r+","+colour.g+","+colour.b+","+colour.a+")";
			
		}
		
	}
	
}