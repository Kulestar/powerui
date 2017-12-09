using PowerUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Json;


namespace PowerTools{
	
	/// <summary>
	/// A JSON API endpoint.
	/// </summary>
	public class Endpoint{
		
		/// <summary>The endpoint's path.</summary>
		public string Path;
		
		/// <summary>
		/// Called when this endpoint receives a request.
		/// </summary>
		public virtual void OnRequest(ContentPackage package,Response output, JSObject request){
			
			// Not implemented - error:
			package.Failed(500);
			
		}
		
		/// <summary>All available endpoints. Use Get.</summary>
		internal static Dictionary<string,Endpoint> All;
		
		
		/// <summary>Adds an endpoint to the set of all available ones.</summary>
		private static void Add(Type pointType){
			
			// Get the name attribute from it (don't inherit):
			At at=Attribute.GetCustomAttribute(pointType,typeof(At),false) as At;
			
			if(at==null){
				// Nope
				return;
			}
			
			// Instance it:
			Endpoint endpoint=(Endpoint)Activator.CreateInstance(pointType);
			endpoint.Path=at.Path;
			
			// Add it:
			All[at.Path]=endpoint;
			
		}
		
		/// <summary>Gets an endpoint by path.
		/// Note that the path should include the version and an initial forward slash.
		/// E.g. /v1/hello/all.</summary>
		public static Endpoint Get(string path){
			
			if(All==null || All.Count==0){
				
				// Make sure this assembly has been started:
				Modular.Start.Now(typeof(Endpoint));
				
				// Create the set:
				All=new Dictionary<string,Endpoint>();
				
				// Load all:
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(Endpoint),delegate(Type type){
					
					// Add it:
					Add(type);
					
				});
				
			}
			
			Endpoint g=null;
			All.TryGetValue(path,out g);
			return g;
		}
		
	}
	
	/// <summary>
	/// The location an endpoint is at. Includes version and forward slash.
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class,Inherited=false)]
	public class At : Attribute{
		
		/// <summary>The endpoint path.</summary>
		public string Path;
		
		public At(string value){
			Path=value;
		}
		
	}
	
}