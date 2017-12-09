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
using System.Collections;
using System.Collections.Generic;
using Pico;


namespace PowerUI{

	/// <summary>
	/// Displays options for precompiling PowerUI. Highly recommended you use this!
	/// </summary>

	public class PrecompileSettings : EditorWindow{
		
		// Add menu item named "Precompile" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Precompile")]
		public static void ShowWindow(){
			
			// Show existing window instance. If one doesn't exist, make one.
			EditorWindow window=EditorWindow.GetWindow(typeof(PrecompileSettings));
			
			// Give it a title:
			#if PRE_UNITY5_3
			window.title="Precompile";
			#else
			GUIContent title=new GUIContent();
			title.text="Precompile";
			window.titleContent=title;
			#endif
			
		}
		
		/// <summary>The precompiled PowerUI module.</summary>
		private Module Module;
		
		
		/// <summary>Sets up the module.</summary>
		private void GetModule(){
			
			// Create a precompileable module:
			Module=new Module("PowerUI");
			
			// Got source folders?
			if(Module.SourceFolders.Count==0){
				
				// Find PowerUI:
				string powerUIPath=PowerUIEditor.GetPowerUIPath();
				
				// Add the source folder(s) now:
				// (We don't precompile the managers because that would break references).
				Module.SourceFolders.Add(powerUIPath+"/Source");
				
			}
			
		}
		
		void OnGUI(){
			
			if(Module==null){
				GetModule();
			}
			
			// The precompile PowerUI tickbox:
			bool isPrecompiled=Module.Precompiled;
			bool tickedPrecompiled=EditorGUILayout.Toggle("Precompile PowerUI",isPrecompiled);
			PowerUIEditor.HelpBox("Precompiles PowerUI. It will appear to freeze - backup first! You must also recompile each time you change platforms - see the Precompiler entry on the PowerUI wiki.");
			
			if(isPrecompiled!=tickedPrecompiled){
				
				// Compile/ revert:
				if(tickedPrecompiled){
					
					// Compile:
					Module.Compile();
					
				}else{
					
					// Revert:
					Module.Revert();
					
				}
				
			}
			
			bool isEditorMode=Module.EditorMode;
			bool tickedEditorMode=EditorGUILayout.Toggle("Editor Mode",isEditorMode);
			PowerUIEditor.HelpBox("Compile with the UNITY_EDITOR flag. Almost always leave this unchecked.");
			
			if(isEditorMode!=tickedEditorMode){
				
				// Update module value:
				Module.EditorMode=tickedEditorMode;
				
				if(Module.Precompiled){
					// Compile the module:
					Module.Compile();
				}
				
			}
			
			if(isPrecompiled && GUILayout.Button("Recompile")){
				
				// Compile the module now:
				Module.Compile();
				
			}
			
		}
		
	}

}