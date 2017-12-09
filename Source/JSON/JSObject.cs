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
using System.Collections;
using System.Collections.Generic;


namespace Json{

	/// <summary>
	/// An object returned from JSON text.
	/// </summary>

	public class JSObject:IEnumerable< KeyValuePair<string,JSObject> >{
		
		/// <summary>The number of values in this object.</summary>
		protected virtual int internalLength{
			get{
				return 0;
			}
		}
		
		/// <summary>The number of values in this object.</summary>
		public int length{
			get{
				return internalLength;
			}
		}
		
		/// <summary>Adds the given string to this object.</summary>
		public void push(string value){
			push(new JSValue(value));
		}
		
		/// <summary>Adds the given JSON object to this object.</summary>
		public virtual void push(JSObject value){
		}
		
		/// <summary>Gets or sets entries from this object.</summary>
		public virtual JSObject this[string index]{
			get{
				return null;
			}
			set{
			}
		}
		
		/// <summary>Gets or sets entries from this object.</summary>
		public JSObject this[int index]{
			get{
				return this[index.ToString()];
			}
			set{
				this[index.ToString()]=value;
			}
		}
		
		/// <summary>Gets the named index as a string, then trims and upper/lowercases it.
		/// Null if the index doesn't exist.</summary>
		public string CaseString(string index,bool upperCase){
			
			string s=String(index);
			
			if(s==null){
				return null;
			}
			
			if(upperCase){
				return s.Trim().ToUpper();
			}
			
			return s.Trim().ToLower();
			
		}
		
		/// <summary>Gets the named index as a string. Null if the index doesn't exist.</summary>
		public string String(string index){
			
			JSObject value=this[index];
			
			if(value==null){
				return null;
			}
			
			return value.ToString();
			
		}
		
		/// <summary>Turns this JSON object into a JSON formatted string.</summary>
		public string ToJSONString(){
			System.Text.StringBuilder builder=new System.Text.StringBuilder();
			ToJSONString(builder);
			return builder.ToString();
		}
		
		/// <summary>Turns this JSON object into a JSON formatted string.</summary>
		public virtual void ToJSONString(System.Text.StringBuilder builder){
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
		public virtual IEnumerator< KeyValuePair<string,JSObject> > GetEnumerator(){
			return null;
		}
		
	}
	
}