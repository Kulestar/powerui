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
	/// Represents the cue: composite property.
	/// </summary>
	
	public class Cue:CssCompositeProperty{
		
		public Cue(){
			// none
		}
		
		public override string[] GetProperties(){
			return new string[]{"cue"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[cue-before || cue-after] | inherit
			*/
			
			return new Spec.OneOf(
				new Spec.AnyOf(
					
					new Spec.Property(this,"cue-before"),
					
					new Spec.Property(this,"cue-after")
					
				),
				new Spec.Literal("inherit")
			);
			
		}
		
	}
	
}



