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

namespace PowerTools{

	/// <summary>
	/// The PowerTools About editor window.
	/// </summary>

	public class About : PowerToolsWindow{
		
		[MenuItem("Window/PowerTools/About")]
		public static void Open(){
			
			OpenWindow(typeof(About),"About PowerTools",ScreenPath("About"));
			
		}
		
	}
	
}