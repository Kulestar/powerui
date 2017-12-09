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
using System.Reflection;
using Dom;


namespace PowerUI.Compression{
	
	/// <summary>
	/// Various compression algorithms available for direct use. You can also delete the ones you don't want.
	/// Keep in mind that zlib is used by WOFF and brotli is used by WOFF2.
	/// Try Compression.Get("zlib").Decompress(aStream,aBlockOfBytes);
	/// </summary>
	
	public static class Compression{
		
		/// <summary>All available compression types. e.g. 'zlib' or 'brotli' are available by default.</summary>
		public static Dictionary<string,Compressor> All;
		
		/// <summary>Adds a compressor the global set.
		/// This is generally done automatically, but you can also add one manually if you wish.</summary>
		/// <param name="compressorType">The type of the compressor to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(Type compressorType){
			
			if(All==null){
				// Create the set:
				All=new Dictionary<string,Compressor>();
			}
			
			// Instance it:
			Compressor compressor=(Compressor)Activator.CreateInstance(compressorType);
			
			string[] names=compressor.GetNames();
			
			if(names==null||names.Length==0){
				return false;
			}
			
			for(int i=0;i<names.Length;i++){
				
				// Grab the name:
				string name=names[i];
				
				if(name==null){
					continue;
				}
				
				// Add it to functions:
				All[name.ToLower()]=compressor;
				
			}
			
			return true;
		}
		
		/// <summary>Attempts to find the named at rule, returning the global instance if it's found.</summary>
		/// <param name="name">The rule to look for.</param>
		/// <returns>The global Compressor if the rule was found; Null otherwise.</returns>
		public static Compressor Get(string name){
			
			if(All==null){
				
				// Load the compressors now!
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(Compressor),
					delegate(Type type){
						// Add it as a compression option:
						Compression.Add(type);
					}
				);
				
			}
			
			Compressor globalFunction=null;
			All.TryGetValue(name,out globalFunction);
			return globalFunction;
		}
		
	}
	
}