using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerUI;

/// <summary>
/// This example shows an alternative way of accepting 
/// mousedowns other than with Nitro by using C# delegates and anonymous delegates.
/// </summary>

public class CSharpExample : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
		// Important note: This script is *after* the manager on the PowerUI-Example object.
		// That'll make sure this Start method runs after the UI is ready to go.
		
		// Get a reference to the main UI document so everything else looks wonderfully familiar:
		var document=UI.document;
		
		// Let's hook up our mouse methods to an element with an ID of 'illBeClickable':
		var myElement=document.getElementById("illBeClickable");
		
		// It could be an SVG element, a MathML element etc.
		// Virtually all of the time, it'll be a HtmlElement.
		
		if(myElement==null){
			// Usually you won't need to do this, but this is just incase!
			Debug.Log("Whoops - It looks like the clickable element got removed!");
		}else{
			// Great, let's setup the events:
			myElement.onmousedown = OnElementMouseDown;
		}
		
		// In many cases there is a group of elements that, when clicked, all do similar things.
		// If this applies to you too, then you can grab all 
		// elements that have a certain tag (such as all 'div' elements)
		// See the OnHeaderMouseDown function for then distinguishing one from another.
		// Also note that querySelector etc is supported.
		// That'll let you write CSS selectors to grab DOM elements that way.
		
		// Or all elements with a certain attribute (class of "button"):
		
		// First, all h4 that are kids of body:
		var allHeaders=document.body.getElementsByTagName("h4");
		
		foreach(var element in allHeaders){
			element.onmousedown = OnHeaderMouseDown;
		}
		
		// Second, all elements that are kids of body and have a class of "button":
		var allButtons=document.body.getElementsByClassName("button");
		
		foreach(var element in allButtons){
			
			// This also shows how to create an "anonymous delegate". 
			// That's one not declared as a seperate function.
			// These are more useful if you have a significant amount of callbacks:
			element.onmousedown = delegate(MouseEvent mouseEvent) {
				
				// mouseEvent.target is the element that actually got clicked
				// Note that it could be e.g an SVG element. So, htmlTarget
				// is it cast as a HtmlElement.
				mouseEvent.htmlTarget.innerHTML="You clicked this!";
				
			};
			
		}
		
	}
	
	/// <summary>Direct is called directly with onmousedown="CSharpExample.Direct".
	/// It has a Preserve attribute which for IL2CPP. As far as it can tell, this function isn't used
	/// so it would strip it out otherwise.
	/// It doesn't matter which Preserve attribute you use - 
	/// either UnityEngine.Scripting.Preserve, Values.Preserve or your own.
	/// </summary>
	[Values.Preserve] // <-- see above!
	public static void Direct(MouseEvent mouseEvent){
		
		// mouseEvent.target is the element that actually got clicked.
		// Note that it could be, e.g. an SVG element. So, htmlTarget
		// is it as a HtmlElement.
		
		mouseEvent.htmlTarget.style.border="2px solid #0000ff";
		
	}
	
	/// <summary>Called when any span gets clicked.
	/// This is linked up in Start as a delegate.</summary>
	private void OnElementMouseDown(MouseEvent mouseEvent){
		
		// mouseEvent.target is the element that actually got clicked.
		// Note that it could be e.g an SVG element. So, htmlTarget
		// is it cast as a HtmlElement.
		mouseEvent.htmlTarget.innerHTML="You clicked it!";
		
	}
	
	/// <summary>Called when any h1 gets clicked.
	/// This is linked up in Start as a delegate.</summary>
	private void OnHeaderMouseDown(MouseEvent mouseEvent){
		
		// mouseEvent.target is the element that actually got clicked.
		// Note that it could be e.g an SVG element. So, htmlTarget
		// is it cast as a HtmlElement.
		
		var target=mouseEvent.htmlTarget;
		
		target.style.color="#ff00ff";
		
		// Sometimes the element itself must store something unique.
		// For that, we reccommend using attributes
		// (<h1 thisIsAn="attribute"... note that they are always lowercase from C#!):
		
		// Grab its category=".." value:
		Debug.Log("Its 'category' attribute is "+target.getAttribute("category"));
		
	}
	
}
