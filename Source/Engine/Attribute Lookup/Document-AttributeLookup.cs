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
	
	public partial class Document{
		
		/// <summary>An attribute index, used to dramatically speed up frequently used unique attributes (like id).</summary>
		public Dictionary<string,AttributeLookup> AttributeIndex;
		
		
		/// <summary>True if more than one attribute is cached.</summary>
		public bool AttributesCached{
			get{
				return (AttributeIndex!=null);
			}
		}
		
		/// <summary>Removes the given element from any attribute caches. You must check AttributesCached first.</summary>
		public void RemoveCachedElement(Element ele){
			
			List<string> toRemove=null;
			
			foreach(KeyValuePair<string,AttributeLookup> kvp in AttributeIndex){
				
				string attribute=kvp.Key;
				AttributeLookup lookup=kvp.Value;
				
				// Got this attribute?
				string value=ele.getAttribute(attribute);
				
				if(value==null){
					continue;
				}
				
				// Remove it:
				if(lookup.Remove(value,ele)){
					
					// Remove the lookup too!
					if(toRemove==null){
						toRemove=new List<string>();
					}
					
					toRemove.Add(attribute);
					
				}
				
			}
			
			if(toRemove==null){
				
				return;
				
			}
		
			if(toRemove.Count==AttributeIndex.Count){
				// Remove the whole thing:
				AttributeIndex=null;
				return;
			}
			
			for(int i=toRemove.Count-1;i>=0;i--){
				
				// Remove from attrib index:
				AttributeIndex.Remove(toRemove[i]);
				
			}
			
		}
		
		/// <summary>Request that the given attribute will be indexed.</summary>
		public void IndexAttribute(string attrib){
			
			attrib=attrib.ToLower();
			
			if(AttributeIndex!=null && AttributeIndex.ContainsKey(attrib)){
				return;
			}
			
			StartAttributeIndex(attrib);
			
		}
		
		/// <summary>Starts indexing the given attribute right now.</summary>
		public void StartAttributeIndex(string attrib){
			
			// Create the lookup:
			AttributeLookup lookup=new AttributeLookup();
			
			// Index the document:
			for(int i=0;i<childNodes_.length;i++){
				Dom.Element e=childNodes_[i] as Dom.Element;
				
				if(e==null){
					continue;
				}
				
				e.AddToAttributeLookup(attrib,lookup);
			}
			
			if(lookup.Count==0){
				// Well.. that was pointless! Nothing to actually index!
				return;
			}
			
			if(AttributeIndex==null){
				// Create the index now:
				AttributeIndex=new Dictionary<string,AttributeLookup>();
			}
			
			AttributeIndex[attrib]=lookup;
			
		}
		
	}
	
}