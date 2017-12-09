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

	public class JSArray:JSObject{
		
		/// <summary>The indexed array mode. See IsIndexed.</summary>
		internal int IndexedMode;
		/// <summary>The held values.</summary>
		public Dictionary<string,JSObject> Values;
		
		
		/// <summary>The number of values in this object.</summary>
		protected override int internalLength{
			get{
				if(Values==null){
					return 0;
				}
				return Values.Count;
			}
		}
		
		/// <summary>Adds the given JSON object to this object.</summary>
		public override void push(JSObject value){
			if(Values==null){
				Values=new Dictionary<string,JSObject>();
			}
			
			// Push:
			Values[length.ToString()]=value;
		}
		
		/// <summary>Gets or sets entries from this object.</summary>
		public override JSObject this[string index]{
			get{
				if(Values==null){
					return null;
				}
				
				JSObject result;
				Values.TryGetValue(index.ToLower(),out result);
				return result;
			}
			set{
				if(Values==null){
					Values=new Dictionary<string,JSObject>();
				}
				
				Values[index.ToLower()]=value;
			}
		}
		
		/// <summary>Is this an indexed array? Note that this is a *best guess*.
		/// Set it manually if you want to ensure it is/ is not.</summary>
		public bool IsIndexed{
			get{
				
				if(IndexedMode!=0){
					
					return (IndexedMode==1);
					
				}
				
				if(Values==null){
					return true;
				}
				
				for(int i=0;i<Values.Count;i++){
					
					if(!Values.ContainsKey(i.ToString())){
						return false;
					}
					
				}
				
				// Great - they were all numeric and in order!
				return true;
				
			}
			set{
				
				if(value){
					IndexedMode=1;
				}else{
					IndexedMode=2;
				}
				
			}
		}
		
		/// <summary>Turns this JSON object into a JSON formatted string.</summary>
		public override void ToJSONString(System.Text.StringBuilder builder){
			
			if(IsIndexed){
				
				builder.Append('[');
				
				if(Values!=null){
					
					for(int i=0;i<Values.Count;i++){
						
						if(i!=0){
							builder.Append(',');
						}
						
						JSObject value;
						Values.TryGetValue(i.ToString(),out value);
							
						if(value==null){
							builder.Append("null");
						}else{
							value.ToJSONString(builder);
						}
						
					}
					
				}
				
				builder.Append(']');
				return;
				
			}
			
			builder.Append('{');
			bool first=true;
			
			if(Values!=null){
				
				foreach(KeyValuePair<string,JSObject> kvp in Values){
					
					if(first){
						first=false;
					}else{
						builder.Append(',');
					}
					
					builder.Append("\""+kvp.Key.Replace("\"","\\\"")+"\":");
					
					if(kvp.Value==null){
						builder.Append("null");
					}else{
						kvp.Value.ToJSONString(builder);
					}
					
				}
				
			}
			
			builder.Append('}');
			
		}
		
		public override string ToString(){
			return ToJSONString();
		}
		
		public override IEnumerator< KeyValuePair<string,JSObject> > GetEnumerator(){
			
			if(Values==null){
				return null;
			}
			
			/*
			if(IndexedMode==1){
				
				for(int i=0;i<Values.Count;i++){
					
					string key=i.ToString();
					
					JSObject value;
					Values.TryGetValue(key,out value);
					yield return new KeyValuePair<string,JSObject>(key,value);
					
				}
				
			}else{
			*/
			return Values.GetEnumerator();
			
		}
		
	}
	
}