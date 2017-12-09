using UnityEngine;
using System.Collections;
using System;
using Loonim;
using Values;

/// <summary>
/// Demonstrates a Loonim graph which blends two images together.
/// </summary>

public class LoonimSimple : MonoBehaviour {
	
	/// <summary>The 'live' result.</summary>
	private Texture Result;
	/// <summary>The source images to blend.</summary>
	public Texture2D SourceImageA;
	/// <summary>The source images to blend.</summary>
	public Texture2D SourceImageB;
	/// <summary>The filter.</summary>
	private SurfaceTexture Filter;
	/// <summary>The draw settings.</summary>
	private DrawInfo DrawInfo;
	/// <summary>Blend mode.</summary>
	public BlendingMode Mode=BlendingMode.Normal;
	/// <summary>Blending weight</summary>
	public float BlendWeight=1f;
	
	
	public void Start(){
		
		// Create it:
		Filter=new SurfaceTexture();
		
		// Apply initial properties (inputs into the filter):
		Filter.Set("srcA",SourceImageA);
		Filter.Set("srcB",SourceImageB);
		Filter.Set("weight",BlendWeight);
		Filter.Set("mode",(int)Mode);
		
		// Build the node graph now (just the one node here too):
		
		Filter.Root=new Blend(
			new Property(Filter,"srcA"),
			new Property(Filter,"srcB"),
			new Property(Filter,"weight"),
			new Property(Filter,"mode")
		);
		
		// Create the draw information:
		// - GPU mode
		// - Size px square
		// - HDR (true)
		DrawInfo=new DrawInfo((int)SourceImageA.width);
		
	}
	
	public void Update(){
		
		// ----- These Filter.Set calls are an editor thing ----
		//    In most cases, you'll have a point in your code when a value changes.
		//    Set them then rather than spamming them like this
		//    (we just unfortunately don't get any events when the inspector changes).
		
		// Update properties (note that this can be sped up by caching the property references - they don't do all that much anyway):
		Filter.Set("weight",BlendWeight);
		Filter.Set("mode",(int)Mode);
		Filter.Set("srcA",SourceImageA);
		
		// -----------------
		
		// Render it now:
		// Filter.Draw renders it 'live' meaning the result is a RenderTexture.
		// This is better than constantly going from a RT to a Tex2D (but note that it only actually redraws when you call Draw).
		Texture newResult=Filter.Draw(DrawInfo);
		
		if(Result!=newResult){
			Result=newResult;
			
			// Update element (getById is short for obtaining it as a HtmlElement):
			PowerUI.UI.document.getById("loonim-image").image=Result;
			
		}
		
	}
	
}