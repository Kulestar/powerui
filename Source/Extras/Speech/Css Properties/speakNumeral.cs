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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the speak-numeral: css property.
	/// </summary>
	
	public class SpeakNumeral:CssProperty{
		
		public static SpeakNumeral GlobalProperty;
		
		
		public SpeakNumeral(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="continuous";
		}
		
		public override string[] GetProperties(){
			return new string[]{"speak-numeral"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



