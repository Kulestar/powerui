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
using Dom;


namespace Speech{
	
	/// <summary>
	/// An audio synthesis engine. Invoked when speech markup is being synthesized.
	/// </summary>
	
	public class SynthEngine{
		
		/// <summary>The meta types that your engine will handle. E.g. "text/sml".</summary>
		public virtual string[] GetTypes(){
			return null;
		}
		
	}
	
}