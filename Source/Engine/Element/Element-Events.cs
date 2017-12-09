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
using PowerUI;


namespace Dom{
	
	/// <summary>
	/// These are the "Global Event Handlers".
	/// </summary>

	public partial class Element{
		
		/// <summary>Called when this element receives a keyup.</summary>
		public Action<KeyboardEvent> onkeyup{
			get{
				return GetFirstDelegate<Action<KeyboardEvent>>("keyup");
			}
			set{
				addEventListener("keyup",new EventListener<KeyboardEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a keydown.</summary>
		public Action<KeyboardEvent> onkeydown{
			get{
				return GetFirstDelegate<Action<KeyboardEvent>>("keydown");
			}
			set{
				addEventListener("keydown",new EventListener<KeyboardEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a mouseup.</summary>
		public Action<MouseEvent> onmouseup{
			get{
				return GetFirstDelegate<Action<MouseEvent>>("mouseup");
			}
			set{
				addEventListener("mouseup",new EventListener<MouseEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a mouseout.</summary>
		public Action<MouseEvent> onmouseout{
			get{
				return GetFirstDelegate<Action<MouseEvent>>("mouseout");
			}
			set{
				addEventListener("mouseout",new EventListener<MouseEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a mousedown.</summary>
		public Action<MouseEvent> onmousedown{
			get{
				return GetFirstDelegate<Action<MouseEvent>>("mousedown");
			}
			set{
				addEventListener("mousedown",new EventListener<MouseEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a mousemove. Note that it must be focused.</summary>
		public Action<MouseEvent> onmousemove{
			get{
				return GetFirstDelegate<Action<MouseEvent>>("mousemove");
			}
			set{
				addEventListener("mousemove",new EventListener<MouseEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a mouseover.</summary>
		public Action<MouseEvent> onmouseover{
			get{
				return GetFirstDelegate<Action<MouseEvent>>("mouseover");
			}
			set{
				addEventListener("mouseover",new EventListener<MouseEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a scrollwheel event.</summary>
		public Action<WheelEvent> onwheel{
			get{
				return GetFirstDelegate<Action<WheelEvent>>("wheel");
			}
			set{
				addEventListener("wheel",new EventListener<WheelEvent>(value));
			}
		}
		
		/// <summary>Called when a form is reset.</summary>
		public Action<FormEvent> onreset{
			get{
				return GetFirstDelegate<Action<FormEvent>>("reset");
			}
			set{
				addEventListener("reset",new EventListener<FormEvent>(value));
			}
		}
		
		/// <summary>Called when a form is submitted.</summary>
		public Action<FormEvent> onsubmit{
			get{
				return GetFirstDelegate<Action<FormEvent>>("submit");
			}
			set{
				addEventListener("submit",new EventListener<FormEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a load event (e.g. iframe).</summary>
		public Action<UIEvent> onload{
			get{
				return GetFirstDelegate<Action<UIEvent>>("load");
			}
			set{
				addEventListener("load",new EventListener<UIEvent>(value));
			}
		}
		
		/// <summary>Called when this element gets focused.</summary>
		public Action<FocusEvent> onfocus{
			get{
				return GetFirstDelegate<Action<FocusEvent>>("focus");
			}
			set{
				addEventListener("focus",new EventListener<FocusEvent>(value));
			}
		}
		
		/// <summary>Called just before this element is focused.</summary>
		public Action<FocusEvent> onfocusin{
			get{
				return GetFirstDelegate<Action<FocusEvent>>("focusin");
			}
			set{
				addEventListener("focusin",new EventListener<FocusEvent>(value));
			}
		}
		
		/// <summary>Called just before this element is blurred.</summary>
		public Action<FocusEvent> onfocusout{
			get{
				return GetFirstDelegate<Action<FocusEvent>>("focusout");
			}
			set{
				addEventListener("focusout",new EventListener<FocusEvent>(value));
			}
		}
		
		/// <summary>Called when this element is unfocused (blurred).</summary>
		public Action<FocusEvent> onblur{
			get{
				return GetFirstDelegate<Action<FocusEvent>>("blur");
			}
			set{
				addEventListener("blur",new EventListener<FocusEvent>(value));
			}
		}
		
		/// <summary>Called when this element receives a full click.</summary>
		public Action<MouseEvent> onclick{
			get{
				return GetFirstDelegate<Action<MouseEvent>>("click");
			}
			set{
				addEventListener("click",new EventListener<MouseEvent>(value));
			}
		}
		
		/// <summary>Used by e.g. input, select etc. Called when its value changes.</summary>
		public Action<Dom.Event> onchange{
			get{
				return GetFirstDelegate<Action<Dom.Event>>("change");
			}
			set{
				addEventListener("change",new EventListener<Dom.Event>(value));
			}
		}
		
	}
	
}