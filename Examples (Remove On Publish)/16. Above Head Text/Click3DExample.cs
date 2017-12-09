using UnityEngine;
using System.Collections;
using PowerUI;


public class Click3DExample : MonoBehaviour {
	
	// For this to work, you must tick the 'Handle 3D Events' box in Window > PowerUI > Input Settings.
	
	// You can catch any event which passes through the UI by defining a method which:
	// - Starts with 'On' followed by your event name (OnMouseDown, OnWheel etc).
	// - Has exactly 1 parameter which accepts the correct event type.
	// See also the event listing or the Mozilla developer network (MDN) to obtain the right type:
	// https://powerui.kulestar.com/events/
	
	// So, the W3C click event is called 'click' and it accepts a PowerUI.MouseEvent event
	// which means it'll look like this:
	public void OnClick(MouseEvent e){
		
		UnityEngine.Debug.Log("Clicked a 3D object!");
		
	}
	
}
