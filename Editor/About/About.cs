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

using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;

namespace PowerUI{

	/// <summary>
	/// Displays some info about PowerUI.
	/// </summary>

	public class About : EditorWindow{
		
		// Add menu item named "About" to the PowerUI menu:
		[MenuItem("Window/PowerUI/About")]
		public static void ShowWindow(){
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(About));
			
			// Give it a title:
			#if PRE_UNITY5_3
			window.title="About PowerUI";
			#else
			GUIContent title=new GUIContent();
			title.text="About PowerUI";
			window.titleContent=title;
			#endif

		}
		
		void OnGUI(){
			
			PowerUIEditor.HelpBox("PowerUI is a large UI framework which renders HTML and CSS.\r\n\r\nHelp: https://powerui.kulestar.com/\r\n\r\nVersion: "+UI.Version);
			
		}
		
	}

}