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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Blaze;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// An object which holds and retrieves different types of graphics
	/// such as animations, videos (pro only) and textures.
	/// </summary>
	
	public class ImagePackage:ContentPackage,AtlasEntity{
		
		/// <summary>The contents of this package. A particular format of image, e.g. a video or an spa etc.</summary>
		public ImageFormat Contents;

		
		/// <summary>Creates a new package for the named file to get. The path must be absolute.
		/// You must then call <see cref="PowerUI.ImagePackage.send"/> to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		public ImagePackage(string src){
			SetPath(src,null);
		}
		
		/// <summary>Creates a new package for the named file to get.
		/// You must then call <see cref="PowerUI.ImagePackage.send"/> to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any (may be null).</param>
		public ImagePackage(string src,Location relativeTo){
			SetPath(src,relativeTo);
		}
		
		/// <summary>Creates a package for the given already loaded contents.</summary>
		public ImagePackage(ImageFormat contents){
			Contents=contents;
		}
		
		/// <summary>Creates an image package containing the given image.</summary>
		/// <param name="image">The image for this image package. Used to display cached graphics.</param>
		public ImagePackage(Texture image){
			SetPath(image.name,null);
			AssignImage(image);
		}
		
		/// <summary>Creates an image package containing the given image.</summary>
		/// <param name="image">The image for this image package. Used to display cached graphics.</param>
		public ImagePackage(string src,Location relativeTo,Texture image){
			SetPath(src,relativeTo);
			AssignImage(image);
		}
		
		/// <summary>If the package contains a video, this gets the material that the video will playback on.</summary>
		public Material GetVideoMaterial(ShaderSet shaders){
			return Contents.GetImageMaterial(shaders.Isolated);
		}
		
		/// <summary>A material with just the single frame on it.</summary>
		public Material GetImageMaterial(ShaderSet shaders){
			return Contents.GetImageMaterial(shaders.Isolated);
		}
		
		/// <summary>A material with just the single frame on it.</summary>
		public Material GetImageMaterial(Shader shader){
			return Contents.GetImageMaterial(shader);
		}
		
		/// <summary>A material with just the single frame on it using the standard UI shader set.</summary>
		public Material GetImageMaterial(){
			return Contents.GetImageMaterial();
		}
		
		public bool MultiThreadDraw(){
			// When drawn to an atlas, the draw function can't be used on another thread.
			return false;
		}
		
		public void GetDimensionsOnAtlas(out int width,out int height){
			
			width=Width;
			height=Height;
			
		}
		
		public bool DrawToAtlas(TextureAtlas atlas,AtlasLocation location){
			
			return Contents.DrawToAtlas(atlas,location);
			
		}
		
		public int GetAtlasID(){
			return Contents.GetAtlasID();
		}
		
		/// <summary>Sends the request off. Callbacks such as onreadystatechange will be triggered.</summary>
		public void send(){
			
			if(readyState_==0){
				// Act like it just opened:
				readyState=1;
			}
			
			// Do we have a file protocol handler available?
			FileProtocol fileProtocol=location.Handler;
			
			if(fileProtocol==null){
				return;
			}
			
			// Some protocols enforce a particular content type.
			// E.g. Dynamic:// always is "dyn".
			string fileType=fileProtocol.ContentType(location);
			
			// Some file formats internally cache.
			// This means we can entirely avoid hitting the protocol system.
			
			// Get the format for the type:
			Contents=ImageFormats.GetInstance(fileType);
			
			// Did it internally cache?
			if(Contents.InternallyCached(location,this)){
				// Yes it did!
				return;
			}
			
			fileProtocol.OnGetGraphic(this);
			
		}
		
		/// <summary>Assign the given image to this package.</summary>
		public void AssignImage(Texture image){
			
			// Get the contents as a picture block:
			PictureFormat picture=Contents as PictureFormat;
			
			if(picture==null){
				// Clear the package:
				Clear();
				
				// Get the picture format:
				Contents=ImageFormats.GetInstance("pict");
				
				// Update picture var:
				picture=Contents as PictureFormat;
			}
			
			// Apply the image:
			picture.Image=image;
			
		}
		
		/// <summary>The system type of the content, e.g. PictureFormat.</summary>
		public Type ContentType{
			get{
				return Contents.GetType();
			}
		}
		
		/// <summary>Assigns the given texture to this package, setting it as a 200 OK.</summary>
		public void GotGraphic(Texture image){
			
			// Assign an image:
			AssignImage(image);
		
			// Straight to RS4:
			Done();
			
		}
		
		#if !MOBILE && !UNITY_WEBGL
		internal override void ReceivedMovieTexture(MovieTexture tex){
			
			// Apply it now:
			VideoFormat video=Contents as VideoFormat;
			video.Video=tex;
			
			base.ReceivedMovieTexture(tex);
			
		}
		#endif
		
		/// <summary>Called by the file handler when the contents are available.</summary>
		public override void ReceivedData(byte[] buffer,int offset,int count){
			ReceiveAllData(buffer, offset, count);
		}
		
		public override void ReceivedAllData(byte[] buffer){
			if(!Contents.LoadData(buffer, this)){
				// Failed:
				Failed(500);
			}
		}
		
		/// <summary>Called when this image is going to be displayed.</summary>
		public void GoingOnDisplay(Css.RenderableData context){
			if(Contents!=null){
				Contents.GoingOnDisplay(context);
			}
		}
		
		/// <summary>Called when this image is no longer being displayed.</summary>
		public void GoingOffDisplay(){
			if(Contents!=null){
				Contents.GoingOffDisplay();
			}
		}
		
		/// <summary>Removes all content from this image package.</summary>
		private void Clear(){
			// Clear any animation:
			GoingOffDisplay();
			Contents=null;
		}
		
		/// <summary>Checks if this package contains something loaded and useable.</summary>
		/// <returns>True if there is a useable graphic in this package.</returns>
		public bool Loaded{
			get{
				return (Contents!=null && Contents.Loaded);
			}
		}
		
		/// <summary>Gets the width of the graphic in this package.
		/// Note that you should check if it is <see cref="PowerUI.ImagePackage.Loaded"/> first.</summary>
		/// <returns>The width of the graphic.</returns>
		public int Width{
			get{
				return Contents.Width;
			}
		}
		
		/// <summary>Gets the height of the graphic in this package.
		/// Note that you should check if it is <see cref="PowerUI.ImagePackage.Loaded"/> first.</summary>
		/// <returns>The height of the graphic.</returns>
		public int Height{
			get{
				return Contents.Height;
			}
		}
		
	}
	
}