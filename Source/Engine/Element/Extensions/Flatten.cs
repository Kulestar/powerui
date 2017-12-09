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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Used to 'flatten' an element.
	/// </summary>

	public partial class HtmlElement{
		
		/// <summary>If a flatten promise is rejected, the error code.</summary>
		public const int FLATTEN_FAILED=10;
		
		/// <summary>Obtains the flat image when CSS -spark-filter is in use.
		/// Note that if you want to know when this image changes, 
		/// add a handler for the imagechange event (Dom.Event).</summary>
		public Texture flatImage{
			get{
				
				// Obtain the raster data from the render data:
				RasterDisplayableProperty rdp=RenderData.RasterProperty;
				
				if(rdp==null){
					return null;
				}
				
				// It's the output property (which might've passed through filters):
				return rdp.Output;
			}
		}
		
		/// <summary>Flattens this element.
		/// Returns a promise which fires when the image has been rendered at least once.
		/// The promise contains a direct reference to the latest image.</summary>
		public Promise flatten(){
			
			// Obtain the raster data from the render data:
			RasterDisplayableProperty rdp=RenderData.RasterProperty;
			
			// Create the resultant promise:
			Promise p=new Promise();
			
			// Check if we already have an image:
			Texture img=rdp==null ? null : rdp.Output;
			
			if(img!=null){
				// Already got one! Resolve right away:
				p.resolve(img);
				return p;
			}
			
			// If we don't have an RDP then request a flatten.
			// Output is null so we know it'll be getting drawn on the next update.
			if(rdp==null){
				
				// (no full Loonim effects on this element):
				style.Computed.ChangeTagProperty("-spark-filter","flatten");
				
			}
			
			Dom.EventListener listener = null;
			
			// Wait for the imagechange event:
			listener = new Dom.EventListener<Dom.Event>(delegate(Dom.Event e){
				
				// Image changed! Remove this listener:
				removeEventListener("imagechange",listener);
				
				// Update the promise:
				Texture current=flatImage;
				
				if(current==null){
					// Unable to flatten this (This should never happen).
					p.reject(FLATTEN_FAILED);
				}else{
					p.resolve(flatImage);
				}
				
			});
			
			// Add it:
			addEventListener("imagechange",listener);
			
			return p;
			
		}
		
	}
	
}