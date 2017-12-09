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
	/// Represents the speak: css property.
	/// </summary>
	
	public class Speak:CssProperty{
		
		public static Speak GlobalProperty;
		
		
		public Speak(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="normal";
		}
		
		public override string[] GetProperties(){
			return new string[]{"speak"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



