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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Blaze;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// An object which holds and retrieves different types of audio
	/// such as for synthesis or various formats (mp3, ogg etc).
	/// </summary>
	
	public partial class AudioPackage:ContentPackage{
		
		/// <summary>The contents of this package. A particular format of audio, e.g. a synth track, ogg etc.</summary>
		public AudioFormat Contents;
		
		
		/// <summary>Creates a new package for the named file to get. The path must be absolute.
		/// You must then call <see cref="PowerUI.AudioPackage.send"/> to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		public AudioPackage(string src){
			SetPath(src,null);
		}
		
		/// <summary>Creates a new package for the named file to get.
		/// You must then call <see cref="PowerUI.AudioPackage.send"/> to perform the request.</summary>
		/// <param name="src">The file to get.</param>
		/// <param name="relativeTo">The path the file to get is relative to, if any (may be null).</param>
		public AudioPackage(string src,Location relativeTo){
			SetPath(src,relativeTo);
		}
		
		/// <summary>Creates a package for the given already loaded contents.</summary>
		public AudioPackage(AudioFormat contents){
			Contents=contents;
		}
		
		/// <summary>Creates an audio package containing the given clip.</summary>
		/// <param name="clip">The clip for this audio package.</param>
		public AudioPackage(AudioClip clip){
			SetPath(clip.name,null);
			AssignClip(clip);
		}
		
		/// <summary>Creates an audio package containing the given clip.</summary>
		/// <param name="clip">The clip for this audio package.</param>
		public AudioPackage(string src,Location relativeTo,AudioClip clip){
			SetPath(src,relativeTo);
			AssignClip(clip);
		}
		
		/// <summary>Sends the request off. Callbacks such as onreadystatechange will be triggered.</summary>
		public void send(){
			
			if(readyState_==0){
				// Act like it just opened:
				readyState=1;
			}
			
			// Do we have a file protocol handler available?
			FileProtocol fileProtocol=location.Handler;
			
			if(fileProtocol==null){
				return;
			}
			
			// Some protocols enforce a particular content type.
			string fileType=fileProtocol.ContentType(location);
			
			// Some file formats internally cache.
			// This means we can entirely avoid hitting the protocol system.
			
			// Get the format for the type:
			Contents=AudioFormats.GetInstance(fileType);
			
			// Did it internally cache?
			if(Contents.InternallyCached(location,this)){
				// Yes it did!
				return;
			}
			
			fileProtocol.OnGetAudio(this);
			
		}
		
		/// <summary>Assign the given clip to this package.</summary>
		public void AssignClip(AudioClip clip){
			
			// Get the contents as an ogg block:
			OggFormat ogg=Contents as OggFormat;
			
			if(ogg==null){
				// Clear the package:
				Clear();
				
				// Get the audio format:
				Contents=AudioFormats.GetInstance("ogg");
				
				// Update ogg var:
				ogg=Contents as OggFormat;
			}
			
			// Apply the clip:
			ogg.Clip=clip;
			
		}
		
		/// <summary>The system type of the content, e.g. OggFormat.</summary>
		public Type ContentType{
			get{
				return Contents.GetType();
			}
		}
		
		/// <summary>Assigns the given clip to this package, setting it as a 200 OK.</summary>
		public void GotClip(AudioClip clip){
			
			// Assign a clip:
			AssignClip(clip);
		
			// Straight to RS4:
			Done();
			
		}
		
		#if !MOBILE && !UNITY_WEBGL
		internal override void ReceivedMovieTexture(MovieTexture tex){
			
			// Apply it now:
			AssignClip(tex.audioClip);
			
			base.ReceivedMovieTexture(tex);
			
		}
		#endif
		
		/// <summary>Called by the file handler when the contents are available.</summary>
		public override void ReceivedData(byte[] buffer,int offset,int count){
			
			if(Contents.LoadData(buffer,this)){
				
				// Base:
				base.ReceivedData(buffer,offset,count);
				
			}else{
				
				// Failed:
				Failed(500);
				
			}
			
		}
		
		/// <summary>Called when this audio should now begin.</summary>
		public void Start(Dom.Node context){
			if(Contents!=null){
				Contents.Start(context);
			}
		}
		
		/// <summary>Called when this audio is no longer being displayed.</summary>
		public void Stop(){
			if(Contents!=null){
				Contents.Stop();
			}
		}
		
		/// <summary>Removes all content from this audio package.</summary>
		private void Clear(){
			// Clear any synth:
			Stop();
			Contents=null;
		}
		
		/// <summary>Checks if this package contains something loaded and useable.</summary>
		/// <returns>True if there is a useable graphic in this package.</returns>
		public bool Loaded{
			get{
				return (Contents!=null && Contents.Loaded);
			}
		}
		
	}
	
}