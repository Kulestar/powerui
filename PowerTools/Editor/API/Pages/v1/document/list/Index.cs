using System;
using System.Collections;
using System.Collections.Generic;
using Json;
using PowerUI;


namespace PowerTools{
	
	[At("/v1/document/list/")]
	public class DocumentListEndpoint : DirectoryEndpoint{
		
		public override void OnRequest(ContentPackage package,Response output, JSObject request){
			
			// Write a jsonlist:
			output.List<DocumentInfo>(Documents,new string[]{"id","src","name"});
			
		}
		
		internal IEnumerable<DocumentInfo> Documents{
			get{
				
				// Get all docs:
				foreach(DocumentEntry doc in PowerUI.DomInspector.GetDocuments(true)){
					
					string src=(doc.Document.location==null) ? "about:blank" : doc.Document.location.absolute;
					
					yield return new DocumentInfo(doc.Document.uniqueID,src,doc.Name);
					
				}
				
			}
		}
		
	}
	
	internal struct DocumentInfo{
		
		public uint id;
		public string src;
		public string name;
		
		
		public DocumentInfo(uint id,string src,string name){
			this.id=id;
			this.src=src;
			this.name=name;
		}
		
	}
	
}