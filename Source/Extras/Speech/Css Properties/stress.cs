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
	/// Represents the stress: css property.
	/// </summary>
	
	public class Stress:CssProperty{
		
		public static Stress GlobalProperty;
		
		
		public Stress(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="50";
		}
		
		public override string[] GetProperties(){
			return new string[]{"stress"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



