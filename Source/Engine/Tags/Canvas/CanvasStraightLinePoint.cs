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
	
	public partial class StraightLinePoint{
		
		/// <summary>Renders a straight line from the previous point to this one.</summary>
		public override void RenderLine(CanvasContext context){
			// Grab the raw drawing data:
			DynamicTexture data=context.ImageData;
			
			// Invert y:
			int endY=data.Height-(int)Y;
			int startY=data.Height-(int)Previous.Y;
			
			// Grab X:
			int endX=(int)X;
			int startX=(int)Previous.X;
			
			data.DrawLine(startX,startY,endX,endY,context.StrokeColour);
		}
		
	}
	
}