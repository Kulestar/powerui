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
	/// Displays a list of supported CSS entities.
	/// </summary>
	
	public class SupportedCSS : EditorWindow{
		
		/// <summary>The list of available CSS properties pulled from Spark.
		/// See Css.CssProperties for the underlying APIs.</summary>
		private static string[] Properties;
		/// <summary>The selected property index in the Properties array.</summary>
		public static int SelectedPropertyIndex;
		/// <summary>The selected property, if any.</summary>
		public static CssProperty SelectedProperty;
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		/// <summary>Approximate property file name.</summary>
		public static string PropertyFile;
		
		
		// Add menu item named "Supported CSS" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Supported CSS")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(SupportedCSS));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Supported CSS";
			#else
			GUIContent title=new GUIContent();
			title.text="Supported CSS";
			Window.titleContent=title;
			#endif
			
			Load();
			
		}
		
		/// <summary>Forces a reload. It's a Very Bad Idea to call this whilst the game is running!</summary>
		public static void Reload(){
			
			Properties=null;
			
			// If your game is running, here be gremlins!
			// It recreates all CSS properties, resulting in multiple instances:
			Css.Start.Reset();
			
			Load();
			
		}
		
		/// <summary>Load all available properties if they've not been loaded yet.</summary>
		public static void Load(){
			
			if(Properties==null){
				
				// Start the CSS engine:
				Css.Start.Now();
				
				// Load the CSS properties:
				LoadAvailableProperties();
				
			}
			
		}
		
		void OnGUI(){
			
			PowerUIEditor.HelpBox("Here's the CSS properties that PowerUI is currently recognising.");
			
			if(Properties==null){
				Load();
			}
			
			// Dropdown list:
			int selected=EditorGUILayout.Popup(SelectedPropertyIndex,Properties);
			
			if(selected!=SelectedPropertyIndex || SelectedProperty==null){
				SelectedPropertyIndex=selected;
				LoadSelected();
			}
			
			// Detailed information about the selected property:
			if(SelectedProperty!=null){
				
				// Show the name:
				EditorGUILayout.LabelField(SelectedProperty.Name, EditorStyles.boldLabel);
				
				string hostName;
				
				// Get as a composite property:
				CssCompositeProperty composite=SelectedProperty as CssCompositeProperty;
				
				if(composite!=null){
					
					// It's a composite property (e.g. font, animation etc).
					// They set multiple properties at once.
					
					EditorGUILayout.LabelField("Composite property");
					
					hostName=SelectedProperty.Name;
					
				}else if(SelectedProperty.IsAlias){
					
					// Get as an alias:
					CssPropertyAlias alias=SelectedProperty as CssPropertyAlias;
					
					// e.g. color-r is an alias of color.
					
					if(alias.Target==null){
						
						// Hmm!
						hostName=SelectedProperty.Name;
						
						// It's not an alias property
						EditorGUILayout.LabelField("Not an alias");
						
					}else{
						
						// Get the target of the alias:
						string aliasedTo=alias.Target.Name;
						
						// It's an alias property
						EditorGUILayout.LabelField("Alias of "+aliasedTo);
						
						hostName=aliasedTo;
						
					}
					
				}else{
					
					hostName=SelectedProperty.Name;
					
					// It's not an alias property
					EditorGUILayout.LabelField("Not an alias/ composite");
					
				}
				
				if(!string.IsNullOrEmpty(PropertyFile)){
					
					PowerUIEditor.HelpBox("To find the source file, search for its name in camel case, like this:");
					
					EditorGUILayout.SelectableLabel(PropertyFile);
					
				}
				
				if(SelectedProperty.NonStandard){
					
					PowerUIEditor.WarnBox("This property is non-standard or not on a standards track.");
					
				}else if(SelectedProperty.NamespaceName=="svg"){
					
					if(GUILayout.Button("View Mozilla Docs (SVG)")){
						
						Application.OpenURL("https://developer.mozilla.org/en-US/docs/Web/SVG/Attribute/"+hostName);
					
					}
					
				}else{
					
					if(GUILayout.Button("View Mozilla Docs")){
						
						Application.OpenURL("https://developer.mozilla.org/en-US/docs/Web/CSS/"+hostName);
					
					}
					
				}
				
			}
			
		}
		
		/// <summary>Converts e.g. color-overlay to colorOverlay.</summary>
		public static string ToCamelCase(string property){
			
			// Remove prefix and trim:
			property=property.Replace("-spark-","").Replace("-"," ").Trim().ToLower();
			
			// Uppercase each word:
			property=System.Globalization.CultureInfo.InvariantCulture.TextInfo.ToTitleCase(property);
			
			if(property==""){
				return "";
			}
			
			// Drop the first one:
			property=char.ToLower(property[0])+property.Substring(1);
			
			// strip spaces and done!
			return property.Replace(" ","");
			
		}
		
		/// <summary>Gets hold of the selected property and figures out the approximate file name.</summary>
		private static void LoadSelected(){
			
			// Get the property name:
			string name=Properties[SelectedPropertyIndex];
			
			// Get the actual property:
			CssProperties.All.TryGetValue(name,out SelectedProperty);
			
			if(SelectedProperty==null){
				PropertyFile=null;
				return;
			}
			
			if(SelectedProperty.IsAlias){
				
				// e.g. color-r is an alias of color.
				
				// Get as an alias:
				CssPropertyAlias alias=SelectedProperty as CssPropertyAlias;
				
				if(alias==null || alias.Target==null){
					return;
				}
				
				// Get the target of the alias:
				string aliasedTo=alias.Target.Name;
				
				PropertyFile=ToCamelCase(aliasedTo);
				
			}else{
				
				// Same as property:
				PropertyFile=ToCamelCase(SelectedProperty.Name);
				
			}
			
		}
		
		/// <summary>Loads the set of CSS properties.</summary>
		public static void LoadAvailableProperties(){
			
			// The result list:
			List<string> properties=new List<string>();
			
			// For each one..
			foreach(KeyValuePair<string,CssProperty> kvp in CssProperties.All){
				
				// Get the property:
				CssProperty property=kvp.Value;
				
				// Skip internal properties:
				if(property.Internal){
					continue;
				}
				
				// Add it to the list:
				properties.Add(kvp.Key);
				
			}
			
			// Sort it:
			properties.Sort();
			
			// Ok!
			Properties=properties.ToArray();
			
		}
		
	}
	
}