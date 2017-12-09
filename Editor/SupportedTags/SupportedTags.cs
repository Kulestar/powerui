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
	/// Displays a list of supported HTML/SVG tags.
	/// </summary>
	
	public class SupportedTags : EditorWindow{
		
		/// <summary>The list of available tags pulled from Dom.
		/// See Dom.TagHandlers for the underlying APIs.</summary>
		private static string[] Tags;
		/// <summary>The selected tag index in the Properties array.</summary>
		public static int SelectedTagIndex;
		/// <summary>The selected tag, if any.</summary>
		public static Type SelectedTag;
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		public static bool NonStandard;
		public static string TagNamespace;
		public static string TagName;
		private static List<Element> Instances;
		
		
		// Add menu item named "Supported Tags" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Supported Tags")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(SupportedTags));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Supported Tags";
			#else
			GUIContent title=new GUIContent();
			title.text="Supported Tags";
			Window.titleContent=title;
			#endif
			
			Load();
			
		}
		
		/// <summary>Forces a reload. It's a Very Bad Idea to call this whilst the game is running!</summary>
		public static void Reload(){
			
			Tags=null;
			
			// If your game is running, here be gremlins:
			Dom.Start.Reset();
			
			Load();
			
		}
		
		/// <summary>Load all available tags if they've not been loaded yet.</summary>
		public static void Load(){
			
			if(Tags==null){
				
				// Load the tags:
				LoadAvailableTags();
				
			}
			
		}
		
		void OnGUI(){
			
			PowerUIEditor.HelpBox("Here's the XML tags (HTML, SVG etc) that PowerUI is currently recognising.");
			
			if(Tags==null){
				Load();
			}
			
			// Dropdown list:
			int selected=EditorGUILayout.Popup(SelectedTagIndex,Tags);
			
			if(selected!=SelectedTagIndex || SelectedTag==null){
				SelectedTagIndex=selected;
				LoadSelected();
			}
			
			// Detailed information about the selected tag:
			if(SelectedTag!=null){
				
				// Show the name:
				EditorGUILayout.LabelField(TagName, EditorStyles.boldLabel);
				
				if(TagNamespace!=""){
					
					EditorGUILayout.LabelField("In namespace "+TagNamespace);
					
				}
				
				if(!string.IsNullOrEmpty(TagName)){
					
					PowerUIEditor.HelpBox("To find the source file, just search for its name in your project:");
					
					EditorGUILayout.SelectableLabel(TagName);
					
				}
				
				if(NonStandard){
					
					PowerUIEditor.WarnBox("This tag is non-standard or not on a standards track.");
					
				}else if(TagNamespace=="xhtml"){
					
					if(GUILayout.Button("View Mozilla Docs")){
						
						Application.OpenURL("https://developer.mozilla.org/en-US/docs/Web/HTML/Element/"+TagName);
					
					}
					
				}else if(TagNamespace=="svg"){
					
					if(GUILayout.Button("View Mozilla Docs (SVG)")){
						
						Application.OpenURL("https://developer.mozilla.org/en-US/docs/Web/SVG/Element/"+TagName);
					
					}
					
				}else if(TagNamespace=="lang"){
					
					PowerUIEditor.HelpBox("Used by localisation files.");
					
				}else{
					
					PowerUIEditor.HelpBox("Custom XML namespace.");
					
				}
				
			}
			
		}
		
		/// <summary>Gets hold of the selected tag and figures out the approximate file name.</summary>
		private static void LoadSelected(){
			
			// Get the tag name:
			string name=Tags[SelectedTagIndex];
			Element inst=Instances[SelectedTagIndex];
			
			// Get the actual handler:
			AllTags.TryGetValue(name,out SelectedTag);
			
			if(SelectedTag==null){
				TagName="";
				TagNamespace="";
				return;
			}
			
			string[] pieces=name.Split(':');
			
			TagNamespace=pieces[0];
			TagName=pieces[1];
			NonStandard=inst.NonStandard;
		}
		
		/// <summary>All tags (set by LoadAvailableTags).</summary>
		private static Dictionary<string,Type> AllTags;
		
		/// <summary>Loads the set of tags.</summary>
		public static void LoadAvailableTags(){
			
			// Refresh the tag engine:
			Dom.Start.Now();
			
			// Get the full list:
			AllTags=Dom.TagHandlers.GetAll();
			
			// The result list:
			List<Element> instances=new List<Element>();
			
			// For each one..
			foreach(KeyValuePair<string,Type> kvp in AllTags){
				
				// Get the handler:
				Type handler=kvp.Value;
				
				// Instance to check:
				Element inst=Activator.CreateInstance(handler) as Element;
				
				if(inst==null){
					// E.g. a text node.
					continue;
				}
				
				inst.Tag=kvp.Key;
				
				// Skip internal tags:
				if(inst.Internal){
					continue;
				}
				
				// Add it to the list:
				instances.Add(inst);
				
			}
			
			// Sort it:
			instances.Sort(delegate(Element a,Element b){
				
				return a.Tag.CompareTo(b.Tag);
				
			});
			
			// Ok! Time to create a textual list:
			Instances=instances;
			
			string[] tags=new string[instances.Count];
			
			for(int i=0;i<instances.Count;i++){
				tags[i]=instances[i].Tag;
			}
			
			Tags=tags;
			
		}
		
	}
	
}