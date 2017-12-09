using UnityEngine;
using System.Collections;
using PowerUI;
using PowerSlide; // <-- Don't forget me!


public class MyPowerSlideAnimations : MonoBehaviour {

	void Start(){
		
		// Get a reference to the main UI document so everything else looks wonderfully familiar:
		var document=UI.document;
		
		// Get the tips element:
		var tips=document.getById("toptips");
		
		// -1- We'll just setup a mousedown event which will trigger the slides to play for us.
		document.getElementById("starter").onmousedown=delegate(MouseEvent e){
			
			// Trigger it! This is done with the slides CSS property, 
			// which is structured exactly the same as the animation CSS property.
			
			// (getById is e.getElementById cast to a HtmlElement so we can get style)
			
			// Runs Resources/powerSlideExample.json on the 'toptips' example, making the whole thing last for 3s.
			// The json file doesn't declare any durations so the 4s is split evenly amongst the slides.
			
			tips.slide("PowerSlideExample.json",3).then(delegate(object o){
				
				// Done! You could also listen for slidesend on tips.
				Dom.Log.Add("Slide complete!");
				
			});
			
		};
		
	}
	
}
