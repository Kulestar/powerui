using System;
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Used by e.g. javascript:..
	/// </summary>
	
	public class JavascriptProtocol:FileProtocol{
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.</summary>
		public override string[] GetNames(){
			return new string[]{"javascript"};		
		}
		
		/// <summary>Get the raw file data.</summary>
		public override void OnGetDataNow(ContentPackage package){
			
			// Unsupported!
			package.Failed(501);
			
		}
		
	}
	
}