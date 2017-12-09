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
	/// Represents the cue-after: css property.
	/// </summary>
	
	public class CueAfter:CssProperty{
		
		public static CueAfter GlobalProperty;
		
		
		public CueAfter(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"cue-after"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			url | none | inherit
			*/
			
			return new Spec.OneOf(
			
				new Spec.FunctionCall("url"),
				new Spec.Literal("none"),
				new Spec.Literal("inherit")
				
			);
			
		}
		
	}
	
}



