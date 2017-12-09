//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright � 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the speak-header: css property.
	/// </summary>
	
	public class SpeakHeader:CssProperty{
		
		public static SpeakHeader GlobalProperty;
		
		
		public SpeakHeader(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="once";
		}
		
		public override string[] GetProperties(){
			return new string[]{"speak-header"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



