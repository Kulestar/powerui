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
	/// A UI inside Unity for running Gulptasks (on Node.js).
	/// </summary>
	
	public class Gulptask : EditorWindow{
		
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		
		
		// Add menu item named "Gulptask" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Gulptask")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(Gulptask));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Gulptask";
			#else
			GUIContent title=new GUIContent();
			title.text="Gulptask";
			Window.titleContent=title;
			#endif
			
		}
		
		private string TaskList = null;
		private bool CheckingInstall = true;
		/// <summary>Reference to Node.js</summary>
		private NodeJS Node;
		
		public Gulptask(){
		}
		
		public void OnEnable(){
			
			// Ensure gulptask is installed:
			NodeJS installerNode = new NodeJS();
			
			if(installerNode.exists("gulp")){
				CheckingInstall = false;
				ListTasks();
			}else{
				installerNode.addEventListener("exit", delegate(Dom.Event e){
					CheckingInstall = false;
					ListTasks();
				});
				
				// Start the install now:
				installerNode.install("gulp");
			}
			
		}
		
		public void RunTask(){
			RunTask("");
		}
		
		public void RunTask(string name){
			NodeJS tasks = new NodeJS();
			
			tasks.addEventListener("exit", delegate(Dom.Event e){
				TaskList = (e as NodeEvent).stdOutput;
			});
			
			// Run the task by its name (blank runs default task):
			tasks.run("gulp", name);
		}
		
		public void ListTasks(){
			
			NodeJS tasks = new NodeJS();
			
			tasks.addEventListener("exit", delegate(Dom.Event e){
				TaskList = (e as NodeEvent).stdOutput;
			});
			
			// Run the task list gulp proc:
			tasks.run("gulp", "--tasks");
			
		}
		
		void OnGUI(){
			
			if(CheckingInstall){
				GUILayout.Label("Installing gulptask into your project..");
				return;
			}
			
			if(TaskList == null){
				GUILayout.Label("Loading task list..");
			}else{
				GUILayout.Label(TaskList);
			}
		}
		
	}
	
}