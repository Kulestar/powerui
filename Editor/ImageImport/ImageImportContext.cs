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

using UnityEditor;
using UnityEngine;
using System.IO;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Displays options for enabling Emoji. All this does is download a bunch of default graphics.
	/// </summary>
	
	public class ImageImportContext:MonoBehaviour{
	
		/// <summary>Automatically applies image settings to the selection.
		/// If it's a folder, it applies to all inside it.</summary>
		[MenuItem("Assets/PowerUI/Auto Image Settings")]
		public static void AutoImageSettings(){
			
			// Get the selection:
			UnityEngine.Object[] assets=UnityEditor.Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
			
			foreach(UnityEngine.Object obj in assets){
			
				// Grab the paths:
				string path=AssetDatabase.GetAssetPath(obj);
				
				// Log a message:
				Debug.Log("Automatically applying import settings to "+path+"..");
				
				if(string.IsNullOrEmpty(path)){
					continue;
				}
				
				// Note: Internally checks if path is a directory.
				ImageImport.Apply(path);
				
			}
			
		}
		
	}
	
}