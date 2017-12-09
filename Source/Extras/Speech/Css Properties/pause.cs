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
using System.Collections;
using System.Collections.Generic;
using Css.AtRules;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the pause: composite property.
	/// </summary>
	
	public class Pause:CssCompositeProperty{
		
		public Pause(){
			// none
		}
		
		public override string[] GetProperties(){
			return new string[]{"pause"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[pause-before || pause-after] | inherit
			*/
			
			return new Spec.OneOf(
				new Spec.AnyOf(
					
					new Spec.Property(this,"pause-before"),
					
					new Spec.Property(this,"pause-after")
					
				),
				new Spec.Literal("inherit")
			);
			
		}
		
	}
	
}



