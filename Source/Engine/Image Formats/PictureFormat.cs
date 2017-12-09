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
using UnityEngine;
using Blaze;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents the default "picture" format. Png, jpeg etc are handled with this.
	/// </summary>
	
	public class PictureFormat:ImageFormat{
		
		/// <summary>The image texture retrieved.</summary>
		public Texture Image;
		/// <summary>An isolated material for this image.</summary>
		public Material IsolatedMaterial;
		
		
		public PictureFormat(){}
		
		public PictureFormat(Texture image){
			Image=image;
		}
		
		/// <summary>Should this image be isolated - i.e. off atlas.</summary>
		public override bool Isolate{
			get{
				return Image is RenderTexture;
			}
		}
		
		public override string[] GetNames(){
			return new string[]{"pict","jpg","jpeg","png","bmp","tga","iff"};
		}
		
		public override FilterMode FilterMode{
			set{
				if(Image==null){
					return;
				}
				
				Image.filterMode=value;
			}
			get{
				return Image.filterMode;
			}
		}
		
		public override int Height{
			get{
				return Image.height;
			}
		}
		
		public override int Width{
			get{
				return Image.width;
			}
		}
		
		public override bool Loaded{
			get{
				return (Image!=null);
			}
		}
		
		public override ImageFormat Instance(){
			return new PictureFormat();
		}
		
		public override Material GetImageMaterial(Shader shader){
			
			if(IsolatedMaterial==null){
				IsolatedMaterial=new Material(shader);
				IsolatedMaterial.SetTexture("_MainTex",Image);
				
				// Clamp the image:
				Image.wrapMode=TextureWrapMode.Clamp;
				
			}
			
			return IsolatedMaterial;
			
		}
		
		public override Texture Texture{
			get{
				return Image;
			}
		}
		
		public override bool LoadFromAsset(UnityEngine.Object asset,ImagePackage package){
			
			Image=asset as Texture2D;
			
			if(Image!=null){
				return true;
			}
			
			if(asset!=null && !(asset is TextAsset)){
				Dom.Log.Add(
					"That's not an image - Url '"+package.location.absolute+"' is a '"+
					asset.GetType()+"'. In Unity this happens if you've got more than one resource with the same name (but different file types)."
				);
				
				return false;
			}
			
			// Try binary:
			return base.LoadFromAsset(asset,package);
		}
		
		public override bool LoadData(byte[] data,ImagePackage package){
			
			// Create it:
			Texture2D image = new Texture2D(0,0);
			
			// Load it:
			image.LoadImage(data);
			
			// Apply:
			Image=image;
			
			return true;
			
		}
		
		public override int GetAtlasID(){
			return Image.GetInstanceID();
		}
		
		public override bool DrawToAtlas(TextureAtlas atlas,AtlasLocation location){
			
			// Only ever called with a static image:
			Color32[] pixelBlock=(Image as Texture2D).GetPixels32();
			
			int index=0;
			int atlasIndex=location.BottomLeftPixel();
			
			int height=Image.height;
			int width=Image.width;
			
			// How many pixels must we add on to the end of the row to get to
			// the start of the row above? This is simply the dimension of the atlas:
			int rowDelta=atlas.Dimension;
			
			for(int h=0;h<height;h++){
				
				Array.Copy(pixelBlock,index,atlas.Pixels,atlasIndex,width);
				index+=width;
				atlasIndex+=rowDelta;
				
			}
			
			return true;
			
		}
		
	}
	
}