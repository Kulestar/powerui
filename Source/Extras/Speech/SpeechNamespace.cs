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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Css;


namespace Speech{
	
	/// <summary>
	/// The SML namespace attribute as used by all Speech nodes.
	/// </summary>
	public class SpeechNamespace : XmlNamespace{
		
		public SpeechNamespace()
			:base("http://www.w3.org/2001/10/synthesis","ssml","application/ssml+xml",typeof(SpeechDocument))
		{
			
		}
		
	}
	
}