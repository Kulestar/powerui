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
	/// Manages all current audio formats.
	/// </summary>
	
	public static class AudioFormats{
		
		/// <summary>The "ogg" format is the default handler.</summary>
		public static string UnrecognisedImageHandler="ogg";
		/// <summary>The set of available formats. Use get to access.</summary>
		public static Dictionary<string,AudioFormat> Formats;
		
		
		/// <summary>Adds the given audio format to the global set for use.
		/// Note that you do not need to call this manually; Just deriving from
		/// AudioFormat is all that is required.</summary>
		/// <param name="formatType">The type for the format to add.</param>
		public static void Add(Type formatType){
			
			if(Formats==null){
				Formats=new Dictionary<string,AudioFormat>();
			}
			
			// Instance it:
			AudioFormat format=(AudioFormat)Activator.CreateInstance(formatType);
			
			string[] nameSet=format.GetNames();
			
			if(nameSet==null){
				return;
			}
			
			foreach(string name in nameSet){
				Formats[name.ToLower()]=format;
			}
			
		}
		
		/// <summary>Gets an instance of a format by the given file type.</summary>
		/// <param name="type">The name of the format, e.g. "ogg".</param>
		/// <returns>An AudioFormat.</returns>
		public static AudioFormat GetInstance(string type){
			AudioFormat handler=Get(type);
			return handler.Instance();
		}
		
		/// <summary>Gets a format by the given file type. Note: These are global!</summary>
		/// <param name="type">The name of the format, e.g. "ogg".</param>
		/// <returns>An AudioFormat if found; unrecognised handler otherwise.</returns>
		public static AudioFormat Get(string type){
			
			if(Formats==null){
				
				// Load the formats now!
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(AudioFormat),
					delegate(Type t){
						// Add it as format:
						AudioFormats.Add(t);
					}
				);
				
			}
			
			if(type==null){
				type="";
			}
			
			AudioFormat result=null;
			if(!Formats.TryGetValue(type.ToLower(),out result)){
				
				// Get the unrecognised handler:
				Formats.TryGetValue(UnrecognisedImageHandler,out result);
				
			}
			
			return result;
		}
		
	}
	
}