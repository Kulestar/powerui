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
using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{

	/// <summary>
	/// A dialogue that blocks the thread when it's opened.
	/// Alert and confirm use these.
	/// </summary>

	public static class BlockingDialogues{
		
		/// <summary>The prompt templates.</summary>
		public static Dictionary<string,string> Templates;
		
		/// <summary>Sets up the blocking dialogues. Must occur ahead of time otherwise Resources.Load would fail on us.</summary>
		public static void Setup(){
			
			Templates=new Dictionary<string,string>();
			Templates["alert"]=GetTemplate("alert");
			Templates["prompt"]=GetTemplate("prompt");
			Templates["confirm"]=GetTemplate("confirm");
			
		}
		
		/// <summary>Gets this dialogues template.</summary>
		public static string GetTemplate(string type){
			
			TextAsset ta=Resources.Load("Standard Blocking Dialogues/"+type) as TextAsset;
			
			if(ta==null){
				return null;
			}
			
			return ta.text;
			
		}
		
		/// <summary>Called when 'ok' is pressed in a blocking dialogue.</summary>
		public static void Ok(UIEvent e){
			
			// Get the window:
			Window window=e.htmlDocument.window;
			
			// Get the dialogue:
			BlockingDialogue bd=window.BlockingDialogue;
			window.BlockingDialogue=null;
			
			if(bd==null){
				return;
			}
			
			// Restore it with a true or content from 'input zone':
			HtmlElement inputZone=null;
			
			if(bd.Type=="prompt"){
				inputZone=window.document.getElementById("spark-prompt-input-zone") as HtmlElement ;
			}
			
			if(inputZone!=null){
				bd.Resume(inputZone.value);
			}else{
				bd.Resume(true);
			}
			
		}
		
		/// <summary>Called when 'cancel' is pressed in a blocking dialogue.</summary>
		public static void Cancel(UIEvent e){
			
			// Get the window:
			Window window=e.htmlDocument.window;
			
			// Get the dialogue:
			BlockingDialogue bd=window.BlockingDialogue;
			window.BlockingDialogue=null;
			
			if(bd==null){
				return;
			}
			
			// Restore it with a false:
			bd.Resume(false);
			
		}
		
		/// <summary>Opens a blocking dialogue in the given window.</summary>
		public static BlockingDialogue Open(string type,Window window,object message){
			
			#if NETFX_CORE
			Dom.Log.Add("Can't block prompt on .NET Core.");
			return null;
			#else
			
			// Get the thread:
			System.Threading.Thread thread=System.Threading.Thread.CurrentThread;
			
			// Safety check: We can only block the 'blockable' thread.
			// This prevents it from blocking the Unity main thread.
			if(thread!=window.BlockableThread){
				UnityEngine.Debug.LogWarning("Failed to open a '"+type+"' dialogue because it was on a thread other than the window's main Javascript thread (and we don't want to block it).");
				return null;
			}
			
			if(window.BlockingDialogue!=null){
				// Hmm, one is already open - we shouldn't open another.
				UnityEngine.Debug.LogWarning("Unexpected '"+type+"' dialogue request. One is already open.");
				return null;
			}
			
			// Create the dialogue:
			BlockingDialogue bd=new BlockingDialogue(type,window);
			
			// Pop now:
			string text;
			Templates.TryGetValue(bd.Type,out text);
			
			if(string.IsNullOrEmpty(text)){
				// UI unavailable.
				return null;
			}
			
			if(window.document.html==null){
				Dom.Log.Add("Can't prompt without a html node.");
				return null;
			}
			
			// Append it:
			HtmlElement background=window.document.createElement("div") as HtmlElement ;
			background.id="spark-prompt-background";
			background.innerHTML="<div id='spark-prompt'>"+text+"</div>";
			window.document.html.appendChild(background);
			
			// Get the message zone:
			HtmlElement content=background.getElementById("spark-prompt-content-zone") as HtmlElement ;
			
			if(content!=null && message!=null){
				
				// Write the message:
				content.innerHTML=message.ToString();
				
			}
			
			// Add the element:
			bd.Element=background;
			
			// Add to the window:
			window.BlockingDialogue=bd;
			
			// Cache the thread:
			bd.Thread=thread;
			
			try{
				
				// Going down! Sleep now:
				System.Threading.Thread.Sleep(Timeout.Infinite);
				
			}catch(System.Threading.ThreadInterruptedException){
				// Back up again!
			}
			
			return bd;
			#endif
			
		}
		
	}
	
}	