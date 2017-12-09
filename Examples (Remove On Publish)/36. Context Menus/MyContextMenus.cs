using UnityEngine;
using System.Collections;
using PowerUI;
using ContextMenus; // <-- Don't forget me!


public class MyContextMenus : MonoBehaviour {

	void Start(){
		
		// Get a reference to the main UI document so everything else looks wonderfully familiar:
		var document=UI.document;
		
		// -1- Setup how we want to request the menu to show up (right click here)
		
		// Setup right click:
		document.addEventListener("click",delegate(MouseEvent e){
			
			// W3C button #2 is right click (or use e.isRightMouse if you prefer):
			if(e.button==2){
				
				// Create the menu (use a custom class if you want):
				var list=new OptionList();
				
				// Display now using whichever input pointer clicked/ tapped.
				// (Pass true to just instantly run the first option - that's great for *left* clicks):
				list.display(e.trigger,false);
				
			}
			
		});
		
		// -2- Setup how we want our elements to respond to the contextmenu event.
		
		// We'll just do it for this one element:
		var myElement=document.getElementById("my2DContext");
		
		myElement.oncontextmenu=delegate(ContextEvent ce){
			
			// If you'd like to use some other widget template, you'd do that here:
			// (This way, your 3D context menus can look totally different, for example).
			// ce.template="menulist";
			
			// Add the options (supports HTML, localisation and custom Option classes):
			ce.add("About",delegate(Option sender){
				
				Debug.Log("Made with love in the UK.");
				
			});
			
			var option=ce.add("Say hello..",delegate(Option sender){
				
				Debug.Log("Please contact me whenever you want - I'd love to hear from you!");
				
			});
			
			// The add methods return the Option object it created.
			// You can add to them too to make a submenu:
			option.add("Email",delegate(Option sender){
				
				Debug.Log("powerui@kulestar.com");
				
			});
			
			option.add("Skype",delegate(Option sender){
				
				Debug.Log("KulestarUK");
				
			});
			
		};
		
	}
	
}
