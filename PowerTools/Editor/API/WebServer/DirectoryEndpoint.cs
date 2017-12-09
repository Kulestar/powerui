using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Json;


namespace PowerTools{
	
	/// <summary>
	/// A directory endpoint lists all available functions within it.
	/// </summary>
	public class DirectoryEndpoint : Endpoint{
		
		public override void OnRequest(ContentPackage package,Response output, JSObject request){
			
			output.Add("{\"functions\":{");
			bool first=true;
			
			// For each endpoint, find which start with my path:
			foreach(KeyValuePair<string,Endpoint> kvp in Endpoint.All){
				
				if(kvp.Key.StartsWith(Path) && !(kvp.Value is DirectoryEndpoint)){
					
					// Return this endpoints self documentation.
					
					if(first){
						first=false;
					}else{
						output.Add(",");
					}
					
					// Strip first fwdslash:
					string endpoint=kvp.Key.Substring(1);
					
					output.Add("\""+endpoint+"\":");
					
					// Must get the doc and write it here.
					output.Add("{}");
					
				}
				
			}
			
			output.Add("}}");
			
		}
		
	}
	
}