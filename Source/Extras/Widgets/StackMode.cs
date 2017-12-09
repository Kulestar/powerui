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


namespace Widgets{
	
	/// <summary>
	/// How a widget deals with other widgets of the same type.
	/// E.g. if you have an inventory and a stats widget that occupy the same space, one probably closes the other when opened.
	/// </summary>
	public enum StackMode{
		
		/// <summary>Directly stacks the widget over the other. This is typical for 'floating' widgets.</summary>
		Over,
		/// <summary>The newer widget hides the older one. When the newer one is closed, the older one appears again.</summary>
		Hide,
		/// <summary>The newer widget closes the older one.</summary>
		Close,
		/// <summary>The newer widget contents are loaded into the existing one, "hijacking" the frame.</summary>
		Hijack
		
	}
	
}