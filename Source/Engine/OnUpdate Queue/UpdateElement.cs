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

namespace PowerUI{
	
	/// <summary>
	/// A faster coroutine without needing a monobehaviour.
	/// This holds a callback which gets executed at a particular rate (or at the framerate if unspecified).
	/// </summary>
	
	public class OnUpdateCallback{
		
		private float Limit;
		private float Counter;
		private UpdateMethod Method;
		public OnUpdateCallback Next;
		public OnUpdateCallback Previous;
		
		/// <summary>Frame time between this being called.</summary>
		public float deltaTime{
			get{
				if(Limit==0f){
					return Time.unscaledDeltaTime;
				}
				
				return Limit;
			}
		}
		
		public OnUpdateCallback(UpdateMethod method,float fps){
			Method=method;
			SetRate(fps);
		}
		
		public OnUpdateCallback(UpdateMethod method){
			Method=method;
		}
		
		public void SetRate(float rate){ // Rate is in calls per second. 0 is once/frame.
			
			if(rate==0f){
				Limit=0f;
			}else{
				Limit=1f/rate;
			}
			
		}
		
		public void Stop(){
			Remove();
		}
		
		public void Remove(){
			
			if(Next==null){
				OnUpdate.LastElement=Previous;
			}else{
				Next.Previous=Previous;
			}
			
			if(Previous==null){
				OnUpdate.FirstElement=Next;
			}else{
				Previous.Next=Next;
			}
			
		}
		
		public void RunMethod(){
			
			// Rate check.
			if(Limit!=0f){
				Counter+=Time.unscaledDeltaTime;
				
				if(Counter<Limit){
					return;
				}else{
					Counter-=Limit;
				}
				
			}
			
			// Run it now:
			Method();
			
		}
		
	}

}