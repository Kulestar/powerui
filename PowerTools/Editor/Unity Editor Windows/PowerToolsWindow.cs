
#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY5
#endif

#if PRE_UNITY5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define PRE_UNITY5_3
#endif


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

using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;
using PowerUI;


namespace PowerTools{

	/// <summary>
	/// Opens up PowerTools in an editor window.
	/// </summary>

	public class PowerToolsWindow : EditorWindow{
		
		/// <summary>The webview.</summary>
		public object WebView;
		/// <summary>The window width.</summary>
		private float Width;
		/// <summary>The window height.</summary>
		private float Height;
		
		/*
		[MenuItem("Window/PowerTools/PowerSlide Editor")]
		public static void PowerSlideWindow(){
			OpenWindow("PowerTools - PowerSlide",ScreenPath("slide"));
		}
		*/
		
		/// <summary>Gets the path to PowerTools and appends #screen=givenValue to it.</summary>
		public static string ScreenPath(string screen){
			
			// Find PowerUI:
			string powerUIPath=PowerUIEditor.GetPowerUIPath()+"/PowerTools/Editor/";
			
			if(screen.Contains(".html")){
				// As-is:
				return powerUIPath+screen;
			}else{
				// As a hash:
				return powerUIPath+"index.html#editor=1&screen="+screen;
			}
			
		}
		
		/// <summary>Opens a window of the given type with the given title and URL.</summary>
		public static PowerToolsWindow OpenWindow(System.Type type,string title,string url){
			
			// Show existing window instance. If one doesn't exist, make one.
			PowerToolsWindow window=EditorWindow.GetWindow(type) as PowerToolsWindow;
			
			// Start editor svc:
			EditorWebAPI.Setup();
			
			// Give it a title:
			#if PRE_UNITY5_3
			window.title=title;
			#else
			GUIContent t=new GUIContent();
			t.text=title;
			window.titleContent=t;
			#endif
			
			// Open the PowerTools window:
			window.WebView = WebHelpers.Open(window,url);
			
			// Cache the size:
			window.Width = window.position.width;
			window.Height = window.position.height;
			
			return window;
		}
		
		public void OnDestroy(){
			Object.DestroyImmediate(WebView as UnityEngine.Object);
		}
		
		public void Update(){
			
			// Changed size?
			if(position.width!=Width || position.height!=Height){
				
				// Cache the size:
				Width=position.width;
				Height=position.height;
				
				// Resize it:
				WebHelpers.Resize(WebView,Width,Height);
				
			}
			
		}
		
	}

}