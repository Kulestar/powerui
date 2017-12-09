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
using System.Collections;
using System.Collections.Generic;
using Blaze;


namespace PowerUI{
	
	/// <summary>
	/// Provides some convenience methods for drawing to a texture.
	/// </summary>
	
	public class DynamicTexture:ImageFormat{
		
		/// <summary>The pending queue of textures which require updates.</summary>
		private static DynamicTexture ToUpdate=null;
		
		/// <summary>Repaints dynamic textures, writing out their pixels.</summary>
		public static void Update(){
			
			// For each one..
			DynamicTexture current=ToUpdate;
			ToUpdate=null;
			
			while(current!=null){
				
				DynamicTexture next=current.NextToUpdate;
				current.NextToUpdate=null;
				
				// Repaint:
				current.Repaint();
				
				// Next:
				current=next;
			}
			
		}
		
		/// <summary>An isolated material for this image.</summary>
		private Material IsolatedMaterial;
		
		/// <summary>A cached Texture_.height. Use Height instead.</summary>
		private int Height_;
		/// <summary>A cached Texture_.width. Use Width instead.</summary>
		private int Width_;
		/// <summary>The pixels of this texture.</summary>
		public Color32[] Pixels;
		/// <summary>True if this texture needs a refresh this frame.</summary>
		internal bool RefreshRequired;
		/// <summary>A placeholder texture which is only used by the atlas.</summary>
		private Texture2D Texture_;
		/// <summary>The rasteriser used to fill with.</summary>
		public Scanner Rasteriser;
		/// <summary>Next in the queue.</summary>
		private DynamicTexture NextToUpdate;
		
		/// <summary>Creates a new dynamic texture of the given dimensions.</summary>
		/// <param name="width">The width of the texture.</param>
		/// <param name="height">The height of the texture.</param>
		public DynamicTexture(int width,int height){
			// Setup the texture:
			Resize(width,height,false);
		}
		
		/// <summary>Creates an empty dynamic texture. Note that this is used internally by canvas.
		/// If used, you must call Resize to set it up.</summary>
		public DynamicTexture(){}
		
		/// <summary>Resize this texture on the X axis.</summary>
		/// <param name="width">The new width.</param>
		public void ResizeX(int width){
			Resize(width,Height_,false);
		}
		
		/// <summary>Resize this texture on the Y axis.</summary>
		/// <param name="height">The new height.</param>
		public void ResizeY(int height){
			Resize(Width_,height,false);
		}
		
		/// <summary>The filter mode of this dynamic texture. The default filtering is point.</summary>
		public override FilterMode FilterMode{
			get{
				if(Texture_==null){
					return FilterMode.Point;
				}
				return Texture_.filterMode;
			}
			set{
				Texture_.filterMode=value;
			}
		}
		
		/// <summary>Resize this texture on the X and Y axis, optionally only doing so if dimensions aren't zero.</summary>
		/// <param name="width">The new width.</param>
		/// <param name="height">The new height.</param>
		public bool Resize(int width,int height,bool clear){
			
			if(width==0 || height==0){
				return false;
			}
			
			if(Texture_!=null){
				
				int prevWidth=Width_;
				int prevHeight=Height_;
				
				if(width==prevWidth && height==prevHeight){
					// No change.
					return false;
				}
				
				Height_=height;
				Width_=width;
				Texture_.Resize(width,height);
				
				// Transfer the pixels:
				Color32[] newPixels=new Color32[width * height];
				
				if(!clear){
					
					// Smallest of the two:
					int minX=prevWidth>width ? width : prevWidth;
					int minY=prevHeight>height ? height : prevHeight;
					
					// (Anything out of range is just a clear pixel)
					int newIndex=0;
					int oldIndex=0;
					
					for(int y=0;y<minY;y++){
						
						for(int x=0;x<minX;x++){
							
							// Transfer:
							newPixels[newIndex+x]=Pixels[oldIndex+x];
							
						}
						
						newIndex+=width;
						oldIndex+=prevWidth;
						
					}
					
					RequestPaint();
					
				}
				
				// Apply:
				Pixels=newPixels;
				
				return true;
				
			}
			
			// Create the texture:
			Texture_=new Texture2D(width,height,TextureFormat.ARGB32,false);
			Texture_.filterMode=FilterMode.Point;
			Height_=height;
			Width_=width;
			
			// Setup the pixels:
			Pixels=new Color32[width * height];
			
			return true;
		}
		
		/// <summary>Fills the given path into this texture.</summary>
		public void Fill(VectorPath path,Color32 colour){
			
			if(Rasteriser==null){
				
				// Setup and start the rasteriser:
				Rasteriser=new Scanner();
				Rasteriser.SDFSize=0;
				Rasteriser.ScaleY=-1f;
				Rasteriser.Start();
				
			}
			
			// Figure out bounds:
			path.RecalculateBounds();
			
			int rWidth=Width;
			int rHeight=Height;
			
			Rasteriser.Rasterise(path,Pixels,rWidth,0,rWidth,rHeight,0f,-rHeight,colour,false);
			RequestPaint();
			
		}
		
		/// <summary>Requests the image to be redrawn. The Flush method will end up being called (if you use it).</summary>
		public void Refresh(){
			RequestPaint();
		}
		
		/// <summary>Requests for this image to be flushed on the next update.</summary>
		public void RequestPaint(){
			
			if(RefreshRequired){
				return;
			}
			
			RefreshRequired=true;
			
			// Enqueue
			NextToUpdate=ToUpdate;
			ToUpdate=this;
			
		}
		
		/// <summary>Tidies up this context.</summary>
		public void Destroy(){
			
			if(Texture_!=null){
				GameObject.Destroy(Texture_);
				Texture_=null;
			}
			
		}
		
		/// <summary>Draws a pixel at the given x/y coordinates to the atlas.</summary>
		/// <param name="x">The x coordinate in pixels from the left of the texture.</param>
		/// <param name="y">The y coordinate in pixels from the bottom of the texture.</param>
		/// <param name="colour">The colour of the pixel to draw.</param>
		public void DrawPixel(int x,int y,Color32 colour){
			if(x<0 || x>=Width_ || y<0 || y>=Height_){
				return;
			}
			Pixels[(y*Width_)+x]=colour;
			RequestPaint();
		}
		
		/// <summary>Draws a line on the atlas from one point to another.</summary>
		/// <param name="x">The x coordinate in pixels from the left of the start of the line.</param>
		/// <param name="y">The y coordinate in pixels from the left of the start of the line.</param>
		/// <param name="x2">The x coordinate in pixels from the left of the end of the line.</param>
		/// <param name="y2">The y coordinate in pixels from the left of the end of the line.</param>
		/// <param name="colour">The colour of the line.</param>
		public void DrawLine(int x,int y,int x2, int y2,Color32 colour){
			
			if(Texture_==null){
				return;
			}
			
			int w=x2-x;
			int h=y2-y;
			int dx1=0;
			int dy1=0;
			int dx2=0;
			int dy2=0;
			
			if(w<0){
				dx1=-1;
			}else if(w>0){
				dx1=1;
			}
			dx2=dx1;
			
			if(h<0){
				dy1=-1;
			}else if(h>0){
				dy1=1;
			}
			
			if(w<0){
				w=-w;
			}
			
			if(h<0){
				h=-h;
			}
			
			if(w<=h){
				int flip=w;
				w=h;
				h=flip;
				dy2=dy1;
				dx2=0;           
			}
			
			int numerator=w>>1;
			for(int i=0;i<=w;i++){
				DrawPixel(x,y,colour);
				numerator+=h;
				if(numerator>=w){
					numerator-=w;
					x+=dx1;
					y+=dy1;
				}else{
					x+=dx2;
					y+=dy2;
				}
			}
			RequestPaint();
		}
		
		/// <summary>Draws a filled circle on the atlas.</summary>
		/// <param name="x0">The x coordinate of the circles center in pixels from the left.</param>
		/// <param name="y0">The y coordinate of the circles center in pixels from the bottom.</param>
		/// <param name="radius">The radius of the circle, in pixels.</param>
		/// <param name="colour">The colour of the circle.</param>
		public void DrawCircle(int x0, int y0, int radius,Color32 colour){
			
			if(Texture_==null){
				return;
			}
			
			int y=0;
			int x=radius;
			int yChange=0;
			int radiusError=0;
			int xChange=1-radius*2;
			
			while(x>=y){
				// Filled circle:
				DrawLine(x + x0, y + y0,-x + x0, y + y0,colour);
				DrawLine(y + x0, x + y0,-y + x0, x + y0,colour);
				DrawLine(-y + x0, -x + y0,y + x0, -x + y0,colour);
				DrawLine(-x + x0, -y + y0,x + x0, -y + y0,colour);
				
				y++;
				radiusError+=yChange;
				yChange+=2;
				if(((radiusError<<1)+xChange)>0){
					x--;
					radiusError+=xChange;
					xChange+=2;
				}
			}
			
			RequestPaint();
		}
		
		/// <summary>Override this to use the special update queue - 
		/// essentially it avoids spamming writing out pixels (canvas does this part)
		/// and also avoids spam redrawing the actual graphic (but canvas doesn't do this; metering that is up to you).</summary>
		public virtual void Flush(){}
		
		/// <summary>Wipes the graphic clean using transparent black.</summary>
		public void Clear(){
			Clear(new Color32(0,0,0,0));
		}
		
		/// <summary>Wipes the graphic clean.</summary>
		/// <param name="clearColour">The colour to set the whole graphic to.</param>
		public void Clear(Color32 clearColour){
			
			if(Texture_==null){
				return;
			}
			
			for(int i=0;i<Pixels.Length;i++){
				Pixels[i]=clearColour;
			}
			RequestPaint();
		}
		
		/// <summary>Called by the update system to redraw this texture (only if it requires it).</summary>
		private void Repaint(){
			if(!RefreshRequired || Texture_==null){
				return;
			}
			
			// Flush now (optional method):
			try{
				Flush();
			}catch(Exception e){
				Dom.Log.Add("DynamicTexture: "+e);
			}
			
			RefreshRequired=false;
			Texture_.SetPixels32(Pixels);
			
			// Upload changes:
			Texture_.Apply(false);
		}
		
		public override string[] GetNames(){
			return new string[]{"-spark-dyn"};
		}
		
		public override Material GetImageMaterial(Shader shader){
			
			if(IsolatedMaterial==null){
				IsolatedMaterial=new Material(shader);
				IsolatedMaterial.SetTexture("_MainTex",Texture_);
			}
			
			return IsolatedMaterial;
		}
		
		public override Texture Texture{
			get{
				return Texture_;
			}
		}
		
		public override bool Isolate{
			get{
				return true;
			}
		}
		
		public override ImageFormat Instance(){
			return new DynamicTexture();
		}
		
		public override int Height{
			get{
				return Height_;
			}
		}
		
		public override int Width{
			get{
				return Width_;
			}
		}
		
		public override bool Loaded{
			get{
				return (Texture_!=null);
			}
		}
		
		public override void ClearX(){
			Texture_=null;
		}
		
	}
	
}