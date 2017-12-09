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
	/// Tools for dealing with named characters (like nbsp)
	/// </summary>
	
	public class NamedCharacters : EditorWindow{
		
		public static bool Html5SetInUse;
		private static EditorWindow Window;
		public static bool BothSetsOk;
		
		// Add menu item named "Named Characters" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Named Characters")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(NamedCharacters));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Named Characters";
			#else
			GUIContent title=new GUIContent();
			title.text="Named Characters";
			Window.titleContent=title;
			#endif
			
			CheckSetVersion();
			
		}
		
		/// <summary>Path to the 'Named Characters' folder.</summary>
		public static string NCPath{
			get{
				return PowerUIEditor.GetPowerUIPath()+"/Named Characters";
			}
		}
		
		/// <summary>Gets the set which is currently in use.</summary>
		private static void CheckSetVersion(){
			
			string path=NCPath;
			
			// HTML5 set is in use if..:
			string infoFile=path+"/SetInUse.txt";
			Html5SetInUse=File.Exists(infoFile) && File.ReadAllText(infoFile)=="Html5";
			
			// Got the sets?
			BothSetsOk=File.Exists(path+"/Html4-Set.txt") && File.Exists(path+"/Html5-Set.txt");
			
		}
		
		/// <summary>Builds a faster mapping for PowerUI to use. Essentially compiles charcodes into a string.</summary>
		public static void BuildSet(string fromPath,string toPath){
			
			// Get the entity file:
			string file=File.ReadAllText(fromPath);
			
			// Ultra-light parsing follows.
			int position=0;
			
			// Create a writer:
			BinaryIO.Writer writer=new BinaryIO.Writer();
			
			while(true){
				
				// Find the next newline:
				int endOfLine=Dom.StringReader.NextIndexOf(position,file,'\n');
				
				// reader.Position->endOfLine is our line (inclusive).
				string line=file.Substring(position,endOfLine-position);
				
				string[] pieces=line.Split(',');
				
				string character="";
				
				for(int i=1;i<pieces.Length;i++){
					
					int charcode=int.Parse(pieces[i],System.Globalization.NumberStyles.HexNumber);
					character+=char.ConvertFromUtf32(charcode);
					
				}
				
				// Emit UTF8:
				writer.WriteString(pieces[0]);
				writer.WriteString(character);
				
				// Advance:
				position=endOfLine+1;
				
				// Done?
				if(endOfLine>=file.Length){
					// Yep!
					break;
				}
				
			}
			
			// Write out now:
			System.IO.File.WriteAllBytes(toPath,writer.GetResult());
			
		}
		
		private static void ChangeSet(bool useHTML5){
			
			// Ensure we're up to date:
			CheckSetVersion();
			
			if(!BothSetsOk){
				Debug.LogError("Character sets were not available.");
				return;
			}
			
			if(useHTML5==Html5SetInUse){
				Debug.LogError("Already using that set.");
				return;
			}
			
			Html5SetInUse=useHTML5;
			
			// Get the path:
			string path=NCPath;
			string installedPath=path+"/Resources/NamedCharacters.bytes";
			
			string toInstall=useHTML5? "5" : "4";
			
			System.IO.File.WriteAllText(path+"/SetInUse.txt","Html"+toInstall);
			
			// Write out now:
			BuildSet(path+"/Html"+toInstall+"-Set.txt",installedPath);
			
		}
		
		void OnGUI(){
			
			PowerUIEditor.HelpBox("Named characters are things like &nbsp; or &shy;. HTML5 introduced lots more of them so PowerUI includes both the HTML5 and HTML4 sets. By default, the HTML4 set is used (because it's smaller and has great coverage anyway; 2.6kb vs 29kb).");
			
			if(!BothSetsOk){
				
				PowerUIEditor.WarnBox("Unable to find the named character sets. There should be files called Html4-Set.txt and Html5-Set.txt.");
				return;
				
			}
			
			bool useHtml5=EditorGUILayout.Toggle("Use HTML5 Set",Html5SetInUse);
			
			if(useHtml5!=Html5SetInUse){
				
				// Change set:
				ChangeSet(useHtml5);
				
				// Update value:
				Html5SetInUse=useHtml5;
				
			}
			
		}
		
	}
	
}