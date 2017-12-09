using System;
using System.Collections;
using System.Collections.Generic;
using Json;
using PowerUI;


namespace PowerTools{
	
	[At("/v1/file/list/")]
	public class FileListEndpoint : DirectoryEndpoint{
		
		public override void OnRequest(ContentPackage package,Response output, JSObject request){
			
			// Write a jsonlist:
			output.List<FileInfo>(Files,new string[]{"path"});
			
		}
		
		internal IEnumerable<FileInfo> Files{
			get{
				
				yield return new FileInfo("test/path");
				
			}
		}
		
	}
	
	internal struct FileInfo{
		
		public string path;
		
		public FileInfo(string path){
			this.path=path;
		}
	}
	
}