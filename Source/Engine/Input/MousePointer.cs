using Dom;
using Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A mouse as an input. You can use multiple mice if needed - just add more to InputPointer.AllRaw.
	/// (Generally though if there's more than one then they're touch pointers).
	/// </summary>
	public class MousePointer : InputPointer{
		
		public MousePointer(){
			
		}
		
		/// <summary>The type of input pointer.</summary>
		public override string pointerType{
			get{
				return "mouse";
			}
		}
		
		public override bool HandleEvent(UnityEngine.Event current){
			
			// Consider scroll too:
			if(current.type==EventType.ScrollWheel){
				
				// Trigger the scrollwheel event:
				PowerUI.Input.OnScrollWheel(current.delta);
				return true;
				
			}
			
			return base.HandleEvent(current);
		}
		
		public override bool Relocate(out Vector2 delta){
			
			// Get the current mouse position:
			Vector2 position=UnityEngine.Input.mousePosition;
			
			// Change the position:
			return TryChangePosition(position,true,out delta);
		}
		
	}
	
}