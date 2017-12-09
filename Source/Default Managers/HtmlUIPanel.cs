
#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	#define PRE_UNITY5
#endif

// Note: New Unity UI is only available for 4.6+

#if UNITY_4_6 || UNITY_4_7 || !PRE_UNITY5

using System;
using UnityEngine;
using UnityEngine.EventSystems;


namespace PowerUI{
	
	/// <summary>
	/// Makes it easy to place a PowerUI panel on a Unity UI.
	/// Note that it's not reccommended to mix UI frameworks like this!
	/// This route performs much worse than it otherwise could do.
	/// </summary>
	
	public class HtmlUIPanel : PowerUI.Manager, HtmlUIBase
	{
		
		/// <summary>The PowerUI UI itself. Use this to get through to document etc.</summary>
		internal FlatWorldUI HtmlUI;
		/// <summary>A reference to the rect transform.</summary>
		internal RectTransform RectTransform;
		
		
		/// <summary>Used by HtmlUIBase - the screen region of this panel.</summary>
		public RectTransform screenRect{
			get{
				return RectTransform;
			}
		}
		
		/// <summary>A quick route to the DOM document (useful for e.g. doc.getElementById).</summary>
		public override HtmlDocument document{
			get{
				return HtmlUI.document;
			}
		}
		
		public override void OnEnable () {
			
			// Watch for any file changes:
			Watch();
			
			// Get the UnityUI image panel from the GO:
			UnityEngine.UI.RawImage img=GetComponent<UnityEngine.UI.RawImage>();
			
			// Generate a new UI:
			HtmlUI=new FlatWorldUI("HtmlContent");
			
			// Get dimensions:
			RectTransform dimensions=GetComponent<RectTransform>();
			
			RectTransform=dimensions;
			
			Rect rect=dimensions.rect;
			
			// The virtual screen size:
			HtmlUI.SetDimensions((int)rect.width,(int)rect.height);
			
			// Load from HtmlFile or Src using the PowerUIManager.Navigate function:
			Navigate(document);
			
			// The UI should accept input:
			HtmlUI.AcceptInput=true;
			
			// Apply texture:
			img.texture=HtmlUI.Texture;
			
			// Add an updater which looks out for resizes right before the FWUI redraws:
			HtmlUI.OnUpdate=delegate(){
				
				// Get the current rectangle:
				Rect currentRect=dimensions.rect;
				
				if(HtmlUI.SetDimensions((int)currentRect.width,(int)currentRect.height)){
					
					// Resized! Get the texture again:
					img.texture=HtmlUI.Texture;
					
				}
				
			};
			
			// Include the Unity UI when handling input:
			if(PowerUI.Input.UnityUICaster==null){
				
				// Get the caster:
				PowerUI.Input.UnityUICaster=img.canvas.GetComponent<UnityEngine.UI.GraphicRaycaster>();
				
			}
			
		}
		
		// OnDisable is called when the manager script component is disabled. You don't need this.
		public override void OnDisable () {
		}
		
	}
	
}

#endif