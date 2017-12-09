using UnityEngine;
using System.Collections;
using PowerUI;


public class Text3DExample : MonoBehaviour {
	
	/// <summary>Messages to cycle through.</summary>
	public string[] Messages=new string[]{"TRUTH","JUSTICE","LIBERTY"};
	/// <summary>The current index in the messages.</summary>
	private int MessageIndex=0;
	
	
	void Start(){
		
		// Note that component is placed below the WorldUI Helper component.
		// That'll make this start method run when the WorldUI is ready to go.
		
		// So, you'll first want the document - that's this:
		var document = GetComponent<PowerUI.Manager>().Document;
		
		// (If you want the WorldUI instance, then use document.worldUI)
		// var w=document.worldUI;
		
		// Get the #message element:
		var msg=document.getElementById("message");
		
		// Hook up the onanimationcycle event:
		// (@keyframes is doing all the timing for us):
		msg.addEventListener("animationiteration",delegate(PowerUI.AnimationEvent e){
			
			// Write out the next message:
			msg.innerHTML=Messages[MessageIndex++];
			
			// Cycle index:
			if(MessageIndex >= Messages.Length){
				MessageIndex=0;
			}
			
		});
		
	}
	
}
