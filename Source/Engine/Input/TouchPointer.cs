using Dom;
using Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// Covers both FingerPointer and StylusPointer.
	/// </summary>
	public class TouchPointer : InputPointer{
		
		public Vector2 LatestPosition;
		/// <summary>The latest pointer pressure. It's usually 1 for fingers.</summary>
		public float LatestPressure=1f;
		public float Radius;
		public float RadiusVariance;
		private string IDText;
		
		/// <summary>ID of this touch.</summary>
		public string identifier{
			get{
				
				if(IDText==null){
					IDText=ID.ToString();
				}
				
				return IDText;
			}
		}
		
		/// <summary>JS API.</summary>
		public float radiusX{
			get{
				return Radius;
			}
		}
		
		/// <summary>JS API.</summary>
		public float radiusY{
			get{
				return Radius;
			}
		}
		
		/// <summary>JS API. The current amount of force.</summary>
		public float force{
			get{
				return Pressure;
			}
		}
		
		/// <summary>RadiusX==RadiusY (Unity doesn't provide the info we need) so this is always 0.</summary>
		public float rotationAngle{
			get{
				return 0f;
			}
		}
		
		/// <summary>The width of the active pointer area in CSS pixels.</summary>
		public override double width{
			get{
				return Radius*2;
			}
		}
		
		/// <summary>The height of the active pointer area in CSS pixels.</summary>
		public override double height{
			get{
				return Radius*2;
			}
		}
		
		/// <summary>The twist/ rotationAngle.</summary>
		public override float twist{
			get{
				return rotationAngle;
			}
		}
		
		/// <summary>Updates the stylus info.</summary>
		public virtual void UpdateStylus(float angleX,float angleY){}
		
		public TouchPointer(){
			FireTouchEvents=true;
		}
		
		public override bool Relocate(out Vector2 delta){
			
			// Died?
			if(!StillAlive){
				Removed=true;
				
				// Mouse up etc:
				SetPressure(0f);
				delta=Vector2.zero;
				return true;
			}
			
			// Clear still alive:
			StillAlive=false;
			Vector2 position=LatestPosition;
			
			// Position's Y value is inverted, so flip it:
			// (Can we use rawPosition here? What is it?)
			position.y=ScreenInfo.ScreenY-1f-position.y;
			
			// Moved?
			if(position.x==ScreenX && position.y==ScreenY){
				
				// Nope!
				delta=Vector2.zero;
				return false;
				
			}
			
			// Delta:
			delta=new Vector2(
				position.x - ScreenX,
				position.y - ScreenY
			);
			
			// Update position:
			ScreenX=position.x;
			ScreenY=position.y;
			
			return true;
			
		}
		
	}
	
}