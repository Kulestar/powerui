#if UNITY_STANDALONE || UNITY_ANDROID
using UnityEngine;
using Css;
using Dom;
using System.IO;


namespace PowerUI{
   
    /// <summary>
    /// This protocol is used if you have an image file on the screen.
    /// You must use streamedassets://pathToImage.png to access it.
    /// Nota that this will not work on all platforms (namely it won't work on webplayer)
    /// </summary>
   
    public class StreamedAssetsProtocol:DriveProtocol{
       
        /// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
        public override string[] GetNames(){
            return new string[]{"streamedassets"};
        }
		
		/// <summary>Get the raw file data.</summary>
		public override void OnGetDataNow(ContentPackage package){
			
			// Path without the protocol:
			string rawPath=Application.streamingAssetsPath + "/ " + package.location.Path;
			
			// If it contains a : and starts with / then chop it off:
			if(rawPath!=null && rawPath.IndexOf(':')!=-1 && rawPath[0]=='/'){
				rawPath=rawPath.Substring(1);
			}
			
			if(File.Exists(rawPath)){
				
				// Partial?
				int partialStart;
				int partialEnd;
				
				byte[] data;
				
				if(package.GetRange(out partialStart,out partialEnd)){
					
					// Partial request.
					FileStream fs=new FileStream(rawPath,FileMode.Open,FileAccess.Read);
					
					// Seek there:
					fs.Seek(partialStart,SeekOrigin.Begin);
					
					// Setup Content-Range:
					package.SetPartialResponse(partialStart,partialEnd,(int)fs.Length);
					
					int contentLength=(partialEnd+1)-partialStart;
					
					// Read that many bytes into our buffer:
					data=new byte[contentLength];
					
					// Read:
					fs.Read(data,0,data.Length);
					
					// Got headers:
					package.ReceivedHeaders(contentLength);
					
					// Ok!
					fs.Close();
					
				}else{
					
					// Get the bytes:
					data=File.ReadAllBytes(rawPath);
					
					// Got headers:
					package.ReceivedHeaders(data.Length);
					
				}
				
				// Got data:
				package.ReceivedData(data,0,data.Length);
				
			}else{
				
				// Let the package know:
				package.Failed(404);
				
			}
			
		}
       
    }
   
}
#endif