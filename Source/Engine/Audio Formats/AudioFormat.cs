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
using Blaze;
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a specific type of audio format, e.g. a synth file or ogg.
	/// </summary>
	
	[Values.Preserve]
	public class AudioFormat{
		
		/// <summary>The clip to use during playback.</summary>
		public AudioClip Clip;
		/// <summary>The current source it's being played on.</summary>
		internal AudioSource Source;
		
		
		/// <summary>The set of lowercase file types that this format will handle.</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		/// <summary>Is this audio loaded?</summary>
		public virtual bool Loaded{
			get{
				return (Clip!=null);
			}
		}
		
		/// <summary>The length of the clip in seconds.</summary>
		public virtual float Duration{
			get{
				
				if(Clip==null || Clip.length==0f){
					return -1f;
				}
				
				return Clip.length;
			}
		}
		
		/// <summary>Some formats may cache their result internally. This checks and updates if it has.</summary>
		public virtual bool InternallyCached(Location path,AudioPackage package){
			return false;
		}
		
		/// <summary>Creates an instance of this format.</summary>
		public virtual AudioFormat Instance(){
			return null;
		}
		
		/// <summary>Attempt to load the audio from a Unity resource.</summary>
		public virtual bool LoadFromAsset(UnityEngine.Object asset,AudioPackage package){
			
			Clip=asset as AudioClip;
			
			if(Clip!=null){
				return true;
			}
			
			// Note: the full file should be called something.bytes for this to work in Unity.
			TextAsset text=asset as TextAsset;
			
			if(text==null){
				
				package.Failed(404);
				return false;
				
			}
			
			byte[] binary=text.bytes;
			
			package.ReceivedHeaders(binary.Length);
			
			// Apply it now:
			package.ReceivedData(binary,0,binary.Length);
			
			return true;
			
		}
		
		/// <summary>Use this to seek.</summary>
		public virtual float CurrentTime{
			get{
				if(Source!=null){
					return Source.time;
				}
				return 0f;
			}
			set{
				if(Source!=null){
					Source.time=value;
				}
			}
		}
		
		/// <summary>Loads the raw block of data into an object of this format.</summary>
		public virtual bool LoadData(byte[] data,AudioPackage package){
			return false;
		}
		
		/// <summary>Change the paused state of the audio.</summary>
		public virtual void SetPause(bool paused){
			
			if(Source==null){
				return;
			}
			
			if(Source.isPlaying!=paused){
				return;
			}
			
			if(paused){
				Source.Pause();
			}else{
				Source.Play();
			}
			
		}
		
		/// <summary>Called when this audio is expected to start. Note that context may be null.</summary>
		public virtual void Start(Dom.Node context){
			
			// Play the clip in the context's doc:
			WorldUI wUI=(context.document as HtmlDocument).worldUI;
			
			GameObject root;
			
			if(wUI==null){
				root=UI.GUINode;
			}else{
				root=wUI.gameObject;
			}
			
			GameObject host=new GameObject();
			host.transform.parent=root.transform;
			Source=host.AddComponent<AudioSource>();
			
			// 3D space if worldUI; 2D space if main UI:
			Source.spatialBlend=(wUI==null) ? 0f : 1f;
			
			Source.clip=Clip;
			Source.Play();
			
		}
		
		/// <summary>Called when this audio should stop.</summary>
		public virtual void Stop(){
			
			if(Source==null){
				return;
			}
			
			// Destroy the host:
			GameObject.Destroy(Source.gameObject);
			Source=null;
			
		}
		
	}
	
}