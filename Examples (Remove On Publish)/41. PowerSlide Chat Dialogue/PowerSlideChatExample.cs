using UnityEngine;
using System.Collections;
using PowerUI;
using PowerSlide; // <-- Don't forget me!


public class PowerSlideChatExample : MonoBehaviour {

	void Start(){
		
		// Get a reference to the main UI document so everything else looks wonderfully familiar:
		var document=UI.document;
		
		// Using a username variable:
		UI.Variables["Username"]="LuckyWin44";
		
		// Add a handler which will provide information about the speakers:
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
			// 41.PowerSlide Chat Dialogue/Resources/Dialogue/myDialogue.json
			// as well as the "subtitles" template, which is in:
			// 41.PowerSlide Chat Dialogue/Subtitles Template
			
			// Note: Use {language} to localise the path. E.g. "myDialogue_{language}" => "Dialogue/myDialogue_en.json"
			
			// playDialogue is slightly different from startDialogue:
			// -> The promise returned by playDialogue runs when the dialogue has *finished*
			// -> The promise returned by startDialogue runs when the dialogue has loaded and *started*
			
			document.playDialogue("myDialogue","subtitles").then(delegate(object o){
				
				// You could also listen out for the slidesend event 
				// on the document if you prefer.
				Debug.Log("Dialogue finished!");
				
			});
			
		};
		
	}
	
}