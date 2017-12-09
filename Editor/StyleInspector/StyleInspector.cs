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
	/// A simple stylesheet inspector.
	/// </summary>
	
	public class StyleInspector : EditorWindow{
		
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		
		
		// Add menu item named "Stylesheet Inspector" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Stylesheet Inspector")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(StyleInspector));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Stylesheet Inspector";
			#else
			GUIContent title=new GUIContent();
			title.text="Stylesheet Inspector";
			Window.titleContent=title;
			#endif
			
		}
		
		private string ComputedNodeData;
		private string[] SheetNames;
		private int SheetIndex;
		/// <summary>The document entry being viewed.</summary>
		public static DocumentEntry Entry;
		
		
		// Called at 100fps.
		void Update(){
			
			if(!DomInspector.DocumentsAvailable){
				
				// Reload:
				DomInspector.GetDocuments(true);
				
			}
			
		}
		
		internal void BuildString(Css.StyleSheet sheet){
			
			string result="";
			
			foreach(Css.Rule rule in sheet.rules){
				
				Css.StyleRule style=rule as Css.StyleRule;
				
				if(style==null){
					result+=rule.GetType()+"\r\n";
				}else{
					result+=style.Selector.ToString()+"\r\n";
				}
				
			}
			
			ComputedNodeData=result;
			
		}
		
		public void RebuildSheets(ReflowDocument reflowDoc){
			
			// List all stylesheets in the doc.
			int x=1;
			string[] names=new string[reflowDoc.styleSheets.Count];
			int i=0;
			
			foreach(StyleSheet sheet in reflowDoc.styleSheets){
				
				string name=sheet.href;
				
				if(name==null){
					name="Inline stylesheet "+x;
					x++;
				}
				
				names[i++]=DomInspector.ListableName(name);
				
			}
			
			SheetNames=names;
			
		}
		
		void OnGUI(){
			
			// Doc dropdown:
			DocumentEntry entry=DomInspector.DocumentDropdown(Entry);
			
			if(entry==null){
				return;
			}
			
			ReflowDocument reflowDoc=entry.Document as ReflowDocument;
			
			if(reflowDoc==null){
				Entry=entry;
				PowerUIEditor.HelpBox("Not a suitable document (it can't be styled).");
				return;
			}
			
			if(Entry!=entry){
				// Changed!
				Entry=entry;
				ComputedNodeData=null;
				
				// Rebuild list of stylesheets.
				RebuildSheets(reflowDoc);
			}
			
			// Draw a refresh button:
			if(GUILayout.Button("Refresh sheets")){
				
				// Refresh now:
				RebuildSheets(reflowDoc);
				
			}
			
			// Draw dropdown list now!
			int selectedIndex=EditorGUILayout.Popup(SheetIndex,SheetNames);
			
			if(selectedIndex!=SheetIndex || ComputedNodeData==null){
				
				SheetIndex=selectedIndex;
				
				try{
					BuildString(reflowDoc.styleSheets[SheetIndex]);
				}catch(Exception e){
					ComputedNodeData="<b>Error whilst building node data</b>\r\n"+e.ToString();
				}
				
			}
			
			GUILayout.Label(ComputedNodeData);
			
		}
		
	}
	
}