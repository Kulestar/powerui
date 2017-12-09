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
	
	public class SupportedNodes : EditorWindow{
		
		/// <summary>The list of available nodes.</summary>
		private static string[] Nodes;
		/// <summary>The selected node index in the Properties array.</summary>
		public static int SelectedIndex;
		/// <summary>The selected node, if any.</summary>
		public static TextureNodeMeta SelectedNode;
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		/// <summary>Instances of the supported nodes.</summary>
		private static List<TextureNodeMeta> Instances;
		
		
		/// <summary>Gets a list of all available nodes.</summary>
		public static List<TextureNodeMeta> GetNodes(bool reload){
			
			if(reload){
				Reload();
			}else if(Instances==null){
				Load();
			}
			
			return Instances;
			
		}
		
		// Add menu item named "Supported Nodes" to the Loonim menu:
		[MenuItem("Window/Loonim/Supported Nodes")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(SupportedNodes));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Supported Nodes";
			#else
			GUIContent title=new GUIContent();
			title.text="Supported Nodes";
			Window.titleContent=title;
			#endif
			
			Load();
			
		}
		
		/// <summary>Forces a reload. It's a Very Bad Idea to call this whilst the game is running!</summary>
		public static void Reload(){
			
			Nodes=null;
			
			// If your game is running, here be gremlins:
			TextureNodes.All=null;
			
			Load();
			
		}
		
		/// <summary>Load all available nodes if they've not been loaded yet.</summary>
		public static void Load(){
			
			if(Nodes==null){
				
				// Load the nodes:
				LoadAvailableNodes();
				
			}
			
		}
		
		void HelpBox(string message){
			
			#if !PRE_UNITY3_5
			UnityEditor.EditorGUILayout.HelpBox(message,MessageType.Info);
			#endif
			
		}
		
		void OnGUI(){
			
			HelpBox("Here's the nodes that Loonim is currently recognising.");
			
			if(Nodes==null){
				Load();
			}
			
			
			// Dropdown list:
			int selected=EditorGUILayout.Popup(SelectedIndex,Nodes);
			
			if(selected!=SelectedIndex || SelectedNode==null){
				SelectedIndex=selected;
				LoadSelected();
			}
			
			// Detailed information about the selected node:
			if(SelectedNode!=null){
				
				// Show the name:
				EditorGUILayout.LabelField(SelectedNode.Name, EditorStyles.boldLabel);
				
				if(!string.IsNullOrEmpty(SelectedNode.Name)){
					
					HelpBox("To find the source file, try searching for this:");
					
					EditorGUILayout.SelectableLabel(SelectedNode.ID+"-"+SelectedNode.Name+".cs");
					
				}
				
			}
			
		}
		
		/// <summary>Gets hold of the selected node and figures out the approximate file name.</summary>
		private static void LoadSelected(){
			
			// Get the node:
			SelectedNode=Instances[SelectedIndex];
			
		}
		
		/// <summary>All nodes (set by LoadAvailableNodes).</summary>
		private static Dictionary<int,TextureNodeMeta> AllNodes;
		
		/// <summary>Loads the set of nodes.</summary>
		public static void LoadAvailableNodes(){
			
			// Refresh the node set:
			TextureNodes.All=null;
			TextureNodes.Get(0);
			
			// Get the full list:
			AllNodes=TextureNodes.All;
			
			
			// The result list:
			List<TextureNodeMeta> meta=new List<TextureNodeMeta>();
			
			// For each one..
			foreach(KeyValuePair<int,TextureNodeMeta> kvp in AllNodes){
				
				// Add it to the list:
				meta.Add(kvp.Value);
				
			}
			
			// Sort it:
			meta.Sort(delegate(TextureNodeMeta a,TextureNodeMeta b){
				
				return a.Name.CompareTo(b.Name);
				
			});
			
			// Ok! Time to create a textual list:
			Instances=meta;
			
			string[] names=new string[meta.Count];
			
			for(int i=0;i<meta.Count;i++){
				names[i]=meta[i].Name;
			}
			
			Nodes=names;
			
		}
		
	}
	
}