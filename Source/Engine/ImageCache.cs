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
	/// There may be times when you want to display an image for which you only have the
	/// Texture2D object, a RenderTexture or an SPA file. In this case, you can add it to the cache with a given name and then
	/// access it in PowerUI using cache://theNameYouUsed. The most useful method for this is <see cref="PowerUI.ImageCache.Add"/>.
	/// This avoids having to serialize the image and unserialize it again (i.e. as with the data:// URLs).
	/// </summary>
	
	public static class ImageCache{
		
		/// <summary>The set of all cached textures.</summary>
		private static Dictionary<string,ImageFormat> Lookup=new Dictionary<string,ImageFormat>();
		
		/// <summary>Adds an image to the cache. Texture2D or RenderTexture.</summary>
		/// <param name="address">The name to use to find your image.</param>
		/// <param name="image">The image to store in the cache.</param>
		public static void Add(string address,Texture image){
			Lookup[address]=new PictureFormat(image);
		}
		
		/// <summary>Adds an image to the cache. Used by e.g. SPA etc.</summary>
		/// <param name="address">The name to use to find your image.</param>
		/// <param name="image">The image to store in the cache.</param>
		public static void Add(string address,ImageFormat image){
			Lookup[address]=image;
		}
		
		/// <summary>Gets a named image from the cache.</summary>
		/// <param name="address">The name of the image to find.</param>
		/// <returns>A Texture2D if it's found; null otherwise.</returns>
		public static ImageFormat Get(string address){
			ImageFormat result;
			Lookup.TryGetValue(address,out result);
			return result;
		}
		
		/// <summary>Removes an image from the cache.</summary>
		/// <param name="address">The name of the image to remove.</param>
		public static void Remove(string address){
			Lookup.Remove(address);
		}
		
		/// <summary>Clears the cache of all its contents.</summary>
		public static void Clear(){
			Lookup.Clear();
		}

	}
	
}