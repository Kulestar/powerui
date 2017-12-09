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

namespace PowerUI{
	
	/// <summary>
	/// Represents which direction a marquee scrolls in.
	/// Numbered such that certain values can be quickly found:
	/// less than 3 means it's vertical.
	/// Odd means it travels in the opposite direction to a normal scroll.
	/// </summary>
	
	public enum MarqueeDirection:int{
		Down=1,
		Up=2,
		Right=3,
		Left=4
	}
	
}