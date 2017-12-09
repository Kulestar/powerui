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
	/// Represents the pause-before: css property.
	/// </summary>
	
	public class PauseBefore:CssProperty{
		
		public static PauseBefore GlobalProperty;
		
		
		public PauseBefore(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"pause-before"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			time | percentage | inherit
			*/
			
			return new Spec.OneOf(
				
				new Spec.ValueType(typeof(Css.Units.TimeUnit)),
				new Spec.ValueType(typeof(Css.Units.PercentUnit)),
				new Spec.Literal("inherit")
				
			);
			
		}
		
	}
	
}



