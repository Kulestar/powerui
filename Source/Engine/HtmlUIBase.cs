#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	#define PRE_UNITY5
#endif

// Note: New Unity UI is only available for 4.6+

#if UNITY_4_6 || UNITY_4_7 || !PRE_UNITY5

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
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Used when embedding PowerUI in a Unity UI.
	/// Separated from HtmlUIPanel so PowerUI can be 
	/// precompiled without breaking all your references.
	/// </summary>
	public interface HtmlUIBase{
		
		/// <summary>The rectangle this UI is in.</summary>
		RectTransform screenRect{
			get;
		}
		
		/// <summary>The HTML document this UI is holding.</summary>
		HtmlDocument document{
			get;
		}
		
	}
	
}

#endif