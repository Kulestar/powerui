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
	/// Holds a set of source files.
	/// </summary>
	
	public class SourceFileSet{
		
		public List<string> Files=new List<string>();
		public List<string> Ignores=new List<string>();
		
		
		/// <summary>Is the file/directory with the given name ignored?</summary>
		public bool IsIgnored(string name){
			return Ignores.Contains(name);
		}
		
		/// <summary>Ignores the given file/directory name. E.g. "Editor" will ignore all folders called editor.</summary>
		public void Ignore(string name){
			Ignores.Add(name);
		}
		
		/// <summary>Adds a set of files/directories to this set.</summary>
		public void Add(string[] paths){
			
			for(int i=0;i<paths.Length;i++){
				
				Add(paths[i]);
				
			}
			
		}
		
		/// <summary>Adds a file/directory to this set.</summary>
		public void Add(string basePath){
			
			// Make things a little easier:
			basePath=basePath.Replace("\\","/");
			
			string[] pieces=basePath.Split('/');
			
			string name=pieces[pieces.Length-1];
			
			if(IsIgnored(name)){
				// E.g. Editor folder - ignore it.
				return;
			}
			
			// Is it a directory?
			FileAttributes attribs=File.GetAttributes(basePath);
			
			if((attribs&FileAttributes.Directory)==FileAttributes.Directory){
				// It's a directory.
				
				// Add all files:
				Add(Directory.GetFiles(basePath));
				
				// Add all subdirs:
				Add(Directory.GetDirectories(basePath));
				
				return;
				
			}
			
			// Must end in .cs or .preco:
			if(basePath.EndsWith(".cs") || basePath.EndsWith(".preco")){
				
				// Add as a file:
				Files.Add(basePath);
				
			}
			
		}
		
		/// <summary>Adds (or removes) a ".preco" extension to all the files in this set.
		/// Do note that this precompiler copies all source files first.</summary>
		public void Rename(bool addExtension){
			
			for(int i=0;i<Files.Count;i++){
				
				// Get the path:
				string path=Files[i];
				
				// Already contains the extension?
				bool hasExtension=path.EndsWith(".preco");
				
				if(hasExtension == addExtension){
					continue;
				}
				
				bool hasMeta=File.Exists(path+".meta");
				
				if(addExtension){
					
					// Add the extension:
					File.Move(path,path+".preco");
					
					// Same for the meta file, if it exists:
					if(hasMeta){
						
						// Rename the meta file too:
						File.Move(path+".meta",path+".preco.meta");
						
					}
					
				}else{
					
					// Remove the extension:
					string pathNoPreco=path.Substring(0,path.Length-6);
					
					File.Move(path,pathNoPreco);
					
					// Same for the meta file, if it exists:
					if(hasMeta){
						
						// Rename the meta file too:
						File.Move(path+".meta",pathNoPreco+".meta");
						
					}
					
				}
				
			}
			
			Files.Clear();
			
		}
		
	}
	
}