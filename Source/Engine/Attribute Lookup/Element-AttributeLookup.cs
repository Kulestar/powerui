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


namespace Dom{
	
	
	public partial class Element{
		
		
		/// <summary>Adds this element to all fast lookups.</summary>
		public void AddToAttributeLookups(){
			
			foreach(KeyValuePair<string,AttributeLookup> kvp in document.AttributeIndex){
				
				AddToAttributeLookup(kvp.Key,kvp.Value);
				
			}
			
		}
		
		/// <summary>Adds this element to the given attribute lookup.</summary>
		public void AddToAttributeLookup(string attrib,AttributeLookup lookup){
			
			// Got the attribute?
			string value;
			if(Properties.TryGetValue(attrib,out value)){
				lookup.Add(value,this);
			}
			
			// Any kids got it?
			if(childNodes_==null){
				return;
			}
			
			// Add each one..
			for(int i=0;i<childNodes_.length;i++){
				
				// Add child elements too:
				Dom.Element el=(childNodes_[i] as Dom.Element );
				
				if(el==null){
					// E.g. text node.
					continue;
				}
				
				// Add:
				el.AddToAttributeLookup(attrib,lookup);
				
			}
			
		}
		
		
	}
	
}