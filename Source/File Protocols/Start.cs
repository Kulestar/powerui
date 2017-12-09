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
using PowerUI;


namespace Modular{
	
	public static partial class Main{
		
		/// <summary>
		/// Sets up the Dom module.
		/// </summary>
		public static void Start_Protocols(StartInfo info){
			
			// Find file protocols:
			info.Scanner.FindAllSubTypes(typeof(FileProtocol),delegate(Type type){
				
				// Add it:
				FileProtocols.Add(type);
				
			});
			
			// (That's so it'll "scan" any added assemblies for custom protocols).
			
		}
		
	}

}
