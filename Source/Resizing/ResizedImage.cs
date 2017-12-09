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

namespace PowerUI{
	
	/// <summary>
	/// Represents an image that has been resized by the ImageResizer.
	/// Cached such that they don't get resized again.
	/// </summary>
	
	public class ResizedImage{
		
		/// <summary>The new width.</summary>
		public int Width;
		
		/// <summary>New height.</summary>
		public int Height;
		
		/// <summary>Original width.</summary>
		public int OriginalWidth;
		
		/// <summary>Original height.</summary>
		public int OriginalHeight;
		
		/// <summary>The new resized image.</summary>
		public Texture2D Image;
		
		
		public ResizedImage(Texture2D image){
			
			// Get the settings:
			Image=image;
			Width=image.width;
			Height=image.height;
			
		}
		
	}

}