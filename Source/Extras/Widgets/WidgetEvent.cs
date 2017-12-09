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

using Dom;
using System;
using UnityEngine;
using PowerUI;


namespace Widgets{
	
	/// <summary>
	/// Represents a context event.
	/// Extend this (with a partial class) if you want to add custom things 
	/// to pass through to the widget which will actually handle the display.
	/// </summary>
	
	public partial class WidgetEvent : UIEvent{
		
		/// <summary>The origin widget.</summary>
		public Widget widget;
		
		
		public WidgetEvent(string type,object init):base(type,init){
		}
		
	}
	
}

namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		public void addEventListener(string name,Action<Widgets.WidgetEvent> method){
			addEventListener(name,new EventListener<Widgets.WidgetEvent>(method));
		}
		
	}
	
}