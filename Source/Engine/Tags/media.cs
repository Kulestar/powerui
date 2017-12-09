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
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// The HTMLMediaElement API. Shared by audio and video.
	/// </summary>
	
	public class HtmlMediaElement:HtmlElement{
		
		/// <summary>The audio source for this audio/video.</summary>
		public AudioSource Audio;
		
		
		/// <summary>The src attribute.</summary>
		public string src{
			get{
				return getAttribute("src");
			}
			set{
				setAttribute("src", value);
			}
		}
		
		/// <summary>The mediagroup attribute.</summary>
		public string mediaGroup{
			get{
				return getAttribute("mediagroup");
			}
			set{
				setAttribute("mediagroup", value);
			}
		}
		
		/// <summary>The loop attribute.</summary>
		public bool loop{
			get{
				return GetBoolAttribute("loop");
			}
			set{
				SetBoolAttribute("loop",value);
			}
		}
		
	}
	
}