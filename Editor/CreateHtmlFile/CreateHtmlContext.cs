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
	/// Useful context option for creating a new HTML file.
	/// </summary>
	
	public class CreateHtmlContext:MonoBehaviour{
	
		/// <summary>Creates a new HTML file.</summary>
		[MenuItem("Assets/Create/HTML File")]
		[MenuItem("Assets/PowerUI/New HTML File")]
		public static void CreateHtmlFile(){
			
			// Get the selection:
			UnityEngine.Object[] assets=UnityEditor.Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
			
			foreach(UnityEngine.Object obj in assets){
				
				// Grab the path:
				string path=AssetDatabase.GetAssetPath(obj);
				
				if(string.IsNullOrEmpty(path)){
					continue;
				}
			
				// Dir or file?
				FileAttributes attribs=File.GetAttributes(path);

				// Is it a directory?
				if((attribs & FileAttributes.Directory)==FileAttributes.Directory){
					
					if(!File.Exists(path+"/MyNewHtml.html")){
						// Write a blank file:
						File.WriteAllText(path+"/MyNewHtml.html","");
					}else{
						// Count until we hit one that doesn't exist.
						int count=1;
						
						while(File.Exists(path+"/MyNewHtml-"+count+".html")){
							count++;
						}
						
						// Write it out now:
						File.WriteAllText(path+"/MyNewHtml-"+count+".html","");
						
					}
					
					break;
					
				}
				
			}
			
			// Refresh the database:
			AssetDatabase.Refresh();
			
		}
		
	}
	
}