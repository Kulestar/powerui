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

using System;
using PowerUI;
using UnityEngine;

/// <summary>
/// Shows how to call C# directly from your html.
/// This receives an onsubmit call when a form is submitted through onsubmit="FormExampleHandler.OnSubmit".
/// That maps to this class and its OnSubmit method.
/// </summary>

public static class FormExampleHandler{
	
	/// <summary>Called when the form is submitted through onsubmit="FormExampleHandler.OnSubmit".</summary>
	/// <param name="form">A simple object holding the values of all your form inputs by name.</param>
	public static void OnSubmit(FormEvent form){
		Debug.Log("Form submitted!");
		
		// Usage is simply form["fieldName"], or form.Checked("fieldName") for easily checking if a checkbox is ticked.
		// You can alternatively use e.g. UI.document.getElementById("aFieldsID").value and manage radio inputs manually.
		
		Debug.Log("handling a form with C#.");
		
		// Give a feedback message to show something's happened:
		UI.document.getElementById("csMessage").innerHTML="Please check the console!";
		
		// And simply log all the fields of the form:
		Debug.Log("Your name: "+form["yourName"]);
		Debug.Log("Awesome? "+form.Checked("awesome"));
		Debug.Log("Epic? "+form.Checked("epic"));
		Debug.Log("Pretty? "+form.Checked("purdy"));
		Debug.Log("Unique? "+form.Checked("unique"));
		Debug.Log("Wonderful? "+form.Checked("wonderful"));
		Debug.Log("Your dropdown selection: "+form["favourite"]);
		Debug.Log("Your Bio: "+form["myBio"]);
		
		// Block the default (so it doesn't actually submit it)
		form.preventDefault();
		
	}
	
}