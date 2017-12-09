using System;
using System.Collections;
using System.Collections.Generic;
using Json;
using PowerUI;
using Dom;


namespace PowerTools{
	
	[At("/v1/document/content/")]
	public class DocumentContentEndpoint : DirectoryEndpoint{
		
		public override void OnRequest(ContentPackage package,Response output, JSObject request){
			
			// Doc ID:
			uint id = uint.Parse( package.location.searchParams["docid"] );
			
			// Get that doc:
			Document doc = DomInspector.GetByUniqueID(id);
			
			if(doc==null){
				// Not found!
				// output.Error("");
				return;
			}
			
			// Get its complete contents:
			string innerHTML = doc.innerML;
			
			// Emit as a JSON object:
			JSArray json=new JSArray();
			
			// Add markup:
			json["markup"] = new JSValue(innerHTML);
			
			// Ok!
			output.Write(json);
			
		}
		
	}
	
}