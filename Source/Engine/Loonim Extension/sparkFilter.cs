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
using Loonim;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// A delegate used by the filter CSS property.
	/// </summary>
	public delegate void FilterEventDelegate(RenderableData context,SurfaceTexture texture);
	
	/// <summary>
	/// Represents the -spark-filter: css property.
	/// </summary>
	
	public class Filter:CssProperty{
		
		public static Filter GlobalProperty;
		
		
		/// <summary>Builds a Loonim filter from the given set of one or more CSS functions.</summary>
		public static void BuildFilter(Css.Value value,RenderableData context,FilterEventDelegate fe){
			
			if(value is Css.Functions.UrlFunction){
				
				// - (A URL to an SVG filter) (not supported yet here)
				// - A URL to a Loonim filter (with a property called source0)
				
				// Load it now!
				DataPackage dp=new DataPackage(value.Text,context.Document.basepath);
				
				dp.onload=delegate(UIEvent e){
					
					// Got the data! Load a filter now:
					SurfaceTexture st=null;
					
					// Create:
					st=new SurfaceTexture();
					
					// Load it:
					st.Load(dp.responseBytes);
					
					// Run the callback:
					fe(context,st);
					
				};
				
				// Send:
				dp.send();
				
			}else{
				
				Loonim.TextureNode first=null;
				Loonim.TextureNode last=null;
				
				// - One or more filter functions
				if(value is Css.CssFunction){
					
					// Just one:
					first=last=value.ToLoonimNode(context);
					
				}else if(value is Css.ValueSet){
					
					// Many
					for(int i=0;i<value.Count;i++){
						
						// Convert to a Loonim node:
						Loonim.TextureNode next=value[i].ToLoonimNode(context);
						
						if(next==null){
							// Quit!
							first=null;
							last=null;
							break;
						}
						
						// Hook it on:
						if(first==null){
							first=last=next;
						}else{
							// Add it to the chain:
							next.Sources[0]=last;
							last=next;
						}
						
					}
					
				}
				
				SurfaceTexture st=null;
				
				if(last!=null){
					
					// Create the texture:
					st=new SurfaceTexture();
					st.Set("source0",(UnityEngine.Texture)null);
					st.Root=last;
					first.Sources[0]=new Loonim.Property(st,"source0");
				}
				
				// Run the callback now!
				fe(context,st);
				
			}
			
		}
		
		public Filter(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-filter"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			RenderableData rd=style.RenderData;
			RasterDisplayableProperty rdp=rd.RasterProperty;
			
			if( value==null || value.IsType(typeof(Css.Keywords.None)) ){
				
				// Remove RDP
				if(rdp!=null){
					
					// Destroy it:
					rdp.Destroy();
					
					// Request a layout:
					rdp.RequestLayout();
					
					// Clear:
					rd.RasterProperty=null;
					rdp=null;
					
				}
				
			}else if(rdp==null){
				
				// RDP required:
				rd.RasterProperty=rdp=new RasterDisplayableProperty(rd);
				
				// Update the filter now!
				BuildFilter(value,rd,delegate(RenderableData ctx,SurfaceTexture tex){
					
					// Hopefully tex is not null!
					// If so, we can write it into ctx.RasterProperty, which is hopefully 
					// still available:
					RasterDisplayableProperty filterRdp=ctx.RasterProperty;
					
					if(filterRdp!=null){
						
						// Alright - update it with the filter (which might be null).
						// The filter goes between the FlatWorldUI and the Output.
						filterRdp.SetFilter(tex);
						
						// Request a layout:
						filterRdp.RequestLayout();
						
					}
					
				});
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}