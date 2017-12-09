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
using Css;

namespace PowerUI{
	
	/// <summary>
	/// Represents a canvas which lets you draw 2D shapes and polygons on the UI.
	/// </summary>
	
	[Dom.TagName("canvas")]
	public class HtmlCanvasElement:HtmlElement{
		
		/// <summary>The 2D canvas context. Use getContext("2D") or context to obtain.</summary>
		private CanvasContext2D Context2D;
		
		
		/// <summary>The width.</summary>
		public int width{
			get{
				int w;
				if(!int.TryParse(getAttribute("width"),out w) || w<0){
					w=300;
				}
				return w;
			}
			set{
				setAttribute("width", value.ToString());
			}
		}
		
		/// <summary>The height.</summary>
		public int height{
			get{
				int h;
				if(!int.TryParse(getAttribute("height"),out h) || h<0){
					h=150;
				}
				return h;
			}
			set{
				setAttribute("height", value.ToString());
			}
		}
		
		internal override void RemovedFromDOM(){
			
			base.RemovedFromDOM();
			
			// Destroy the context:
			if(Context2D!=null){
				Context2D.Destroy();
				Context2D=null;
			}
			
		}
		
		/// <summary>Gets a rendering context for this canvas.</summary>
		public override CanvasContext getContext(string contextName){
			// Lowercase it:
			contextName=contextName.ToLower();
			
			// Is it the 2D context?
			if(contextName=="2d"){
				return context2D;
			}
			
			return null;
		}
		
		/// <summary>The 2D canvas context.</summary>
		public CanvasContext2D context2D{
			get{
				
				if(Context2D==null){
					Context2D=new CanvasContext2D(this);
				}
				
				// If a layout is pending, redraw right now:
				RequireLayout();
				
				return Context2D;
			}
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			if(Context2D!=null){
				Context2D.UpdateDimensions(box);
			}
			
		}
		
		/// <summary>Gets canvas data (png only). If you want it as a byte[], use context.pngData instead.</summary>
		public override string toDataURL(string mime){
			
			mime=mime.ToLower().Trim();
			
			if(Context2D==null || mime!="text/png"){
				return "";
			}
			
			// Get PNG data:
			byte[] data=Context2D.pngData;
			
			if(data==null){
				return "";
			}
			
			return "data:image/png;base64,"+System.Convert.ToBase64String(data);
		}
		
	}
	
	
	public partial class HtmlElement{
		
		/// <summary>Gets canvas data (png only). If you want it as a byte[], use context.pngData instead.</summary>
		public virtual string toDataURL(string mime){
			return "";
		}
		
		/// <summary>Gets a rendering context for this canvas (if it is a canvas element!).</summary>
		/// <param name="text">The context type e.g. "2D".</param>
		public virtual CanvasContext getContext(string text){
			return null;
		}
		
	}
	
}