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
using Dom;
using Blaze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Css;


namespace Speech{
	
	/// <summary>
	/// A base class for all speech markup tag types. This is used to distictively identify them.
	/// </summary>
	
	[SpeechNamespace]
	[Dom.TagName("Default")]
	public class SpeechElement:Element, IRenderableNode{
		
		/// <summary>This elements style.</summary>
		public ElementStyle Style;
		
		
		public SpeechElement(){
			Style=new ElementStyle(this);
		}
		
		/// <summary>This nodes computed style.</summary>
		public ComputedStyle ComputedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.
		/// This does not include the elements own padding/margin/border.</summary>
		public void GetWidthBounds(out float min,out float max){
			
			min=0f;
			max=0f;
			
			// For each child, get its width bounds too.
			if(RenderData.FirstBox==null){
				return;
			}
			
			if(childNodes_!=null){
				
				// Current line:
				float cMin=0f;
				float cMax=0f;
				
				for(int i=0;i<childNodes_.length;i++){
					
					Node child=childNodes_[i];
					
					Css.IRenderableNode renderable=(child as Css.IRenderableNode);
					
					if(renderable==null){
						continue;
					}
					
					float bMin;
					float bMax;
					
					if(child is PowerUI.RenderableTextNode){
						
						// Always get bounds:
						renderable.GetWidthBounds(out bMin,out bMax);
						
					}else{
						
						// Get the first box from the render data:
						RenderableData rd=renderable.RenderData;
						LayoutBox box=rd.FirstBox;
						
						if(box==null){
							continue;
						}
						
						// If it's inline (or float) then it's additive to the current line.
						if((box.DisplayMode & DisplayMode.OutsideBlock)!=0 && box.FloatMode==FloatMode.None){
							
							// Line break!
							cMin=0f;
							cMax=0f;
							
						}
						
						// Get an explicit width:
						bool wasAuto;
						bMin=rd.GetWidth(true,out wasAuto);
							
						if(bMin==float.MinValue){
							
							// Get the bounds:
							renderable.GetWidthBounds(out bMin,out bMax);
							
						}else{
							
							bMax=bMin;
							
						}
						
						// Add margins etc:
						float extraStyle=(
							box.Border.Left+box.Border.Right+
							box.Padding.Left+box.Padding.Right+
							box.Margin.Left+box.Margin.Right
						);
						
						bMin+=extraStyle;
						bMax+=extraStyle;
						
					}
					
					// Apply to line:
					cMin+=bMin;
					cMax+=bMax;
					
					// Longest line?
					if(cMin>min){
						min=cMin;
					}
					
					if(cMax>max){
						max=cMax;
					}
					
				}
				
			}
			
		}
		
		/// <summary>Called when this element goes offscreen.</summary>
		public void WentOffScreen(){
			
			RenderableData renderable=RenderData;
			renderable.WentOffScreen();
			
			// Apply to all virtual elements:
			VirtualElements virts=renderable.Virtuals;
			
			if(virts!=null){
				
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
				
					// Tell it that it's gone offscreen:
					IRenderableNode irn=(kvp.Value as IRenderableNode);
					
					if(irn!=null){
						irn.WentOffScreen();
					}
					
				}
				
			}
			
			if(childNodes_!=null){
				
				for(int i=0;i<childNodes_.length;i++){
					
					// Get as a HTML node:
					IRenderableNode htmlNode=(childNodes_[i] as IRenderableNode);
					
					if(htmlNode==null){
						return;
					}
					
					// Call offscreen:
					htmlNode.WentOffScreen();
					
				}
				
			}
			
		}
		
		/// <summary>Called during the box compute process. Useful if your element has clever dimensions, such as the img tag or words.</summary>
		public virtual void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
		}
		
		/// <summary>Called during a global layout event on all elements.</summary>
		public virtual void OnRender(Renderman renderer){}
		
		/// <summary>Called when an attribute of the element was changed.
		/// Returns true if the method handled the change to prevent unnecessary checks.</summary>
		public override bool OnAttributeChange(string property){
			
			if(property=="style"){
				Style.cssText=getAttribute("style");
				return true;
			}
			
			// Style refresh:
			if(Style.Computed.FirstMatch!=null){
				// This is a runtime attribute change.
				// We must consider if it's affecting the style or not:
				Style.Computed.AttributeChanged(property);
			}
			
			if(property=="id"){
				return true;
			}else if(property=="class"){
				return true;
			}
			
			return false;
		}
		
		/// <summary>Gets the first element which matches the given selector.</summary>
		public Element querySelector(string selector){
			HTMLCollection results=querySelectorAll(selector,true);
			
			if(results==null || results.length==0){
				return null;
			}
			
			return results[0] as Element;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector){
			return querySelectorAll(selector,false);
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector,bool one){
		
			// Create results set:
			HTMLCollection results=new HTMLCollection();
			
			if(string.IsNullOrEmpty(selector)){
				// Empty set:
				return results;
			}
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selector,this);
			
			// Read a value:
			Css.Value value=lexer.ReadValue();
			
			// Read the selectors from the value:
			List<Selector> selectors=new List<Selector>();
			Css.CssLexer.ReadSelectors(null,value,selectors);
			
			// Create a blank event to store the targets, if any:
			CssEvent e=new CssEvent();
			
			// Perform the selection process:
			querySelectorAll(selectors.ToArray(),results,e,false);
			
			return results;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selectors">The selectors to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public void querySelectorAll(Selector[] selectors,INodeList results,CssEvent e,bool one){
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				Node node=childNodes_[i];
				Element child=node as Element;
				IRenderableNode irn=(child as IRenderableNode);
				
				if(child==null || irn==null){
					continue;
				}
				
				ComputedStyle cs=irn.ComputedStyle;
				
				for(int s=0;s<selectors.Length;s++){
					
					// Match?
					if(selectors[s].StructureMatch(cs,e)){
						// Yep!
						results.push(node);
						
						if(one){
							return;
						}
					}
					
				}
				
				irn.querySelectorAll(selectors,results,e,one);
				
				if(one && results.length==1){
					return;
				}
			}
			
		}
		
		/// <summary>This nodes render data.</summary>
		public RenderableData RenderData{
			get{
				return Style.Computed.RenderData;
			}
		}
		
		/// <summary>This elements style.</summary>
		public override ElementStyle style{
			get{
				return Style;
			}
		}
		
		/// <summary>Gets the computed style of this element.</summary>
		public Css.ComputedStyle computedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Called when a font-face is ready.</summary>
		public void FontLoaded(PowerUI.DynamicFont font){
			
			if(childNodes_==null){
				return;
			}
			
			int count=childNodes_.length;
			
			for(int i=0;i<count;i++){
				IRenderableNode node=(childNodes_[i] as IRenderableNode);
				
				if(node==null){
					continue;
				}
				
				node.FontLoaded(font);
			}
			
		}
		
	}
	
}