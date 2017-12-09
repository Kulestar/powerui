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
	/// Represents the speak-punctuation: css property.
	/// </summary>
	
	public class SpeakPunctuation:CssProperty{
		
		public static SpeakPunctuation GlobalProperty;
		
		
		public SpeakPunctuation(){
			GlobalProperty=this;
			Inherits=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"speak-punctuation"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



