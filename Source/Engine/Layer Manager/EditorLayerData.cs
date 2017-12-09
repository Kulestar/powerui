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

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


public class EditorLayerData{
	
	public int LayerID;
	public string LayerName;
	public bool HideFromCameras;
	public SerializedObject Manager;
	public SerializedProperty Property;
	
	
	public EditorLayerData(SerializedObject manager,SerializedProperty property,string layerName,int id,bool hide){
		LayerID=id;
		Manager=manager;
		Property=property;
		LayerName=layerName;
		HideFromCameras=hide;
	}
	
}

#endif