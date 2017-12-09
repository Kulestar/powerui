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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// A group of multiple URLs (usually for images). Used by srcset.
	/// </summary>
	
	public class ContentGroup{
		
		/// <summary>Index of the src='' entry (-1 if none).</summary>
		public int SrcIndex=-1;
		/// <summary>The entries in the group.</summary>
		public List<ContentEntry> Entries=new List<ContentEntry>();
		
		
		/// <summary>Adds the src='' entry.</summary>
		public void AddSrc(string src){
			
			if(SrcIndex==-1){
				// Ordinary add:
				SrcIndex=Add(src,null);
				return;
			}
			
			// Update the SrcIndex entry:
			Entries[SrcIndex].Src=src;
			
		}
		
		/// <summary>Adds the given location to the group with an optional descriptor.</summary>
		public int Add(string src,string descriptor){
			
			// 1x by default:
			float descriptorValue=1f;
			int descriptorType=ContentEntry.DENSITY_DESCRIPTOR;
			
			if(!string.IsNullOrEmpty(descriptor)){
				
				// First, trim it:
				descriptor=descriptor.Trim();
				
				if(descriptor.Length>0){
					
					// Get the last character - either a 'w' or 'x':
					char end=descriptor[descriptor.Length-1];
					
					// Chop it off:
					descriptor=descriptor.Substring(0,descriptor.Length-1);
					
					// Parse it:
					float.TryParse(descriptor,out descriptorValue);
					
					if(end=='w'){
						
						// Width descriptor:
						descriptorType=ContentEntry.WIDTH_DESCRIPTOR;
						
					}
					
				}
				
			}
			
			// Create the entry and add it:
			ContentEntry ce=new ContentEntry();
			ce.Src=src;
			ce.Type=descriptorType;
			ce.Descriptor=descriptorValue;
			
			Entries.Add(ce);
			
			return Entries.Count-1;
			
		}
		
		/// <summary>Selects the content with the most suitable pixel density.</summary>
		public ContentEntry BestByDensity{
			get{
				return MostSuitable(ScreenInfo.DevicePixelRatio);
			}
		}
		
		/// <summary>Selects the most suitable entry for the given density/ width descriptor.</summary>
		public ContentEntry MostSuitable(float descriptor){
		
			ContentEntry current=null;
			float acceptedDelta=0f;
			
			// For each entry..
			for(int i=0;i<Entries.Count;i++){
				
				// Get the entry:
				ContentEntry entry=Entries[i];
				
				// The difference between this device and the entry:
				float ratioDelta=entry.Descriptor - descriptor;
				
				if(ratioDelta==0f){
					// Insta-win:
					return entry;
				}
				
				// We want the ratioDelta that is closest to zero and ideally positive.
				
				// So, this one 'wins' if any of these is true:
				// - It's the first one
				// - ratioDelta is +ve and acceptedDelta is -ve
				// - ratioDelta is closer to zero than acceptedDelta
				
				if(
					current==null || 
					(ratioDelta > 0f && acceptedDelta < 0f) ||
					Math.Abs(ratioDelta) < Math.Abs(acceptedDelta)
				){
					current=entry;
					acceptedDelta=ratioDelta;
				}
				
			}
			
			return current;
		}
		
	}
	
	/// <summary>A single entry in a ContentGroup. Used by srcset.</summary>
	public class ContentEntry{
		
		/// <summary>Used by ContentEntry.Type; set if this is a width descriptor.</summary>
		public const int WIDTH_DESCRIPTOR=1;
		/// <summary>Used by ContentEntry.Type; set if this is a pixel density descriptor.</summary>
		public const int DENSITY_DESCRIPTOR=2;
		
		/// <summary>The entry type.</summary>
		public int Type;
		/// <summary>The src of the content.</summary>
		public string Src;
		/// <summary>This contents pixel density or width descriptor.</summary>
		public float Descriptor=1f;
		
	}
	
}