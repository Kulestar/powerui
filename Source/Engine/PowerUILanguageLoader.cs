//--------------------------------------
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using PowerUI;


namespace Dom{

	/// <summary>
	/// This class loads data for a language within Unity at runtime.
	/// The language must always be located in the Resources folder, under Resources/Languages/{path}.
	/// </summary>

	public class PowerUILanguageLoader:LanguageLoader{
		
		/// <summary>The information about all available languages.</summary>
		public static string MetaPath="Languages/languages";
		
		public PowerUILanguageLoader(string path):base(path){
			
		}
		
		/// <summary>Loads the language metadata (language names and their codes).</summary>
		public void Setup(){
			
			// Load language metadata now:
			string xml=(UnityEngine.Resources.Load(MetaPath) as UnityEngine.TextAsset).text;
			LanguageInfo.Load(xml);
			
		}
		
		/// <summary>Loads an XML file at the given absolute path.</summary>
		public override void LoadFile(string path,LanguageTextEvent onFileAvailable){
			
			// Create a data package:
			DataPackage package=new DataPackage(path);
			
			// Setup load event:
			package.onload=delegate(UIEvent e){
				
				// Run the callback:
				onFileAvailable(package.responseText);
				
			};
			
			// Get:
			package.send();
			
		}
		
	}
	
}