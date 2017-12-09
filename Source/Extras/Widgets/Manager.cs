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
	
	/// <summary>
	/// A widget system manager. There's one per document (available as document.widgets). Handles e.g. popping open a widget.
	/// </summary>
	
	public class Manager{
		
		/// <summary>The document that is being managed.</summary>
		public HtmlDocument Document;
		/// <summary>The group of open widgets.</summary>
		public WidgetGroup Widgets;
		
		
		public Manager(HtmlDocument doc){
			Document=doc;
			Widgets=new WidgetGroup(this);
		}
		
		/// <summary>Builds a set of globals from an array of data (of the form 'key',value,'key',value..).</summary>
		public static Dictionary<string,object> buildGlobals(object[] data){
			
			if(data==null || data.Length==0){
				return null;
			}
			
			if(data.Length==1){
				
				if(data[0] is Dictionary<string,object>){
					return data[0] as Dictionary<string,object>;
				}
				
			}
			
			// Must be a multiple of 2:
			if((data.Length % 2)!=0){
				throw new Exception("Didn't recognise this as a valid globals set.");
			}
			
			// # of globals:
			int count=data.Length/2;
			
			// Create the set:
			Dictionary<string,object> result=new Dictionary<string,object>(count);
			
			// Alternates between key and value:
			int index=0;
			
			for(int i=0;i<count;i++){
				
				// Get k/v:
				string key=data[index++] as string;
				object value=data[index++] as object;
				
				if(key==null){
					continue;
				}
				
				// Add it:
				result[key]=value;
				
			}
			
			return result;
		}
		
		/// <summary>Gets a widget by ID. Null if out of range.</summary>
		public Widget get(int id){
			return this[id];
		}
		
		/// <summary>Gets a widget by ID. Null if out of range.</summary>
		public Widget this[int id]{
			get{
				
				if(id<0 || id>=Widgets.Widgets.Count){
					return null;
				}
				
				// Get it:
				return Widgets.Widgets[id];
			}
		}
		
		/// <summary>Gets the widget of the given type pointing at the given URL.</summary>
		public Widget get(string type,string url){
			return Widgets.get(type,url);
		}
		
		/// <summary>Closes an open widget or opens it if it wasn't already.</summary>
		public Widget cycle(string type,string url,Dictionary<string,object> globals){
			return Widgets.cycle(type,url,globals);
		}
		
		/// <summary>Closes an open widget or opens it if it wasn't already.
		/// globalData alternates between a string (key) and a value (object).
		/// I.e 'key',value,'key2',value..</summary>
		public Widget cycle(string type,string url,params object[] globalData){
			return Widgets.cycle(type,url,buildGlobals(globalData));
		}
		
		/// <summary>Loads a widget of the given type.</summary>
		public Promise load(string typeName){
			return load(typeName,null,(Dictionary<string,object>)null);
		}
		
		/// <summary>Opens a widget optionally with globals. Of the form 'key',value,'key2',value..
		/// returning a promise which runs when the widgets 'load' event occurs.</summary>
		public Promise load(string typeName,string url,params object[] globalData){
			return Widgets.load(typeName,url,Manager.buildGlobals(globalData));
		}
		
		/// <summary>Opens a widget, returning a promise which runs when the widgets 'load' event occurs.</summary>
		public Promise load(string typeName,string url,Dictionary<string,object> globals){
			return Widgets.load(typeName,url,globals);
		}
		
		/// <summary>Opens a widget of the given type and points it at the given URL.
		/// globalData alternates between a string (key) and a value (object).
		/// I.e 'key',value,'key2',value..</summary>
		public Widget open(string type,string url,params object[] globalData){
			return Widgets.open(type,url,buildGlobals(globalData));
		}
		
		/// <summary>Creates a new widget of the given type and points it at the given URL.
		/// Creates a set of global variables available to the JS in the widget.</summary>
		public Widget open(string type,string url,Dictionary<string,object> globals){
			return Widgets.open(type,url,globals);
		}
		
		/// <summary>Closes a widget. Just a convenience version of widget.close();</summary>
		public void close(string type,string url){
			
			// Try getting it:
			Widget w=get(type,url);
			
			if(w!=null){
				w.close();
			}
			
		}
		
		/// <summary>True if the named widget template is available.</summary>
		public bool has(string template){
			return widgetTypes!=null && widgetTypes.ContainsKey(template);
		}
		
		/// <summary>The available widget template types.</summary>
		public static Dictionary<string,Type> widgetTypes;
		
		/// <summary>Adds the given type as an available widget type.</summary>
		public static void Add(Type type){
			
			if(widgetTypes==null){
				// widgetTypes the set now:
				widgetTypes=new Dictionary<string,Type>();
			}
			
			// Get the name attribute from it (don't inherit):
			#if NETFX_CORE
			TagName tagName=type.GetTypeInfo().GetCustomAttribute(typeof(TagName),false) as TagName;
			#else
			TagName tagName=Attribute.GetCustomAttribute(type,typeof(TagName),false) as TagName;
			#endif
			
			if(tagName==null){
				// Nope!
				return;
			}
			
			string tag=tagName.Tags;
			
			if(!string.IsNullOrEmpty(tag)){
				
				// Add now:
				widgetTypes[tag]=type;
				
			}
			
		}
		
	}
	
}

namespace PowerUI{

	public partial class HtmlDocument{
		
		/// <summary>Instance of a widget manager. May be null.</summary>
		private Widgets.Manager widgets_;
		
		/// <summary>The document.widgets API. Read only.</summary>
		[Obsolete("This API has been renamed to 'document.widgets' to better represent what it does.")]
		public Widgets.Manager sparkWindows{
			get{
				return widgets;
			}
		}
		
		/// <summary>The document.widgets API. Read only.</summary>
		public Widgets.Manager widgets{
			get{
				
				if(widgets_==null){
					widgets_=new Widgets.Manager(this);
				}
				
				return widgets_;
			}
		}
		
	}
	
}