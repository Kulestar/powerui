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
	/// Caches AssetBundles by their original URI. Generally you'd use the rather weird looking
	/// bundle URI schema bundle://https://cdn.yoursite.com/.. instead of using this directly.
	/// (That URI would result in an entry here with an address of "https://cdn...").
	/// </summary>
	
	public static class Bundles{
		
		/// <summary>Bundles loading.</summary>
		public static Dictionary<string,DataPackage> Loading=new Dictionary<string,DataPackage>();
		
		/// <summary>The set of all cached bundles.</summary>
		private static Dictionary<string,AssetBundle> Lookup=new Dictionary<string,AssetBundle>();
		
		/// <summary>Adds a bundle to the cache.</summary>
		/// <param name="address">The name to use to find your bundle.</param>
		/// <param name="bundle">The bundle to store in the cache.</param>
		public static void Add(string address,AssetBundle bundle){
			Lookup[address]=bundle;
		}
		
		/// <summary>Gets a named bundle from the cache.</summary>
		/// <param name="address">The name of the bundle to find.</param>
		/// <returns>A Texture2D if it's found; null otherwise.</returns>
		public static AssetBundle Get(string address){
			AssetBundle result;
			Lookup.TryGetValue(address,out result);
			return result;
		}
		
		/// <summary>Removes a bundle from the cache.</summary>
		/// <param name="address">The name of the bundle to remove.</param>
		public static void Remove(string address){
			Lookup.Remove(address);
		}
		
		/// <summary>Clears the cache of all its contents.</summary>
		public static void Clear(){
			Lookup.Clear();
		}

	}
	
}