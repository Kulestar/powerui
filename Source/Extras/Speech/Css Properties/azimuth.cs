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
	/// Represents the azimuth: css property.
	/// </summary>
	
	public class Azimuth:CssProperty{
		
		public static Azimuth GlobalProperty;
		
		
		public Azimuth(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="center";
		}
		
		public override string[] GetProperties(){
			return new string[]{"azimuth"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



