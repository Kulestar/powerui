using UnityEngine;
using System.Collections;
using PowerUI;

public class AdvancedHealth : MonoBehaviour {
	
	/// <summary>An instance of the power bar - a dynamic graphic.</summary>
	public PowerBar PowerBarGraphic;
	/// <summary>An instance of the health bar - a dynamic graphic.</summary>
	public HealthBar HealthBarGraphic;
	
	
	// Use this for initialization
	void Start () {
		
		/* Future update: This will instead be bars of any shape using SVG and stroke */
		
		// Create our dynamic textures:
		PowerBarGraphic=new PowerBar();
		HealthBarGraphic=new HealthBar();
		
		// Get a reference to the document:
		var document=UI.document;
		
		// Get the elements (using the shortcut for HtmlElement):
		var powerBar=document.getById("powerbar");
		var healthBar=document.getById("healthbar");
		
		// Apply the graphics to them:
		powerBar.SetImage(PowerBarGraphic);
		healthBar.SetImage(HealthBarGraphic);
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// Increase health/power by 0.2 a second:
		PowerBarGraphic.IncreasePower(0.2f*Time.deltaTime);
		
		HealthBarGraphic.IncreaseHealth(0.2f*Time.deltaTime);
		
	}
	
}