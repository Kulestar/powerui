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
	/// Displays some input settings for PowerUI.
	/// </summary>

	public class Upgrade : EditorWindow{
		
		// Add menu item named "Upgrading" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Upgrading")]
		public static void ShowWindow(){
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(Upgrade));
			
			// Give it a title:
			#if PRE_UNITY5_3
			window.title="Upgrade notices";
			#else
			GUIContent title=new GUIContent();
			title.text="Upgrade notices";
			window.titleContent=title;
			#endif

		}
		
		/// <summary>A timer which causes Update to run once every 2 seconds.</summary>
		private int UpdateTimer;
		/// <summary>The tickboxes.</summary>
		public SettingTickbox[] Settings;
		
		/// <summary>Creates the settings array.</summary>
		private void CreateSettings(){
			
			Settings=new SettingTickbox[]{
				new SettingTickbox("Hide the warning","ACCEPTED_DOM_NOTICE","I'm not upgrading or I've upgraded - hide the warning."),
				new SettingTickbox("Show required changes","LEGACY_DOM_UPGRADE","Removes the new APIs such that code which requires upgrading will fail to compile. Convert to setAttribute and getAttribute."),
				new SettingTickbox("Use the legacy mode","LEGACY_DOM","This will stop major javascript libraries like jQuery from working properly but you won't have to upgrade anything."),
			};
			
		}
		
		// Called at 100fps.
		void Update(){
			UpdateTimer++;
			
			if(UpdateTimer<100){
				return;
			}
			
			UpdateTimer=0;
			
			if(Settings==null){
				CreateSettings();
			}
			
			// Reduced now to once every second.
			foreach(SettingTickbox setting in Settings){
				
				// Check for the symbol:
				setting.Update();
				
			}
			
		}
		
		void OnGUI(){
			
			if(Settings==null){
				CreateSettings();
			}
			
			PowerUIEditor.HelpBox("If you're a new user, just hide the warning below. If you're upgrading from a previous version, you'll need to update all uses of element['attribute'] to the standard element.getAttribute and setAttribute instead. This is primarily used by custom elements. If you're not sure what you need to change, just tick the 'Show required changes' option below, and each line will error for you. If you don't want to use Javascript libraries anyway, use the legacy mode.");
			
			foreach(SettingTickbox setting in Settings){
				
				// Draw it:
				setting.OnGUI();
				
			}
			
		}
		
	}

}