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
using PowerUI;


namespace PowerUI{
	
	/// <summary>
	/// PowerUI creates one of these automatically if it's needed.
	/// It causes the Update routine to occur.
	/// </summary>
	
	public class StandardUpdater:MonoBehaviour{
		
		public void Update(){
			UI.InternalUpdate();
		}
		
		#if NoPowerUIInput
		#else
		public void OnGUI(){
			
			Event current=Event.current;
			EventType type=current.type;
			
			if(type==EventType.Repaint || type==EventType.Layout){
				return;
			}
			
			if(type==EventType.KeyUp){
				
				// Key up:
				PowerUI.Input.OnKeyPress(false,current.character,(int)current.keyCode);
				
			}else if(type==EventType.KeyDown){
				
				// Key down:
				PowerUI.Input.OnKeyPress(true,current.character,(int)current.keyCode);
				
			}else if(Input.SystemMouse!=null){
				
				// Handle it:
				Input.SystemMouse.HandleEvent(current);
				
			}
			
		}
		#endif
		
		public void OnDisable(){
			// Called when a scene changes.
			UI.Destroy();
		}
		
		public void OnApplicationQuit(){
			
			// Run OnBeforeUnload, if an event is still attached:
			if(UI.document!=null){
				
				// Run onbeforeunload (always trusted):
				UI.document.window.dispatchEvent(new BeforeUnloadEvent());
				
			}
			
			// Make sure all timers are halted:
			UITimer.OnUnload(null);
			
		}
		
	}
	
}