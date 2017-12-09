using Dom;
using Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A finger input. How rude.
	/// </summary>
	public class FingerPointer : TouchPointer{
		
		/// <summary>The type of input pointer.</summary>
		public override string pointerType{
			get{
				return "touch";
			}
		}
		
	}
	
}