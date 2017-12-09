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
using UnityEngine;
using Css;


namespace PowerUI{
	
	/// <summary>A delegate used when PowerUI generates a new camera. See UI.OnCreatedCamera to use this.</summary>
	/// <param name="camera">The newly created camera.</param>
	public delegate void CameraCreated(Camera camera);
	
	/// <summary>
	/// When cameras are placed on the main UI, the UI must be broken up and rendered by multiple cameras.
	/// This class represents one of those cameras in a stack.
	/// </summary>
	
	public class UICamera{
		
		/// <summary>The renderer that will render this camera.</summary>
		public Renderman Renderer;
		/// <summary>The camera itself.</summary>
		public Camera SourceCamera;
		/// <summary>Cameras are stored as a linked list. This is the camera after this one.</summary>
		public UICamera CameraAfter;
		/// <summary>How far the camera is. Essentially defines the amount of z-index space.</summary>
		public float CameraDistance;
		/// <summary>Cameras are stored as a linked list. This is the camera before this one.</summary>
		public UICamera CameraBefore;
		/// <summary>The parent gameobject.</summary>
		public GameObject Gameobject;
		/// <summary>The cameras gameobject.</summary>
		public GameObject CameraObject;
		
		
		/// <summary>Creates a new UI Camera which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer that will render this camera.</param>
		public UICamera(Renderman renderer){
			Renderer=renderer;
			
			// Create the root gameobject:
			Gameobject=new GameObject();
			
			// Create camera gameobject:
			CameraObject=new GameObject();
			
			// Parent the camera to the root:
			CameraObject.transform.parent=Gameobject.transform;
			
			// Add a camera:
			SourceCamera=CameraObject.AddComponent<Camera>();
			
			// Set the clear flags:
			SourceCamera.clearFlags=CameraClearFlags.Depth;
			
			// Set the culling mask:
			SourceCamera.cullingMask=(1<<UI.Layer);
			
			// Make it forward rendered:
			SourceCamera.renderingPath=RenderingPath.Forward;
			
			// Setup the cameras distance:
			SetCameraDistance(UI.GetCameraDistance());
			
			// Setup the field of view:
			SetFieldOfView(UI.GetFieldOfView());
			
			// Parent it to the root:
			Gameobject.transform.parent=UI.GUINode.transform;
			
			// Call the camera creation method:
			UI.CameraGotCreated(SourceCamera);
			
		}
		
		/// <summary>Sets the depth of the camera. 
		/// This affects its height and also where it renders on the screen (before/ after others).</summary>
		public void SetDepth(int depth){
			// Apply the depth:
			SourceCamera.depth=depth;
			
			// Figure out the vertical height - that's where to place this camera so it doesn't overlap any others:
			float verticalHeight=(depth-UI.CameraDepth)*(ScreenInfo.WorldSize.y*2f);
			
			// Apply the location:
			Gameobject.transform.localPosition=new Vector3(0f,verticalHeight,0f);
		}
		
		/// <summary>Sets the distance of the camera, essentially defining the amount of z-index available.</summary>
		/// <param name="distance">The distance of the camera from the origin along the z axis in world units.</param>
		public void SetCameraDistance(float distance){
			CameraDistance=distance;
			SourceCamera.farClipPlane=distance*2f;
			CameraObject.transform.localPosition=new Vector3(0f,0f,-distance);
		}
		
		/// <summary>Sets the field of view of the UI camera.</summary>
		/// <param name="fov">The field of view, in degrees.</param>
		public void SetFieldOfView(float fov){
			SourceCamera.fieldOfView=fov;
		}
		
		/// <summary>Permanently destroys this UI camera.</summary>
		public void Destroy(){
			if(Gameobject!=null){
				GameObject.Destroy(Gameobject);
				Gameobject=null;
			}
		}
		
	}
	
}