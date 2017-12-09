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
using System.Collections;
using System.Collections.Generic;


namespace Loonim{
	
	/// <summary>
	/// Displays a list of supported texture nodes.
	/// </summary>
	
	public class ShaderSettings : EditorWindow{
	
		
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		/// <summary>The tickboxes.</summary>
		public static SettingTickbox[] Settings;
		
		
		/// <summary>Gets the graphics settings.</summary>
		public static SerializedObject GetGraphics(){
			return new SerializedObject(
				AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/GraphicsSettings.asset")[0]
			);
		}
		
		/// <summary>Adds/ removes all available Loonim shaders from the 'always included' list.
		/// To not exclude certain ones, just delete the files.</summary>
		public static void AddOrRemoveShaders(bool add){
			
			// Open up graphics settings:
			SerializedObject graphics=GetGraphics();
			
			// Get hold of the 'always included shaders' property:
			SerializedProperty prop=graphics.FindProperty("m_AlwaysIncludedShaders");
			
			// Always remove first:
			RemoveLoonimShaders(prop,true);
			
			if(add){
				
				// Add now:
				AddLoonimShaders(prop);
				
			}
			
			// Save:
			graphics.ApplyModifiedProperties();
			
		}
		
		/// <summary>Removes all Loonim/* shaders from the given property array of shaders.</summary>
		public static void AddLoonimShaders(SerializedProperty shaderArray){
			
			// Get all available shaders:
			string[] guids = AssetDatabase.FindAssets("t:Shader");
			
			for(int i=0;i<guids.Length;i++){
				
				// Get the path:
				string assetPath = AssetDatabase.GUIDToAssetPath( guids[i] );
				
				// Load the shader:
				Shader shader = AssetDatabase.LoadAssetAtPath(assetPath,typeof(Shader)) as Shader;
				
				if(shader==null){
					continue;
				}
				
				// If the shader starts with Loonim/, add it:
				if(!shader.name.StartsWith("Loonim/")){
					continue;
				}
				
				// Loonim shader! Add it now:
				shaderArray.arraySize++;
				
				// Create a new entry:
				SerializedProperty entry=shaderArray.GetArrayElementAtIndex(shaderArray.arraySize - 1);
				
				// Update it:
				entry.objectReferenceValue=shader;
				
			}
			
		}
		
		/// <summary>True if graphics settings already contains at least one Loonim shader.</summary>
		public static bool ContainsLoonimShaders(){
			
			// Open up graphics settings:
			SerializedObject graphics=GetGraphics();
			
			// Get hold of the 'always included shaders' property:
			SerializedProperty shaderArray=graphics.FindProperty("m_AlwaysIncludedShaders");
			
			int size=shaderArray.arraySize;
			
			// Backwards because we're removing entries:
			for(int i=size-1;i>=0;i--){
				
				// Get the entry:
				SerializedProperty entry=shaderArray.GetArrayElementAtIndex(i);
				
				// If the ref'd shader is a Loonim one..
				Shader shader=entry.objectReferenceValue as Shader;
				
				if(shader!=null && shader.name.StartsWith("Loonim/")){
					return true;
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>Removes all Loonim/* shaders from the given property array of shaders.</summary>
		public static void RemoveLoonimShaders(SerializedProperty shaderArray,bool removeNulls){
			
			int size=shaderArray.arraySize;
			
			// Backwards because we're removing entries:
			for(int i=size-1;i>=0;i--){
				
				// Get the entry:
				SerializedProperty entry=shaderArray.GetArrayElementAtIndex(i);
				
				// If the ref'd shader is a Loonim one..
				Shader shader=entry.objectReferenceValue as Shader;
				
				if(shader==null){
					if(removeNulls){
						shaderArray.DeleteArrayElementAtIndex(i);
					}
					continue;
				}
				
				if(shader.name.StartsWith("Loonim/")){
					
					// Same function twice is correct here!
					
					// Got one! Remove the value:
					shaderArray.DeleteArrayElementAtIndex(i);
					
					// And remove the array entry:
					shaderArray.DeleteArrayElementAtIndex(i);
					
				}
				
			}
			
		}
		
		// Add menu item named "Shader Settings" to the Loonim menu:
		[MenuItem("Window/Loonim/Shader Settings")]
		public static void ShowWindow(){
			
			Settings=new SettingTickbox[]{
				new SettingTickbox(
					"Include Shaders",
					"Loonim uses many shaders for its GPU based drawing pipeline. "+
					"On some platforms, Unity will strip the shaders out. "+
					"Tick the box in here to ensure they're always included. "+
					"If you don't want to include all of them, just delete the unwanted shader files.",
					delegate(SettingTickbox box){
						
						// Add or remove shaders:
						AddOrRemoveShaders(box.Active);
						
					}
				)
			};
			
			// Check if we contain at least 1 shader:
			Settings[0].Active=ContainsLoonimShaders();
			
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(ShaderSettings));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Shader Settings";
			#else
			GUIContent title=new GUIContent();
			title.text="Shader Settings";
			Window.titleContent=title;
			#endif
			
		}
		
		void OnGUI(){
			
			if(Settings==null){
				return;
			}
			
			foreach(SettingTickbox setting in Settings){
				
				// Draw it:
				setting.OnGUI();
				
			}
			
		}
		
	}
	
}