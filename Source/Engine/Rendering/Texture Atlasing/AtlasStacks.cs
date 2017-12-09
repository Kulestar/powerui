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
using Blaze;


namespace PowerUI{

	/// <summary>
	/// All the major stacks used by PowerUI. Each one represents a stack of texture atlases which are shared across all UI's.
	/// </summary>
	
	public static class AtlasStacks{
		
		/// <summary>The spacing around letters on the text atlas.</summary>
		private static int RawTextSpacing=2;
		/// <summary>The spacing around images on the graphics atlas.</summary>
		private static int RawGraphicsSpacing=0;
		/// <summary>The max frequency of optimise calls, in seconds.</summary>
		private const int OptimiseFrequency=20;
		/// <summary>A counter which tracks optimise calls and throttles them to at most once every 30s.</summary>
		private static int OptimiseCount;
		/// <summary>The max frame count until an optimise happens.</summary>
		private static int OptimiseFrameCount=OptimiseFrequency*UI.DefaultRate;
		/// <summary>The max available atlas size. See MaxSize.</summary>
		private static int RawMaxSize=2048;
		/// <summary>The minimum starting atlas size.</summary>
		public static int InitialSize=1024;
		
		/// <summary>The atlas stack used for rendering SDF text. All fonts share the same atlases.</summary>
		public static AtlasStack Text;
		/// <summary>A stack of temporarily buffered atlases.</summary>
		// public static AtlasStack Cache;
		/// <summary>The stack used for rendering all graphics. Shared across all UI's.</summary>
		public static AtlasStack Graphics;
		
		
		/// <summary>The spacing around letters on the text atlas.</summary>
		public static int TextSpacing{
			get{
				if(Text==null){
					return RawTextSpacing;
				}
				return Text.Spacing;
			}
			set{
				if(Text==null){
					RawTextSpacing=value;
					return;
				}
				
				Text.Spacing=value;
			}
		}
		
		/// <summary>The spacing around images on the graphics atlas.</summary>
		public static int GraphicsSpacing{
			get{
				if(Graphics==null){
					return RawGraphicsSpacing;
				}
				return Graphics.Spacing;
			}
			set{
				if(Graphics==null){
					RawGraphicsSpacing=value;
					return;
				}
				
				Graphics.Spacing=value;
				
				if(value!=0){
					
					// Column mode works best when spacing is active:
					Graphics.Mode=AtlasingMode.Columns;
					
				}
				
			}
		}
		
		/// <summary>Called when the global UI rate changes.</summary>
		public static void SetRate(int rate){
			OptimiseFrameCount=OptimiseFrequency*rate;
		}
		
		/// <summary>Creates our stacks.</summary>
		public static void Start(){
			
			// What's the max available texture size? All screens allow textures that fill them, so we'll base this off the screen size.
			int size=UnityEngine.Screen.width;
			
			if(UnityEngine.Screen.height>size){
				size=UnityEngine.Screen.height;
			}
			
			// Round size up to the nearest multiple of 2.
			int power=1;
			while(power<size){
				power*=2;
			}
			
			RawMaxSize=power;
			
			if(SystemInfo.maxTextureSize>RawMaxSize){
				// This isn't always avaiable (or gets set to -1)
				RawMaxSize=SystemInfo.maxTextureSize;
			}
			
			if(InitialSize>RawMaxSize){
				InitialSize=RawMaxSize;
			}
			
			// Create our stacks:
			
			// Alpha8 not supported by ReadPixels - have to use CPU copy mode :(
			Text=new AtlasStack(TextureFormat.Alpha8,InitialSize);
			Blaze.TextureCameras.CPUCopyMode=true;
			Text.CPUAccess=true;
			
			Text.LockFilterMode(FilterMode.Bilinear);
			Text.Mode=AtlasingMode.Columns;
			Text.Spacing=RawTextSpacing;
			
			// Cache=new AtlasStack(InitialSize);
			Graphics=new AtlasStack(InitialSize);
			Graphics.Spacing=RawGraphicsSpacing;
			
			if(RawGraphicsSpacing!=0){
				
				// Column mode works best when spacing is active:
				Graphics.Mode=AtlasingMode.Columns;
				
			}
			
		}
		
		/// <summary>Called at the global rate to e.g. optimise the atlases or trigger threaded draws.</summary>
		public static void Update(){
			
			OptimiseCount++;
			
			if(OptimiseCount>=OptimiseFrameCount){
				
				// Clear:
				OptimiseCount=0;
				
				// Optimise now:
				Optimise();
				
			}
			
		}
		
		/// <summary>Attempts to optimise the major atlases. Only does anything if they need it.</summary>
		public static void Optimise(){
			
			// Optimise both:
			bool changed=Text.OptimiseIfNeeded();
			changed|=Graphics.OptimiseIfNeeded();
			
			// Did anything happen? If so, we need to redraw:
			if(changed){
				// Global redraw now:
				// Note! This call happens just before all the layouts do.
				// That means we will actually get the layouts happening on this same frame.
				UI.RequestFullLayout();
			}
			
		}
		
		/// <summary>Clears all stacks.</summary>
		public static void Clear(){
			Text.Clear();
			Graphics.Clear();
		}
		
		/// <summary>Flushes all atlases that require it.</summary>
		public static void Flush(){
			Text.Flush();
			Graphics.Flush();
		}
		
		/// <summary>The maximum possible size that atlases can be on this hardware.</summary>
		public static int MaxSize{
			get{
				return RawMaxSize;
			}
		}
		
		/// <summary>The size of atlases to use. Auto clipped by MaxSize.</summary>
		public static int AtlasSize{
			get{
				return InitialSize;
			}
			set{
				if(value>RawMaxSize){
					value=RawMaxSize;
				}
				
				InitialSize=value;
			}
		}
		
	}
	
}