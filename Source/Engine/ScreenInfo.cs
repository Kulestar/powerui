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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dom;


namespace PowerUI{

	/// <summary>
	/// Provides general information about the screen such as where the corners are in world units.
	/// Note: The world origin is in the middle of the screen.
	/// The provided world screen origin is the top left corner.
	/// </summary>

	public static class ScreenInfo{
		
		/// <summary>The DPI of a CSS pixel.</summary>
		public const int CssPixelDpi=96;
		/// <summary>The screen width in pixels.</summary> 
		public static int ScreenX;
		/// <summary>The screen height in pixels.</summary>
		public static int ScreenY;
		/// <summary>The width of the screen in pixels as a float.</summary>
		public static float ScreenXFloat;
		/// <summary>The height of the screen in pixels as a float.</summary>
		public static float ScreenYFloat;
		/// <summary>How many pixels are gained/lost per unit depth on screen size. Varies with field of view.</summary>
		public static float DepthDepreciation;
		/// <summary>The height/width of the screen in world units at zero depth.</summary>
		public static Vector2 WorldSize=Vector2.zero;
		/// <summary>The amount of world units per screen pixel at zero depth.</summary>
		public static Vector2 WorldPerPixel=Vector2.zero;
		/// <summary>The location of the screen origin (top left corner) in world units at zero depth.</summary>
		public static Vector3 WorldScreenOrigin=Vector3.zero;
		/// <summary>The cached previous orientation. Either Unknown, LandscapeLeft or Portrait.</summary>
		private static DeviceOrientation PreviousOrientation=DeviceOrientation.Unknown;
		
		
		/// <summary>A standard test to see if the device is oriented landscape.</summary>
		public static bool IsLandscape(){
			
			// Get orientation:
			UnityEngine.DeviceOrientation orient=UnityEngine.Input.deviceOrientation;
			
			if(orient==UnityEngine.DeviceOrientation.Unknown){
				
				// Unknown - check for landscape based on width being bigger:
				return UnityEngine.Screen.width>UnityEngine.Screen.height;
				
			}
			
			return (orient==UnityEngine.DeviceOrientation.LandscapeLeft || orient==UnityEngine.DeviceOrientation.LandscapeRight);
			
		}
		
		/// <summary>The device's DPI / ScreenInfo.CssPixelDPI.</summary>
		public static float DevicePixelRatio{
			get{
				return (float)ScreenInfo.Dpi / (float)ScreenInfo.CssPixelDpi;
			}
		}
		
		/// <summary>Causes all settings here to be refreshed on the next update.</summary>
		public static void Clear(){
			ScreenX=ScreenY=0;
		}
		
		/// <summary> Checks if the screen size changed and repaints if it did.
		/// Called by <see cref="UI.Update"/>.</summary>
		public static void Update(){
		
			if(UI.GUICamera==null){
				// PowerUI is offline.
				return;
			}
			
			bool changedX=false;
			bool changedY=false;
			
			if(UnityEngine.Screen.width!=ScreenX){
				ScreenX=UnityEngine.Screen.width;
				ScreenXFloat=(float)ScreenX;
				changedX=true;
			}
			
			if(UnityEngine.Screen.height!=ScreenY){
				ScreenY=UnityEngine.Screen.height;
				ScreenYFloat=(float)ScreenY;
				changedY=true;
			}
			
			HtmlDocument document=UI.document;
			
			// Device orientation changed?
			bool landscape=IsLandscape();
			Css.MediaType media;
			
			if(PreviousOrientation==DeviceOrientation.Unknown){
				
				// First time. Straight set it:
				PreviousOrientation=landscape?DeviceOrientation.LandscapeLeft : DeviceOrientation.Portrait;
				
			}else{
				
				// Changed?
				if(landscape && PreviousOrientation!=DeviceOrientation.LandscapeLeft){
					
					// Orientation changed! Update previous:
					PreviousOrientation=landscape?DeviceOrientation.LandscapeLeft : DeviceOrientation.Portrait;
					
					// Inform main UI media rules:
					media=document.MediaIfExists;
					
					if(media!=null){
						// Nudge it!
						media.Landscape=landscape;
					}
					
					// Fire the rotation event now (on the window):
					// We're using absolute here because 'deviceorientation' is the actual angle of the device.
					DeviceOrientationEvent e=new DeviceOrientationEvent("deviceorientationabsolute");
					e.absolute=true;
					e.SetTrusted();
					document.window.dispatchEvent(e);
					
				}
				
			}
			
			if(!changedX && !changedY){
				return;
			}
			
			// Nudge the matrices:
			UI.GUICamera.ResetWorldToCameraMatrix();
			UI.GUICamera.ResetProjectionMatrix();
			UI.GUICamera.ResetAspect();
			
			// Firstly, find the bottom left and top right corners at UI.CameraDistance z units away (zero z-index):
			Vector3 bottomLeft=UI.GUICamera.ScreenToWorldPoint(new Vector3(0f,0f,UI.CameraDistance));
			Vector3 topRight=UI.GUICamera.ScreenToWorldPoint(new Vector3(ScreenX,ScreenY,UI.CameraDistance));
			
			
			// With those, we can now find the size of the screen in world units:
			WorldSize.x=topRight.x-bottomLeft.x;
			WorldSize.y=topRight.y-bottomLeft.y;
			
			// Finally, calculate WorldPerPixel at zero depth:
			
			// Mapping PX to world units:
			WorldPerPixel.x=WorldSize.x/(float)ScreenX;
			WorldPerPixel.y=WorldSize.y/(float)ScreenY;
			
			// Set where the origin is. All rendering occurs relative to this point.
			// It's offset by 0.2 pixels to target a little closer to the middle of each pixel. This helps Pixel filtering look nice and clear.
			WorldScreenOrigin.y=bottomLeft.y + (0.4f * WorldPerPixel.y);
			WorldScreenOrigin.x=bottomLeft.x - (0.4f * WorldPerPixel.x);
			
			// Update main document's viewport:
			document.Viewport.Update(ScreenXFloat,ScreenYFloat);
			
			// Fire the resize event (doesn't bubble):
			Dom.Event resize=new Dom.Event("resize");
			resize.SetTrusted(false);
			document.window.dispatchEvent(resize);
			
			// Inform main UI media rules:
			media=document.MediaIfExists;
			
			// Resize:
			if(changedX){
				
				int w=(int)ScreenXFloat;
				
				document.RequestLayout();
				
				if(media!=null){
					// Nudge it!
					media.Width=w;
				}
				
			}
			
			if(changedY){
				
				int h=(int)ScreenYFloat;
				
				document.RequestLayout();
				
				if(media!=null){
					// Nudge it!
					media.Height=h;
				}
				
			}
			
		}
		
		/// <summary>The screen DPI.</summary>
		public static int Dpi{
			get{
				int dpi=(int)UnityEngine.Screen.dpi;
				
				if(dpi==0){
					// Assume CSS DPI:
					return ScreenInfo.CssPixelDpi;
				}
				
				return dpi;
				
			}
		}
		
	}
	
}