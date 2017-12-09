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
using Dom;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;


namespace PowerUI{
	
	/// <summary>
	/// Displays options for enabling Emoji. All this does is download a bunch of default graphics.
	/// </summary>
	
	public class EmojiSettings : EditorWindow{
	
		/// <summary>The current decompression status. Emoji icons are received as a unitypackage, decompressed and then have import settings applied.</summary>
		public static int Status;
		/// <summary>Tracks refreshing of the window when a download is in progress.</summary>
		public static int RefreshTimer;
		/// <summary>True when a download is in progress.</summary>
		public static bool IsDownloading;
		/// <summary>A static reference to the currently open Emoji window, if there is one.</summary>
		public static EditorWindow Window;
		/// <summary>The latest download request.</summary>
		private static XMLHttpRequest Request;
		/// <summary>The editable path that emoji icons will be saved in.</summary>
		public static string DownloadPath="Resources/Characters/";
		
		// Add menu item named "Emoji" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Emoji")]
		public static void ShowWindow(){
			Status=0;
			IsDownloading=false;
			
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(EmojiSettings));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Emoji Settings";
			#else
			GUIContent title=new GUIContent();
			title.text="Emoji Settings";
			Window.titleContent=title;
			#endif

		}
		
		void Update(){
			RefreshTimer++;
			
			if(Status==2){
				TryImportSettings();
			}
			
			if(RefreshTimer==5){
				RefreshTimer=0;
				Repaint();
			}
		}
		
		void OnGUI(){
			
			PowerUIEditor.HelpBox("By default to prevent bloat, Emoji images are not included in the project. "+
								"Either add your own to Resources/Characters/ (e.g. 1f004.png by default), or download the Phantom Open Emoji set below.");
			
			DownloadPath=EditorGUILayout.TextField("Save to..",DownloadPath);
			
			if(IsDownloading){
				GUILayout.Label("Downloading set.. ("+(Request.progress*100f)+"%)",EditorStyles.boldLabel);
			}else if(Status==1){
				GUILayout.Label("Unpacking..",EditorStyles.boldLabel);
			}else if(Status==2){
				GUILayout.Label("Applying import settings..",EditorStyles.boldLabel);
			}else if(Status==3){
				GUILayout.Label("Import successful!",EditorStyles.boldLabel);
			}else if(GUILayout.Button("Download Icons")){
				DownloadIcons();
			}
			
		}
		
		/// <summary>Attempts to apply import settings to images in icon path.</summary>
		public static void TryImportSettings(){
			// Grab the files in icon path:
			string[] fileSet=Directory.GetFiles(IconPath);
			int count=fileSet.Length;
			
			if(count<=1){
				return;
			}
			
			Status=3;
			
			// Finally, update all images with the required import settings:
			ImageImport.ApplyToAll(fileSet);
		}
		
		/// <summary>Downloads the icons now.</summary>
		public static void DownloadIcons(){
			if(IsDownloading){
				return;
			}
			IsDownloading=true;
			
			Request=new XMLHttpRequest();
			
			Request.open("get","http://powerui.kulestar.com/emoji/phantomOpenEmoji.unitypackage");
			
			Request.onreadystatechange=delegate(Dom.Event e){
				
				if(Request.readyState==4){
					
					IsDownloading=false;
					
					if(Request.ok){
					
						// Create the directory, if needed:
						if(!Directory.Exists(IconPath)){
							Directory.CreateDirectory(IconPath);
						}
						
						// Write out the bytes:
						RemovePackage();
						
						File.WriteAllBytes(IconPath+"phantomOpenEmoji.unitypackage",Request.responseBytes);
						// Save the changes:
						AssetDatabase.Refresh();
						
						Status=1;
						if(Window!=null){
							Window.Repaint();
						}
						
						// Next, pull in the package:
						AssetDatabase.ImportPackage(IconPath+"phantomOpenEmoji.unitypackage",false);
						
						// Next, remove it:
						RemovePackage();
						
						// Make sure the database is up to date:
						AssetDatabase.Refresh();
						// And save all the changes:
						AssetDatabase.SaveAssets();
						
						Status=2;
						if(Window!=null){
							Window.Repaint();
						}
						
					}else{
						Debug.LogError("HTTP Error getting "+Request.location.absolute+": "+Request.statusCode);
					}
					
					if(Window!=null){
						Window.Repaint();
					}
					
				}
				
			};
			
			// Send it off:
			Request.send();
		}
		
		/// <summary>Gets the path that downloads will be saved to.</summary>
		public static string IconPath{
			get{
				string result=DownloadPath.Replace("\\","/");
				if(!result.EndsWith("/")){
					result+="/";
				}
				return "Assets/"+result;
			}
		}
		
		/// <summary>Deletes the downloaded package if it exists.</summary>
		public static void RemovePackage(){
			if(File.Exists(IconPath+"phantomOpenEmoji.unitypackage")){
				// It exists - delete it:
				AssetDatabase.DeleteAsset(IconPath+"phantomOpenEmoji.unitypackage");
				// Save the asset changes:
				AssetDatabase.Refresh();
			}
		}
		
	}
	
}