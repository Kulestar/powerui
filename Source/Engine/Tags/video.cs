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
using Css;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles a video. Note that videos can also be used for the css background-image property.
	/// You must also set the height and width of this element using either css or height="" and width="".
	/// </summary>
	
	[Dom.TagName("video")]
	public class HtmlVideoElement:HtmlMediaElement{
		
		/// <summary>The underlying image package. Can be null.</summary>
		public ImagePackage RawImage{
			get{
				Css.BackgroundImage bg=RenderData.BGImage;
				
				if(bg==null || bg.Image==null){
					return null;
				}
				
				return bg.Image;
			}
		}
		
		/// <summary>The height attribute.</summary>
		public string height{
			get{
				return getAttribute("height");
			}
			set{
				setAttribute("height", value);
			}
		}
		
		/// <summary>The poster attribute.</summary>
		public string poster{
			get{
				return getAttribute("poster");
			}
			set{
				setAttribute("poster", value);
			}
		}
		
		/// <summary>The video frame height of the image in CSS pixels.</summary>
		public ulong videoHeight{
			get{
				ImagePackage img=RawImage;
				if(img==null || img.Contents==null || !img.Contents.Loaded){
					return 0;
				}
				
				return (ulong)img.Height;
			}
		}
		
		/// <summary>The video frame width of the image in CSS pixels.</summary>
		public ulong videoWidth{
			get{
				ImagePackage img=RawImage;
				if(img==null || img.Contents==null || !img.Contents.Loaded){
					return 0;
				}
				
				return (ulong)img.Width;
			}
		}
		
		/// <summary>The width attribute.</summary>
		public string width{
			get{
				return getAttribute("width");
			}
			set{
				setAttribute("width", value);
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				style.backgroundImage="url(\""+getAttribute("src").Replace("\"","\\\"")+"\")";
				return true;
			}
			
			return false;
		}
		
		public override void OnChildrenLoaded(){
			// Does this video tag have <source> elements as kids?
			string src=getAttribute("src");
			
			if(src!=null){
				
				// Base:
				base.OnChildrenLoaded();
				
				return;
			}
			
			// Grab the kids:
			NodeList kids=childNodes_;
			
			if(kids==null){
				
				// Base:
				base.OnChildrenLoaded();
				
				return;
			}
			
			// For each child, grab it's src value. Favours .ogg.
			
			foreach(Node child in kids){
				// Grab the src:
				string childSrc=child.getAttribute("src");
				
				if(childSrc==null){
					continue;
				}
				
				#if !MOBILE && !UNITY_WEBGL
				// End with ogg, or do we have no source at all?
				if(src==null || childSrc.ToLower().EndsWith(".ogg")){
					src=childSrc;
				}
				#else
				// End with spa, or do we have no source at all?
				if(src==null || childSrc.ToLower().EndsWith(".spa")){
					src=childSrc;
				}
				#endif
				
			}
			
			if(src!=null){
				// Apply it now:
				style.backgroundImage="url(\""+src.Replace("\"","\\\"")+"\")";
			}
			
			// Base:
			base.OnChildrenLoaded();
			
		}
		
		#if !MOBILE && !UNITY_WEBGL
		/// <summary>The source movie texture.</summary>
		public override MovieTexture video{
			get{
				// Grab the background image:
				BackgroundImage image=RenderData.BGImage;
				
				if(image==null || image.Image==null){
					// Not got a background image. Stop there.
					return null;
				}
				
				VideoFormat video=image.Image.Contents as VideoFormat;
				
				if(video==null){
					// Not ready yet.
					return null;
				}
				
				// Grab the video:
				return video.Video;
			}
		}
		#endif
		
	}
	
	#if !MOBILE && !UNITY_WEBGL
	/// <summary>
	/// This class extends HtmlElement to include an easy to use element.video property (unavailable on mobile).
	/// </summary>
	
	public partial class HtmlElement{
		
		/// <summary>Gets this element as a video element.</summary>
		public HtmlVideoElement videoElement{
			get{
				return this as HtmlVideoElement;
			}
		}
		
		/// <summary>The source movie texture.</summary>
		public virtual MovieTexture video{
			get{
				return null;
			}
		}
		
		/// <summary>Is the video playing?</summary>
		public bool playing{
			get{
				return video.isPlaying;
			}
		}
		
		/// <summary>Is the video paused?</summary>
		public bool paused{
			get{
				return !video.isPlaying;
			}
		}
		
		/// <summary>Stops the video.</summary>
		public void stop(){
			MovieTexture movie=video;
			
			if(!movie.isPlaying){
				return;
			}
			
			movie.Stop();
			
			// Fire an onstop event:
			Run("onstop",this);
		}
		
		/// <summary>Pauses the video.</summary>
		public void pause(){
			MovieTexture movie=video;
			
			if(!movie.isPlaying){
				return;
			}
			
			movie.Pause();
			
			// Fire an onpause event:
			Run("onpause",this);
		}
		
		/// <summary>Plays the video.</summary>
		public void play(){
			MovieTexture movie=video;
			
			if(movie.isPlaying){
				return;
			}
			
			movie.Play();
			
			if(this["audio"]!=null){
				
				playAudio();
				
			}
			
			// Fire an onplay event:
			Run("onplay",this);
		}
		
		public void playAudio(){
			
			playAudio(rootGameObject);
			
		}
		
		public void stopAudio(){
			removeAudio();
		}
		
		public void removeAudio(){
			
			HtmlVideoElement tag=videoElement;
			
			// Get the audio source:
			AudioSource source=tag.Audio;
			
			if(source==null){
				return;
			}
			
			GameObject.Destroy(source);
			tag.Audio=null;
			
		}
		
		public void playAudio(GameObject parent){
			
			AudioSource source=videoElement.Audio;
			
			if(source!=null){
				source.Play();
				return;
			}
			
			if(parent==null){
				parent=new GameObject();
			}
			
			AudioClip clip=audioTrack;
			source=parent.GetComponent<AudioSource>();
			
			if(source==null){
				source=parent.AddComponent<AudioSource>();
			}
			
			source.clip=clip;
			source.Play();
			
			// Apply to video handler:
			videoElement.Audio=source;
			
		}
		
		/// <summary>Gets the audio track of the video.</summary>
		public AudioClip audioTrack{
			get{
				return video.audioClip;
			}
		}
		
	}
	#endif
	
}