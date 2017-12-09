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
	/// Manages all current image formats.
	/// </summary>
	
	public static class ImageFormats{
		
		/// <summary>The "picture" format (jpeg, png etc) is the default handler.</summary>
		public static string UnrecognisedImageHandler="pict";
		/// <summary>The set of available formats. Use get to access.</summary>
		public static Dictionary<string,ImageFormat> Formats;
		
		
		/// <summary>Adds the given image format to the global set for use.
		/// Note that you do not need to call this manually; Just deriving from
		/// ImageFormat is all that is required.</summary>
		/// <param name="formatType">The type for the format to add.</param>
		public static void Add(Type formatType){
			
			if(Formats==null){
				Formats=new Dictionary<string,ImageFormat>();
			}
			
			// Instance it:
			ImageFormat format=(ImageFormat)Activator.CreateInstance(formatType);
			
			string[] nameSet=format.GetNames();
			
			if(nameSet==null){
				return;
			}
			
			foreach(string name in nameSet){
				Formats[name.ToLower()]=format;
			}
			
		}
		
		/// <summary>Gets an instance of a format by the given file type.</summary>
		/// <param name="type">The name of the format, e.g. "png".</param>
		/// <returns>An ImageFormat.</returns>
		public static ImageFormat GetInstance(string type){
			ImageFormat handler=Get(type);
			return handler.Instance();
		}
		
		/// <summary>Gets a format by the given file type. Note: These are global!</summary>
		/// <param name="type">The name of the format, e.g. "png".</param>
		/// <returns>An ImageFormat if found; unrecognised handler otherwise.</returns>
		public static ImageFormat Get(string type){
			
			if(Formats==null){
				
				// Load the formats now!
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(ImageFormat),
					delegate(Type t){
						// Add it as format:
						ImageFormats.Add(t);
					}
				);
				
			}
			
			if(type==null){
				type="";
			}
			
			ImageFormat result=null;
			if(!Formats.TryGetValue(type.ToLower(),out result)){
				
				// Get the unrecognised handler:
				Formats.TryGetValue(UnrecognisedImageHandler,out result);
				
			}
			
			return result;
		}
		
	}
	
}