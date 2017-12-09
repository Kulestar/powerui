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

using System;
using Dom;


namespace Json{

	/// <summary>
	/// Represents an actual JSON value.
	/// </summary>

	public class JSValue:JSObject{
		
		/// <summary>The raw string value.</summary>
		public string Value;
		
		
		/// <summary>Creates an empty JSON value.</summary>
		public JSValue(){}
		
		/// <summary>Creates a new JSON value for the given string.</summary>
		public JSValue(string value){
			Value=value;
		}
		
		public override void ToJSONString(System.Text.StringBuilder builder){
			
			builder.Append('"');
			
			// \\, " and anything below 0x20:
			for(int i=0;i<Value.Length;i++){
				
				// Charcode:
				int code=(int)Value[i];
				
				if(code=='\\'){
					
					// Escape it:
					builder.Append("\\\\");
					
				}else if(code=='"'){
					
					// Escape it:
					builder.Append("\\\"");
					
				}else if(code=='\n'){
					
					// Escape it:
					builder.Append("\\n");
					
				}else if(code=='\r'){
					
					// Escape it:
					builder.Append("\\r");
					
				}else if(code=='\t'){
					
					// Escape it:
					builder.Append("\\t");
					
				}else if(code<0x20){
					
					// Escape it.
					builder.Append("\\u"+code.ToString("X4"));
					
				}else{
					
					// Append:
					builder.Append(Value[i]);
					
				}
				
			}
			
			builder.Append('"');
			
		}
		
		public override string ToString(){
			return Value;
		}
		
	}
	
}