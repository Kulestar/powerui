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
using InfiniText;


namespace PowerUI{

	/// <summary>
	/// Represents a font suitable for displaying text on the screen with.
	/// There is one of these per document. It's that way because each document can potentially
	/// name a font-family something different (via @font-face).
	/// </summary>

	public class DynamicFont{
		
		/// <summary>The default font family. This is just the very first loaded family, or the internal font.</summary>
		public static DynamicFont DefaultFamily;
		/// <summary>The font that will attempt to load entirely if no other font is available. Absolute last resort.</summary>
		public static string InternalFont="DejaVu";
		
		
		/// <summary>Creates a new font by loading the font from resources.</summary>
		/// <param name="name">The name of the font to load.</param>
		/// <returns>A new dynamic font.</returns>
		public static DynamicFont Get(string name){
			
			// Start fonts if it needs it:
			Fonts.Start();
			
			// Create it:
			DynamicFont font=new DynamicFont(name);
			
			// Is a font family available already?
			font.Family=Fonts.Get(name);
			
			if(font.Family==null){
				
				// Try loading the faces from Resources:
				font.LoadFaces();
				
			}
			
			return font;
		}
		
		public static DynamicFont GetDefaultFamily(){
			if(DefaultFamily==null){
				DefaultFamily=Get(InternalFont);
			}
			
			return DefaultFamily;
		}
		
		/// <summary>The font family name. E.g. "Vera".</summary>
		public string Name;
		/// <summary>The underlying InfiniText font family.</summary>
		public FontFamily Family;
		
		
		/// <summary>Creates a new displayable font.</summary>
		/// <param name="name">The name of the font.</param>
		public DynamicFont(string name){
			Name=name;
		}
		
		/// <summary>True if this font is ready to go.</summary>
		public bool Loaded{
			get{
				return (Family!=null);
			}
		}
		
		/// <summary>Loads this font from the local project files. Should be a folder named after the font family and in it a series of font files.</summary>
		/// <returns>True if at least one font face was loaded.</returns>
		public bool LoadFaces(){
			
			if(string.IsNullOrEmpty(Name)){
				// This case results in every text asset being checked otherwise!
				return false;
			}
			
			// Load all .ttf/.otf files:
			object[] faces=Resources.LoadAll(Name,typeof(TextAsset));
			
			// For each font face..
			for(int i=0;i<faces.Length;i++){
				
				// Get the raw font face data:
				byte[] faceData=((TextAsset)faces[i]).bytes;
				
				if(faceData==null || faceData.Length==0){
					// It's a folder.
					continue;
				}
				
				// Load the font face - adds itself to the family within it if it's a valid font:
				try{
					
					FontFace loaded;
					
					if(Fonts.DeferLoad){
						loaded=FontLoader.DeferredLoad(faceData);
					}else{
						loaded=FontLoader.Load(faceData);
					}
					
					if(loaded!=null && Family==null){
						// Grab the family:
						Family=loaded.Family;
					}
					
				}catch{
					// Unity probably gave us a copyright file or something like that.
					// Generally the loader will catch this internally and return null.
				}
				
			}
			
			if(Family==null){
				return false;
			}
			
			return true;
			
		}
		
	}
	
}