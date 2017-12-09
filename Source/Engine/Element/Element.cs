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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// This delegate is used for hooking up c# methods with mouse and keyboard events.
	/// Note that in general Nitro is best used for handling these.
	/// </summary>
	public delegate void OnDomEvent(UIEvent uiEvent);
	
	/// <summary>
	/// This represents a html element in the DOM.
	/// </summary>
	[HtmlNamespace]
	[Dom.TagName("Default")]
	public partial class HtmlElement:Element, IRenderableNode{
		
		/// <summary>Internal use only. The style of this element. Use <see cref="PowerUI.HtmlElement.style"/> instead.</summary>
		public ElementStyle Style;
		/// <summary>Internal use only. Children being rendered are set here. This allows multiple threads to access the DOM.</summary>
		public NodeList KidsToRender;
		
		
		public HtmlElement(){
			Style=new ElementStyle(this);
		}
		
		/// <summary>This nodes computed style.</summary>
		public ComputedStyle ComputedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>This nodes render data.</summary>
		public RenderableData RenderData{
			get{
				return Style.Computed.RenderData;
			}
		}
		
		/// <summary>An enumerated version of the contenteditable attribute.</summary>
		public string contentEditable{
			get{
				string rawValue=getAttribute("contenteditable");
				if(rawValue=="inherit"){
					return rawValue;
				}
				
				if(rawValue!=null && rawValue!="0" && rawValue!="false"){
					return "true";
				}
				
				return "false";
			}
			set{
				setAttribute("contenteditable", value);
			}
		}
		
		/// <summary>Is this element content editable?</summary>
		public bool isContentEditable{
			get{
				return GetBoolAttribute("contenteditable");
			}
			set{
				if(value){
					contentEditable="true";
				}else{
					contentEditable=null;
				}
			}
		}
		
		/*
		public override void OnChildrenLoaded(){
			
			// Construct the selector structure now:
			// Style.Computed.RefreshStructure();
			
		}
		*/
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.
		/// This does not include the elements own padding/margin/border.</summary>
		public virtual void GetWidthBounds(out float min,out float max){
			
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
					
					IRenderableNode renderable=(child as IRenderableNode);
					
					if(renderable==null){
						continue;
					}
					
					float bMin;
					float bMax;
					
					if(child is RenderableTextNode){
						
						// Always get bounds:
						renderable.GetWidthBounds(out bMin,out bMax);
						
					}else{
						
						// Get the first box from the render data:
						ComputedStyle cs=renderable.ComputedStyle;
						RenderableData rd=renderable.RenderData;
						LayoutBox box=rd.FirstBox;
						
						if(box==null){
							continue;
						}
						
						int displayMode=box.DisplayMode;
						
						// If it's inline (or float) then it's additive to the current line.
						if((displayMode & DisplayMode.OutsideBlock)!=0 && box.FloatMode==FloatMode.None){
							
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
						
						// Add margins etc (NB: These are calculated twice due to %):
						BoxStyle padding=cs.GetPaddingBox(displayMode);
						BoxStyle border=cs.GetBorderBox(displayMode);
						
						// Compute the initial margin:
						bool marginAuto=false;
						BoxStyle margin=cs.GetMarginBox(displayMode,box.FloatMode,ref marginAuto);
						
						float extraStyle=(
							border.Left+border.Right+
							padding.Left+padding.Right+
							margin.Left+margin.Right
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
		
		/// <summary>Focuses this element so it receives events such as keypresses.</summary>
		public void focus(){
			
			HtmlDocument doc=htmlDocument;
			
			if(doc.activeElement==this){
				return;
			}
			
			// Focus out first:
			FocusEvent fe=new FocusEvent("focusout");
			
			// It's trusted but doesn't bubble:
			fe.SetTrusted(false);
			fe.focusing=this;
			fe.relatedTarget=this;
			
			if(doc.activeElement!=null){
				if(!doc.activeElement.dispatchEvent(fe)){
					return;
				}	
			}
			
			// Focus in next:
			fe.Reset();
			fe.EventType="focusin";
			fe.relatedTarget=doc.activeElement;
			
			if(!dispatchEvent(fe)){
				return;
			}
			
			// Blur next:
			fe.Reset();
			fe.relatedTarget=null;
			fe.EventType="blur";
			
			PowerUI.Input.LastFocusedDocument=doc;
			
			if(doc.activeElement!=null){
				
				(doc.activeElement as HtmlElement).Unfocus(fe);
			}
			
			doc.activeElement=this;
			
			// Update local style:
			Style.Computed.RefreshLocal();
			
			#if MOBILE
			// Should we pop up the mobile keyboard?
			KeyboardMode mobile=OnShowMobileKeyboard();
			if(Input.HandleKeyboard(mobile)){
				Input.KeyboardText=value;
			}
			
			#endif
			
			// Reset so we can recycle it:
			fe.Reset();
			fe.EventType="focus";
			
			// Dispatch and check if it got cancelled:
			if(dispatchEvent(fe)){
				
				// Run the default for this element:
				OnFocusEvent(fe);
				
			}
			
		}
		
		/// <summary>Unfocuses this element.</summary>
		public void blur(){
			FocusEvent fe=new FocusEvent("blur");
			fe.SetTrusted(false);
			Unfocus(fe);
		}
		
		/// <summary>Unfocuses this element so it will no longer receive events like keypresses.</summary>
		private void Unfocus(FocusEvent fe){
			
			HtmlDocument doc=htmlDocument;
			
			if(doc.activeElement!=this){
				return;
			}
			
			doc.activeElement=null;
			
			// Update local style:
			Style.Computed.RefreshLocal();
			
			#if MOBILE
			// Attempt to hide the keyboard.
			Input.HandleKeyboard(null);
			#endif
			
			htmlDocument.window.Event=null;
			
			if(dispatchEvent(fe)){
				
				// Run the default:
				OnBlurEvent(fe);
				
			}
			
		}
		
		/// <summary>Called when a default scrollwheel event occurs (focused element).</summary>
		/// <param name="clickEvent">The event that represents the wheel scroll.</param>
		public virtual void OnWheelEvent(WheelEvent e){
			
		}
		
		/// <summary>Called when a default key press occurs.</summary>
		/// <param name="clickEvent">The event that represents the key press.</param>
		public virtual void OnKeyPress(KeyboardEvent pressEvent){
			
		}
		
		/// <summary>True if this element has some form of background applied to it.</summary>
		public bool HasBackground{
			get{
				return RenderData.HasBackground;
			}
		}
		
		/// <summary>Does this element get reset with the form?</summary>
		internal virtual bool IsFormResettable{
			get{
				return false;
			}
		}
		
		/// <summary>Does this element get submitted with the form?</summary>
		internal virtual bool IsFormSubmittable{
			get{
				return false;
			}
		}
		
		/// <summary>Does this element list in form.elements?</summary>
		internal virtual bool IsFormListed{
			get{
				return false;
			}
		}
		
		/// <summary>Can this element have a label?</summary>
		internal virtual bool IsFormLabelable{
			get{
				return false;
			}
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
		
		internal override void ResetVariable(string name){
			OnResetAllVariables();
			base.ResetVariable(name);
		}
		
		/// <summary>Requests all child elements to reload their &variables; if they have any.</summary>
		internal override void ResetAllVariables(){
			OnResetAllVariables();
			base.ResetAllVariables();
		}
		
		/// <summary>Called when the DOM changed.</summary>
		internal override void ChangedDOM(){
			
			// Request a layout:
			htmlDocument.RequestLayout();
			
		}
		
		/// <summary>Called when this element got added to the DOM.</summary>
		internal override void AddedToDOM(){
			
			HtmlDocument doc=htmlDocument;
			
			if(doc!=null){
				
				if(doc.AttributeIndex!=null){
					// Index element if needed:
					AddToAttributeLookups();
				}
				
			}
			
			// Update its css by telling it the parent changed.
			// This affects inherit, height/width etc.
			Style.Computed.ParentChanged();
			
			if(doc!=null){
				// Request a layout:
				doc.RequestLayout();
			}
			
		}
		
		/// <summary>Called when this element got removed from the DOM.</summary>
		internal override void RemovedFromDOM(){
			
			if(htmlDocument.AttributeIndex!=null){
				
				// Remove this from the DOM attribute cache:
				htmlDocument.RemoveCachedElement(this);
				
			}
			
			// Remove handler:
			OnRemovedFromDOM();
			
			// Let the style know we went offscreen:
			RenderableData renderable=RenderData;
			renderable.WentOffScreen();
			
			// Apply to all virtual elements:
			VirtualElements virts=renderable.Virtuals;
			
			if(virts!=null){
			
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
				
					// Remove it:
					kvp.Value.RemovedFromDOM();
					
				}
				
			}
			
			base.RemovedFromDOM();
			
			// Request a layout:
			htmlDocument.RequestLayout();
			
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
		
		/// <summary>Appends the given element defined as text.</summary>
		/// <param name="html">The html to append, e.g. "<div id='someNewElement'></div>".</param>
		/// <returns>The newly created element.</returns>
		public void appendChild(string html){
			append(html);
		}
		
		/// <summary>Replaces this element with the given html.</summary>
		public void replaceWith(string html){
			
			// Insert html before this:
			before(html);
			
			// remove this:
			remove();
			
		}
		
		/// <summary>Applies to iframes. The document contained in the iframe itself.</summary>
		public HtmlDocument contentDocument{
			get{
				return lastChild as HtmlDocument;
			}
		}
		
		/// <summary>Scrolls the element by the given values.</summary>
		/// <param name="x">The change in x pixels.</param>
		/// <param name="y">The change in y pixels.</param>
		public void scrollBy(float x,float y){
			if(x==0f && y==0f){
				return;
			}
			
			// Get the scroll value (if there is one):
			Css.Value scroll=Style.Computed.Scroll;
			
			if(scroll==null){
				scrollTo(x,y);
				return;
			}
			
			float top=(scroll[0]==null) ? 0f : scroll[0].GetDecimal(RenderData,null);
			float left=(scroll[1]==null) ? 0f : scroll[1].GetDecimal(RenderData,null);
			
			scrollTo(left+x,top+y);
			
		}
		
		/// <summary>Scrolls the element to the given exact values.</summary>
		/// <param name="x">The x offset in pixels.</param>
		/// <param name="y">The y offset in pixels.</param>
		public void scrollTo(float x,float y){
			
			// Straight through to CSS:
			Style.scrollLeft=x+"px";
			Style.scrollTop=y+"px";
			
		}
		
		/// <summary>Forces a layout to occur if one is required.
		/// You should almost never need to call this directly - it's only needed if you want to read the fully
		/// computed size of an element immediately after having updated its style.</summary>
		public void RequireLayout(){
			htmlDocument.Renderer.Layout();
		}
		
		/// <summary>The amount of pixels the content of this element is scrolled horizontally.</summary>
		public int scrollLeft{
			get{
				RequireLayout();
				
				// Get the scroll value (if there is one):
				Css.Value scroll=Style.Computed.Scroll;
				
				if(scroll==null || scroll[1]==null){
					return 0;
				}
				
				return scroll[1].GetInteger(RenderData,null);
				
			}
			set{
				scrollTo(value,scrollTop);
			}
		}
		
		/// <summary>The amount of pixels the content of this element is scrolled vertically.</summary>
		public int scrollTop{
			get{
				RequireLayout();
				
				// Get the scroll value (if there is one):
				Css.Value scroll=Style.Computed.Scroll;
				
				if(scroll==null || scroll[0]==null){
					return 0;
				}
				
				return scroll[0].GetInteger(RenderData,null);
				
			}
			set{
				scrollTo(scrollLeft,value);
			}
		}
		
		/// <summary>The height of the content inside this element.</summary>
		public int contentHeight{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.ContentHeight;
			}
		}
		
		/// <summary>The width of the content inside this element.</summary>
		public int contentWidth{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.ContentWidth;
			}
		}
		
		/// <summary>The height of this element.</summary>
		public int pixelHeight{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.Height;
			}
		}
		
		/// <summary>The width of this element.</summary>
		public int pixelWidth{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.Width;
			}
		}
		
		/// <summary>The height of this element.</summary>
		public int offsetHeight{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.Height;
			}
		}
		
		/// <summary>The width of this element.</summary>
		public int offsetWidth{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.Width;
			}
		}
		
		/// <summary>The parent that offsetTop and offsetLeft are relative to.</summary>
		public Element offsetParent{
			get{
				// Body or the root element always returns null:
				if(Tag=="body" || parentElement==null){
					return null;
				}
				
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				if(box==null || RenderData.Ancestor==null || box.PositionMode==PositionMode.Fixed){
					return null;
				}
				
				// We've got a precomputed property available:
				return RenderData.Ancestor.Node as Element;
			}
		}
		
		/// <summary>The top offset relative to the offsetParent.</summary>
		public int offsetTop{
			get{
				
				// Body element always returns 0:
				if(Tag=="body"){
					return 0;
				}
				
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.ParentOffsetTop;
			}
		}
		
		/// <summary>The left offset relative to the offsetParent.</summary>
		public int offsetLeft{
			get{
				
				// Body element always returns 0:
				if(Tag=="body"){
					return 0;
				}
				
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.ParentOffsetLeft;
			}
		}
		
		/// <summary>The width of the top border of an element in pixels.</summary>
		public int clientTop{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.Border.Top;
			}
		}
		
		/// <summary>The width of the left border of an element in pixels.</summary>
		public int clientLeft{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.Border.Left;
			}
		}
		
		/// <summary>The inner width of this element, (minus scrollbars).</summary>
		public int clientWidth{
			get{
				return scrollWidth;
			}
		}
		
		/// <summary>The inner height of this element, (minus scrollbars).</summary>
		public int clientHeight{
			get{
				return scrollHeight;
			}
		}
		
		/// <summary>The height of this element without margins or borders.</summary>
		public int scrollHeight{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.InnerHeight;
			}
		}
		
		/// <summary>The width of this element without margins or borders.</summary>
		public int scrollWidth{
			get{
				RequireLayout();
				LayoutBox box=Style.Computed.FirstBox;
				return (box==null)? 0 :(int)box.InnerWidth;
			}
		}
		
		public virtual bool Checked{
			get{
				return false;
			}
			set{}
		}
		
		/// <summary>The attributes of this element (DOM spec compliant mapping for Properties).</summary>
		public Dictionary<string,string> attributes{
			get{
				return Properties;
			}
		}
		
		/// <summary>The gameObject that this element is ultimately parented to.</summary>
		public GameObject rootGameObject{
			get{
				WorldUI wUI=worldUI;
				
				if(wUI==null){
					return UI.GUINode;
				}
				
				return wUI.gameObject;
			}
		}
		
		/// <summary>The world UI this element belongs to.</summary>
		public WorldUI worldUI{
			get{
				return htmlDocument.worldUI;
			}
		}
		
		/// <summary>Gets or sets the image from the background of this element.</summary>
		public Texture image{
			get{
				
				BackgroundImage img=RenderData.BGImage;
				
				if(img==null){
					return null;
				}
				
				if(img.Image==null){
					return null;
				}
				
				// Get the picture format:
				PictureFormat picture=img.Image.Contents as PictureFormat;
				
				if(picture==null){
					return null;
				}
				
				return picture.Image;
			}
			set{
				
				BackgroundImage img=RenderData.BGImage;
				
				if(value==null){
					if(img!=null){
						RenderData.BGImage=null;
						htmlDocument.RequestLayout();
					}
				}else{
					if(img==null){
						img=new BackgroundImage(RenderData);
						RenderData.BGImage=img;
					}
					img.UpdateImage(value);
				}
			}
		}
		
		/// <summary>Sets the given image as the background of this element.</summary>
		public void SetImage(ImageFormat value){
			
			BackgroundImage img=RenderData.BGImage;
			
			if(value==null){
				if(img!=null){
					RenderData.BGImage=null;
					htmlDocument.RequestLayout();
				}
			}else{
				if(img==null){
					img=new BackgroundImage(RenderData);
					RenderData.BGImage=img;
				}
				img.SetImage(new ImagePackage(value));
			}
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="duration">The time, in seconds, to take animating the properties.</param>
		/// <param name="cssTimeFunction">The timing function (CSS).
		/// Can be anything that's valid CSS; e.g. "steps(3)" or "ease". Note that "ease" is the default in CSS.</summary>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float duration,string cssTimeFunction){	
			
			// Load the CSS function:
			Css.Value value=Css.Value.Load(cssTimeFunction);
			
			Blaze.VectorPath timeFunc=null;
			
			if(value!=null){
				
				// Get as a path:
				timeFunc=value.GetPath(null,null);
				
			}
			
			return new UIAnimation(this,css,duration,timeFunc);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="duration">The time, in seconds, to take animating the properties.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float duration){
			return new UIAnimation(this,css,duration,null);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="duration">The time, in seconds, to take animating the properties.</param>
		/// <param name="timeFunction">A (0,0) to (1,1) graph which describes the timing function.
		/// Linear is the default.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float duration,Blaze.VectorPath timeFunction){
			return new UIAnimation(this,css,duration,timeFunction);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <param name="timeToAccelAndDecel">The time, in seconds, to take accelerating and decelerating.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime,float timeToAccelAndDecel){
			return animate(css,constantSpeedTime,timeToAccelAndDecel,timeToAccelAndDecel);
		}
		
		/// <summary>Animates css properties on this element.</summary>
		/// <param name="css">A set of target css properties, e.g. "rotate-x:45deg;scale-y:110%;".</param>
		/// <param name="constantSpeedTime">The time, in seconds, to take animating the properties at a constant speed.</param>
		/// <param name="timeToAccelerate">The time, in seconds, to take accelerating.</param>
		/// <param name="timeToDecelerate">The time, in seconds, to take decelerating.</param>
		/// <returns>An animation instance which can be used to track progress.</returns>
		public UIAnimation animate(string css,float constantSpeedTime,float timeToAccelerate,float timeToDecelerate){
			
			// Increase total duration:
			constantSpeedTime+=timeToAccelerate+timeToDecelerate;
			
			// The graph is an ease curve (cubic bezier):
			Blaze.VectorPath path=new Blaze.VectorPath();
			
			path.CurveTo(
				
				// First control point (on the left):
				timeToAccelerate/constantSpeedTime,0f,
				
				// Second control point (on the right):
				1f-(timeToDecelerate/constantSpeedTime),1f,
				
				// End point:
				1f,1f
				
			);
			
			return new UIAnimation(this,css,constantSpeedTime,path);
		}
		
		/// <summary>Gets or sets if this element is focused.</summary>
		public bool focused{
			
			get{
				return (htmlDocument.activeElement==this);
			}
			set{
				if(value==true){
					focus();
				}else{
					blur();
				}
			}
		}
		
		/// <summary>This elements tab index.</summary>
		public int tabIndex{
			get{
				string value=getAttribute("tabindex");
				int v;
				
				if(value==null || !int.TryParse(value, out v)){
					return -1;
				}
				
				return v;
			}
			set{
				setAttribute("tabindex", value.ToString());
			}
		}
		
        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <returns>The style declaration describing the element.</returns>
        public Css.ComputedStyle getComputedStyle(){
			return getComputedStyle(null);
        }
		
        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <param name="pseudo">The optional pseudo selector to use.</param>
        /// <returns>The style declaration describing the element.</returns>
        public Css.ComputedStyle getComputedStyle(string pseudo){
			
			if(string.IsNullOrEmpty(pseudo)){
				return Style.Computed;
			}
			
			// Get the particular pseudo-element:
			pseudo=pseudo.ToLower().Trim();
			
			// Check if it contains :
			int index=pseudo.LastIndexOf(':');
			
			if(index!=-1){
				// Chop from there:
				pseudo=pseudo.Substring(index+1);
			}
			
			IRenderableNode el=Style.Computed.GetVirtualChild(pseudo) as IRenderableNode;
			
			if(el==null){
				return null;
			}
			
			return el.ComputedStyle;
			
		}
		
		/// <summary>Gets the computed style of this element.</summary>
		public Css.ComputedStyle computedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Gets the style of this element.</summary>
		public override Css.ElementStyle style{
			get{
				return Style;
			}
		}
		
	}
	
}