using UnityEngine;
using System.Collections;

public class Text3DCameraController : MonoBehaviour {
	
	float Direction=-10f;
	
	void Update () {
		
		transform.Rotate(0f,Direction*Time.deltaTime,0f);
		
		float angle=transform.rotation.eulerAngles.y;
		
		if(angle<0f){
			angle+=360f;
		}
		
		if(Direction<0f){
			
			if(angle<290f && angle>180f){
				// Reverse!
				Direction=-Direction;
			}
			
		}else if(angle>70f && angle<180f){
			
			// Reverse!
			Direction=-Direction;
			
		}
		
	}
	
}
