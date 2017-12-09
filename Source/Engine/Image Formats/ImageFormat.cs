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
using Css;
using Blaze;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a specific type of image format, e.g. a video or an SVG.
	/// </summary>
	
	[Values.Preserve]
	public class ImageFormat{
		
		/// <summary>The set of lowercase file types that this format will handle.</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		/// <summary>The height of the image.</summary>
		public virtual int Height{
			get{
				return 0;
			}
		}
		
		/// <summary>The width of the image.</summary>
		public virtual int Width{
			get{
				return 0;
			}
		}
		
		/// <summary>The texture this format holds (if any).</summary>
		public virtual Texture Texture{
			get{
				return null;
			}
		}
		
		/// <summary>Get/set the filter mode.</summary>
		public virtual FilterMode FilterMode{
			get{
				return FilterMode.Point;
			}
			set{}
		}
		
		/// <summary>Is this image loaded?</summary>
		public virtual bool Loaded{
			get{
				return false;
			}
		}
		
		/// <summary>Get the identifier used for this content on an atlas.</summary>
		public virtual int GetAtlasID(){
			return 0;
		}
		
		/// <summary>Should this image be isolated - i.e. off atlas.</summary>
		public virtual bool Isolate{
			get{
				return false;
			}
		}
		
		/// <summary>Creates an instance of this format.</summary>
		public virtual ImageFormat Instance(){
			return null;
		}
		
		/// <summary>A single-frame image material using the standard UI shader set. Used for e.g. videos and animations.</summary>
		public Material GetImageMaterial(){
			return GetImageMaterial(PowerUI.ShaderSet.Standard.Normal);
		}
		
		/// <summary>A single-frame image material. Used for e.g. videos and animations.</summary>
		public virtual Material GetImageMaterial(Shader shader){
			return null;
		}
		
		/// <summary>Some formats may cache their result internally. This checks and updates if it has.</summary>
		public virtual bool InternallyCached(Location path,ImagePackage package){
			return false;
		}
		
		/// <summary>Attempt to load the image from a Unity resource.</summary>
		public virtual bool LoadFromAsset(UnityEngine.Object asset,ImagePackage package){
			
			// Note: the full file should be called something.bytes for this to work in Unity.
			TextAsset text=asset as TextAsset;
			
			if(text==null){
				
				package.Failed(404);
				return false;
				
			}
			
			byte[] binary=text.bytes;
			
			package.ReceivedHeaders(binary.Length);
			
			// Apply it now:
			package.ReceivedData(binary,0,binary.Length);
			
			return true;
			
		}
		
		/// <summary>Loads the raw block of data into an object of this format.</summary>
		public virtual bool LoadData(byte[] data,ImagePackage package){
			return false;
		}
		
		/// <summary>Called when the host element is drawing.</summary>
		public virtual void OnLayout(RenderableData context,LayoutBox box,out float width,out float height){
			width=Width;
			height=Height;
		}
		
		/// <summary>Resets this image format container.</summary>
		public virtual void ClearX(){
		}
		
		/// <summary>Called when this image is going to be displayed.</summary>
		public virtual void GoingOnDisplay(Css.RenderableData context){
		}
		
		/// <summary>Called when this image is going to stop being displayed.</summary>
		public virtual void GoingOffDisplay(){
		}
		
		/// <summary>Draws this image to the given atlas.</summary>
		public virtual bool DrawToAtlas(TextureAtlas atlas,AtlasLocation location){
			return false;
		}
		
	}
	
}