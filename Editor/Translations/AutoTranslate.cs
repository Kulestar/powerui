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
using Json;


namespace PowerUI{
	
	/// <summary>
	/// Displays options for automatically translating your UI language files.
	/// </summary>
	
	public class AutoTranslate : EditorWindow{
		
		/// <summary>True if the API key section is visible.</summary>
		// private static bool ShowAPIKey;
		/// <summary>Set to true if a translation is in progress.</summary>
		public static bool Translating;
		/// <summary>The selected source file index in the AvailableSource array.</summary>
		private static UnityEngine.Object SelectedFile;
		/// <summary>The path to PowerUI.</summary>
		public static string PowerUIPath;
		/// <summary>The selected target language index in the AvailableTargets array.</summary>
		private static int SelectedTarget;
		/// <summary>The access key to use when translating.</summary>
		// public static string TranslationKey;
		/// <summary>The available target languages. Loaded from the languages.txt file.</summary>
		public static string[] AvailableTargets;
		/// <summary>The last opened translate window.</summary>
		public static EditorWindow TranslateWindow;
		/// <summary>The available target language codes. Loaded from the languages.txt file.</summary>
		public static string[] AvailableTargetCodes;
		/// <summary>The 2 char source language code, lowercase.</summary>
		private static string SourceLanguage;
		/// <summary>The target basepath. E.g. Assets/Langs/Resources/Login/</summary>
		private static string TargetBasepath;
		/// <summary>Set if the latest translate attempt failed.</summary>
		public static string LatestError;
		/// <summary>Set if the latest translate attempt failed.</summary>
		public static string LatestMessage;
		
		
		/// <summary>The target file path.</summary>
		public static string TargetPath{
			get{
				return TargetBasepath+TargetLanguage+".xml";
			}
		}
		
		/// <summary>The target language code.</summary>
		public static string TargetLanguage{
			get{
				return AvailableTargetCodes[SelectedTarget];
			}
		}
		
		/// <summary>The filepath to the source file.</summary>
		public static string SourcePath{
			get{
				if(SelectedFile==null){
					return null;
				}
				
				return AssetDatabase.GetAssetPath(SelectedFile);
			}
		}
		
		// Add menu item named "Auto Translate" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Auto Translate")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			TranslateWindow=EditorWindow.GetWindow(typeof(AutoTranslate));

			// Give it a title:
			#if PRE_UNITY5_3
			TranslateWindow.title="Auto Translate";
			#else
			GUIContent title=new GUIContent();
			title.text="Auto Translate";
			TranslateWindow.titleContent=title;
			#endif
			
			Setup();
		
		}
		
		public static void Setup(){
			
			Translating=false;
			
			// Get the translation key, if we have one:
			// TranslationKey=GetTranslationKey();
			// Setup the foldout:
			// ShowAPIKey=(TranslationKey=="");
			
			// Setup the PowerUI path:
			PowerUIPath=PowerUIEditor.GetPowerUIPath();
			
			// Load the target languages:
			LoadAvailableLanguages();
			
		}
		
		void OnGUI(){
			
			if(AvailableTargets==null){
				Setup();
			}
			
			PowerUIEditor.HelpBox("This simplifies translating your project by sending off any language file for auto translation. Please note that you will need to quality check the results.");
			
			/*
			ShowAPIKey = EditorGUILayout.Foldout(ShowAPIKey,"API Key");
			
			if(ShowAPIKey){
				if(GUILayout.Button("Get Free API Key")){
					Application.OpenURL("http://help.kulestar.com/translate-powerui/#getAKey");
				}
				
				// The translation key (Bing API Key):
				string result=EditorGUILayout.TextField("API Key",TranslationKey);
				
				if(result!=TranslationKey){
					TranslationKey=result;
					SaveTranslationKey();
				}
			}
			*/
			
			// Translate from:
			GUILayout.Label("Variables File",EditorStyles.boldLabel);
			
			UnityEngine.Object o=SelectedFile;
			
			SelectedFile=EditorGUILayout.ObjectField(o, typeof(TextAsset), false);
			
			if(SelectedFile!=o){
				
				// They changed the file. Discover which language it is now.
				FindInformation();
				
			}
			
			// Translate to language:
			GUILayout.Label("Translate To",EditorStyles.boldLabel);
			SelectedTarget=EditorGUILayout.Popup(SelectedTarget,AvailableTargets);
			
			if(Translating){
				GUILayout.Label("Translating..",EditorStyles.boldLabel);
			}else if(SelectedFile==null){
				
				PowerUIEditor.WarnBox("Please select the file you'd like to translate.");
				
			}else if(TargetBasepath==null){
				
				PowerUIEditor.WarnBox("The file you've selected isn't a suitable language file. It must be named with a 2 character language code, such as 'en.xml'.");
				
			}else{
				
				if(GUILayout.Button("Translate Now")){
					Perform();
				}
				
				string path=TargetPath;
				
				PowerUIEditor.HelpBox("From "+SourceLanguage+" into file "+path);
				
				if(File.Exists(path)){
					
					PowerUIEditor.WarnBox("The target file exists and will be overwritten.");
					
				}
				
			}
			
			if(LatestError!=null){
				
				PowerUIEditor.ErrorBox(LatestError);
				
			}else if(LatestMessage!=null){
				
				PowerUIEditor.HelpBox(LatestMessage);
				
			}
			
		}
		
		/// <summary>Finds the info such as source language.</summary>
		private static void FindInformation(){
			
			string srcLanguage=null;
			string targetBasepath=null;
			
			// First, is it an xml file?
			string assetPath=SourcePath;
			
			if(assetPath!=null){
				
				if(assetPath.EndsWith(".xml")){
					
					// Get the filename:
					string fileName=System.IO.Path.GetFileName(assetPath);
					
					// Src language is always the filename:
					srcLanguage=fileName.Split('.')[0].Trim().ToLower();
					
					if(srcLanguage.Length==2){
						
						// Get basepath (Unity always uses /):
						targetBasepath=System.IO.Path.GetDirectoryName(assetPath)+"/";
						
					}else{
						srcLanguage=null;
					}
					
				}
				
			}
			
			SourceLanguage=srcLanguage;
			TargetBasepath=targetBasepath;
			
		}
		
		/// <summary>Loads the set of translate to languages from the languages.txt file.</summary>
		public static void LoadAvailableLanguages(){
			// The path to the languages.txt file:
			string languagesFile=TranslationsPath+"languages.txt";
			
			// Does it exist?
			if(!File.Exists(languagesFile)){
				Debug.Log("Warning: No languages file found! languages.txt stores the languages available to translate to.");
				return;
			}
			
			// Setup the lists of the codes and names:
			List<string> languageCodes=new List<string>();
			List<string> languageNames=new List<string>();
			
			// Grab the set of available languages:
			string[] available=File.ReadAllLines(languagesFile);
			
			// For each of the available languages..
			for(int i=0;i<available.Length;i++){
				string languageLine=available[i].Trim();
				
				if(languageLine==""){
					continue;
				}
				
				// Each line is e.g. en=English. Split it up:
				string[] pieces=languageLine.Split('=');
				
				// Safety check:
				if(pieces.Length!=2){
					continue;
				}
				
				// Add the language code:
				languageCodes.Add(pieces[0].ToLower());
				
				// Add the language name:
				languageNames.Add(pieces[1]);
			}
			
			// Apply the new sets:
			AvailableTargetCodes=languageCodes.ToArray();
			AvailableTargets=languageNames.ToArray();
		}
		
		/// <summary>Performs the translation using options selected in the open window.</summary>
		public static void Perform(){
			
			if(Translating){
				return;
			}
			
			// Get settings:
			string sourcePath=SourcePath;
			
			// Create the info object:
			TranslationInfo info=new TranslationInfo(SourceLanguage,TargetLanguage);
			
			// Add a group to translate:
			int state=info.AddGroupFromFile(TargetPath,sourcePath);
			
			if(state!=1){
				
				if(state==2){
					LatestError="Translate source file not found: "+sourcePath;
				}else{
					LatestError="Translation file '"+sourcePath+"' contained no variables!";
				}
				
				return;
			}
			
			// We're good to go!
			Translating=true;
			LatestMessage=null;
			LatestError=null;
			
			// Hook up it's callback:
			info.OnComplete=OnTranslationDone;
			
			// Start translating:
			info.Translate();
			
		}
		
		private static void OnTranslationDone(TranslationInfo info){
			Translating=false;
			
			if(info.Error==null){
				LatestError=null;
				LatestMessage="Success!";
			}else{
				LatestMessage=null;
				LatestError="No translation result available. "+info.Error;
			}
			
			// Repaint the Window:
			TranslateWindow.Repaint();
			
		}
		
		/// <summary>The filepath to the Translations editor folder.</summary>
		public static string TranslationsPath{
			get{
				return PowerUIPath+"/Editor/Translations/";
			}
		}
		
		/*
		/// <summary>The filepath to the file that holds the translation key.</summary>
		public static string TranslationKeyFile{
			get{
				return TranslationsPath+"translate-key.txt";
			}
		}
		
		/// <summary>Saves the TranslationKey into TranslationKeyFile.</summary>
		public static void SaveTranslationKey(){
			SaveTranslationKey(TranslationKey);
		}
		
		/// <summary>Saves the given TranslationKey into the TranslationKeyFile.</summary>
		/// <param name="key">The key to save.</param>
		public static void SaveTranslationKey(string key){
			File.WriteAllText(TranslationKeyFile,key);
		}
		
		/// <summary>Loads the translation key from the TranslationKeyFile.</summary>
		/// <returns>The loaded key.</returns>
		public static string GetTranslationKey(){
			if(File.Exists(TranslationKeyFile)){
				return File.ReadAllText(TranslationKeyFile);
			}
			
			return "";
		}
		*/
		
	}
	
}