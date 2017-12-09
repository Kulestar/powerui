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
	
	public class CreateSimpleUIContext:MonoBehaviour{
	
		/// <summary>Creates a new HTML file.</summary>
		[MenuItem("GameObject/UI/PowerUI/Main UI (One per scene)")]
		[MenuItem("Assets/PowerUI/Create Main UI (One per scene)")]
		public static void CreateSimpleUI(){
			
			// Already got one?
			if(GameObject.Find("main-ui")!=null){
				Debug.LogError("Only one main UI instance is required per scene.");
				return;
			}
			
			// Create a gameobject:
			GameObject mainUI=new GameObject();
			
			// Give it a name:
			mainUI.name="main-ui";
			
			// Attach a simple UI manager to it:
			mainUI.AddComponent<PowerUI.Manager>();
			
		}
		
	}
	
}