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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerUI;
using Dom;
using Css;


namespace ContextMenus{
	
	/// <summary>
	/// A group of options.
	/// </summary>
	
	public class OptionList{
		
		/// <summary>Used when an option is triggered (usually by an onclick attribute).</summary>
		[Values.Preserve]
		public static void ResolveOptionFromClick(Dom.Event e){
			
			// Get the src element:
			HtmlElement src=(e.srcElement as HtmlElement);
			
			// Get the optindex:
			int index;
			
			if(!int.TryParse(e.srcElement.getAttribute("optindex"),out index)){
				return;
			}
			
			// Resolve the option from just an index and a document ref:
			Option option=ResolveOption(index,src.widget);
			
			if(option!=null){
				
				// Set element which may be useful for submenu's:
				option.buttonElement=src;
				
				// Run it!
				option.run();
				
			}
			
		}
		
		/// <summary>Resolves an option from a context menu widget's document.</summary>
		public static Option ResolveOption(int index,Widgets.Widget widget){
			
			// These widgets are always 'ContextMenuWidget' objects.
			// They also only ever display one list - submenus are separate widgets.
			OptionList list=(widget as ContextMenuWidget).List;
			
			if(list==null){
				return null;
			}
			
			// Awesome - we can now get the actual option instance:
			return list[index];
			
		}
		
		/// <summary>The 3D range an option can work from.</summary>
		public float range;
		/// <summary>The list that this option belongs to.</summary>
		public OptionList parent;
		/// <summary>The source trigger.</summary>
		public IEventTarget trigger;
		/// <summary>A visible widget containing the list.</summary>
		public Widgets.Widget widget;
		/// <summary>The element that this is a list for. Related to trigger.</summary>
		public Element triggerElement{
			get{
				return trigger as Element;
			}
		}
		/// <summary>The GO that this is the list for. Related to trigger.</summary>
		public GameObject triggerGameObject{
			get{
				EventTarget3D et3D = (trigger as EventTarget3D);
				
				if(et3D==null){
					return null;
				}
				
				return et3D.gameObject;
			}
		}
		/// <summary>The available options.</summary>
		public List<Option> options=new List<Option>();
		
		/// <summary>True if the trigger is a 3D object.</summary>
		public bool is3D{
			get{
				return triggerGameObject!=null;
			}
		}
		
		/// <summary>True if this list is open on the UI. Also see widget.</summary>
		public bool isOpen{
			get{
				return widget!=null;
			}
		}
		
		/// <summary>Creates a context menu event. Used during the option collect process.</summary>
		public ContextEvent createEvent(){
			return createEvent("contextmenu");
		}
		
		/// <summary>Creates a context menu event. Used during the option collect process.</summary>
		public ContextEvent createEvent(string name){
			
			ContextEvent ce=new ContextEvent(name,null);
			ce.SetTrusted();
			ce.SetModifiers();
			ce.list=this;
			return ce;
			
		}
		
		/// <summary>Closes any visible version of this list only. Does not close parent menu's.
		/// E.g. closes a sub-menu if you're on one.</summary>
		public void close(){
			close(false);
		}
		
		/// <summary>Closes any visible version of this list.</summary>
		public void close(bool all){
			
			if(all && parent!=null){
				// Go up the parent tree to close the root.
				parent.close(true);
				return;
			}
			
			// Clear all option elements:
			for(int i=0;i<options.Count;i++){
				options[i].buttonElement=null;
			}
			
			// Got a widget?
			if(widget!=null){
				
				// Yep - close it!
				widget.close();
				widget=null;
				
			}
			
		}
		
		/// <summary>Runs this option (or option 0 if this is a list).</summary>
		public virtual void run(){
			
			if(isOpen){
				// Sub-menu and it's open. Close it.
				close();
				return;
			}
			
			// If the parent menu is open then we're actually opening up a submenu.
			if(parent.isOpen){
				
				// Opening a sub-menu. Get the parent widget:
				ContextMenuWidget cmw=(parent.widget as ContextMenuWidget);
				
				// Open it (relative to the parent - if the parent closes, it does too):
				widget=parent.widget.open(cmw.SubMenuType,null,buildGlobals(null));
				
			}else{
				
				// Directly run option 0:
				run(0);
				
			}
			
		}
		
		/// <summary>Runs the option at the given index. Closes the menu if it's open.</summary>
		public void run(int index){
			
			// Get it:
			Option op=this[index];
			
			if(op!=null){
				
				// Note that run may cause a sub-option to run/ open a sub-menu.
				// It may also cause the menu to close.
				op.run();
				
			}
			
		}
		
		/// <summary>Collects options and displays the menu at the pointers position.
		/// Run this on the 'root' option list only (don't call it on submenu's).</summary>
		public void display(InputPointer ip,bool instant){
			
			// First, collect the options:
			ContextEvent ce=collectOptions(ip);
			
			if(instant){
				// Won't actually display - just immediately run op 0:
				run();
				return;
			}
			
			// Even if it collected nothing, attempt to display it.
			// The event contains the position for us.
			
			// Get the widget manager:
			Widgets.Manager widgets=(ce.contextDocument as HtmlDocument).widgets;
			
			// Open a widget (closing an existing one):
			widget=widgets.get(ce.template,null);
			
			if(widget!=null){
				widget.close();
			}
			
			widget=widgets.open(ce.template,null,buildGlobals(ce));
			
		}
		
		/// <summary>Creates the set of globals to pass through to a standard widget.
		/// Note that the only required value is 'options' (which is set to this list).
		/// Others are 'x' and 'y' - pixel values which originate from the ContextEvent's clientX/Y.</summary>
		public Dictionary<string,object> buildGlobals(ContextEvent ce){
			
			// Create:
			Dictionary<string,object> set=new Dictionary<string,object>();
			
			// Add options:
			set["options"]=this;
			
			// top/left:
			if(ce!=null){
				
				set["x"]=ce.clientX;
				set["y"]=ce.clientY;
				
			}
			
			return set;
			
		}
		
		/// <summary>Collects options at the given pointer location.</summary>
		public ContextEvent collectOptions(InputPointer ip){
			// Create an oncontextmenu event:
			ContextEvent ce=createEvent();
			ce.trigger=ip;
			ce.clientX=ip.DocumentX;
			ce.clientY=ip.DocumentY;
			
			// Collect from a 2D element:
			trigger=ip.ActiveOverTarget;
			
			if(trigger!=null){
				
				// Collect:
				trigger.dispatchEvent(ce);
				return ce;
				
			}
			
			return ce;
		}
		
		/// <summary>Gets the 2D location the root list will appear at.
		/// Doesn't figure out where submenu's should go.</summary>
		public Vector2 rootScreenLocation{
			get{
				
				if(triggerGameObject!=null){
					
					// Locate it at the gameobject:
					Camera cam=PowerUI.Input.CameraFor3DInput;
					
					if(cam!=null){
						
						// Map the GO to a screen point:
						Vector2 point=cam.WorldToScreenPoint(triggerGameObject.transform.position);
						
						// Get the coords:
						return new Vector2(
							point.x,
							ScreenInfo.ScreenY-1-point.y
						);
						
					}
					
				}
				
				if(triggerElement!=null){
					
					// Set the context position (at the midpoint of the element):
					IRenderableNode irn=(triggerElement as IRenderableNode);
					
					if(irn!=null){
						
						// Get the computed style:
						ComputedStyle cs=irn.ComputedStyle;
						
						// Midpoint:
						return new Vector2(
							cs.GetMidpointX(),
							cs.GetMidpointY()
						);
						
					}
					
				}
				
				return Vector2.zero;
			}
		}
		
		/// <summary>If this list is visible on the UI, relocate it.
		/// Used if e.g. a gameobject is moving around.</summary>
		public void relocate(){
			
			// Get the location:
			// Vector2 pos=rootScreenLocation;
			
			
		}
		
		/// <summary>Collects options from the given gameobject (or any parent in the hierarchy).</summary>
		public ContextEvent collectOptions(GameObject go){
			
			trigger=go.getEventTarget();
			
			if(go==null){
				return null;
			}
			
			// Create a context event:
			ContextEvent ce=createEvent();
			
			// Locate it at the gameobject:
			Vector2 pos=rootScreenLocation;
			ce.clientX=pos.x;
			ce.clientY=pos.y;
			
			trigger=PowerUI.Input.ResolveTarget(go);
			
			if(trigger!=null){
				
				// Great - dispatch to it (which can change the coords if it wants):
				trigger.dispatchEvent(ce);
				
			}
			
			return ce;
			
		}
		
		/// <summary>Collects options from the given HTML element.</summary>
		public ContextEvent collectOptions(Element e){
			
			trigger=e;
			
			if(e==null){
				return null;
			}
			
			// Create a context event:
			ContextEvent ce=createEvent();
			
			// Locate it at the element:
			Vector2 pos=rootScreenLocation;
			ce.clientX=pos.x;
			ce.clientY=pos.y;
			
			// Collect:
			e.dispatchEvent(ce);
			
			return ce;
			
		}
		
		/// <summary>The first option or null.</summary>
		public Option firstOption{
			get{
				return this[0];
			}
		}
		
		/// <summary>Gets an option from this list safely - this won't throw any errors.</summary>
		public Option this[int index]{
			get{
				if(index<0||index>=options.Count){
					return null;
				}
				
				return options[index];
			}
		}
		
		/// <summary>The length of the list.</summary>
		public int length{
			get{
				return options.Count;
			}
		}
		
		/// <summary>True if there's more than one option (or sub-option).</summary>
		public bool hasOptions{
			get{
				return (options.Count>0);
			}
		}
		
		/// <summary>Adds the given option to the list. Typically the markup would be a HTML variable.
		/// Note that you can also create your own option class (inherit from Option) and add that instead.</summary>
		public Option add(string markup,OptionEventMethod method){
			Option option=new Option(markup,method);
			add(option);
			return option;
		}
		
		/// <summary>Adds the given option to the list.</summary>
		public void add(Option option){
			
			// Add to the set:
			option.index=options.Count;
			options.Add(option);
			option.parent=this;
			
		}
		
	}
	
}