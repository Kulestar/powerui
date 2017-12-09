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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Represents the type of mobile keyboard that can show up.
	/// Similar to Unities TouchScreenKeyboardType, however also has a None option
	/// to allow an element to not display any keyboard at all.
	/// </summary>
	
	public class KeyboardMode{
	
		/// <summary>True if this keyboard should hide the input.</summary>
		public bool Secret;
		/// <summary>True if this keyboard should be multiline.</summary>
		public bool Multiline;
		/// <summary>The text that should show up to start with.</summary>
		public string StartText;
		
		#if MOBILE
		/// <summary>The type of keyboard to display.</summary>
		public TouchScreenKeyboardType Type=TouchScreenKeyboardType.Default;
		#endif
		
	}
	
}