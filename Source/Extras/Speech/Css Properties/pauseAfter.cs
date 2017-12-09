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
	/// Represents the pause-after: css property.
	/// </summary>
	
	public class PauseAfter:CssProperty{
		
		public static PauseAfter GlobalProperty;
		
		
		public PauseAfter(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"pause-after"};
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



