using UnityEngine;
using System.Collections;
using PowerUI;

/// <summary>
/// An example directly building a Flat World UI.
/// Note that you can also use a normal WorldUI and just tick the 'Make It Flat' box.
/// This example is more targeted at those of you who want a texture.
/// </summary>
public class FlatInWorldUI : PowerUI.Manager{
	
	/// <summary>Amount of pixel space the UI has.</summary>
	public int Width=600;
	/// <summary>Amount of pixel space the UI has.</summary>
	public int Height=400;
	/// <summary>The FlatWorldUI itself.</summary>
	internal FlatWorldUI FlatUI;
	/// <summary>Is input enabled for this FlatWorldUI?</summary>
	public bool InputEnabled=true;
	
	
	public override void OnEnable () {
		
		// Generate a new UI using the given virtual screen dimensions
		// (the name is optional but helps debug the document in Window > PowerUI > Dom Inspector):
		FlatUI=new FlatWorldUI(gameObject.name,Width,Height);
		
		// Use PowerUI.Manager's Navigate function (which reads either Url or HtmlFile depending on which you set):
		Navigate(FlatUI.document);
		
		// Apply to the material on this gameObjects mesh renderer:
		// (Note: You can also just grab FlatUI.Texture - this is just a convenience function)
		FlatUI.ApplyTo(gameObject);
		
		if(InputEnabled){
			// Accept input from the collider on the gameObject:
			FlatUI.AcceptInputFrom(gameObject);
		}
	}
	
}
