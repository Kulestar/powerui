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
	/// Represents the pitch: css property.
	/// </summary>
	
	public class Pitch:CssProperty{
		
		public static Pitch GlobalProperty;
		
		
		public Pitch(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="medium";
		}
		
		public override string[] GetProperties(){
			return new string[]{"pitch"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



