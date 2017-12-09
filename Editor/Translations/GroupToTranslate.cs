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

using PowerUI.Http;
using Json;
using Dom;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// A group of variables to translate.
	/// </summary>
	
	public class GroupToTranslate{
		
		/// <summary>The file that the results will be saved to.</summary>
		public string TargetPath;
		/// <summary>The variables to be translated.</summary>
		public VariableSet Variables;
		
		
		public GroupToTranslate(string targetPath,VariableSet toTranslate){
			TargetPath=targetPath;
			Variables=toTranslate;
		}
		
		/// <summary>Called when the translation is complete.</summary>
		/// <param name="results">The translated result set.</param>
		public void Complete(JSArray results){
			
			// Results is the assoc array. Build the XML now:
			bool first=true;
			StringBuilder builder=new StringBuilder();
			
			foreach(KeyValuePair<string,JSObject> kvp in results){
				
				if(first){
					first=false;
				}else{
					builder.Append("\r\n");
				}
				
				builder.Append("<var name='"+kvp.Key+"'>\r\n\t");
				builder.Append(kvp.Value);
				builder.Append("\r\n</var>");
				
			}
			
			// Get the result:
			string result=builder.ToString();
			
			// Write it out now:
			File.WriteAllText(TargetPath,result);
			
		}
		
	}
	
}