using UnityEngine;
using System.Collections;


namespace PowerUI{
	
	/// <summary>
	/// A helper for creating WorldUI's. This helper is totally optional - see the WorldUI and FlatWorldUI classes.
	/// </summary>
	
	public class WorldUIHelper : PowerUI.Manager { // It's a manager so it can share the same inspector properties
		
		/// <summary>Is input enabled for WorldUI's?</summary>
		[Tooltip("Disable input for extra performance")]
		public bool InputEnabled=true;
		/// <summary>The pixel width of the virtual screen.</summary>
		[Tooltip("Height comes from the aspect ratio of your scale.")]
		public int PixelWidth=800;
		/// <summary>Makes the UI always face the camera.</summary>
		[Tooltip("It'll always face a camera (Camera.main by default)")]
		public bool AlwaysFaceTheCamera;
		/// <summary>Set the WorldUI into PP mode.</summary>
		[Tooltip("The UI will scale itself so it's always pixel perfect")]
		public bool PixelPerfect;
		/// <summary>The expiry for the WorldUI.</summary>
		[Tooltip("The amount of seconds until the WorldUI destroys itself. 0 means no expiry time.")]
		public float Expiry=0f;
		/// <summary>The worldUI instance. Available after OnEnable.</summary>
		internal WorldUI WorldUI;
		[Tooltip("Renders your WorldUI to a texture so it's totally flat. "+
			"Don't use this if you want to use lighting or text-extrude, but do use it if you have a large DOM.")]
		public bool MakeItFlat;
		
		
		public override void OnEnable () {
			
			// Watch for any file changes:
			Watch();
			
			// Dump renderer/filter, unless it's flat:
			MeshRenderer mr=gameObject.GetComponent<MeshRenderer>();
			
			// True if we've got the visual guide:
			bool hasGuide=false;
			
			if( !MakeItFlat && mr!=null && mr.material!=null && mr.material.name.StartsWith("worldUIMaterial") ){
				
				// Remove it:
				GameObject.Destroy(mr);
				
				hasGuide=true;
				
				// Remove filter too:
				MeshFilter filter=gameObject.GetComponent<MeshFilter>();
				
				if(filter!=null){
					
					GameObject.Destroy(filter);
					
				}
				
			}
			
			// First, figure out the 'aspect ratio' of the scale:
			Vector3 scale=transform.localScale;
			float yAspect=scale.z / scale.x;
			
			// Calc the number of pixels:
			int height=(int)((float)PixelWidth * yAspect);
			
			if(MakeItFlat){
				
				// Create it as a FlatWorldUI instead:
				FlatWorldUI fwUI = new FlatWorldUI(PixelWidth,height);
				WorldUI = fwUI;
				
				// Grab the texture and apply it to the material:
				if(mr!=null){
					Material mat = new Material(mr.material.shader);
					mat.mainTexture = fwUI.texture;
					mr.material=mat;
					
					// The flat version is upside down relative to the 3D one:
					mat.mainTextureScale = new Vector2(-1f, -1f);
					mat.mainTextureOffset = new Vector2(1f, 1f);
				}
				
				// Give it some content using PowerUI.Manager's Navigate method:
				// (Just so we can use the same Html/ Url fields - it's completely optional)
				Navigate(WorldUI.document);
				
				if(InputEnabled){
					
					// Create a box collider:
					BoxCollider bc = gameObject.AddComponent<BoxCollider>();
					
					// Accept input from it:
					fwUI.AcceptInputFrom(bc);
					
				}
				
			}else{
				
				// Reset local scale:
				// transform.localScale=Vector3.one;
				
				// Generate a new UI (the name is optional).
				// The two numbers are the dimensions of our virtual screen:
				WorldUI=new WorldUI(gameObject.name,PixelWidth,height);
				
				// Settings:
				WorldUI.PixelPerfect=PixelPerfect;
				WorldUI.AlwaysFaceCamera=AlwaysFaceTheCamera;
				
				if(Expiry!=0f){
					
					WorldUI.SetExpiry(Expiry);
					
				}
				
				// Give it some content using PowerUI.Manager's Navigate method:
				// (Just so we can use the same Html/ Url fields - it's completely optional)
				Navigate(WorldUI.document);
				
				// Parent it to the GO:
				WorldUI.ParentToOrigin(transform);
				
				if(hasGuide){
					// Rotate it 90 degrees about x (to match up with the guide):
					// WorldUI.transform.localRotation=Quaternion.AngleAxis(90f,new Vector3(1f,0f,0f));
				}
				
				// Set the scale such that the width "fits".
				// The panel will be PixelWidth wide, so we want to divide by that to get to '1'.
				// Note that the 10 is because a plane is 10 units wide.
				// We then multiply it by whatever scale (on x) the user originally wanted.
				// The y scale is accounted for by the computed pixel height (we don't want to distort it).
				float scaleFactor=(10f * scale.x) / (float)PixelWidth;
				
				WorldUI.transform.localScale=new Vector3(scaleFactor,scaleFactor,scaleFactor);
				
				// Optionally accept input:
				WorldUI.AcceptInput=InputEnabled;
				
			}
			
			// Input is always inverted for these:
			WorldUI.InvertResolve=true;
			
		}
		
		public override void OnDisable(){
			
			// Overriden so we don't destroy the main UI!
			
		}
		
	}

}