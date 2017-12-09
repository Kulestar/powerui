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
using Dom;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Displays all requests that PowerUI is making.
	/// </summary>
	
	public class NetworkInspector : EditorWindow{
		
		/// <summary>Persist between plays.</summary>
		public static bool Persist=false;
		/// <summary>The max number of displayed requests.</summary>
		public static int BufferSize=200;
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		
		// Add menu item named "Network Inspector" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Network Inspector")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(NetworkInspector));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Network Inspector";
			#else
			GUIContent title=new GUIContent();
			title.text="Network Inspector";
			Window.titleContent=title;
			#endif
			
		}
		
		/// <summary>A ring buffer of the requests caught by this inspector.</summary>
		public int Fill;
		public int Progress;
		public Vector2 ScrollPosition;
		public ContentPackage[] Requests;
		
		public void Clear(){
			Requests=null;
			Progress=0;
			Fill=0;
		}
		
		void OnEnable(){
			
			// Clear requests:
			if(!Persist){
				Clear();
			}
			
			FileProtocol.OnRequestStarted=delegate(EventTarget et){
				
				ContentPackage cp=et as ContentPackage;
				
				if(cp!=null){
					
					// Create set:
					if(Requests==null){
						Requests=new ContentPackage[BufferSize];
					}
					
					// Add to the buffer:
					Requests[Progress]=cp;
					
					// Update fill:
					if(Fill!=Requests.Length){
						Fill++;
					}
					
					// Wrap progress:
					Progress++;
					if(Progress==Requests.Length){
						Progress=0;
					}
					
				}
				
			};
			
		}
		
		void OnGUI(){
			
			if(Requests==null){
				PowerUIEditor.HelpBox("Monitors all requests being made via PowerUI. Use either XMLHttpRequest or e.g. DataPackage to list here (and note that both work with all your supported protocols, such as 'resources://').");
				return;
			}
			
			ScrollPosition=EditorGUILayout.BeginScrollView(ScrollPosition);
			
			// For each record..
			int index=Progress-Fill;
			
			if(index<0){
				index+=Requests.Length;
			}
			
			for(int i=0;i<Fill;i++){
				
				// Get the request:
				ContentPackage req=Requests[index];
				
				// Just a rough location label for now:
				GUILayout.Label(req.location.absolute);
				
				int readyState=req.readyState;
				
				string stateMessage="";
				switch(readyState){
					case 1:
						stateMessage="open";
					break;
					case 2:
						stateMessage="waiting";
					break;
					case 3:
						stateMessage="headers";
					break;
					case 4:
						stateMessage="finished";
					break;
				}
				
				if(readyState==4){
					stateMessage+=" ("+req.statusCode+")";
				}
				
				GUILayout.Label("State: "+stateMessage);
				
				// Move to next index:
				index++;
				
				if(index==Requests.Length){
					index=0;
				}
				
			}
			
			EditorGUILayout.EndScrollView();
			
		}
		
	}
	
}