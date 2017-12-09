using Dom;
using Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// A fixed camera input. It stays in a particular spot on the screen and only "relocates" when a camera is transformed.
	/// Think VR input setups where the mouse is right in the middle of the screen and it moves as the camera does.
	/// </summary>
	public class CameraPointer : InputPointer{
		
		/// <summary>The camera doing the pointing.
		private Camera Camera_;
		/// <summary>The transform it's connected to.
		private Transform Transform_;
		
		/// <summary>The camera doing the pointing.</summary>
		public Camera Camera{
			get{
				return Camera_;
			}
			set{
				Camera_=value;
				
				if(value==null){
					Transform_=null;
				}else{
					Transform_=Camera_.transform;
				}
				
			}
		}
		
		/// <param name="cam">The camera that is doing the pointing. This is actually optional, but it's a very good idea
		/// to use it. When given, it'll invalidate the pointer whenever that camera is transformed in some way.
		/// I.e. the camera rotates and the pointer updates what is under it.</param>
		/// <param name="position">The screen-space coordinates, in pixels, of where the pointer will be.
		/// The top left corner is 0,0.</param>
		public CameraPointer(Camera cam,Vector2 position){
			Camera=cam;
			Position=position;
		}
		
		/// <param name="cam">The camera that is doing the pointing. This is actually optional, but it's a very good idea
		/// to use it. When given, it'll invalidate the pointer whenever that camera is transformed in some way.
		/// I.e. the camera rotates and the pointer updates what is under it.</param>
		/// <param name="relativeX">The screen-space X coordinate, in 0-1 (0.5 is 50%), of where the pointer will be.
		/// The top left corner is 0,0.</param>
		/// <param name="relativeY">The screen-space Y coordinate, in 0-1 (0.5 is 50%), of where the pointer will be.
		/// The top left corner is 0,0.</param>
		public CameraPointer(Camera cam,float relativeX,float relativeY){
			Camera=cam;
			Position=new Vector2(relativeX * UnityEngine.Screen.width, relativeY * UnityEngine.Screen.height);
		}
		
		public override bool Relocate(out Vector2 delta){
			
			// Camera moved?
			if(Transform_!=null && Transform_.hasChanged){
				// Reset:
				Transform_.hasChanged=false;
				base.Relocate(out delta);
				return true;
			}
			
			// Default otherwise:
			return base.Relocate(out delta);
		}
		
	}
	
}