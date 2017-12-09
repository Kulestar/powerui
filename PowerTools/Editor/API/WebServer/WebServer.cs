using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Text;
using PowerUI;

 
namespace PowerTools{
	
	/// <summary>
	/// An embedded web server for communicating between 
	/// the Unity editor and Safari embedded within it (only used in the editor when PowerTools is open).
	/// </summary>
	
    public class WebServer{
		
		/// <summary>A HTTP listener.</summary>
		private HttpListener Listener;
		/// <summary>The port the server is on.</summary>
		private int Port;
		/// <summary>The base location.</summary>
		private Dom.Location BaseLocation;
		
		/// <summary>Creates and starts a server on the given port.</summary>
		public WebServer(int port){
			Port=port;
		}
		
		/// <summary>True if the server is up.</summary>
		public bool Started{
			get{
				return Listener!=null;
			}
		}
		
		/// <summary>Starts the server.</summary>
		public bool Start(){
			
			if(Started){
				return true;
			}
			
			Listener = new HttpListener();
			
			string baseLocation="http://localhost:"+Port+"/";
			
			// Http only:
			Listener.Prefixes.Add(baseLocation);
			
			BaseLocation=new Dom.Location(baseLocation);
			
			try{
				// Go!
				Listener.Start();
				
				// Await:
				Listener.BeginGetContext(new AsyncCallback(OnRequest),Listener);
			}catch(Exception e){
				// Already running.
				Listener=null;
				UnityEngine.Debug.LogError("Error: "+e);
			}
			
			return Started;
		}
		
		/// <summary>Just like FileProtocol. Called when a request was made to the server.
		/// Respond to the package to handle the request 
		/// (in the same way a FileProtocol would respond to it).</summary>
		public virtual void OnGetDataNow(ContentPackage package){}
		
		/// <summary>Stops the server.</summary>
		public void Stop(){
			
			if(Listener!=null){
				Listener.Stop();
				Listener=null;
			}
			
		}
		
		private void OnRequest(IAsyncResult result){
			
			// Get the listener:
			HttpListener listener=(HttpListener)result.AsyncState;
			HttpListenerContext context=listener.EndGetContext(result);
			
			// Start listening again:
			Listener.BeginGetContext(new AsyncCallback(OnRequest),Listener);
			
			try{
				
				// The request was..
				HttpListenerRequest request=context.Request;
				
				// Obtain a response object.
				HttpListenerResponse response = context.Response;
				
				// Build it as a data package:
				DataPackage package=new DataPackage();
				
				// Parse the full URL:
				package.location=new Dom.Location(request.RawUrl,BaseLocation);
				
				// Payload if there is one:
				if(request.HttpMethod=="POST"){
					
					// Read the payload:
					byte[] payload=new byte[(int)request.ContentLength64];
					request.InputStream.Read(payload,0,payload.Length);
					
					package.request=payload;
				}
				
				// Add a handler for onload and onerror:
				package.onload=package.onerror=delegate(PowerUI.UIEvent e){
					
					try{
						
						// Status:
						response.StatusCode=package.statusCode;
						
						// Content type header:
						string type=package.contentType;
						
						if(type!=null){
							response.ContentType=type;
						}
						
						// (Other headers such as xdomain):
						response.Headers["Access-Control-Allow-Origin"]="*";
						
						var responseHeaders=package.responseHeaders;
						
						if(responseHeaders.lines!=null){
							
							foreach(KeyValuePair<string,List<string>> kvp in responseHeaders.lines){
								
								if(kvp.Key=="" || kvp.Key=="content-type"){
									continue;
								}
								
								// Get header name capitalized:
								string header=PowerUI.Http.Headers.ToHeaderCapitals(kvp.Key);
								
								// First value only:
								response.Headers[header]=kvp.Value[0];
								
							}
							
						}
						
						// Write to the response stream now:
						System.IO.Stream output = response.OutputStream;
						
						if(package.responseBytes!=null){
							response.ContentLength64 = package.responseBytes.Length;
							output.Write(package.responseBytes,0,package.responseBytes.Length);
						}
						
						// Close the output stream:
						output.Close();
						
					}catch(Exception er){
						
						UnityEngine.Debug.LogError("PowerTools HTTP API error: "+er);
						
					}
				};
				
				// Hand over:
				OnGetDataNow(package);
				
			}catch(Exception er){
				
				UnityEngine.Debug.LogError("PowerTools HTTP API error: "+er);
				
			}
		}
		
		~WebServer(){
			Stop();
		}
		
	}
	
}