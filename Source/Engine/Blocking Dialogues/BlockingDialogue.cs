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


namespace PowerUI{

	/// <summary>
	/// A dialogue that blocks the thread when it's opened.
	/// Alert and confirm use these.
	/// </summary>

	public class BlockingDialogue{
		
		/// <summary>The window that opened it.</summary>
		public Window Window;
		/// <summary>The dialogue type. alert/confirm.</summary>
		public string Type;
		/// <summary>The response from the user.</summary>
		public object Response;
		#if !NETFX_CORE
		/// <summary>The blocked JS thread.</summary>
		internal System.Threading.Thread Thread;
		#endif
		/// <summary>The UI element of the background. Contains the whole prompt.</summary>
		public HtmlElement Element;
		
		
		public BlockingDialogue(string type,Window window){
			
			Type=type;
			Window=window;
			
		}
		
		/// <summary>The response as a boolean.</summary>
		public bool OkResponse{
			get{
				
				if(Response==null || !(Response is bool)){
					return false;
				}
				
				return (bool)Response;
				
			}
		}
		
		/// <summary>Resumes the thread.</summary>
		public void Resume(object response){
			
			if(Element!=null){
				// Remove the UI:
				Element.parentNode.removeChild(Element);
				Element=null;
			}
			
			// Update the response:
			Response=response;
			
			#if !NETFX_CORE
			// Give it a kick!
			Thread.Interrupt();
			#endif
			
		}
		
	}
	
}