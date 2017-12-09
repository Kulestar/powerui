// ---------------------------
//   Standard Module System
//      MIT Licensed
// (Extend and use as you wish)
// ---------------------------

using System;
using System.Collections;
using System.Collections.Generic;


namespace Modular{

	/// <summary>
	/// Metadata about a starter method.
	/// Typically used to define the order in which they load if there's multiple modules around.
	/// </summary>
	[Values.Preserve]
	public class Meta : Attribute{
		
		public int Priority;
		
		
		public Meta(){}
		
		public Meta(int priority){
			Priority=priority;
		}
		
	}
	
}