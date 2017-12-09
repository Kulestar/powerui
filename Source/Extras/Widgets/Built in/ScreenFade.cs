// MIT license (Free to do whatever you want with)
// Originates from the PowerUI Wiki: http://powerui.kulestar.com/wiki/index.php?title=Screen_Fading_(Fade_to_black/_Whiteouts)
// It's formatted as a widget so you can stack other things on top of it.

using System;
using PowerUI;
using Widgets;
using System.Collections;
using System.Collections.Generic;
	
/// <summary>
/// Fades the screen to a specified colour in a specified amount of time.
/// </summary>

[Dom.TagName("screenfade")]
public class ScreenFade : Widgets.Widget{
	
	/// <summary>A helper function for instantly removing a screen fade.</summary>
	public static void Close(PowerUI.HtmlDocument doc){
		doc.widgets.close("screenfade",null);
	}
	
	/// <summary>A helper function for fading the screen in the given document.</summary>
	public static Promise Fade(PowerUI.HtmlDocument doc,UnityEngine.Color to,float timeInSeconds){
		
		// Open up the widget:
		return doc.widgets.load("screenfade",null,"to",to,"time",timeInSeconds);
		
	}
	
	public override StackMode StackMode{
		get{
			// Hijack an existing widget so we can fade from the 'current' colour onwards
			return StackMode.Hijack;
		}
	}
	
	/// <summary>The depth that this type of widget lives at.</summary>
	public override int Depth{
		get{
			// Very high (always right at the front)
			return 100000;
		}
	}
	
	/// <summary>Called when asked to fade.</summary>
	public override void Load(string url,Dictionary<string,object> globals){
		
		// Element is not null when we 'hijacked' an existing widget (and we're fading from its current color instead).
		if(element==null){
			
			// Write the HTML now:
			SetHtml("<div style='width:100%;height:100%;position:fixed;top:0px;left:0px;'></div>");
			
		}
		
		// Don't run the load event - we're delaying it:
		RunLoad=false;
		
		// Get the colour:
		UnityEngine.Color colour=GetColour("to",globals,UnityEngine.Color.black);
		
		// Get the time:
		float time=(float)GetDecimal("time",globals,0);
		
		// Run the animation:
		element.animate("background-color:"+colour.ToCss()+";",time).OnDone(delegate(UIAnimation animation){
			
			// If the opacity is 0, close the widget:
			if(colour.a<=0.001f){
				close();
			}
			
			// Run the load event now!
			LoadEvent(globals);
			
		});
		
	}
	
}

namespace PowerUI{
	
	public partial class HtmlDocument{
		
		/// <summary>Fades the screen to the given colour in the given amount of time.</summary>
		public Promise fade(UnityEngine.Color col,float time){
			return ScreenFade.Fade(this,col,time);
		}
		
	}
	
}