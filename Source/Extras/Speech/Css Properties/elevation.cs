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
	/// Represents the elevation: css property.
	/// </summary>
	
	public class Elevation:CssProperty{
		
		public static Elevation GlobalProperty;
		
		
		public Elevation(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="level";
		}
		
		public override string[] GetProperties(){
			return new string[]{"elevation"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



