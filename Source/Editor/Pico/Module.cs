//--------------------------------------
//                Pico
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;


namespace Pico{
	
	/// <summary>
	/// Holds settings about a precompiled "module" - that's just a group of source files compiled together under one name.
	/// </summary>
	
	public class Module{
		
		public const string Target="Assets/Precompiled/";
		
		/// <summary>The name of the module.</summary>
		public string Name;
		/// <summary>Was this compiled in editor mode?</summary>
		public bool EditorMode;
		/// <summary>True if the DLL exists.</summary>
		private int DllExists_=-1;
		/// <summary>The set of source directories.</summary>
		public List<string> SourceFolders=new List<string>();
		
		
		/// <summary>True if the DLL exists.</summary>
		public bool Precompiled{
			get{
				
				if(DllExists_==-1){
					DllExists_=File.Exists(DllPath)? 1 : 0;
				}
				
				return DllExists_==1;
				
			}
		}
		
		/// <summary>Does this module exist?</summary>
		public bool Exists{
			get{
				return Directory.Exists(SettingsPath);
			}
		}
		
		/// <summary>Path to the settings file.</summary>
		public string SettingsPath{
			get{
				return Target+Name+"-Settings.conf";
			}
		}
		
		/// <summary>The path to the DLL.</summary>
		public string DllPath{
			get{
				return Target+Name+".dll";
			}
		}
		
		/// <summary>Creates a settings object for a given module name.</summary>
		public Module(string name){
			Name=name;
			
			// Load the settings:
			LoadSettings();
			
		}
		
		/// <summary>Reverts the precompilation of this module.</summary>
		public void Revert(){
			
			// Clear exists:
			DllExists_=-1;
			
			// Delete the .dll:
			if(Precompiled){
				File.Delete(DllPath);
			}
			
			// Go through the source files and rename any which are ".preco". Do that like so:
			SourceFileSet files=GetFileSet();
			
			// Rename:
			files.Rename(false);
			
			// Inform the asset DB:
			AssetDatabase.Refresh();
			
		}
		
		/// <summary>Recompiles this module.</summary>
		public void Compile(){
			
			// Clear exists:
			DllExists_=-1;
			
			// Write out the settings:
			WriteSettings();
			
			// Get the file set:
			SourceFileSet files=GetFileSet();
			
			// Rename the files, adding a "preco" extension:
			files.Rename(true);
			
			if(Precompiler.Build(this)){
				
				Debug.Log("Precompiled "+Name);
				
			}else{
				
				// Revert the files:
				files.Rename(false);
				
			}
			
			// Inform the asset DB:
			AssetDatabase.Refresh();
			
			
		}
		
		/// <summary>The full set of source files in this module.</summary>
		public SourceFileSet GetFileSet(){
			
			// Create the source file set:
			SourceFileSet files=new SourceFileSet();
			
			// Ignore "Editor" folders:
			files.Ignore("Editor");
			
			// Ignore SVN folders:
			files.Ignore(".svn");
			
			// Collect all source files now:
			foreach(string sourceDir in SourceFolders){
				files.Add(sourceDir);
			}
			
			return files;
			
		}
		
		/// <summary>Write out the settings.</summary>
		public void WriteSettings(){
			
			if(!Directory.Exists(Target)){
				Directory.CreateDirectory(Target);
			}
			
			// Could ignore platform specific folders here too!
			string settings="Editor="+EditorMode+"\r\nExt=preco";
			
			// Collect all source files now:
			foreach(string sourceDir in SourceFolders){
				settings+="\r\nSrc="+sourceDir;
			}
			
			// Write out settings:
			File.WriteAllText(SettingsPath,settings);
			
		}
		
		/// <summary>Loads the settings for this module.</summary>
		public void LoadSettings(){
			
			if(!File.Exists(SettingsPath)){
				EditorMode=false;
				return;
			}
			
			// Load the settings file:
			string[] settings=File.ReadAllLines(SettingsPath);
			
			for(int i=0;i<settings.Length;i++){
				
				// Get the line:
				string line=settings[i];
				
				// Equals is at..
				int equalsIndex=line.IndexOf('=');
				
				if(equalsIndex==-1){
					continue;
				}
				
				// Split there (exclusive):
				string name=line.Substring(0,equalsIndex).Trim().ToLower();
				string data=line.Substring(equalsIndex+1).Trim();
				
				if(name=="editor"){
					
					// Editor mode?
					EditorMode=(data.ToLower()=="true");
					
				}else if(name=="src"){
					
					// Path:
					SourceFolders.Add(data);
					
				}
				
			}
			
		}
		
	}
	
}