// ---------------------------
//   Standard Module System
//      MIT Licensed
// (Extend and use as you wish)
// ---------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace Modular{

	/// <summary>
	/// A group of modules with the same starter priority.
	/// </summary>
	[Values.Preserve]
	public class StarterGroup{
		
		/// <summary>The methods in this group.</summary>
		public List<MethodInfo> Methods=new List<MethodInfo>();
		
	}

}