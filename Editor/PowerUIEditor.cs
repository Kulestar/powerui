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

using System.IO;
using UnityEditor;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A class used to locate where the PowerUI folder is in the project.
	/// </summary>

	public static class PowerUIEditor{
		
		public static void WarnBox(string message){
			HelpBox(message,"warning");
		}
		
		public static void ErrorBox(string message){
			HelpBox(message,"error");
		}
		
		public static void HelpBox(string message){
			HelpBox(message,"info");
		}
		
		public static void HelpBox(string message,string type){
			#if !PRE_UNITY3_5
			
			MessageType mType;
			
			if(type=="info"){
				mType=MessageType.Info;
			}else if(type=="error"){
				mType=MessageType.Error;
			}else{
				mType=MessageType.Warning;
			}
			
			UnityEditor.EditorGUILayout.HelpBox(message,mType);
			#endif
		}
		
		private static string _PowerUIPath;
		
		/// <summary>Finds where the PowerUI tree is.</summary>
		public static string GetPowerUIPath(){
			if(_PowerUIPath == null){
				// Let's go looking for it!
				_PowerUIPath = FindPowerUIIn("Assets");
			}
			return _PowerUIPath;
		}
		
		/// <summary>Looks for PowerUI in the given folder.</summary>
		public static string FindPowerUIIn(string folder){
			
			if(Directory.Exists(folder+"/PowerUI/Source")){
				return folder+"/PowerUI";
			}
			
			// Check in subfolders:
			string[] subfolders=Directory.GetDirectories(folder);
			
			// For each one..
			for(int i=0;i<subfolders.Length;i++){
				// Check if it's in there:
				string path=FindPowerUIIn(subfolders[i]);
				
				if(path!=null){
					// Yep! We got it.
					return path;
				}
				
			}
			
			return null;
			
		}
		
	}
	
}