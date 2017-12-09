// MIT license (Free to do whatever you want with)

using System;
using PowerUI;
using PowerSlide;


namespace Widgets{
	
	/// <summary>
	/// Displays dialogue in the middle at the bottom.
	/// </summary>
	
	[Dom.TagName("subtitles")]
	public class SubtitlesWidget : DialogueWidget{
		
		/// <summary>This one can only display one slide at a time. This is the slide itself.
		/// Note that you should use allActive instead of this.</summary>
		private DialogueSlide Current_;
		
		/// <summary>Called when the given slide requested to display.
		/// Note that multiple slides may request to be visible at the same time.</summary>
		protected override void Show(DialogueSlide dialogue){
			
			Current_=dialogue;
			
			// Get the html:
			string html;
			
			if(dialogue.isOptions){
				
				// Showing an options menu.
				html="<div class='subtitle-header'>"+dialogue.markup+"</div>";
				
				// Add each option:
				DialogueSlide[] options=dialogue.options;
				
				for(int i=0;i<options.Length;i++){
					
					// Get the option:
					DialogueSlide option=options[i];
					
					// Write to the HTML (padded inline-block):
					html+="<div class='subtitle-option' "+OptionMouseDown(option)+">"+option.markup+"</div>";
					
				}
			
			}else if(dialogue.speakerCount>0){
				
				// Show the speaker names at the top instead:
				html="<div class='subtitle-header'>";
				
				// For each speaker (there can be more than one):
				for(int i=0;i<dialogue.speakers.Length;i++){
					
					if(i!=0){
						// Multiple speakers - just add a comma between them:
						html+=", ";
					}
					
					html+=dialogue.speakers[i].FullName;
					
				}
				
				html+="</div><br>"+dialogue.markup;
				
				// We'll only display the first speakers chat head, if it has one:
				string headUrl=dialogue.speakers[0].ChatHeadUrl;
				
				if(!string.IsNullOrEmpty(headUrl)){
					
					// Great - apply mood:
					headUrl=dialogue.applyMood(headUrl);
					
					// Display it:
					html+="<div class='subtitle-chathead'><img src='"+headUrl+"'></div>";
					
				}
				
			}else{
				
				// Just the markup:
				html=dialogue.markup;
				
			}
			
			// Write to subtitle-text (a child of 'element'):
			element.getElementById("subtitle-text").innerHTML=html;
			
			// Click to continue - only show if it's cued (either because it's an options menu, or it waits):
			// Display the "click to continue" option, using getById to cast to HtmlElement:
			element.getById("click-to-continue").style.display=dialogue.cued ? "block" : "none";
			
		}
		
		/// <summary>Called when the given slide requested to hide.
		/// Note that there may be multiple visible slides.</summary>
		protected override void Hide(DialogueSlide dialogue){
			
			// Just empty the dialogue from the UI (this assumes there's only one).
			// If you want to be able to display multiple entries, you could use:
			// List<DialogueSlide> all=allActive;
			
			if(Current_!=dialogue){
				return;
			}
			
			element.getElementById("subtitle-text").innerHTML="";
			
		}
		
		/// <summary>Called when the dialogue is now waiting for a cue event.</summary>
		// protected override void WaitForCue(SlideEvent e){}
		
		/// <summary>Called when the dialogue got cued.</summary>
		// protected override void Cued(SlideEvent e){}
		
	}
	
}