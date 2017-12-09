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


namespace PowerUI{
	
	/// <summary>
	/// An event used when a WorldUI expires. The best way to add extra data to this event is to inherit from WorldUI and add to that.
	/// Returning false prevents the default destroy action (allowing you to e.g. destroy after an animated fade out).
	/// </summary>
	
	public delegate bool WorldUIExpiryEvent(WorldUI ui);
	
}