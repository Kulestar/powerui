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
	/// Displays a handy option for renaming font files so PowerUI can read them.
	/// </summary>
	
	public class RenameFontsContext:MonoBehaviour{
	
		/// <summary> Handy option for adding .bytes to font names.</summary>
		[MenuItem("Assets/PowerUI/Rename Font (Just adds .bytes)")]
		public static void RenameFonts(){
			
			// Get the selection:
			UnityEngine.Object[] assets=UnityEditor.Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
			
			foreach(UnityEngine.Object obj in assets){
			
				// Grab the path:
				string path=AssetDatabase.GetAssetPath(obj);
				
				// Log a message:
				Debug.Log("Adding .bytes to files in "+path+"..");
				
				if(string.IsNullOrEmpty(path)){
					continue;
				}
				
				// Dir or file?
				FileAttributes attribs=File.GetAttributes(path);

				// Is it a directory?
				if((attribs & FileAttributes.Directory)==FileAttributes.Directory){
				
					// Get the files:
					string[] files=Directory.GetFiles(path);
					
					for(int i=0;i<files.Length;i++){
						
						AddBytes(files[i]);
						
					}
					
				}else{
					
					// It's a file:
					AddBytes(path);
					
				}
				
			}
			
			AssetDatabase.Refresh();
			
		}
		
		/// <summary>Adds .bytes to the given file, if it needs it.</summary>
		private static void AddBytes(string file){
			
			string lowercase=file.ToLower();
			
			if(lowercase.EndsWith(".bytes") || lowercase.EndsWith(".txt") || lowercase.EndsWith("readme")){
				return;
			}
			
			if(lowercase.EndsWith(".ttf") || lowercase.EndsWith(".otf")){
				
				Debug.Log("Renaming asset..");
				File.Move(file,file+".bytes");
				
			}
			
		}
		
	}
	
}