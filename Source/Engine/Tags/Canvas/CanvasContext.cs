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
using InfiniText;


namespace PowerUI{
	
	/// <summary>
	/// Represents a canvas context which lets you draw 2D shapes and polygons on the UI.
	/// See the canvas html tag for more information.
	/// </summary>
	
	public class CanvasContext{
		
		/*
		#region GPU Accelerated Canvas
		
		/// <summary>Used by Loonim in GPU draw mode. The draw target.</summary>
		public Loonim.DrawStack DrawSurface;
		/// <summary>Used by Loonim in GPU draw mode. 
		/// This is the thing we draw to the draw surface.</summary>
		public Loonim.MaterialStackNodeCleared DrawNode;
		/// <summary>Helps build stroke meshes.</summary>
		private Loonim.StrokePathMesh StrokeHelper=null;
		
		
		/// <summary>Runs this context in GPU mode.</summary>
		public void Accelerate(){
			
			// Create the DrawStack (which holds the actual target texture; 
			// they're stacks because Loonim would normally stack nodes in there):
			DrawSurface = new Loonim.DrawStack();
			
			// (We only ever need one of these nodes as canvas doesn't retain any other information by design)
			// I.e. it just needs to be able to splat 1 mesh at a time.
			DrawNode = new Loonim.MaterialStackNodeCleared();
			DrawNode.Stack = DrawSurface;
			
			// It'll always use the 'clipping path' node (#114):
			DrawNode.Material = new Material(Shader.Find("Loonim/114"));
			
			// Next, we need both a triangulator (fill) and a stroke helper (stroke):
			StrokeHelper=new Loonim.StrokePathMesh();
			
		}
		
		#endregion
		*/
		
		/// <summary>The rasteriser used to fill with.</summary>
		public Scanner Rasteriser;
		
		/// <summary>True if the path requires clipping for stroke/fill.
		/// A path is clipped to avoid huge delays caused by points at infinity.</summary>
		internal bool Clip_;
		/// <summary>The underlying path.</summary>
		public VectorPath Path;
		/// <summary>The element for the canvas that this is a context for.</summary>
		private HtmlCanvasElement Canvas;
		/// <summary>The target rendering plane. Always exists.</summary>
		private DynamicTexture ImageData_;
		/// <summary>The target rendering plane. Always exists.</summary>
		public DynamicTexture ImageData{
			get{
				return ImageData_;
			}
		}
		
		/// <summary>The current fill colour. See fillStyle. Default is black.</summary>
		public Color FillColour=new Color(0,0,0,1f);
		/// <summary>The current stroke colour. See strokeStyle. Default is black.</summary>
		public Color StrokeColour=new Color(0,0,0,1f);
		
		
		/// <summary>Creates a new canvas context for the given canvas element.</summary>
		/// <param name="canvas">The canvas for a particular canvas element.</param>
		public CanvasContext(HtmlCanvasElement canvas){
			// Apply the tag:
			Canvas=canvas;
			Path=new VectorPath();
			ImageData_=new DynamicTexture();
			canvas.SetImage(ImageData_);
		}
		
		/// <summary>True if the canvas has data ready to be uploaded to the GPU.</summary>
		public bool AlreadyDrawnTo{
			get{
				return ImageData.RefreshRequired;
			}
		}
		
		/// <summary>Creates an infinitext glyph from the path you've just made.</summary>
		public Glyph ToGlyph(){
			
			Glyph glyph=new Glyph();
			
			glyph.FirstPathNode=Path.FirstPathNode;
			glyph.LatestPathNode=Path.LatestPathNode;
			glyph.PathNodeCount=Path.PathNodeCount;
			
			// Compute the meta:
			glyph.RecalculateMeta();
			
			// Squish it to being at most 1 unit tall:
			if(glyph.Height>1f){
				
				glyph.Scale(1f/glyph.Height);
				
			}
			
			// Setup a default advance width:
			glyph.AdvanceWidth=glyph.Width;
			
			// Clear the path:
			Path.Clear();
			
			return glyph;
			
		}
		
		/// <summary>This canvas as a png. Null if it contains nothing.</summary>
		public byte[] pngData{
			get{
				
				if(ImageData_.Texture==null){
					return null;
				}
				
				return (ImageData_.Texture as Texture2D).EncodeToPNG();
			}
		}
		
		/// <summary>Starts creating a path on this context. Used to draw and fill in any kind of shape.</summary>
		public void beginPath(){
			// Just clear the path:
			Path.Clear();
			Clip_=false;
		}
		
		/// <summary>Closes the current path such that it forms a loop by drawing a line to the first node.</summary>
		public void closePath(){
			Path.ClosePath();
			Clip_=true;
		}
		
		/// <summary>The current number of nodes on the current path.</summary>
		public int PathNodeCount{
			get{
				return Path.PathNodeCount;
			}
		}
		
		/// <summary>Adds a line from the current pen location to the given point. Note that nothing will
		/// be seen until you call a fill or stroke method.</summary>
		public void lineTo(float x,float y){
			
			if(Path.PathNodeCount==0){
				
				Path.MoveTo(0f,0f);
				
			}
			
			Path.LineTo(x,y);
			Clip_=true;
			
		}
		
		/// <summary>Creates an arc around the given circle center. Note that nothing will
		/// be seen until you call a fill or stroke method.</summary>
		public void arc(float centerX,float centerY,float radius,float sAngle,float eAngle){
			arc(centerX,centerY,radius,sAngle,eAngle,false);
		}
		
		/// <summary>Creates an arc around the given circle center. Note that nothing will
		/// be seen until you call a fill or stroke method.</summary>
		public void arc(float centerX,float centerY,float radius,float sAngle,float eAngle,bool counterClockwise){
			
			VectorPoint previous=Path.LatestPathNode;
			
			float x0;
			float y0;
			
			if(previous==null){
				x0=0f;
				y0=0f;
			}else{
				x0=previous.X;
				y0=previous.Y;
			}
			
			// Clockwise eAngle > sAngle; counter clockwise otherwise.
			if(eAngle>sAngle){
				if(counterClockwise){
					// Get them both in range:
					eAngle=eAngle%(Mathf.PI*2f);
					sAngle=sAngle%(Mathf.PI*2f);
					
					// Reduce eAngle by a full rotation so it's smaller:
					eAngle-=(Mathf.PI*2f);
				}
			}else if(!counterClockwise){
				// Get them both in range:
				eAngle=eAngle%(Mathf.PI*2f);
				sAngle=sAngle%(Mathf.PI*2f);
					
				// Reduce sAngle by a full rotation so it's smaller:
				sAngle-=(Mathf.PI*2f);
			}
			
			
			// First, figure out where the actual start is.
			// It's radius units to the right of center, then rotated through radius around center.
			// Thus we have a triangle with hyp length of 'radius' and an angle of sAngle:
			float startX=radius * (float) Math.Cos(sAngle);
			float startY=radius * (float) Math.Sin(sAngle);
			
			// Now find the end point, using exactly the same method:
			float endX=radius * (float) Math.Cos(eAngle);
			float endY=radius * (float) Math.Sin(eAngle);
			
			// We now have an arc from the current position to endX/endY.
			// The start and exit node angles are usefully just offset from the given ones.
			// This is because an sAngle of zero should create an arc which starts travelling downwards.
			// (Or upwards if it's counter clockwise):
			
			// Where does the arc start from?
			float arcStartX=centerX+startX;
			float arcStartY=centerY+startY;
			
			if(Path.FirstPathNode==null){
				// This occurs if the arc is the first thing we draw. No line is drawn to it.
				Path.MoveTo(arcStartX,arcStartY);
			}else if(arcStartX!=x0 || arcStartY!=y0){
				// Draw a line to this point:
				Path.LineTo(arcStartX,arcStartY);
			}
			
			// Create the new arc node:
			ArcLinePoint arcNode=new ArcLinePoint(centerX+endX,centerY+endY);
			
			// Apply the radius:
			arcNode.Radius=radius;
			
			// Apply the angles:
			arcNode.StartAngle=sAngle;
			arcNode.EndAngle=eAngle;
			
			// Apply the center:
			arcNode.CircleCenterX=centerX;
			arcNode.CircleCenterY=centerY;
			
			// Add the other end:
			Path.AddPathNode(arcNode);
			Clip_=true;
		}
		
		/// <summary>Is the specified point in (not on) the current path?</summary>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		public bool isPointInPath(float x,float y){
			VectorPoint node=Path.FirstPathNode;
			
			// For each one..
			while(node!=null){
				
				// Got a match?
				if(node.X==x && node.Y==y){
					return true;
				}
				
				// Hop to the next one:
				node=node.Next;
				
			}
			
			return false;
		}
		
		public void curveTo(float c1x,float c1y,float c2x,float c2y,float x,float y){
			Path.CurveTo(c1x,c1y,c2x,c2y,x,y);
			Clip_=true;
		}
		
		public void quadraticCurveTo(float cx,float cy,float x,float y){
			Path.QuadraticCurveTo(cx,cy,x,y);
			Clip_=true;
		}
		
		/// <summary>Tidies up this context.</summary>
		public void Destroy(){
			
			if(ImageData_!=null){
				ImageData_.Destroy();
			}
			
		}
		
		/// <summary>Adds an arc between the given points using the pen point as a control point. 
		/// Note that nothing will be seen until you call a fill or stroke method.</summary>
		public void arcTo(float x1,float y1,float x2,float y2,float radius){
			
			VectorPoint previous=Path.LatestPathNode;
			
			float x0;
			float y0;
			
			if(previous==null){
				x0=0f;
				y0=0f;
			}else{
				x0=previous.X;
				y0=previous.Y;
			}
			
			// How this works:
			// Line from 0->1.
			// Another from 2->1, thus forming a 'triangle'.
			// Slide a circle of the given radius in this triangle such that it's
			// as far in as it will go whilst just touching both lines.
			// Draw an arc from the two intersection points of the circle with the triangle.
			
			// What we need to find:
			// - Circle center.
			// - The start and end angles. Always clockwise.
			
			// Create the new arc node:
			ArcLinePoint arcNode=new ArcLinePoint(x2,y2);
			
			// Apply the radius:
			arcNode.Radius=radius;
			
			// First find the angle of 1 from +ve x. This is just an Atan2 call:
			float angleLine2=(float)Math.Atan2(y0-y1,x0-x1);
			
			// Find the angle of 2 relative to 1.
			// As we know the angle of 1 relative to +ve x, we can find 1 relative to +ve x
			// and then grab the difference.
			float rotateThrough=(float)Math.Atan2(y2-y1,x2-x1)-angleLine2;
			
			// Find half of the angle:
			float halfAngle=rotateThrough/2f;
			
			// What's the distance of point 1 to the circle center?
			float distanceToCenter=radius/(float)Math.Cos(halfAngle);
			
			// Resolve the x coordinate of the center:
			arcNode.CircleCenterX=(distanceToCenter * (float) Math.Cos(halfAngle - angleLine2)) + x1;
			
			// Resolve the y coordinate of the center:
			arcNode.CircleCenterY=(distanceToCenter * (float) Math.Sin(halfAngle - angleLine2)) + y1;
			
			// Apply the angles:
			arcNode.StartAngle= - rotateThrough - angleLine2;
			arcNode.EndAngle=arcNode.StartAngle - rotateThrough;
			
			float arcStartNodeX;
			float arcStartNodeY;
			arcNode.SampleAt(0f,out arcStartNodeX,out arcStartNodeY);
			
			float arcEndNodeX;
			float arcEndNodeY;
			arcNode.SampleAt(1f,out arcEndNodeX,out arcEndNodeY);
			
			if(Path.FirstPathNode==null){
				// This occurs if the arc is the first thing we draw. No line is drawn to it.
				Path.MoveTo(arcStartNodeX,arcStartNodeY);
			}else if(x0!=arcStartNodeX || y0!=arcStartNodeY){
				Path.LineTo(arcStartNodeX,arcStartNodeY);
			}
			
			// Apply the node location:
			arcNode.X=arcEndNodeX;
			arcNode.Y=arcEndNodeY;
			
			// Add the other end:
			Path.AddPathNode(arcNode);
			Clip_=true;
		}
		
		/// <summary>Gets the signed angle from one vector, the first, to another.</summary>
		public float Angle(float x0,float y0,float x1,float y1){
			float dot=Vector2.Dot(new Vector2(x0,y0),new Vector2(x1,y1));
			return (float)Math.Atan2(Vector3.Cross(new Vector3(x0,y0,0f),new Vector3(x1,y1,0f)).magnitude,dot);
		}
		
		/// <summary>Draws the outline of path you created, and doesn't reset the path, using the stroke style.</summary>
		public void stroke(){
			
			if(Clip_){
				// Clip the path first.
				Clip_=false;
				
				// Clip with a 50px safety zone on all sides for strokes.
				Path.Clip(-50f,-50f,ImageData.Width+50f,ImageData.Height+50f);
			}
			
			// For each line..
			
			DynamicTexture img=ImageData_;
			VectorPoint node=Path.FirstPathNode;
			
			// For each one..
			while(node!=null){
				
				// Render it as a line (if it has one; checks internally):
				if(node.HasLine){
					
					// Render the line from the next nodes point of view:
					node.RenderLine(this);
					
				}
				
				// Hop to the next one:
				node=node.Next;
				
			}
			
			// Request a paint:
			img.RequestPaint();
			
		}
		
		/// <summary>Fills the current path with a solid colour. The colour used originates from the fillStyle.</summary>
		public void fill(){
			
			if(Path.FirstPathNode==null){
				return;
			}
			
			if(Clip_){
				// Clip the path first.
				Clip_=false;
				
				// Clip with a 50px safety zone on all sides for strokes.
				Path.Clip(-50f,-50f,ImageData.Width+50f,ImageData.Height+50f);
			}
			
			if(Rasteriser==null){
				
				// Setup and start the rasteriser:
				Rasteriser=new Scanner();
				Rasteriser.SDFSize=0;
				Rasteriser.ScaleY=-1f;
				Rasteriser.Start();
				
			}
			
			// Figure out bounds:
			Path.RecalculateBounds();
			
			DynamicTexture img=ImageData_;
			
			int rWidth=img.Width;
			int rHeight=img.Height;
			
			Rasteriser.Rasterise(Path,img.Pixels,rWidth,0,rWidth,rHeight,0f,-rHeight,FillColour,false);
			
			img.RequestPaint();
			
		}
		
		/// <summary>Moves the current pen location to the given point. Used when drawing paths.</summary>
		public void moveTo(float x,float y){
			
			Path.MoveTo(x,y);
			
		}
		
		/// <summary>Applies the image data so it's ready for rendering.</summary>
		public void UpdateDimensions(LayoutBox box){
			
			int w=(int)box.InnerWidth;
			int h=(int)box.InnerHeight;
			
			DynamicTexture img=ImageData_;
			
			if(w==img.Width && h==img.Height){
				// No change. Stop there.
				return;
			}
			
			// Resize the texture (clearing it):
			img.Resize(w,h,true);
			
		}
		
		/// <summary>The canvas element that this is the context for.</summary>
		public HtmlElement canvas{
			get{
				return Canvas;
			}
		}
		
		/// <summary>Applies the current fill style.</summary>
		public string fillStyle{
			set{
				
				// Apply the colour:
				Css.Value v=Css.Value.Load(value);
				
				if(v!=null && v.IsColour){
					FillColour=v.GetColour(null,null);
				}else{
					FillColour=new Color(0f,0f,0f,0f);
				}
				
			}
			get{
				return "";
			}
		}
		
		/// <summary>Applies the current stroke style.</summary>
		public string strokeStyle{
			set{
				
				// Apply the colour:
				Css.Value v=Css.Value.Load(value);
				
				if(v!=null && v.IsColour){
					StrokeColour=v.GetColour(null,null);
				}else{
					StrokeColour=new Color(0f,0f,0f,0f);
				}
				
			}
			get{
				return "";
			}
		}
		
		/// <summary>The width of the canvas context.</summary>
		public int width{
			get{
				return ImageData_.Width;
			}
			set{
				ImageData_.ResizeX(value);
			}
		}
		
		/// <summary>The height of the canvas context.</summary>
		public int height{
			get{
				return ImageData_.Height;
			}
			set{
				ImageData_.ResizeY(value);
			}
		}
		
		/// <summary>Useful method to clear the whole context.</summary>
		public void clear(){
			if(ImageData_!=null){
				ImageData_.Clear();
			}
			
			Path.Clear();
		}
		
		/// <summary>Clears the specified region of the canvas.</summary>
		public void clearRect(int xStart,int yStart,int width,int height){
			fillRect(xStart,yStart,width,height,new Color32(0,0,0,0));
		}
		
		/// <summary>Fills the specified box region using the current fillStyle.</summary>
		public void fillRect(int xStart,int yStart,int width,int height){
			fillRect(xStart,yStart,width,height,FillColour);
		}
		
		/// <summary>Fills the specified box region using the given colour.</summary>
		public void fillRect(int xStart,int yStart,int rectWidth,int rectHeight,Color32 colour){
			
			// First invert y. This is because the canvas API is from the top left corner.
			DynamicTexture img=ImageData_;
			
			int yEnd=img.Height-yStart;
			
			int xEnd=xStart+rectWidth;
			yStart=yEnd-rectHeight;
			
			// Clip the region to the available space:
			if(xStart<0){
				xStart=0;
			}
			
			if(yStart<0){
				yStart=0;
			}
			
			if(xEnd>img.Width){
				xEnd=img.Width;
			}
			
			if(yEnd>img.Height){
				yEnd=img.Height;
			}
			
			// Fill each pixel:
			for(int y=yStart;y<yEnd;y++){
				// Get the index of this row of pixels.
				int index=(y*img.Width);
				
				for(int x=xStart;x<xEnd;x++){
					img.Pixels[x+index]=colour;
				}
			}
			
			img.RequestPaint();
			
		}
		
	}
	
}