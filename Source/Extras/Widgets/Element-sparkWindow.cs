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
using Widgets;


namespace PowerUI{
	
	public partial class HtmlElement{
		
		/// <summary>Gets the spark widget that holds this element.</summary>
		public Widgets.Widget sparkWindow{
			get{
				return widget;
			}
		}
		
		/// <summary>Gets the widget that holds this element.</summary>
		public Widgets.Widget widget{
			get{
				// Go up the dom looking for the first element with a '-spark-widget-id' attribute:
				string widgetID=getAttribute("-spark-widget-id");
				
				if(widgetID==null){
					
					Dom.Node parent=parentNode;
					
					// If the parent is a document, we may need to skip over it (if we're inside an iframe).
					while(parent is Dom.Document){
						
						// Skip:
						parent=parent.parentNode;
						
					}
					
					// Get it as a HTML element:
					HtmlElement parentHtml=parent as HtmlElement;
					
					if(parentHtml==null){
						return null;
					}
					
					return parentHtml.widget;
					
				}
				
				// Got a widget!
				int id;
				if(int.TryParse(widgetID,out id)){
					
					// Get it by ID:
					return htmlDocument.widgets[id];
					
				}
				
				return null;
			}
		}
		
	}
	
}

namespace Dom{
	
	public partial class Event{
		
		/// <summary>The widget that the *currentTarget* of the event is in.</summary>
		public Widgets.Widget targetWidget{
			get{
				return (currentTarget as PowerUI.HtmlElement).widget;
			}
		}
		
	}
	
}