using System;
using UnityEngine;


namespace JavascriptExample {
	
	/// <summary>
	/// This class is used in the JavascriptExample.
	/// It shows that Javascript can call C# functions directly and work with your games objects.
	/// Note that you can block calls like this with security domains.
	/// </summary>

	public static class JsExampleClass{
		
		public static void Hello(){
			
			Debug.Log("Hello from C#!");
			
		}
		
	}

}