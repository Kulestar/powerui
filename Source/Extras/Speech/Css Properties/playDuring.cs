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
	/// Represents the play-during: css property.
	/// </summary>
	
	public class PlayDuring:CssProperty{
		
		public static PlayDuring GlobalProperty;
		
		
		public PlayDuring(){
			GlobalProperty=this;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"play-during"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



