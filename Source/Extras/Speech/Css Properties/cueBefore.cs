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
	/// Represents the cue-before: css property.
	/// </summary>
	
	public class CueBefore:CssProperty{
		
		public static CueBefore GlobalProperty;
		
		
		public CueBefore(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"cue-before"};
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



