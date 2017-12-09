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
using Css;
using UnityEngine;
using Blaze;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents the default "ogg" format.
	/// </summary>
	
	public class OggFormat:AudioFormat{
		
		
		public OggFormat(){}
		
		public OggFormat(AudioClip clip){
			Clip=clip;
		}
		
		public override string[] GetNames(){
			return new string[]{"ogg"};
		}
		
		public override AudioFormat Instance(){
			return new OggFormat();
		}
		
	}
	
}