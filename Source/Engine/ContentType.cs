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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Dom;

namespace PowerUI{
	
	/// <summary>
	/// A helper for detecting some file types.
	/// </summary>
	
	public static class ContentType{
		
		/// <summary>Is the given lowercase type a video?</summary>
		/// <param name="type">The content type in lowercase, e.g. "ogg".</param>
		/// <returns>True if the given type is a type of video supported by Unity.</returns>
		public static bool IsVideo(string type){
			
			switch(type){
				case "mov":
				case "mpg":
				case "mpeg":
				case "mp4":
				case "avi":
				case "asf":
				case "ogg":
				case "ogv":
					return true;
			}
			
			return false;
			
		}
		
	}
	
}