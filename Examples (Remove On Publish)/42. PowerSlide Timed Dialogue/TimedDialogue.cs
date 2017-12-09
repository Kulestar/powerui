using UnityEngine;
using System.Collections;
using PowerUI;
using PowerSlide; // <-- Don't forget me!


public class TimedDialogue : MonoBehaviour {

	void Start(){
		
		// Get a reference to the main UI document so everything else looks wonderfully familiar:
		var document=UI.document;
		
		// Using a username variable:
		UI.Variables["Username"]="LuckyWin44";
		
		// Add a handler which will provide information about the speakers (This is exactly the same as the chat dialogue example):
		Speaker.OnGetInfo=delegate(DialogueSlide slide,SpeakerType type,string reference){
			
			// Just a basic reference check here. You would use speaker ID's that make this easy.
			// (E.g. your NPC objects could have a speaker object, 
			// and you'd use whatever mapping you already use to obtain NPC information).
			
			// Create a speaker object (or pull it from your mapping; note that this could also be a child class)
			Speaker s=new Speaker();
			
			// Optionally use type (so you can e.g. have multiple players talking to each other).
			if(type==SpeakerType.Player){
				
				// You can use HTML names (such as a variable here):
				s.FullName="&Username;";
				s.ChatHeadUrl="Chatheads/Player_{mood}.png";
				
			}else if(reference=="joey"){
				
				s.FullName="Joey";
				s.ChatHeadUrl="Chatheads/Joey_{mood}.png";
				
			}
			
			return s;
		};
		
		// Add a mousedown event which will start off the dialogue for us:
		document.getElementById("starter").onmousedown=delegate(MouseEvent e){
			
			// Start it now! This is referring to:
			// 42.PowerSlide Timed Dialogue/Resources/Dialogue/myTimedDialogue.json
			// as well as the "subtitles" template, which is in:
			// 41.PowerSlide Chat Dialogue/Subtitles Template
			
			// Note: Use {language} to localise the path. E.g. "myTimedDialogue_{language}" => "Dialogue/myTimedDialogue_en.json"
			document.startDialogue("myTimedDialogue_{language}","subtitles");
			
			// you can use startDialogue or playDialogue and use the promise they return (Like example #41)
			
		};
		
	}
	
}