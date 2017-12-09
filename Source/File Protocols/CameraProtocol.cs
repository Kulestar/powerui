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


#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY5
#endif

using Css;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// This camera:// protocol enables a link to point to a camera. The path is the same as the path in the hierarchy.
	/// E.g. src="camera://Main Camera" (or use background-image; anything that accepts an image URI)
	/// will display the main camera's output as a transformable image.
	/// </summary>
	
	public class CameraProtocol:FileProtocol{
		
		public override string[] GetNames(){
			return new string[]{"camera"};
		}
		
		public override void OnGetGraphic(ImagePackage package){
			
			// Apply as camera format:
			string path=package.location.Path;
			CameraFormat cmf=package.Contents as CameraFormat;
			
			if(cmf==null){
				
				cmf=new CameraFormat();
				package.Contents=cmf;
				
			}
			
			// Update path:
			cmf.SetPath(path);
			
			// Ready:
			package.Done();
			
			
		}
		
	}
	
}