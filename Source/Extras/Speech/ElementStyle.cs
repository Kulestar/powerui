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
using Dom;
using Blaze;
using UnityEngine;
using Css;

namespace Css{
	
	/// <summary>
	/// Represents SpeechElement.style.
	/// It hosts the computed style amongst other things.
	/// </summary>
	
	public partial class ElementStyle{
		
		/// <summary>The voice in use.</summary>
		public string voice{
			set{
				Set("voice",value);
			}
			get{
				return GetString("voice");
			}
		}
		
	}
	
}