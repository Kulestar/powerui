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
	
	/// <summary>
	/// A lookup is used to cache e.g. elements by ID.
	/// They hugely increase lookup speed and are automatically created for high traffic attributes (like ID).
	/// </summary>
	
	public class AttributeLookup{
		
		/// <summary>The raw lookup.</summary>
		public Dictionary<string,AttributeLookupLink> Lookup;
		
		
		public AttributeLookup(){
			
			Lookup=new Dictionary<string,AttributeLookupLink>();
			
		}
		
		/// <summary>How many unique values are in this lookup? This is *not* the number of elements.</summary>
		public int Count{
			get{
				return Lookup.Count;
			}
		}
		
		/// <summary>Adds the given element to this lookup.</summary>
		public void Add(string key,Element ele){
			
			// Create link:
			AttributeLookupLink link=new AttributeLookupLink(ele);
			
			// Already got a link?
			AttributeLookupLink chain;
			if(!Lookup.TryGetValue(key,out chain)){
				
				// Add it now:
				Lookup.Add(key,link);
				return;
			}
			
			// Follow the chain to the end and add it there:
			// We do this because it's rare in comparison to finding the "first" one which is always at the front.
			while(chain.Next!=null){
				chain=chain.Next;
			}
			
			// Add to the end:
			chain.Next=link;
			
		}
		
		/// <summary>Removes the given element value from this lookup.</summary>
		/// <returns>True if the cache should also be removed.</returns>
		public bool Remove(string key,Element ele){
			
			AttributeLookupLink chain;
			if(!Lookup.TryGetValue(key,out chain)){
				return false;
			}
			
			AttributeLookupLink previous=null;
			
			// Scan the chain looking for ele:
			while(chain!=null){
				
				if(chain.Element==ele){
					// Chop it out.
					
					if(previous==null){
						
						// Removing the first one.
						
						if(chain.Next==null){
							
							// Obliterate it!
							Lookup.Remove(key);
							
							if(Lookup.Count==0){
								// Remove this cache.
								return true;
							}
							
							return false;
							
						}else{
							
							// We're going to keep this link in the lookup, 
							// rather than removing it and putting the next one in instead.
							
							chain.Element=chain.Next.Element;
							chain.Next=chain.Next.Next;
							
						}
						
					}else{
						
						previous.Next=chain.Next;
						
					}
					
				}
				
				previous=chain;
				chain=chain.Next;
				
			}
		
			return false;
			
		}
		
	}
	
}