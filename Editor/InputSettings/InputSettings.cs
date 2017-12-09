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

	public class InputSettings : EditorWindow{
		
		// Add menu item named "Input Settings" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Input Settings")]
		public static void ShowWindow(){
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(InputSettings));
			
			// Give it a title:
			#if PRE_UNITY5_3
			window.title="Input Settings";
			#else
			GUIContent title=new GUIContent();
			title.text="Input Settings";
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
				#if !UNITY_4_6 && !UNITY_4_7 && PRE_UNITY5
					new SettingTickbox("Isolate UI classes","IsolatePowerUI","This isolates the 'UI' class inside the PowerUI namespace just incase you've got a class called UI of your own."),
				#endif
				new SettingTickbox("Disable Input","NoPowerUIInput","Disable PowerUI's input handling. You'll need to use e.g. Input.ElementFromPoint to find an element at a particular point on the UI."),
				new SettingTickbox("Handle 3D Input","Enable3DInput","Enables PowerUI's input events to reach gameObjects when they don't hit a UI. This is an easy way to avoid 'Click through' and is required for 3D context menus. See '3D Input' on the wiki for usage."),
				new SettingTickbox("Manual 3D Input Linking","Input3DManualMode","By default PowerUI will search your gameObjects for suitable methods via reflection when you first dispatch an event to it. If you want slightly better performance, you can manually link them via gameObject.getEventTarget().addEventListener in e.g. an Awake method."),
				new SettingTickbox("Disable Unity UI Input","DisableUnityUIInput","Disable PowerUI checking for UnityUI hits. Ticking this will gain a little performance if you're not using PowerUI inside Unity UI's.")
				//new SettingTickbox("Focus Graph","PowerUIFGraph","Pressing the arrow keys will cause PowerUI to move focus to the nearest input.")
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
			
			foreach(SettingTickbox setting in Settings){
				
				// Draw it:
				setting.OnGUI();
				
			}
			
		}
		
	}

}