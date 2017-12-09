using UnityEngine;
using System.Collections;
using PowerUI;


public class SimpleHealthPoints : MonoBehaviour {
	
	// My current points:
	public int Points;
	// My current health:
	public float Health;
	// The green bar:
	private HtmlElement GreenBar;
	
	// Update is called once per frame
	void Update () {
		
		// Point counter.
		// We're going to be extra generous and give a point every frame!
		AddPoints(1);
		
		
		// Basic health bar.
		// Update current health:
		Health+=20f * Time.deltaTime;
		
		// Make sure it doesn't go higher than 100%:
		if(Health>100f){
			Health=100f;
		}
		
		// Get the green bit of the bar:
		if(GreenBar==null){
			// (Cast to a HtmlElement, because we want to access the style property):
			GreenBar=(UI.document.getElementById("basic-bar-internal") as HtmlElement);
		}
		
		// Set the health (as the width of the green part of the bar):
		GreenBar.style.width=Health+"%";
		
	}
	
	/// <summary>Adds the given points to Points, then tells the UI it changed.</summary>
	public void AddPoints(int pointsToAdd){
	
		// Bump it up by the points to add:
		Points+=pointsToAdd;
		
		// Write it out to the UI (as a variable - you could also just use innerHTML if you wanted):
		UI.Variables["Points"]=Points.ToString();
		
	}
	
}
