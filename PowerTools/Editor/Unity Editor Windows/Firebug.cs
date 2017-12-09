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
	/// The PowerTools Firebug editor window.
	/// </summary>

	public class Firebug : PowerToolsWindow{
		
		[MenuItem("Window/PowerTools/Firebug")]
		public static void Open(){
			
			OpenWindow(typeof(Firebug),"Firebug",ScreenPath("Firebug"));
			
		}
		
	}
	
}