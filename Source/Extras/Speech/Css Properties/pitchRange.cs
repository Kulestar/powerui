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
	/// Represents the pitch-range: css property.
	/// </summary>
	
	public class PitchRange:CssProperty{
		
		public static PitchRange GlobalProperty;
		
		
		public PitchRange(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="50";
		}
		
		public override string[] GetProperties(){
			return new string[]{"pitch-range"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



