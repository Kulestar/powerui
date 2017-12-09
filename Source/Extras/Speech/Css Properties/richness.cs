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
	/// Represents the richness: css property.
	/// </summary>
	
	public class Richness:CssProperty{
		
		public static Richness GlobalProperty;
		
		
		public Richness(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="50";
		}
		
		public override string[] GetProperties(){
			return new string[]{"richness"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



