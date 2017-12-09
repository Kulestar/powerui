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

#if UNITY_EDITOR && !UNITY_WEBPLAYER

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	public delegate void HtmlFileChange(string path,string content);
	
	/// <summary>Looks out for any html file changes.</summary>

	public class LiveHtml{
		
		/// <summary>Used when looking out for a particular file.</summary>
		private string Target;
		private FileSystemWatcher Watcher;
		private HtmlFileChange OnFileChanged;
		
		
		/// <summary>Looks out for any html file changes.</summary>
		public LiveHtml(string target,HtmlFileChange onFileChanged){
			
			OnFileChanged=onFileChanged;
			
			Watcher=new FileSystemWatcher();
			
			if(target==null){
				Watcher.Path="Assets";
			}else{
				Target=target;
				Watcher.Path=Path.GetDirectoryName(target);
			}
			
			Watcher.IncludeSubdirectories=true;
			
			// Watch for changes in LastWrite times, and the renaming of files or directories:
			Watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

			// Add event handlers:
			Watcher.Changed+=OnSystemFileChanged;
			
			/*
			Watcher.Created+=OnSystemFileChanged;
			Watcher.Deleted+=OnSystemFileChanged;
			Watcher.Renamed+=OnRenamed;
			*/
			
			// Begin watching.
			Watcher.EnableRaisingEvents=true;
			
		}
		
		private void OnSystemFileChanged(object source,FileSystemEventArgs e){
			
			// Sync the new/deleted file:
			CheckIfHtml(e.FullPath);
			
		}

		private void OnRenamed(object source,RenamedEventArgs e){
			
			// Sync the new one:
			CheckIfHtml(e.FullPath);
			
		}
		
		private void CheckIfHtml(string path){
			
			if(path==Target){
				
				
				
			}else if(path.EndsWith(".html") || path.EndsWith(".htm")){
				
				// Got a html file.
				OnFileChanged(path,File.ReadAllText(path));
				
			}
			
		}
		
		public void Stop(){
			Watcher.EnableRaisingEvents=false;
			Watcher=null;
		}
		
		public void Continue(){
			Watcher.EnableRaisingEvents=true;
		}
		
	}
	
}

#endif