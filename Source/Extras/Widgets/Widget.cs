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
using PowerUI;
using System.Collections;
using System.Collections.Generic;


namespace Widgets{
	
	/// <summary>
	/// An instance of an open widget.
	/// </summary>
	
	[Values.Preserve]
	public partial class Widget : WidgetGroup{
		
		/// <summary>
		/// Closes the widget that the given event originated from.
		/// </summary>
		[Values.Preserve]
		public static void CloseThis(UIEvent e){
			
			// Get the widget and close it:
			e.targetWidget.close();
			
		}
		
		/// <summary>True if the load event should be triggered.</summary>
		public bool RunLoad=true;
		/// <summary>Index in managers array.</summary>
		private int Index_; 
		/// <summary>Index in managers array.</summary>
		public int Index{
			get{
				return Index_;
			}
			set{
				Index_=value;
				
				// Update the attribute:
				if(element!=null){
					element.setAttribute("-spark-widget-id", value.ToString());
				}
				
			}
		}
		/// <summary>The widget type. E.g. "floating".</summary>
		public string Type;
		/// <summary>This widgets actual depth.</summary>
		public int ActiveDepth;
		/// <summary>The location of this widget.</summary>
		public string Location;
		/// <summary>The root element which contains this widget. All templates must have one root only (watch out for comments and text!).</summary>
		public HtmlElement element;
		/// <summary>The iframe. May be null.</summary>
		public HtmlElement frame;
		/// <summary>If this widget is in the 'hide other' stacking mode, this is the widget it hid.</summary>
		public Widget HidWidget;
		/// <summary>If this widget is hidden, the one that hid it.</summary>
		public Widget HidBy;
		/// <summary>The document to load content into. May be null.</summary>
		public HtmlDocument contentDocument;
		
		
		/// <summary>The document this widget is in.</summary>
		public HtmlDocument document{
			get{
				return Manager.Document;
			}
		}
		
		/// <summary>Closes the widget and its kids.</summary>
		public void close(){
			close(true);
		}
		
		/// <summary>Closes the widget and its kids, optionally avoiding removing the actual element.</summary>
		private void close(bool removeFrame){
			
			// Close all kids:
			foreach(Widget w in Widgets){
				w.close(false);
			}
			
			if(element==null){
				return;
			}
			
			if(removeFrame){
				
				// Run the close events:
				OnClose();
				trigger("close");
				trigger("animatehide");
				
				if(HidWidget!=null){
					// Display hid widget again:
					HidWidget.Visibility(true,null);
					HidWidget=null;
				}
				
				// Clear the doc:
				if(contentDocument!=null){
					contentDocument.clear();
				}
				
				// Destroy the element:
				element.parentNode.removeChild(element);
				
				// Remove from parent:
				Parent.Remove(this);
				
				element=null;
				
			}
			
		}
		
		/// <summary>The widget at the back and same depth as this one.</summary>
		public Widget backSameDepth{
			get{
				Widget current=previousSameDepth;
				
				while(current!=null){
					Widget next=current.previousSameDepth;
					
					if(next==null){
						return current;
					}
					
					current=next;
				}
				
				return this;
			}
		}
		
		/// <summary>The widget before this one (further back) of the same type.</summary>
		public Widget previousSameDepth{
			get{
				
				if(Index<=0){
					return null;
				}
				
				Widget w=Parent.Widgets[Index-1];
				
				if(w.ActiveDepth!=ActiveDepth){
					return null;
				}
				
				return w;
			}
		}
		
		/// <summary>The widget at the front and same depth as this one.</summary>
		public Widget frontSameDepth{
			get{
				Widget current=nextSameDepth;
				
				while(current!=null){
					Widget next=current.nextSameDepth;
					
					if(next==null){
						return current;
					}
					
					current=next;
				}
				
				return this;
			}
		}
		
		/// <summary>The widget after this one (further forward) of the same type.</summary>
		public Widget nextSameDepth{
			get{
				
				if(Index>=Parent.Widgets.Count){
					return null;
				}
				
				Widget w=Parent.Widgets[Index+1];
				
				if(w.ActiveDepth!=ActiveDepth){
					return null;
				}
				
				return w;
			}
		}
		
		/// <summary>Sends this widget to the back.</summary>
		public void sendBackward(){
			
			// Insert element before the one at the back:
			Widget back=previousSameDepth;
			
			if(back!=this){
				element.parentNode.insertBefore(element,back.element);
			}
			
		}
		
		/// <summary>Sends this widget to the back.</summary>
		public void sendToBack(){
			
			// Insert element before the one at the back:
			Widget back=backSameDepth;
			
			if(back!=this){
				element.parentNode.insertBefore(element,back.element);
			}
			
		}
		
		/// <summary>Moves this widget forward one place.</summary>
		public void bringForward(){
			
			// Insert element after the one in front:
			Widget front=nextSameDepth;
			
			if(front!=this){
				element.parentNode.insertAfter(element,front.element);
			}
			
		}
		
		/// <summary>Brings this widget to the front.</summary>
		public void bringToFront(){
			
			// Insert element after the one at the front:
			Widget front=frontSameDepth;
			
			if(front!=this){
				element.parentNode.insertAfter(element,front.element);
			}
			
		}
		
		/// <summary>Hides/ shows the widget (without actually closing it).</summary>
		public void Visibility(bool visible,Widget hidBy){
			
			if(element==null){
				return;
			}
			
			HidBy=hidBy;
			element.style.display=visible?"block":"none";
			
		}
		
		/// <summary>True if these widgets stack.</summary>
		public virtual StackMode StackMode{
			get{
				return StackMode.Close;
			}
		}
		
		/// <summary>The depth that this type of widget lives at.</summary>
		public virtual int Depth{
			get{
				return 0;
			}
		}
		
		/// <summary>An element with the '-spark-title' attribute.</summary>
		public HtmlElement TitleElement{
			get{
				return element.getElementByAttribute("-spark-title",null) as HtmlElement;
			}
		}
		
		/// <summary>Handles events on the widget itself.</summary>
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(bubblePhase){
				OnEvent(e);
			}
			
			return base.HandleLocalEvent(e,bubblePhase);
		}
		
		/// <summary>Called when the widget receives an event.</summary>
		protected virtual void OnEvent(Dom.Event e){}
		
		/// <summary>Called when the widget is closing.</summary>
		protected virtual void OnClose(){}
		
		/// <summary>Navigates the widget to the given URL. Should only be used once; 
		/// close and open another widget (or use links inside the iframe).</summary>
		public virtual void Goto(string url,Dictionary<string,object> globals){
			
			// Set location:
			Location=url;
			
			if(contentDocument==null){
				
				// Doesn't navigate.
				return;
				
			}
			
			// Use an event which runs between the clear and load:
			contentDocument.AfterClearBeforeSet=delegate(Dom.Event acs){
				
				// Hook up title change:
				contentDocument.ontitlechange=delegate(Dom.Event e){
					
					// Set the 'title' element, if we have one:
					HtmlElement t=TitleElement;
					
					if(t!=null){
						t.innerHTML=contentDocument.title;
					}
					
				};
				
				// Apply globals to the script engine when it's ready:
				if(globals!=null){
				
					contentDocument.addEventListener("scriptenginestart",delegate(Dom.Event e){
					
						foreach(KeyValuePair<string,object> kvp in globals){
							
							// Skip if it starts with -spark-
							if(kvp.Key.StartsWith("-spark-")){
								continue;
							}
							
							// Set it:
							contentDocument.setJsVariable(kvp.Key,kvp.Value);
							
						}
						
					});
					
				}
				
			};
			
			// Run the open events:
			trigger("open");
			trigger("animateshow");
			
			// Create the location (relative to basepath by default):
			Dom.Location loc=new Dom.Location(url,document.basepath);
			
			// Navigate CD now:
			contentDocument.location=loc;
			
		}
		
		/// <summary>Triggers a 'widget{name}' event in the root document.</summary>
		public void trigger(string name){
			
			// Create the event:
			WidgetEvent e=new WidgetEvent("widget"+name,null);
			e.SetTrusted();
			
			// Trigger it now:
			document.dispatchEvent(e);
			
			// On this object (which is an event target):
			dispatchEvent(e);
			
		}
		
		/// <summary>Triggers a '{name}' event on the widget itself, and optionally on the source element.</summary>
		public void trigger(string name,Dictionary<string,object> globals){
			
			// Create the event:
			Dom.Event e=document.createEvent("uievent",name);
			e.SetTrusted();
			
			if(element!=null){
				// Trigger it now on the element:
				element.dispatchEvent(e);
			}
			
			// On this object (which is an event target):
			dispatchEvent(e);
			
			// Try on the original anchor element too:
			HtmlElement source=GetAnchor(globals);
			
			if(source!=null){
				
				// Trigger there too:
				source.dispatchEvent(e);
				
			}
			
		}
		
		/// <summary>The anchor element that triggered a widget to open. Null if there wasn't one.</summary>
		public HtmlElement GetAnchor(Dictionary<string,object> globals){
			
			return Get<HtmlElement>("-spark-anchor",globals);
			
		}
		
		/// <summary>When a widget hides another, it may result in a linked list of hidden widgets.
		/// This essentially finds the front of the linked list.</summary>
		public Widget GetVisibleWidget(){
			
			Widget current=HidBy;
			
			while(current!=null){
				Widget next=current.HidBy;
				
				if(next==null){
					return current;
				}
				
				current=next;
			}
			
			return this;
			
		}
		
		/// <summary>Gets a global of the given name as a colour.</summary>
		public UnityEngine.Color GetColour(string name,Dictionary<string,object> globals,UnityEngine.Color defaultValue){
			
			// Get the raw value:
			object value=Get<object>(name,globals);
			
			UnityEngine.Color result=defaultValue;
			
			if(value!=null){
				
				if(value is string){
					
					// Map it to an actual colour:
					result=Css.ColourMap.GetColour((string)value);
					
				}else if(value is UnityEngine.Color){
					
					result=(UnityEngine.Color)value;
					
				}else if(value is UnityEngine.Color32){
					
					result=(UnityEngine.Color32)value;
					
				}else{
					
					Ignored(name,"colour");
					
				}
				
			}
			
			return result;
			
		}
		
		/// <summary>Gets a global of the given name as an integer.</summary>
		public int GetInteger(string name,Dictionary<string,object> globals,int defaultValue){
			return (int)GetDecimal(name,globals,defaultValue);
		}
		
		/// <summary>Gets a global of the given name as a decimal.</summary>
		public double GetDecimal(string name,Dictionary<string,object> globals,double defaultValue){
			
			object value=Get<object>(name,globals);
			
			double result=defaultValue;
		
			if( value!=null ){
				
				if(value is string){
					
					// Parse it as a CSS value:
					Css.Value unit=Css.Value.Load((string)value);
					
					// Get as a decimal:
					result=unit.GetDecimal(null,null);
					
				}else if(value is int){
					
					result=(double)((int)value);
					
				}else if(value is float){
					
					result=(double)((float)value);
					
				}else if(value is double){
					
					result=(double)value;
					
				}else{
					
					Ignored(name,"number");
					
				}
				
			}
			
			return result;
			
		}
		
		/// <summary>Called when a widget parameter value was ignored.</summary>
		private void Ignored(string name,string type){
			
			Dom.Log.Add("Warning: Ignored a widget parameter called '"+name+"' - expected some kind of "+type+".");
			
		}
		
		/// <summary>Gets a global of the given name.</summary>
		public T Get<T>(string name,Dictionary<string,object> globals){
			
			object value;
			if(globals!=null && globals.TryGetValue(name.Trim().ToLower(),out value)){
				
				return (T)value;
				
			}
			
			// Not found.
			return default(T);
			
		}
		
		/// <summary>Adds style if it was required.</summary>
		protected void AddStyle(){
			
			string stylePath="resources://"+Type+"-style.html";
			HtmlDocument hostDoc=Parent.WidgetHostDocument;
			
			if(hostDoc.GetStyle(stylePath)==null){
				
				// Create a link tag:
				HtmlElement he=hostDoc.createElement("link") as HtmlElement;
				
				// Setup (causes it to load now):
				he.setAttribute("type", "text/css");
				he.setAttribute("href", stylePath);
				
				// Append it (not actually required):
				// element.appendChild(he);
				
			}
			
		}
		
		/// <summary>Writes the widgets HTML now. Collects element and optionally an iframe.</summary>
		public void SetHtml(string html){
			
			if(element!=null){
				// Remove element:
				element.parentNode.removeChild(element);
			}
			
			// Either use before() or direct insert:
			int domIndex=0;
			
			if(Index!=(Parent.Widgets.Count-1)){
				
				// Insert at the child index of the widget after this one:
				Widget afterThis=Parent.Widgets[Index+1];
				domIndex=afterThis.element.childIndex;
				
			}
			
			// Ok! Get the host:
			HtmlElement widgetHost=Parent.WidgetHostElement;
			
			// Insert now!
			widgetHost.insertInnerHTML(domIndex,html);
			
			// Grab the child at 'index':
			element=widgetHost.childNodes[domIndex] as HtmlElement;
			
			if(element==null){
				// Text or comment template error.
				throw new Exception("The widget template for '"+Type+"' starts with e.g. text/ a comment.");
			}
			
			// Search for an iframe (optional):
			frame=element.getElementByTagName("iframe") as HtmlElement;
			
			if(frame!=null){
				contentDocument=frame.contentDocument;
			}
			
			// Update the attribute:
			element.setAttribute("-spark-widget-id", Index.ToString());
			
		}
		
		/// <summary>Attempts to run the load event.</summary>
		public void TryLoadEvent(Dictionary<string,object> globals){
			
			if(RunLoad){
				LoadEvent(globals);
			}
			
		}
		
		/// <summary>Triggers a load event.</summary>
		protected void LoadEvent(Dictionary<string,object> globals){
			
			// Done! Trigger a 'load' event.
			// It will run on element (the widget itself), the Widget object and 
			// (if there is one), the original anchor tag.
			trigger("load",globals);
			
		}
		
		/// <summary>Loads the contents of this widget now.</summary>
		public virtual void Load(string url,Dictionary<string,object> globals){
			
			// Get the template:
			DataPackage package=new DataPackage("resources://"+Type+"-template.html",null);
			
			package.onload=delegate(UIEvent e){
				
				// Write the HTML:
				SetHtml(package.responseText);
				
				// Append style if it was required:
				AddStyle();
				
				// Load now:
				Goto(url,globals);
				
			};
			
			package.onerror=delegate(UIEvent e){
				
				// Error!
				Dom.Log.Add("Widget template for '"+Type+"' was not found. Tried "+package.location);
				
			};
			
			// Send now:
			package.send();
			
		}
		
	}
	
}