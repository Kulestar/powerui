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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	#define PRE_UNITY4
#endif

using System;
using Css;


/// <summary>
/// Globally pools batches to prevent destroying and recreating meshes on heavily active UI's.
/// </summary>

namespace PowerUI{

	public static class UIBatchPool{
		
		/// <summary>Batches are pooled to prevent destroying and recreating meshes. This is the first in the pool linked list.</summary>
		public static UIBatch First;
		
		
		/// <summary>Is the pool empty?</summary>
		public static bool Empty{
			get{
				return (First==null);
			}
		}
		
		/// <summary>Clears the pool.</summary>
		public static void Clear(){
			
			UIBatch current=First;
			First=null;
			
			while(current!=null){
				current.Destroy();
				current=current.BatchAfter;
			}
			
		}
		
		/// <summary>Adds the given chain of batches to the pool.</summary>
		public static void AddAll(UIBatch first,UIBatch last){
			
			if(first==null){
				return;
			}
			
			last.BatchAfter=First;
			First=first;
			
		}
		
		/// <summary>Hides all the pooled batches.</summary>
		public static void HideAll(){
			
			UIBatch current=First;
			
			while(current!=null){
				UnityEngine.GameObject obj=current.Mesh.OutputGameObject;
				
				if(obj!=null){
					// Hide it:
					#if PRE_UNITY4
					obj.active=false;
					#else
					obj.SetActive(false);
					#endif
				}
				
				current=current.BatchAfter;
			}
			
		}
		
		/// <summary>Adds the given batch to the pool.</summary>
		public static void Add(UIBatch batch){
			
			batch.BatchAfter=First;
			First=batch;
			
			// Hide it:
			#if PRE_UNITY4
			batch.Mesh.OutputGameObject.active=false;
			#else
			batch.Mesh.OutputGameObject.SetActive(false);
			#endif
			
		}
		
		/// <summary>Gets a batch from the pool. Null if the pool is empty.</summary>
		public static UIBatch Get(Renderman renderer){
			if(First==null){
				return null;
			}
			
			UIBatch result=First;
			First=result.BatchAfter;
			result.BatchAfter=null;
			result.Setup=false;
			
			// Get the GO:
			UnityEngine.GameObject gameobject=result.Mesh.OutputGameObject;
			
			if(gameobject==null){
				// This occurs when a WorldUI gets destroyed but put its batches in the pool.
				// We've already removed the first so just go recursive - chances are the next one will be ok.
				return Get(renderer);
			}
			
			// Show it:
			#if PRE_UNITY4
			gameobject.active=true;
			#else
			gameobject.SetActive(true);
			#endif
			
			if(result.Renderer!=renderer){
				result.ChangeRenderer(renderer);
			}
			
			return result;
		}
		
		
	}
	
}