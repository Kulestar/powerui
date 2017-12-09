using UnityEngine;
using System.Collections;
using System;
using Loonim;
using Values;


public class LoonimAdvanced : MonoBehaviour {
	
	/// <summary>The 'live' result.</summary>
	private Texture Result;
	/// <summary>The filter.</summary>
	private SurfaceTexture Filter;
	/// <summary>The draw settings.</summary>
	private DrawInfo DrawInfo;
	/// <summary>Size in pixels.</summary>
	public float Size=256;
	/// <summary>Voronoi contour density.</summary>
	public float Contours=30f;
	/// <summary>Voronoi uniformity (affects how random it looks).</summary>
	public float Uniformity=0.8f;
	/// <summary>Voronoi frequency.</summary>
	public float Frequency=8f;
	
	
	public void Start(){
		
		// Create it:
		Filter=new SurfaceTexture();
		
		// Apply initial properties (inputs into the filter):
		Filter.Set("contours",Contours);
		Filter.Set("uniformity",Uniformity);
		Filter.Set("frequency",Frequency);
		
		// Start building our voronoi node.
		// Voronoi is, in short, a kind of cellular noise.
		Voronoi v=new Voronoi();
		
		// *All* are required:
		v.Frequency=new Property(Filter,"frequency");
		v.Distance=new Property((int)VoronoiDistance.Euclidean);
		v.DrawMode=new Property((int)VoronoiDraw.Normal);
		v.MinkowskiNumber=new Property(0f);
		v.Function=new Property((int)VoronoiMethod.F2minusF1);
		v.Jitter=new Property(Filter,"uniformity");
		
		// If we just displayed 'v' as our root node, we'd just 
		// get some voronoi noise which looks like a bunch of black and white balls.
		
		// So, next, we'll tonemap for interesting effects.
		
		// Create a sine wave with a variable frequency (to be used by the tonemapper):
		TextureNode graph=new ScaleInput(
			new SineWave(),
			new Property(Filter,"contours")
		);
		
		// Create the tonemapper node now.
		// -> We're mapping the black->white range of pixels from the voronoi noise via our sine graph.
		// That creates a kind of hypnotic black-and-white 'rippling' effect.
		ToneMap map=new ToneMap(v,graph);
		
		// Let's make it some more exciting colours.
		// Create a gradient from a background->foreground colour, and use lookup to map from our b+w to that gradient.
		Blaze.Gradient2D rawGradient=new Blaze.Gradient2D();
		rawGradient.Add(0f,Color.blue);
		rawGradient.Add(1f,Color.white);
		
		// Create that lookup now:
		Filter.Root=new Lookup(
			new Loonim.Gradient(rawGradient,null),
			map
		);
		
		// Create the draw information:
		// - GPU mode
		// - Size px square
		// - HDR (true)
		DrawInfo=new DrawInfo((int)Size);
		
	}
	
	public void Update(){
		
		// ----- These Filter.Set calls are an editor thing ----
		//    In most cases, you'll have a point in your code when a value changes.
		//    Set them then rather than spamming them like this
		//    (we just unfortunately don't get any events when the inspector changes).
		
		// Update properties (note that this can be sped up by caching the property references - they don't do all that much anyway):
		Filter.Set("contours",Contours);
		Filter.Set("uniformity",Uniformity);
		Filter.Set("frequency",Frequency);
		
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