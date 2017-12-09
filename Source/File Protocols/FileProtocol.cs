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
using Dom;


namespace PowerUI{
	
	/// <summary>A delegate for dealing with event targets.</summary>
	public delegate void InternalEventHandler(EventTarget et);
	
	/// <summary>
	/// Represents a custom protocol:// as used by PowerUI files.
	/// For example, if you wish to deliver content in a custom way to PowerUI, implement a new FileProtocol (e.g. 'cdn')
	/// Then, setup its OnGetGraphic function.
	/// </summary>
	
	[Values.Preserve]
	public class FileProtocol{
		
		/// <summary>An event called when any request is started.
		/// Useful for debugging networking (and is used by the network inspector).</summary>
		public static InternalEventHandler OnRequestStarted;
		
		/// <summary>Attempts to get cached data for the given location. Used to enable caching (off by default).</summary>
		public virtual CachedContent GetCached(Dom.Location location){
			
			return null;
			
		}
		
		/// <summary>Gets the domain data for the given location.</summary>
		public virtual DomainData GetDomain(Dom.Location location){
			
			return Cache.GetDomain(location.host);
			
		}
		
		/// <summary>Gets the relative path for the given location.</summary>
		public virtual string GetRelative(Dom.Location location){
			
			return location.relative;
			
		}
		
		/// <summary>Attempts to get cached data for the given location.</summary>
		public CachedContent GetCached(Dom.Location location,bool create){
			
			// Get the cache domain data (never null):
			DomainData domain=GetDomain(location);
			
			if(domain.Content==null){
				
				if(create){
					// Create the content set:
					domain.Content=new CachedContentSet(domain);
				}else{
					return null;
				}
				
			}
			
			// Get the entry:
			string path=GetRelative(location);
			
			CachedContent e;
			if(!domain.Content.Entries.TryGetValue(path,out e) && create){
				
				// Create the entry:
				e=new CachedContent(domain.Content, path);
				
				// Add it:
				domain.Content.Add(path,e);
				
			}
			
			return e;
		}
		
		/// <summary>Returns all protocol names:// that can be used for this protocol.
		/// e.g. new string[]{"cdn","net"}; => cdn://file.png or net://file.png</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		/// <summary>Used to determine the content type for the file from the given path.</summary>
		public virtual string ContentType(Location path){
			return path.Filetype;
		}
		
		/// <summary>Does the item at the given location have full access to the code security domain?
		/// Used by Nitro. If it does not have full access, the Nitro security domain is asked about the path instead.
		/// If you're unsure, leave this false! If your game uses simple web browsing, 
		/// this essentially stops somebody writing dangerous Nitro on a remote webpage and directing your game at it.</summary>
		public virtual bool FullAccess(Location path){
			return false;
		}
		
		/// <summary>Gets binary data, checking the cache first.</summary>
		public void OnGetData(ContentPackage package){
			
			// Attempt to hit the cache:
			CachedContent e=GetCached(package.location);
			
			if(e!=null){
				
				if(e.Expired){
					
					// Expire it!
					e.Set.Entries.Remove(e.Path);
					
					// Save the set:
					e.Set.Save();
					
				}else{
					
					// Successful hit - respond with a 304 (not modified):
					package.NotModified(e);
					
					return;
					
				}
				
			}
			
			// Ordinary request:
			OnGetDataNow(package);
			
		}
		
		/// <summary>Get generic binary at the given path using this protocol. Used for e.g. fonts.
		/// Once it's been retrieved, this must call package.GotData(theText) internally.
		/// Note that you can call this directly to avoid all cache checking.</summary>
		public virtual void OnGetDataNow(ContentPackage package){}
		
		/// <summary>Get the file at the given path as a MovieTexture (Unity Pro only!), Texture2D,
		/// SPA Animation or DynamicTexture using this protocol.
		/// Once it's been retrieved, this must call package.GotGraphic(theObject) internally.</summary>
		public virtual void OnGetGraphic(ImagePackage package){
			
			// Get the data - it'll call package.Done which will handle it for us:
			OnGetData(package);
			
		}
		
		/// <summary>Get the file at the given path as any supported audio format.
		/// Once it's been retrieved, this must call package.GotAudio(theObject) internally.</summary>
		public virtual void OnGetAudio(AudioPackage package){
			
			// Get the data - it'll call package.Done which will handle it for us:
			OnGetData(package);
			
		}
		
		/// <summary>The user clicked on the given link which points to the given path.</summary>
		public virtual void OnFollowLink(HtmlElement linkElement,Location path){
			
			// Apply location:
			linkElement.document.location=path;
			
		}
		
	}
	
}