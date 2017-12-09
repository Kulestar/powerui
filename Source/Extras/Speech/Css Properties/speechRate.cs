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
	/// Represents the speech-rate: css property.
	/// </summary>
	
	public class SpeechRate:CssProperty{
		
		public static SpeechRate GlobalProperty;
		
		
		public SpeechRate(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="medium";
		}
		
		public override string[] GetProperties(){
			return new string[]{"speech-rate"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



