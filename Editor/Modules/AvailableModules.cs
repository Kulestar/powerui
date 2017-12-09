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

#if PRE_UNITY5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define PRE_UNITY5_3
#endif

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;
using Dom;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// The modules available to PowerUI.
	/// </summary>
	
	public class AvailableModules : EditorWindow{
		
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		
		
		// Add menu item named "Available Modules" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Available Modules")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(AvailableModules));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Available Modules";
			#else
			GUIContent title=new GUIContent();
			title.text="Available Modules";
			Window.titleContent=title;
			#endif
			
		}
		
		void OnGUI(){
			
			PowerUIEditor.HelpBox("Coming soon! We'll list out the modules you've got here.");
			
		}
		
	}
	
}