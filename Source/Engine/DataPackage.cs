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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using Dom;
using PowerUI.Http;


namespace PowerUI{
	
	/// <summary>A delegate used for callbacks when generic binary is done loading and can now be used.</summary>
	public delegate void OnDataReady(DataPackage package);
	
	/// <summary>A delegate for methods called when a package changes state.</summary>
	public delegate void OnPackageChange(ContentPackage package);
	
	
	/// <summary>
	/// Retrieves a block of binary data for use on the UI. Used by e.g. fonts.
	/// </summary>
	
	public class DataPackage : ContentPackage{
		
		/// <summary>The data that was retrieved. You must check if it is Ok first.</summary>
		public byte[] responseBytes;
		
		
		public DataPackage(){}
		
		public DataPackage(string src):this(src,null){
			
		}
		
		/// <summary>Creates a new package to get the named file.
		/// You must then call Get to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any (may be null).</param>
		public DataPackage(string src,Location relativeTo){
			SetPath(src,relativeTo);
		}
		
		/// <summary>The response text.</summary>
		public string responseText{
			get{
				if(responseBytes==null){
					return null;
				}
				return System.Text.Encoding.UTF8.GetString(responseBytes);
			}
		}
		
		/// <summary>The HTTP status of the response.</summary>
		public ushort status{
			get{
				return (ushort)statusCode;
			}
		}
		
		/// <summary>Sends the request off. Callbacks such as onreadystatechange will be triggered.</summary>
		public virtual void send(){
			
			if(readyState_==0){
				// Act like it just opened:
				readyState=1;
			}
			
			// Do we have a file protocol handler available?
			FileProtocol fileProtocol=location.Handler;
			
			if(fileProtocol!=null){
				fileProtocol.OnGetData(this);
			}
			
		}
		
		/// <summary>Got data is called by the file handler when the response is received.</summary>
		public override void ReceivedData(byte[] buffer,int offset,int count){
			ReceiveAllData(buffer, offset, count);
		}
		
		public override void ReceivedAllData(byte[] buffer){
			this.responseBytes=buffer;
		}
		
	}
	
	/// <summary>
	/// A package of content such as binary data, text or an image.
	/// </summary>
	
	public partial class ContentPackage : EventTarget{
		
		/// <summary>Same as XMLHTTPRequest.readyState.</summary>
		internal int readyState_;
		/// <summary>Same as XMLHTTPRequest.readyState.</summary>
		public int readyState{
			get{
				return readyState_;
			}
			set{
				readyState_=value;
				
				if(value==1){
					
					// Trigger the request init handle:
					if(FileProtocol.OnRequestStarted!=null){
						
						// Invoke it now:
						FileProtocol.OnRequestStarted(this);
						
					}
					
				}
				
				Dom.Event e=new Dom.Event();
				e.SetTrusted();
				e.EventType="readystatechange";
				dispatchEvent(e);
				
				if(value==4){
					
					// load or error too:
					UIEvent uie=new UIEvent();
					uie.SetTrusted();
					
					if(ok){
						// Load event:
						uie.EventType="load";
					}else{
						// Non 2xx response status (and not a redirect).
						uie.EventType="error";
					}
					
					dispatchEvent(uie);
					
				}
				
				if(value==1 || value==4){
					
					// Tell the doc:
					if(hostDocument!=null){
						hostDocument.ResourceStatus(this,4);
					}
					
				}
			}
		}
		
		/// <summary>A document to inform when the resource has loaded.</summary>
		public Document hostDocument;
		/// <summary>A location that the request was redirected to.</summary>
		public Location redirectedTo;
		/// <summary>The URI that was requested.</summary>
		public Location location;
		/// <summary>E.g. posted data.</summary>
		public byte[] request;
		/// <summary>The type of file that was requested (e.g. "woff" or "ttf")</summary>
		public string type;
		/// <summary>A HTTP status code if there is one.</summary>
		public int statusCode=200;
		/// <summary>Total content length.</summary>
		public int contentLength;
		/// <summary>Bytes received.</summary>
		public int bytesReceived;
		/// <summary>The internal request headers. See requestHeaders.</summary>
		private Headers requestHeaders_;
		/// <summary>The internal response headers. See responseHeaders.</summary>
		private Headers responseHeaders_;
		/// <summary>An internal abortable object.</summary>
		internal IAbortable abortableObject;
		/// <summary>The method to use.</summary>
		public string requestMethod=null;
		
		
		/// <summary>True if the content should always be interpreted as a video.</summary>
		public virtual bool ForceVideo{
			get{
				return false;
			}
		}
		
		/// <summary>The headers to send with this request. They're lowercase. Status line is indexed as the empty string.</summary>
		public Headers requestHeaders{
			get{
				if(requestHeaders_==null){
					requestHeaders_=new Headers();
				}
				return requestHeaders_;
			}
		}
		
		/// <summary>The document that this target belongs to.</summary>
		internal override Document eventTargetDocument{
			get{
				return hostDocument;
			}
		}
		
		/// <summary>All available headers, parsed as soon as they become available. Status line is indexed as the empty string.</summary>
		public Headers responseHeaders{
			get{
				if(responseHeaders_==null){
					responseHeaders_=new Headers();
				}
				return responseHeaders_;
			}
		}
		
		/// <summary>Sets the PostData from the given JSON object. Changes the content type too.</summary>
		public void setRequestBody(Json.JSObject toPost){
			
			// Make sure it's set to JSON:
			requestHeaders["content-type"]="application/json";
			
			// Flatten it:
			string flat=Json.JSON.Stringify(toPost);
			
			// Set as post data:
			setRequestBody(flat);
			
		}
		
		/// <summary>Sets the PostData from the given UTF8 string.</summary>
		/// <param name="toPost">The string to POST.</param>
		public void setRequestBody(string toPost){
			request=System.Text.Encoding.UTF8.GetBytes(toPost);
		}
		
		/// <summary>The request body as text.</summary>
		public string requestText{
			get{
				if(request==null){
					return null;
				}
				return System.Text.Encoding.UTF8.GetString(request);
			}
		}
		
		/// <summary>Gets or sets the raw response headers.</summary>
		public string rawResponseHeaders{
			get{
				return responseHeaders.ToString();
			}
			set{
				responseHeaders.Apply(value);
			}
		}
		
		/// <summary>Gets or sets the raw response headers.</summary>
		public string rawRequestHeaders{
			get{
				return requestHeaders.ToString();
			}
			set{
				requestHeaders.Apply(value);
			}
		}
		
		/// <summary>The complete response status header. E.g. "HTTP/1.1 200 OK"</summary>
		public string statusText{
			get{
				return responseHeaders.status;
			}
		}
		
		/// <summary>True if this request has started up.</summary>
		public bool started{
			get{
				return (readyState!=0);
			}
		}
		
		/// <summary>True if this request has at least some data.</summary>
		public bool ready{
			get{
				return (readyState>1);
			}
		}
		
		/// <summary>Sets up the filepath to the given url which may be relative to a given location.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any. May be null.</param>
		protected void SetPath(string src,Location relativeTo){
			location=new Location(src,relativeTo);
			type=location.Filetype.ToLower();
			
			if(relativeTo!=null && relativeTo.document!=null){
				hostDocument=relativeTo.document;
			}
		}
		
		/// <summary>True if there is no error and the text is ok.</summary>
		public bool ok{
			get{
				return (statusCode>=200 && statusCode<300) || statusCode==304;
			}
		}
		
		/// <summary>True if there was an error and the text is not ok.</summary>
		public bool errored{
			get{
				return !ok;
			}
		}
		
		/// <summary>The response content type.</summary>
		public string contentType{
			get{
				return responseHeaders["content-type"];
			}
		}
		
		/// <summary>Aborts this request.</summary>
		public void abort(){
			
			// trigger an abort event:
			UIEvent e=new UIEvent();
			e.EventType="abort";
			e.SetTrusted();
			dispatchEvent(e);
			
			if(abortableObject!=null){
				// Aborted it:
				abortableObject.abort();
			}
			
		}
		
		/// <summary>Download progress.</summary>
		public float progress{
			get{
				
				if(contentLength==0){
					// Length unknown:
					return 0f;
				}
				
				return (float)bytesReceived / (float)contentLength;
				
			}
		}
		
		/// <summary>Timeout in ms. Default is 0.</summary>
		public int timeout{
			get{
				
				// Got one?
				if(abortableObject==null){
					return 0;
				}
				
				float timeout=abortableObject.timeout;
				
				if(timeout==float.MaxValue){
					return 0;
				}
				
				return (int)(timeout * 1000f);
			}
			set{
				
				if(abortableObject!=null){
					abortableObject.timeout=(float)value / 1000f;
				}
				
			}
		}
		
		/// <summary>Adds the given form to this request.
		/// Note that if you wish to also use custom headers with a form, call this first.
		/// Then, add to the Headers property.</summary>
		/// <param name="form">The form to attach to this request.</param>
		public void AttachForm(UnityEngine.WWWForm form){
			
			// Get the payload:
			request=form.data;
			
			// Add each header (but don't overwrite):
			Headers rH=requestHeaders;
			
			foreach(KeyValuePair<string,string> kvp in form.headers){
				
				// Add:
				rH.AddIfNew(kvp.Key,kvp.Value);
				
			}
			
		}
		
		/// <summary>The request has timed out.</summary>
		public void TimedOut(){
			
			statusCode=408;
			Dom.Event e=new Dom.Event();
			e.SetTrusted();
			e.EventType="timeout";
			dispatchEvent(e);
			
			// RS4:
			readyState=4;
			
		}
		
		/// <summary>Sets up status code and contentLength from the headers.
		/// Returns true if we're redirecting.</summary>
		private bool LoadStatusAndLength(){
			
			// Pull the status code:
			string statusLine=responseHeaders.status;
			
			if(statusLine!=null){
				
				int statusCodeStart=statusLine.IndexOf(' ') + 1;
				int statusCodeEnd=statusLine.IndexOf(' ',statusCodeStart);
				
				// Pull the code:
				string code=statusLine.Substring(statusCodeStart, statusCodeEnd - statusCodeStart);
				
				if(!int.TryParse(code,out statusCode)){
					// Assume 200:
					statusCode=200;
				}
				
			}
			
			// Get the content length:
			string cLength=responseHeaders["content-length"];
			
			if(cLength!=null){
				int.TryParse(cLength,out contentLength);
			}
			
			// Did it try to set any cookies? Handle those:
			Cookies.Handle(this);
			
			// Remote timeout?
			if(statusCode==408){
				TimedOut();
				return false;
			}
			
			// Redirect?
			return (statusCode>=300 && statusCode<400) && statusCode!=304;
		}
		
		/// <summary>All headers are ready. Returns true if we're redirecting.</summary>
		internal bool ReceivedHeaders(){
			if(LoadStatusAndLength()){
				// Redirecting.
				return true;
			}
			
			readyState=2;
			return false;
		}
		
		/// <summary>All headers are ready.</summary>
		internal void ReceivedHeaders(int length){
			contentLength=length;
			readyState=2;
		}
		
		/// <summary>All headers are ready. Returns true if we're redirecting.</summary>
		internal bool ReceivedHeaders(string rawHeaders){
			responseHeaders.Apply(rawHeaders);
			
			if(LoadStatusAndLength()){
				// Redirecting.
				return true;
			}
			
			readyState=2;
			return false;
			
		}
		
		/// <summary>A 304 not modified response.</summary>
		public void NotModified(CachedContent cacheEntry){
			
			long age=(long)( (DateTime.UtcNow-cacheEntry.SavedAt).TotalSeconds );
			
			statusCode=304;
			responseHeaders.status="HTTP/2.0 304 Not Modified";
			responseHeaders["Age"]=age.ToString();
			responseHeaders["Date"]=cacheEntry.SavedAt.ToString(Cookie.DateTimePattern);
			
			if(cacheEntry.HasExpiry){
				responseHeaders["Expires"]=cacheEntry.Expiry.ToString(Cookie.DateTimePattern);
			}
			
			if(!string.IsNullOrEmpty(cacheEntry.ETag)){
				
				responseHeaders["ETag"]=cacheEntry.ETag;
				
			}
			
			responseHeaders["X-Spark-Cache"]="OK";
			
			// Received:
			byte[] data=cacheEntry.Data;
			ReceivedData(data,0,data.Length);
			
		}
		
		/// <summary>Sets the Content-Range response.</summary>
		public void SetPartialResponse(int start,int end,int total){
			
			statusCode=206;
			responseHeaders.status="HTTP/2.0 206 Partial Content";
			responseHeaders["Content-Range"]=start+"-"+end+"/"+total;
			
		}
		
		/// <summary>Gets the range request header.</summary>
		public bool GetRange(out int start,out int end){
			
			// Get the range:
			string range=requestHeaders["range"];
			
			if(string.IsNullOrEmpty(range)){
				
				// Nope!
				start=0;
				end=0;
				return false;
				
			}
			
			// Tidy:
			range=range.Trim();
			
			int equals=range.IndexOf('=');
			int dash=range.LastIndexOf('-');
			
			// Try parsing end:
			int.TryParse(range.Substring(dash+1),out end);
			
			// Try parsing start (this is fine if equals is -1):
			start=equals+1;
			int.TryParse(range.Substring(start,dash-start),out start);
			
			return true;
			
		}
		
		/// <summary>Goes straight to ready state 4 and the given status.</summary>
		public void Failed(int status){
			
			statusCode=status;
			
			if(readyState_==1){
				// No headers received; trigger it anyway:
				ReceivedHeaders();
			}
			
			// RS4:
			readyState=4;
			
		}
		
		/// <summary>Goes straight to ready state 4, status 200.</summary>
		internal void Done(){
			
			if(readyState_==1){
				// No headers received; trigger it anyway:
				ReceivedHeaders();
			}
			
			// RS4:
			readyState=4;
			
		}
		
		/// <summary>Received text data.</summary>
		public void ReceivedText(string text){
			byte[] data=System.Text.Encoding.UTF8.GetBytes(text);
			ReceivedData(data,0,data.Length);
		}
		
		/// <summary>Received a block of data.</summary>
		/// <param name="buffer">The data that was recieved.</param>
		public virtual void ReceivedData(byte[] buffer,int offset,int count){
			
			if(readyState_==1){
				// No headers received; trigger it anyway:
				ReceivedHeaders();
			}
			
			bytesReceived+=count;
			
			if(contentLength == -1){
				// If we recieved 0, we're done:
				readyState = (count == 0) ? 4 : 3;
			}else if(bytesReceived>=contentLength){
				readyState=4;
			}else{
				readyState=3;
			}
			
		}
		
		private System.IO.MemoryStream _stream;
		
		/// <summary>Use this to combine all the data into one buffer.
		/// Call it from RecieveData and override ReceivedAllData to get that single result.</summary>
		public void ReceiveAllData(byte[] buffer, int offset, int count){
			
			if(readyState_==1){
				// No headers received; trigger it anyway:
				ReceivedHeaders();
			}
			
			if(_stream == null){
				_stream = new System.IO.MemoryStream();
			}
			
			bytesReceived+=count;
			
			_stream.Write(buffer, offset, count);
			
			if(readyState == 4){
				return;
			}
			
			bool done = (contentLength == -1) ? (count == 0) : (bytesReceived>=contentLength);
			
			if(done){
				// Received all of the data!
				byte[] data = _stream.ToArray();
				_stream = null;
				Callback.MainThread(delegate(){
					ReceivedAllData(data);
					readyState=4;
				});
			}else{
				readyState=3;
			}
			
		}
		
		public virtual void ReceivedAllData(byte[] buffer){
		}
		
		#if !MOBILE && !UNITY_WEBGL
		internal virtual void ReceivedMovieTexture(UnityEngine.MovieTexture tex){
			
			// Trigger a state 3:
			readyState=3;
			
		}
		#endif
		
	}
	
}