using UnityEngine;
using System.Collections;
using PowerUI;
using Dom;


public class FocusGraphExamples : MonoBehaviour {
	
	void Start(){
		
		// Hook up the keydown event for the main UI:
		UI.document.onkeydown+=delegate(KeyboardEvent e){
			
			// Note that if nothing is focused, the keyboard event
			// will go to the Input.Unhandled EventTarget
			// (unlike on the web, where it gets sent to <body>).
			
			// Grab the keycode (as a Unity keycode so it's easier to work with):
			KeyCode key=e.unityKeyCode;
			
			// Was it an arrow key?
			// If so, we'll move the focus in that direction.
			
			// Note: e.document is the document that the event came from.
			// e.htmlDocument is it cast to a HtmlDocument for convenience.
			
			switch(key){
				
				case KeyCode.RightArrow:
					// Right arrow key.
					e.htmlDocument.MoveFocusRight();
				break;
				
				case KeyCode.LeftArrow:
					// Left arrow key.
					e.htmlDocument.MoveFocusLeft();
				break;
				
				case KeyCode.UpArrow:
					// Up arrow key.
					e.htmlDocument.MoveFocusUp();
				break;
				
				case KeyCode.DownArrow:
					// Down arrow key.
					e.htmlDocument.MoveFocusDown();
				break;
				
			}
			
		};
		
	}
	
}
