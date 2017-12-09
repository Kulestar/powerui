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

using UnityEngine;
using System.Collections;


namespace PowerUI{
	
	/// <summary>
	/// This class helps resize images.
	/// </summary>
	
	public static class ImageResizer{
		
		/// <summary>Resize the given image with the given scale ratio.</summary>
		public static Texture2D Resize(Texture2D original,float ratio){
			
			// Get the original size:
			int maxY=original.height;
			int maxX=original.width;
			
			// What's the target size?
			int height=(int)(maxY * ratio);
			int width=(int)(maxX * ratio);
			
			if(height==maxY && width==maxX){
				
				// Unchanged:
				return original;
				
			}
			
			Color32[] texColors = original.GetPixels32();
			Color32[] newColors = new Color32[width * height];
			
			float ratioX = 1f / ((float)width / (float)(maxX-1));
			float ratioY = 1f / ((float)height / (float)(maxY-1));
			
			for (int y = 0; y < height; y++) {
				
				int yFloor = (int)Mathf.Floor (y * ratioY);
				int y1 = yFloor * maxX;
				int y2 = (yFloor+1) * maxX;
				int yw = y * width;
		 
				for (int x = 0; x < width; x++){
					int xFloor = (int)Mathf.Floor (x * ratioX);
					float xLerp = x * ratioX-xFloor;
					newColors[yw + x] = ColorLerpUnclamped (ColorLerpUnclamped (texColors[y1 + xFloor], texColors[y1 + xFloor+1], xLerp),
															ColorLerpUnclamped (texColors[y2 + xFloor], texColors[y2 + xFloor+1], xLerp),
															y*ratioY-yFloor);
				}
			}
			
			original.Resize(width,height,TextureFormat.ARGB32,false);
			
			original.SetPixels32(newColors);
			
			// Flush the texture:
			original.Apply();
			
			// Return it:
			return original;
			
		}
		
		/// <summary>Lerps from one colour to another.</summary>
		private static Color32 ColorLerpUnclamped(Color32 c1,Color32 c2,float value){
			return new Color32 ((byte)(c1.r + (c2.r - c1.r)*value), 
								(byte)(c1.g + (c2.g - c1.g)*value), 
								(byte)(c1.b + (c2.b - c1.b)*value), 
								(byte)(c1.a + (c2.a - c1.a)*value)
								);
		}
		
	}

}