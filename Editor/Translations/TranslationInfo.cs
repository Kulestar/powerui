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

using System;
using System.IO;
using UnityEditor;
using Json;
using Dom;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>A delegate used when a translation is completed.</summary>
	/// <param name="translation">Information about the translation.</param>
	public delegate void OnTranslated(TranslationInfo translation);
	
	
	/// <summary>
	/// Holds information about a particular translation.
	/// </summary>
	
	public class TranslationInfo{
		
		/// <summary>An error that occured.</summary>
		public string Error;
		/// <summary>The lower case language code to translate to, e.g. fr.</summary>
		public string ToCode;
		/// <summary>The lower case language code to translate from, e.g. en.</summary>
		public string FromCode;
		/// <summary>A delegate triggered when the translation is complete.</summary>
		public OnTranslated OnComplete;
		/// <summary>The groups to translate. Always at least one.</summary>
		public List<GroupToTranslate> Groups=new List<GroupToTranslate>();
		
		
		/// <summary>Holds the given information about a translation.</summary>
		/// <param name="from">The lower case language code to translate from, e.g. en.</param>
		/// <param name="to">The lower case language code to translate to, e.g. fr.</param>
		public TranslationInfo(string from,string to){
			FromCode=from;
			ToCode=to;
			
			// Make sure the parser is ready to go:
			UI.Start(true);
			
		}
		
		/// <summary>Adds a group to translate.</summary>
		public void AddGroup(string targetPath,VariableSet toTranslate){
			
			// Add the group:
			Groups.Add(new GroupToTranslate(targetPath,toTranslate));
			
		}
		
		/// <summary>Adds a group to translate. Returns 1 if it was ok.</summary>
		public int AddGroupFromFile(string targetPath,string sourcePath){
			
			if(!File.Exists(sourcePath)){
				return 2;
			}
			
			// Read the source text:
			string sourceText=File.ReadAllText(sourcePath);
			
			// And parse it into a language set (of variables):
			LanguageGroup source=new LanguageGroup(sourceText);
			
			if(source.Count==0){
				return 3;
			}
			
			AddGroup(targetPath,source);
			return 1;
			
		}
		
		/// <summary>The JSON to send.</summary>
		public JSObject Json{
			get{
				
				JSArray arr=new JSArray();
				arr["from"]=new JSValue(FromCode.ToLower());
				arr["to"]=new JSValue(ToCode.ToLower());
				
				// Build up the groups:
				JSIndexedArray groups=new JSIndexedArray();
				
				for(int i=0;i<Groups.Count;i++){
					
					// Add it:
					groups.push(Groups[i].Variables.ToJson());
					
				}
				
				arr["groups"]=groups;
				
				return arr;
				
			}
		}
		
		/// <summary>Called when the translation is complete.</summary>
		/// <param name="translation">The translated result.</param>
		public void Complete(JSArray results){
			
			// Results contains a block of one or more groups:
			foreach(KeyValuePair<string,JSObject> kvp in results){
				
				// The group it's going into:
				GroupToTranslate target=Groups[int.Parse(kvp.Key)];
				
				// Pass through the value set:
				target.Complete(kvp.Value as JSArray);
				
			}
			
			if(OnComplete!=null){
				
				// Run the callback:
				OnComplete(this);
				
				// Refresh assets:
				AssetDatabase.Refresh();
				
			}
			
		}
		
		/// <summary>Called when the translation failed.</summary>
		public void Errored(string error){
			
			Error=error;
			
			if(OnComplete!=null){
				OnComplete(this);
			}
			
		}
		
		/// <summary>Performs the translation now.</summary>
		public void Translate(){
			
			// Start a request:
			XMLHttpRequest request=new XMLHttpRequest();
			
			request.open("post","http://translate.kulestar.com/v2/machine");
			
			request.onerror=delegate(UIEvent e){
				
				// Failed:
				Errored(request.responseText);
				
			};
			
			request.onload=delegate(UIEvent e){
				
				// Parse the JSON:
				JSObject json=JSON.Parse(request.responseText);
				JSArray results=json==null ? null : (json["results"] as JSArray);
				
				if(results==null){
					
					// Failed:
					Errored(request.responseText);
					
				}else{
					
					// Let it know it completed:
					Complete(results);
					
				}
				
			};
			
			// Send it off:
			request.send(Json);
			
		}
		
	}
	
}