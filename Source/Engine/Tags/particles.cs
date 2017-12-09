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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	#define PRE_UNITY4
#endif

using System;
using Css;
using UnityEngine;

namespace PowerUI{
	
	/// <summary>
	/// Custom tag for inline particles. You can place your UI before and after this as normal.
	/// You must importantly set the path="" attribute to the path in the hierarchy of the particle system itself.
	/// </summary>
	
	[Dom.TagName("particles")]
	public class HtmlParticlesElement:HtmlElement{
		
		/// <summary>The particle system iself. Set with path="hierarchy_path".</summary>
		public GameObject Particles;
		/// <summary>Quick reference to Particles.transform.</summary>
		private Transform ParticleTransform;
		/// <summary>Quick reference to the particle system material.</summary>
		private Material ParticleMaterial;
		/// <summary>A displayableProperty which lets us obtain a custom batch.
		/// In short, this allows us to correctly set material.renderQueue so there's no nasty flicker.</summary>
		private DisplayableProperty BatchProperty;
		/// <summary>The scale of the system.</summary>
		public Vector3 Scale=new Vector3(0.1f,0.1f,0.1f);
		/// <summary>The rotation of the system.</summary>
		public Quaternion Rotation=Quaternion.AngleAxis(180f,Vector3.up);
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="path"){
				// Go get the system now!
				
				// Clear any existing one:
				Destroy();
				
				// Grab the path itself:
				string path=getAttribute("path");
				
				// Get it:
				GameObject gameObject=GameObject.Find(path);
				
				if(gameObject!=null){
					
					// Apply it:
					ApplySystem(gameObject);
					
				}
				
				return true;
			}else if(property=="src"){
				
				// Clear any existing one:
				Destroy();
				
				// Get the system from Resources and instance it:
				GameObject gameObject=GameObject.Instantiate(Resources.Load(getAttribute("src"))) as GameObject;
				
				if(gameObject!=null){
					
					// Apply it:
					ApplySystem(gameObject);
					
				}
				
			}
			
			return false;
		}
		
		/// <summary>Applies the given particle system gameObject to this particles tag.
		/// Essentially makes it get laid out by the UI.
		/// Usually you'd use the path=".." attribute (or anElement["path"]) rather than calling this directly.</summary>
		public void ApplySystem(GameObject gameObject){
			
			if(BatchProperty==null){
				// Create the BP now:
				BatchProperty=new DisplayableProperty(RenderData);
				
				// It's always isolated (as the particle system is on a layer of it's own):
				BatchProperty.Isolate();
				
			}
			
			// Get the material:
			ParticleSystemRenderer partRenderer=gameObject.GetComponent<ParticleSystemRenderer>();
			
			if(partRenderer==null){
				Debug.LogError("The gameObject '"+gameObject.name+"' wasn't a particle system on a <particles> tag (it has no ParticleSystemRenderer)");
				return;
			}
			
			// Apply:
			Particles=gameObject;
			
			// Apply material:
			ParticleMaterial=partRenderer.material;
			
			// Get the transform:
			Transform transform=gameObject.transform;
			ParticleTransform=transform;
			
			// PowerUI layer, unless we're on a WorldUI:
			if(htmlDocument.worldUI==null){
				
				// Update the layer:
				gameObject.layer=UI.Layer;
				
			}
			
			// parent to GUINode:
			transform.parent=rootGameObject.transform;
			
			// Zero local position:
			ParticleTransform.localPosition=Vector3.zero;
			
			// Update scale/rotation:
			Relocate();
			
			// Request a layout:
			htmlDocument.RequestLayout();
			
		}
		
		/// <summary>Applies rotation/scale.</summary>
		public void Relocate(){
			
			if(ParticleTransform==null){
				return;
			}
			
			// Apply rotation/scale:
			ParticleTransform.localRotation=Rotation;
			ParticleTransform.localScale=Scale;
			
		}
		
		public void Destroy(){
			
			if(Particles!=null){
				GameObject.Destroy(Particles);
				Particles=null;
				ParticleTransform=null;
			}
			
		}
		
		/// <summary>Called during the layout pass.</summary>
		public override void OnRender(Renderman renderer){
			
			if(ParticleTransform==null){
				return;
			}
			
			// Grab the computed style and the renderer:
			ComputedStyle computed=Style.Computed;
			LayoutBox box=computed.FirstBox;
			
			if(box==null){
				// display:none.
				return;
			}
			
			// Get the top left inner corner (inside margin and border):
			float width=box.PaddedWidth;
			float height=box.PaddedHeight;
			float top=box.Y+box.Border.Top;
			float left=box.X+box.Border.Left;
			
			// Figure out the middle of that:
			float middleX=left + (width/2);
			float middleY=top + (height/2);
			
			// Map it to our world location:
			ParticleTransform.localPosition=renderer.PixelToWorldUnit(middleX,middleY,computed.ZIndex);
			
			BatchProperty.GotBatchAlready=false;
			
			// Setup the batch (so we can get the queue number):
			renderer.SetupBatch(BatchProperty,null,null);
			
			// Set the particle material to the batch - this'll ensure it gets the right renderQueue:
			renderer.CurrentBatch.Mesh.SetMaterial(ParticleMaterial);
			
		}
		
	}
	
}