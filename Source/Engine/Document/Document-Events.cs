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
using UnityEngine;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a HTML Document. UI.document is the main UI document.
	/// Use UI.document.innerHTML to set its content.
	/// </summary>

	public partial class HtmlDocument{
		
		/// <summary>Called when the title of this document changes.</summary>
		public Action<Dom.Event> ontitlechange{
			get{
				return GetFirstDelegate<Action<Dom.Event>>("titlechange");
			}
			set{
				addEventListener("titlechange",new EventListener<Dom.Event>(value));
			}
		}
		
		/// <summary>Called when the tooltip for this document changes.</summary>
		public Action<Dom.Event> ontooltipchange{
			get{
				return GetFirstDelegate<Action<Dom.Event>>("tooltipchange");
			}
			set{
				addEventListener("tooltipchange",new EventListener<Dom.Event>(value));
			}
		}
		
		/// <summary>Called when the document resizes.</summary>
		public Action<Dom.Event> onresize{
			get{
				return GetFirstDelegate<Action<Dom.Event>>("resize");
			}
			set{
				addEventListener("resize",new EventListener<Dom.Event>(value));
			}
		}
		
		/// <summary>Called when a key goes up.</summary>
		public Action<KeyboardEvent> onkeyup{
			get{
				return GetFirstDelegate<Action<KeyboardEvent>>("keyup");
			}
			set{
				addEventListener("keyup",new EventListener<KeyboardEvent>(value));
			}
		}
		
		/// <summary>Called when a key goes down.</summary>
		public Action<KeyboardEvent> onkeydown{
			get{
				return GetFirstDelegate<Action<KeyboardEvent>>("keydown");
			}
			set{
				addEventListener("keydown",new EventListener<KeyboardEvent>(value));
			}
		}
		
		/// <summary>Called when the mouse moves.</summary>
		public Action<MouseEvent> onmousemove{
			get{
				return GetFirstDelegate<Action<MouseEvent>>("mousemove");
			}
			set{
				addEventListener("mousemove",new EventListener<MouseEvent>(value));
			}
		}
		
		/// <summary>Called when the document is about to be unloaded.</summary>
		public Action<BeforeUnloadEvent> onbeforeunload{
			get{
				return GetFirstDelegate<Action<BeforeUnloadEvent>>("beforeunload");
			}
			set{
				addEventListener("beforeunload",new EventListener<BeforeUnloadEvent>(value));
			}
		}
		
	}
	
}