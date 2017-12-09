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
using Css;
using PowerUI;


namespace Blaze{
	
	/// <summary>
	/// A node which immediately follows an arc line.
	/// This handles the rendering of the arc itself.
	/// </summary>
	
	public partial class ArcLinePoint{
		
		
		/// <summary>Renders an arc from the previous point to this one.</summary>
		public override void RenderLine(CanvasContext context){
			// Grab the raw drawing data:
			DynamicTexture data=context.ImageData;
			
			// Time to go polar!
			// We're going to rotate around the pole drawing one pixel at a time.
			// For the best accuracy, we first need to find out how much to rotate through per pixel.
			
			if(Length==0f){
				// Nothing to draw anyway.
				return;
			}
			
			// How much must we rotate through overall?
			float angleToRotateThrough=EndAngle-StartAngle;
			
			// So arc length is how many pixels long the arc is.
			// Thus to step that many times, our delta angle is..
			float deltaAngle=angleToRotateThrough/Length;
			
			// The current angle:
			float currentAngle=StartAngle;
			
			// The number of pixels:
			int pixelCount=(int)Mathf.Ceil(Length);
			
			if(pixelCount<0){
				// Going anti-clockwise. Invert deltaAngle and the pixel count:
				deltaAngle=-deltaAngle;
				pixelCount=-pixelCount;
			}
			
			// Step pixel count times:
			for(int i=0;i<pixelCount;i++){
				// Map from polar angle to coords:
				float x=Radius * (float) Math.Cos(currentAngle);
				float y=Radius * (float) Math.Sin(currentAngle);
				
				// Draw the pixel:
				data.DrawPixel((int)(CircleCenterX+x),data.Height-(int)(CircleCenterY+y),context.StrokeColour);
				
				// Rotate the angle:
				currentAngle+=deltaAngle;
			}
			
		}
		
	}
	
}