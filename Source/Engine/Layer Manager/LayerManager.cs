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
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{

	/// <summary>
	/// Adds layers from script from runtime or editor code. This is purely for convenience and has no overheads;
	/// PowerUI only adds it's default 'PowerUI' layer this way. 
	/// You can use this too with the LayerManager.Add method. Note that the layer will be stored permanently.
	/// </summary>

	public static class LayerManager{
		
		/// <summary>The default layer name.</summary>
		public const string DefaultLayerName="PowerUI";
		/// <summmary>A set of layers to add. Usually just the one.</summary>
		private static List<EditorLayerData> LayersToAdd;
		
		
		/// <summary>Adds the standard PowerUI layer, returning it's new ID.</summary>
		/// <param name="layerName">The new layer to add.</summary>
		/// <returns>The ID of the new layer.</returns>
		public static int Add(){
			return Add(DefaultLayerName,true);
		}
		
		/// <summary>Adds the layer with the given name, returning it's new ID.</summary>
		/// <param name="layerName">The new layer to add.</summary>
		/// <returns>The ID of the new layer.</returns>
		public static int Add(string layerName,bool hideFromCameras){
			
			// Pop open a serialized object:
			SerializedObject manager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			
			// Grab the first property:
			#if UNITY_5
			
			SerializedProperty property=manager.FindProperty("layers");
			
			for(int i=8;i<property.arraySize;i++){
				
				// Get the entry:
				SerializedProperty prop=property.GetArrayElementAtIndex(i);
				
				if(prop.stringValue==""){
					
					// Got an empty layer (ID i).
					
					if(EditorApplication.isPlaying){
						
						// Push to layer data:
						if(LayersToAdd==null){
							LayersToAdd=new List<EditorLayerData>();
						}
						
						// Push it in there:
						LayersToAdd.Add(new EditorLayerData(manager,prop,layerName,i,hideFromCameras));
						
						// Hook up an on state changed delegate now:
						EditorApplication.playmodeStateChanged+=PlayModeStateChanged;
						
						if(hideFromCameras){
							// Must also apply at runtime:
							HideFromAllCameras(i);
						}
						
					}else{
						// Great - run immediately!
						ApplyNewLayer(manager,prop,layerName,i,hideFromCameras);
					}
					
					return i;
					
				}
				
			}
			
			#else
			
			SerializedProperty property=manager.GetIterator();
			
			// What's the last assigned layer?
			while(property.Next(true)){
				
				// Grab the name:
				string name=property.name.ToLower();
				
				// Is it a layer?
				if(name.Contains("layer")){
					// Yep - is it's ID bigger than one we've already got to?
					
					// What's the layer ID?
					int layerID=LayerNumber(property.name);
					
					// Is it a builtin layer?
					if(layerID>=8){
						// Nope it's not - Is this layer actually empty?
						
						if(property.stringValue!=""){
							// Nope, it's taken already!
							continue;
						}
						
						// Yes it is - grab the ID:
						int newLayerID=layerID;
						
						if(EditorApplication.isPlaying){
							
							// Push to layer data:
							if(LayersToAdd==null){
								LayersToAdd=new List<EditorLayerData>();
							}
							
							// Push it in there:
							LayersToAdd.Add(new EditorLayerData(manager,property,layerName,newLayerID,hideFromCameras));
							
							// Hook up an on state changed delegate now:
							EditorApplication.playmodeStateChanged+=PlayModeStateChanged;
							
							if(hideFromCameras){
								// Must also apply at runtime:
								HideFromAllCameras(newLayerID);
							}
							
						}else{
							// Great - run immediately!
							ApplyNewLayer(manager,property,layerName,newLayerID,hideFromCameras);
						}
						
						return newLayerID;
					}
					
				}
				
			}
			
			#endif
			
			// No layers available!
			return 0;
		}
		
		/// <summary>Called when the editor playmode state changes.</summary>
		private static void PlayModeStateChanged(){
			
			if(EditorApplication.isPlaying || LayersToAdd==null){
				return;
			}
			
			// Grab the set:
			List<EditorLayerData> layers=LayersToAdd;
			
			// Clear it:
			LayersToAdd=null;
			
			// For each layer..
			foreach(EditorLayerData data in layers){
				// Apply it:
				ApplyNewLayer(data.Manager,data.Property,data.LayerName,data.LayerID,data.HideFromCameras);
			}
		}
		
		/// <summary>Applies and saves the layer property after playmode is done. Makes permanent changes.</summary>
		/// <param name="manager">The object manager which will perform the save.</param>
		/// <param name="property">The property to change.</param>
		/// <param name="newName">The name to permanently apply.</param>
		private static void ApplyNewLayer(SerializedObject manager,SerializedProperty property,string newName,int id,bool hide){
			
			// Write the name to it:
			property.stringValue=newName;
			
			Debug.Log("Automatically added layer '"+newName+"'. This is only done once.");
			
			// Save it:
			manager.ApplyModifiedProperties();
			
			if(hide){
				// Hide from cameras.
				HideFromAllCameras(id);
			}
			
		}
		
		/// <summary>Hides the numbered layers from all cameras in the scene. Note that this is an editor only method.</summary>
		public static void HideFromAllCameras(int layerID){
			
			// Flip it:
			layerID=~(1<<layerID);
			
			foreach(Camera camera in Camera.allCameras){
				if(camera==UI.GUICamera){
					continue;
				}
				
				// Remove from the culling mask:
				camera.cullingMask&=layerID;
			}
			
		}
		
		/// <summary>Gets the layer ID from the given name.</summary>
		private static int LayerNumber(string layer){
			
			// Get the characters:
			char[] pieces=layer.ToCharArray();
			
			// The layer number:
			string number="";
			
			// Get everything that is a number:
			for(int i=0;i<pieces.Length;i++){
				int character=(int)pieces[i];
				
				// Is it 0-9?
				if(character>=48 && character<=57){
					number+=pieces[i];
				}
				
			}
			
			if(number!=""){
				return int.Parse(number);
			}
			
			return 0;
		}
		
	}
	
}
#endif