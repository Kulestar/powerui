using UnityEngine;
using System.Collections;


namespace PowerUI{

	/// <summary>
	/// This is the default PowerUI Manager MonoBehaviour. To use it, it needs to be attached to 
	/// a gameobject in the scene. There's only ever one.
	/// Managers are totally optional too - you can start PowerUI with just UI.Html="..".
	/// </summary>
	
	public class Manager : UnityEngine.MonoBehaviour {
		
		/// <summary>A File containing the html/css/js for the screen.</summary>
		public TextAsset HtmlFile;
		
		/// <summary>Alternative to HtmlFile.</summary>
		[Tooltip("Alternatively, enter a URL")]
		public string Url;
		
		#if UNITY_EDITOR && !UNITY_WEBPLAYER
		/// <summary>The file watcher which does the bulk of the work.</summary>
		private LiveHtml Reloader;
		
		/// <summary>Watch out for any file changes (editor only).</summary>
		[Tooltip("Automatically reloads changed files. Note: This completely ignores any dynamic side effects. Simple previews only.")]
		public bool WatchForChanges;
		
		#endif
		
		/// <summary>A multiplier applied to all non-relative length units. Internally
		/// it's set to the base value for zoom:auto.</summary>
		[Tooltip("The underlying value for zoom:auto. Used to scale your whole UI if you need to.")]
		public float LengthScale = 1f;
		
		/// <summary>Sets zoom:auto to being equal to the devices pixel ratio.</summary>
		[Tooltip("PowerUI will attempt to scale your UI so it's the same physical size. On desktops we can't know the screen DPI (multiple monitors etc) so be careful!")]
		public bool AutomaticallyHandleDpi = true;
		
		#if UNITY_EDITOR
		/// <summary>Watches out for changes.</summary>
		private float _LengthScale = 1f;
		#endif
		/// <summary>The document that this is managing.</summary>
		public HtmlDocument Document;
		
		[SerializeField]
		[HideInInspector] 
		/// <summary>The file path to the Html file. Used by the caching system as an ID.</summary>
		public string HtmlFilePath;
		
		
		/// <summary>The document that this is managing.</summary>
		public virtual HtmlDocument document{
			get{
				return Document;
			}
		}
		
		/// <summary>Applies either Url or HtmlFile to the given document.</summary>
		public void Navigate(HtmlDocument document){
			
			// Set doc:
			Document=document;
			
			// Apply zooms:
			UpdateZoom(false);
			
			// Set the cache path:
			document.cachePath = HtmlFilePath;
			
			// Give it some content:
			if (!string.IsNullOrEmpty(Url)) {
				
				// Navigate it:
				document.location.href=Url;
				
			}else if (HtmlFile != null) {
				
				// Set the innerHTML:
				document.innerHTML = HtmlFile.text;
				
			}else{
				
				// Let's log some info:
				Debug.Log("Please provide a HTML file for your UI. "+
					"If you're stuck, please see the Getting Started guide on the website (http://powerUI.kulestar.com/)"+
					", in the GettingStarted folder, or feel free to contact us! We're always happy to help :)");
				
			}
			
		}
		
		#if UNITY_EDITOR
		/// <summary>Called when e.g. the Url or HtmlFile is changed.</summary>
		public void OnValidate(){
			
			if(Application.isPlaying){
				return;
			}
			
			// Got a HtmlFile?
			if(HtmlFile==null){
				
				HtmlFilePath=null;
				
			}else{
				
				// Get the full asset path (we need something unique to identify the file):
				HtmlFilePath=UnityEditor.AssetDatabase.GetAssetPath(HtmlFile);
				
			}
			
		}
		#endif
		
		/// <summary>Watches for changes if necessary.</summary>
		protected void Watch(){
			
			#if UNITY_EDITOR && !UNITY_WEBPLAYER
			
			if(HtmlFile!=null && WatchForChanges){
				
				// Get the full asset path:
				string fullPath=UnityEditor.AssetDatabase.GetAssetPath(HtmlFile);
				
				// Create the loader:
				Reloader=new LiveHtml(fullPath,
					delegate(string path,string html){
						
						// Write the innerHTML now:
						Document.innerHTML=html;
					
					}
				);
				
			}
			
			#endif
		}
		
		// OnEnable is called when the game starts, or when the manager script component is enabled.
		public virtual void OnEnable () {
			
			// Watch for changes:
			Watch();
			
			// Start:
			UI.Start();
			
			// Load the main UI from the above HtmlFile or Url. Note that UI's don't have to be loaded like this! You
			// can also just set a string of text if needed.
			Navigate(UI.document);
			
		}
		
		/// <summary>Updates the zoom:auto value.</summary>
		public void UpdateZoom (bool requestReflow) {
			
			// Zoom the lengths:
			float zoom=LengthScale;
			
			#if UNITY_EDITOR
			_LengthScale=LengthScale;
			#endif
			
			if (AutomaticallyHandleDpi) {
				
				// Using the web API to get the pixel ratio:
				zoom *= Document.window.devicePixelRatio;
				
			}
			
			// Apply zoom (base for zoom:auto; override by setting some other zoom value):
			Document.Zoom = zoom;
			
			if (requestReflow) {
				
				Document.RequestLayout();
				
			}
			
		}
		
		#if UNITY_EDITOR
		public void Update () {
			
			// Watch out for zoom changes:
			if(LengthScale != _LengthScale){
				
				// Update zoom but with a reflow:
				UpdateZoom(true);
				
				
			}
			
		}
		#endif
		
		// OnDisable is called when the manager script component is disabled. You don't need this.
		public virtual void OnDisable () {
			
			UI.Destroy();
			
			#if UNITY_EDITOR && !UNITY_WEBPLAYER
			
			if(Reloader!=null){
				
				Reloader.Stop();
				Reloader=null;
				
			}
			
			#endif
			
		}
		
	}

}