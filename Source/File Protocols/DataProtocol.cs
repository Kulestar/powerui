using UnityEngine;
using Css;
using Dom;


namespace PowerUI{
   
    /// <summary>
    /// This protocol is used by data:meta/type;base64_data
    /// </summary>
   
    public class DataProtocol:FileProtocol{
       
        /// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
        public override string[] GetNames(){
            return new string[]{"data"};
        }
		
		// Raw binary data
		public override void OnGetDataNow(ContentPackage package){
			
			// Content type header is segments[0]:
			string contentType=package.location.Segments[0];
			
			// Trim it:
			contentType=contentType.Trim();
			
			byte[] data;
			
			if(contentType.EndsWith(";base64")){
				
				// Split it off:
				contentType=contentType.Substring(0,contentType.Length-7);
				
				// The data is at location.Segments[1] as base64:
				data=System.Convert.FromBase64String(package.location.Segments[1]);
				
			}else{
				
				// The data is at location.Segments[1], simply url encoded:
				string unescaped=System.Uri.UnescapeDataString( package.location.Segments[1] );
				
				data=System.Text.Encoding.UTF8.GetBytes( unescaped );
				
			}
			
			// Apply headers:
			package.responseHeaders["content-type"]=contentType;
			package.ReceivedHeaders(data.Length);
			
			// Apply data:
			package.ReceivedData(data,0,data.Length);
			
		}
       
    }
   
}