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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY5
#endif

using System;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Wraps copy/paste functionality into a simple interface.
	/// </summary>
	
	public static class Clipboard{
		
		/// <summary>Pastes the text from the clipboard.</summary>
		public static string Paste(){
			TextEditor editor=new TextEditor();

			#if PRE_UNITY5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			editor.content.text="";
			editor.Paste();
			return editor.content.text;
			#else
			editor.text="";
			editor.Paste();
			return editor.text;
			#endif
			
		}
		
		/// <summary>Copies the given text onto the system clipboard.</summary>
		public static void Copy(string text){
			TextEditor editor=new TextEditor();

			#if PRE_UNITY5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
			editor.content.text=text;
			#else
			editor.text=text;
			#endif

			editor.SelectAll();
			editor.Copy();
		}
		
	}
	
}