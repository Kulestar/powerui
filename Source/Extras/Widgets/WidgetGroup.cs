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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using PowerUI;
using Dom;


namespace Widgets{
	
	/// <summary>A delegate used just before a widget loads.</summary.
	public delegate void WidgetDelegate(Widget w);
	
	/// <summary>
	/// A group of widgets.
	/// </summary>
	public class WidgetGroup:EventTarget{
		
		/// <summary>The widget manager.</summary>
		public Manager Manager;
		/// <summary>The parent group, if any.</summary>
		public WidgetGroup Parent;
		/// <summary>An element to parent child widgets to.</summary>
		private HtmlElement WidgetHostElement_;
		/// <summary>All active widgets, sorted by depth.</summary>
		public List<Widget> Widgets=new List<Widget>();
		
		
		/// <summary>Creates a widget group.</summary>
		public WidgetGroup(Manager manager){
			Manager=manager;
		}
		
		public WidgetGroup(){}
		
		/// <summary>The doc that hosts child widgets.</summary>
		public HtmlDocument WidgetHostDocument{
			get{
				
				Widget w=this as Widget;
				
				if(w==null){
					
					// Use the manager:
					return Manager.Document;
					
				}
				
				// Use the widget's content doc:
				if(w.contentDocument==null){
					
					// Go up a level:
					return Parent.WidgetHostDocument;
					
				}
				
				return w.contentDocument;
				
			}
		}
		
		/// <summary>An element to parent child widgets to.</summary>
		public HtmlElement WidgetHostElement{
			get{
				
				if(WidgetHostElement_==null){
					
					// Get the host doc:
					HtmlDocument hostDoc=WidgetHostDocument;
					
					// Create a 100% * 100% fixed div inside it:
					HtmlElement e=hostDoc.createElement("div") as HtmlElement;
					e.className="spark-widget-host";
					e.style.width="100%";
					e.style.height="100%";
					e.style.position="fixed";
					hostDoc.html.appendChild(e);
					WidgetHostElement_=e;
					
				}
				
				return WidgetHostElement_;
			}
		}
		
		/// <summary>Gets a widget of the given type and pointing at the given URL.</summary>
		/// <returns>Null if not found.</returns>
		public Widget get(string type,string url){
			
			// For each one..
			for(int i=Widgets.Count-1;i>=0;i--){
				
				Widget w=Widgets[i];
				
				// Match?
				if(w.Type==type && (url==null || w.Location==url)){
					return w;
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Closes a widget. Just a convenience version of widget.close();</summary>
		public void close(string type,string url){
			
			// Try getting it:
			Widget w=get(type,url);
			
			if(w!=null){
				w.close();
			}
			
		}
		
		/// <summary>Closes an open widget or opens it if it wasn't already 
		/// optionally with globals. Of the form 'key',value,'key2',value..</summary>
		public Widget cycle(string type,string url,params object[] globalData){
			return cycle(type,url,Manager.buildGlobals(globalData));
		}
		
		/// <summary>Closes an open widget or opens it if it wasn't already.</summary>
		public Widget cycle(string type,string url,Dictionary<string,object> globals){
			
			// Try getting it:
			Widget w=get(type,url);
			
			if(w==null){
				
				// Open it:
				w=open(type,url,globals);
				return w;
				
			}else if(w.HidBy!=null){
				
				// It's actually hidden - unhide it (and hide the visible one).
				
				// Remove it from the linked list of hidden widgets:
				if(w.HidWidget!=null){
					w.HidWidget.HidBy=w.HidBy;
				}
				
				w.HidBy.HidWidget=w.HidWidget;
				
				// Make it visible:
				w.Visibility(true,null);
				
				// Next, hide the visible one:
				Widget visible=w.GetVisibleWidget();
				visible.Visibility(false,w);
				
				return w;
				
			}
			
			// Close it:
			w.close();
			
			return null;
			
		}
		
		/// <summary>Loads a widget of the given type.</summary>
		public Promise load(string typeName){
			return load(typeName,null,(Dictionary<string,object>)null);
		}
		
		/// <summary>Opens a widget optionally with globals. Of the form 'key',value,'key2',value..
		/// returning a promise which runs when the widgets 'load' event occurs.</summary>
		public Promise load(string typeName,string url,params object[] globalData){
			return load(typeName,url,Manager.buildGlobals(globalData));
		}
		
		/// <summary>Opens a widget, returning a promise which runs when the widgets 'load' event occurs.</summary>
		public Promise load(string typeName,string url,Dictionary<string,object> globals){
			
			// Add an event listener just before Load is invoked:
			Promise p=new Promise();
			
			open(
				typeName,
				url,
				delegate(Widget w){
					
					// Add an event cb:
					if(w==null){
						
						// Rejected:
						p.reject("Widget '"+typeName+"' is missing.");
						
					}else{
						
						w.addEventListener("load",delegate(UIEvent e){
							
							// Ok!
							p.resolve(w);
							
						});
						
					}
				},
				globals
			);
			
			return p;
		}
		
		/// <summary>Opens a widget optionally with globals. Of the form 'key',value,'key2',value..</summary>
		public Widget open(string typeName,string url,params object[] globalData){
			return open(typeName,url,null,Manager.buildGlobals(globalData));
		}
		
		/// <summary>Opens a widget.</summary>
		public Widget open(string typeName,string url,Dictionary<string,object> globals){
			return open(typeName,url,null,globals);
		}
		
		/// <summary>Opens a widget.</summary>
		public Widget open(string typeName,string url,WidgetDelegate preload,Dictionary<string,object> globals){
			
			if(Manager.widgetTypes==null){
				
				// Load the widgets now!
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(Widgets.Widget),
					delegate(Type t){
						// Add it as an available widget:
						Manager.Add(t);
					}
				);
				
			}
			
			Type type;
			if(!Manager.widgetTypes.TryGetValue(typeName,out type)){
				
				UnityEngine.Debug.Log("Warning: Requested to open a widget called '"+typeName+"' but it doesn't exist.");
				
				if(preload!=null){
					// Invoke the load method:
					preload(null);
				}
				
				return null;
			}
			
			// Get existing:
			Widget same=get(typeName,null);
			
			// Get stacking behaviour:
			StackMode stacking=StackMode.Close;
			
			if(same!=null){
				stacking=same.StackMode;
			}
			
			object stackModeObj;
			if(globals!=null && globals.TryGetValue("-spark-stack-mode",out stackModeObj)){
				
				string stackMode=stackModeObj.ToString().Trim().ToLower();
				
				if(stackMode=="hide"){
					stacking=StackMode.Hide;
				}else if(stackMode=="close"){
					stacking=StackMode.Close;
				}else if(stackMode=="hijack"){
					stacking=StackMode.Hijack;
				}else{
					stacking=StackMode.Over;
				}
				
			}
			
			if(stacking==StackMode.Hijack){
				
				// Hijack an existing widget! Just load straight into it but clear its event handlers:
				same.RunLoad=true;
				same.ClearEvents();
				
				if(preload!=null){
					preload(same);
				}
				
				same.Load(url,globals);
				same.TryLoadEvent(globals);
				
				return same;
			}
			
			// instance it now:
			Widget w=Activator.CreateInstance(type) as Widget;
			
			if(w==null){
				return null;
			}
			
			if(stacking==StackMode.Hide){
				
				// Hides any widget of the same type.
				
				if(same!=null){
					
					// Make sure it's actually the visible one:
					same=same.GetVisibleWidget();
					
					same.Visibility(false,w);
					w.HidWidget=same;
				}
				
			}else if(stacking==StackMode.Close && same!=null){
				
				// Close all widgets of the same type.
				
				// For each one..
				for(int i=Widgets.Count-1;i>=0;i--){
					
					same=Widgets[i];
					
					// Match?
					if(same.Type==typeName){
						same.close();
					}
					
				}
				
			}
			
			// Apply type:
			w.Type=typeName;
			
			if(preload!=null){
				preload(w);
			}
			
			// Add now:
			SetupWidget(w,url,globals);
			
			return w;
			
		}
		
		/// <summary>Removes the given widget.</summary>
		internal void Remove(Widget w){
			
			if(Widgets[w.Index]!=w){
				return;
			}
			
			// Move everything else over:
			for(int i=w.Index+1;i<Widgets.Count;i++){
				Widgets[i].Index--;
			}
			
			// Remove at its index now:
			Widgets.RemoveAt(w.Index);
			
		}
		
		/// <summary>Inserts the given widget into the list of open widgets and begins loading it.</summary>
		private void SetupWidget(Widget w,string url,Dictionary<string,object> globals){
			
			// Get and apply depth:
			int newWidgetDepth=w.Depth;
			w.ActiveDepth=newWidgetDepth;
			w.Manager=Manager;
			w.Parent=this;
			
			// Add to the end:
			Widgets.Add(w);
			w.Index=Widgets.Count-1;
			
			// Shuffle it forward if it's a lower depth (but don't shuffle forward for ==):
			
			// For each one..
			for(int i=Widgets.Count-1;i>0;i--){
				
				Widget current=Widgets[i-1];
				int depth=current.ActiveDepth;
				
				if(newWidgetDepth<depth){
					
					// Shuffle it forward:
					current.Index=i;
					Widgets[i]=current;
					Widgets[i-1]=w;
					w.Index=i-1;
					
				}else{
					break;
				}
				
			}
			
			// Next, load its content:
			w.Load(url,globals);
			w.TryLoadEvent(globals);
			
		}
		
	}
	
}