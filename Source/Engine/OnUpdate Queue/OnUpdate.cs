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

namespace PowerUI{

	/// <summary>A delegate used whith OnUpdate.</summary>
	public delegate void UpdateMethod();

	/// <summary>
	/// A generic way for modules to hook into OnUpdate.
	/// </summary>
	
	public static class OnUpdate{
		
		public static OnUpdateCallback FirstElement;
		public static OnUpdateCallback LastElement;
		
		
		public static void Update(){
			
			OnUpdateCallback current=FirstElement;
			
			while(current!=null){
				current.RunMethod();
				current=current.Next;
			}
			
		}
		
		public static OnUpdateCallback Add(UpdateMethod callback){
			return Add(callback,0f);
		}
		
		public static OnUpdateCallback Add(UpdateMethod callback,float fps){
			
			if(callback==null){
				return null;
			}
			
			OnUpdateCallback newElement=new OnUpdateCallback(callback,fps);
			
			if(FirstElement==null){
				FirstElement=LastElement=newElement;
			}else{
				newElement.Previous=LastElement;
				LastElement=LastElement.Next=newElement;
			}
			
			return newElement;
		}
		
		public static void Reset(){
			FirstElement=LastElement=null;
		}
		
	}

}