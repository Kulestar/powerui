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


using UnityEngine;
using System.Collections;
using PowerUI;

/// <summary>
/// This script simply shows a message above an object in the gameworld.
/// The message is full HTML, so it can be anything at all.
/// </summary>


public class AboveHeadTextExample : MonoBehaviour {
	
	/// <summary>The white cubes "message gameobject" in the gameworld.</summary>
	public GameObject WhiteCubesMessageNode;
	/// <summary>The black cubes "message gameobject" in the gameworld.</summary>
	public GameObject BlackCubesMessageNode;
	
	
	public void Start(){
		
		// White cube says hello!
		// The white cube is also clickable if you tick the Window > PowerUI > Handle 3D Input box.
		ShowMessage("Hello <b>people</b>!",WhiteCubesMessageNode);
		
		// Black cube says hello!
		ShowMessage("Hi <b>everybody</b>!",BlackCubesMessageNode);
		
	}
	
	/// <summary>Shows the given message above the given object.</summary>
	public void ShowMessage(string message,GameObject aboveObject){
		
		// We'll use WorldUI's for this - no need to mess around with updating etc.
		// As a worldUI is like a small screen, it needs some pixel space - that's how much space the message HTML has (100px x 100px).
		WorldUI messageUI=new WorldUI(110,100);
		
		// Put it in pixel perfect mode - this is what makes it "stick" to the camera:
		messageUI.PixelPerfect=true;
		
		// Write the message to it:
		// Important note! If the message originates from players, don't forget that they could potentially write HTML (scripts especially).
		// textContent is supported too (e.g. messageUI.document.body.textContent) which will write the message "as is".
		
		// We're also going to give it a bit of extra style, e.g. a faded white background:
		messageUI.document.innerHTML="<div style='padding:5px;background:#ffffffaa;text-align:center;'>"+message+"</div>";
		
		// Parent it to and go to the origin of the gameobject:
		messageUI.ParentToOrigin(aboveObject);
		
		// Make the message destroy itself after 5 seconds:
		messageUI.SetExpiry(5f);
		
	}
	
}
