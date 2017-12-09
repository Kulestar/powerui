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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerUI;

namespace PowerUI{
	
	/// <summary>
	/// A cache for resized images. Used with the resize:// protocol.
	/// </summary>
	
	public static class ResizedImages{
		
		/// <summary>The scale amount.</summary>
		public static float Scale=0.5f;
		/// <summary>All resized images.</summary>
		public static Dictionary<string,ResizedImage> Images=new Dictionary<string,ResizedImage>();
		
		
		/// <summary>Get the image for the given URL. May return null if it's not been cached.</summary>
		public static ResizedImage Get(string path){
			
			ResizedImage result;
			Images.TryGetValue(path,out result);
			
			return result;
			
		}
		
		/// <summary>Adds the given image to the cache and auto scales it based on the current screen.</summary>
		public static ResizedImage Add(string path,Texture2D originalImage){
			
			int originalHeight=originalImage.height;
			int originalWidth=originalImage.width;
			
			// Resize it:
			Texture2D resized=ImageResizer.Resize(originalImage,Scale);
			
			// Create and add:
			ResizedImage image=new ResizedImage(resized);
			
			image.OriginalHeight=originalHeight;
			image.OriginalWidth=originalWidth;
			
			// Add:
			Images[path]=image;
			
			return image;
			
		}
		
	}

}