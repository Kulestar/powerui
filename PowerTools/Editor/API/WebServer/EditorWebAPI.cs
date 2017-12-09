using PowerUI;
using System.IO;


namespace PowerTools{
	
	/// <summary>
	/// PowerTools is designed to work locally but can optionally
	/// communicate with a remote service too. (For example, if one of your team members 
	/// would like to work on animations etc from their tablet, then you can simply upload PowerTools to your site
	/// and have it operate as a remote service).
	/// So, in order for this to be highly consistent, both the editor and the remote form implement
	/// the same REST based API.
	/// On top of that, this editor API is consistent with PowerUI itself as 
	/// it uses the same header/ request system.
	/// </summary>
	
	public class EditorWebAPI : WebServer{
		
		private static EditorWebAPI Current;
		
		/// <summary>Starts the web API.</summary>
		public static void Setup(){
			if(Current==null){
				Current=new EditorWebAPI();
			}
			
			Current.Start();
		}
		
		/// <summary>The filepath to PowerTools. Does not end with a forward slash.</summary>
		private string PowerToolsPath;
		
		public EditorWebAPI():base(7823){
			
			// Get PowerTools path:
			PowerToolsPath=PowerUIEditor.GetPowerUIPath()+"/PowerTools/Editor";
			
		}
		
		/// <summary>Get the raw file data (based on the PowerUI FileProtocol system).</summary>
		public override void OnGetDataNow(ContentPackage package){
			
			// The endpoint (this is e.g. /v1/hello):
			string endpointPath=package.location.pathname;
			
			if(endpointPath.StartsWith("/v1")){
				
				if(!endpointPath.EndsWith("/")){
					endpointPath+="/";
				}
				
				// The request payload (can be null):
				Json.JSObject request=Json.JSON.Parse(package.requestText);
				
				package.responseHeaders["content-type"]="application/json; charset=utf-8";
				
				// Get an endpoint:
				Endpoint ep = Endpoint.Get(endpointPath);
				
				if(ep==null){
					
					// Endpoint not found.
					package.Failed(404);
					
				}else{
					
					Response response=new Response();
					
					// Pass the request to the endpoint:
					ep.OnRequest(package,response,request);
					
					// Output the response:
					package.ReceivedText(response.ToString());
					
				}
				
				return;
			}
			
			// Serve a file from PowerTools.
			// The browser embedded in the Unity editor (not PowerUI)
			// is webkit which doesn't allow ajax to file:// uri's.
			if(!File.Exists(PowerToolsPath+endpointPath)){
				
				// Doesn't exist:
				package.Failed(404);
				
			}else{
				
				// Currently always HTML files down here:
				package.responseHeaders["content-type"]="text/html";
				
				// Read the file:
				byte[] data = File.ReadAllBytes(PowerToolsPath+endpointPath);
				
				// Serve it up:
				package.ReceivedData(data,0,data.Length);
				
			}
			
		}
		
	}
	
}