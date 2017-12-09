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
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// A UI Event represents a click or keypress.
	/// An event object is always provided with any onmousedown/onmouseup/onkeydown etc.
	/// </summary>
	
	public partial class UIEvent : Dom.Event{
		
		/// <summary>The source Unity event if there is one.</summary>
		public UnityEngine.Event unityEvent;
		/// <summary>The pointer that triggered this event.</summary>
		public InputPointer trigger;
		/// <summary>A related target, if there is one.</summary>
		public EventTarget relatedTarget;
		
		
		/// <summary>An empty UI event.</summary>
		public UIEvent(){}
		
		/// <summary>Creates an event for the given type.</summary>
		public UIEvent(string type){
			this.EventType=type;
		}
		
		/// <summary>Creates an event for the given type.</summary>
		public UIEvent(string type,bool bubbles){
			this.EventType=type;
			this.bubbles=bubbles;
		}
		
		/// <summary>Creates an event for the given type.</summary>
		public UIEvent(string type,bool bubbles,bool cancelable){
			this.EventType=type;
			this.bubbles=bubbles;
			this.cancelable=cancelable;
		}
		
		/// <summary>Creates a new UI event for the mouse.</summary>
		/// <param name="x">The x location of the mouse.</param>
		/// <param name="y">The y location of the mouse.</param>
		/// <param name="down">True if the button is held down.</param>
		public UIEvent(float x,float y,bool down){
			clientX=x;
			clientY=y;
			heldDown=down;
		}
		
		/// <summary>Creates a new UI event for a keypress.</summary>
		/// <param name="key">The keycode.</param>
		/// <param name="ch">The newly typed character.</param>
		/// <param name="down">True if the key is held down.</param>
		public UIEvent(int key,char ch,bool down){
			keyCode=key;
			character=ch;
			heldDown=down;
		}
		
		public UIEvent(string type,object init):base(type,init){}
		
		/// <summary>Sets up the Modifiers property from the current Unity event allowing things like capsLock to work.</summary>
		public void SetModifiers(){
			unityEvent=UnityEngine.Event.current;
			
			if(unityEvent!=null){
				Modifiers=(uint)unityEvent.modifiers;
			}
			
		}
		
		/// <summary>The view the event came from.</summary>
		public PowerUI.Window view{
			get{
				HtmlDocument doc=htmlDocument;
				if(doc==null){
					return null;
				}
				return doc.window;
			}
		}
		
		/// <summary>If the target is an EvetTarget3D then this will get the GameObject.
		/// Null otherwise.</summary>
		public GameObject targetGameObject{
			get{
				EventTarget3D et3D = (target as EventTarget3D);
				
				if(et3D==null){
					return null;
				}
				
				return et3D.gameObject;
			}
		}
		
		/// <summary>A convenience shortcut to save casting 'target' to a HtmlElement.
		/// It's like this because SVG can also generate events.</summary>
		public HtmlElement htmlTarget{
			get{
				return target as HtmlElement;
			}
		}
		
		/// <summary>Gets the keycode as a UnityEngine.KeyCode.</summary>
		public KeyCode unityKeyCode{
			get{
				return (KeyCode)keyCode;
			}
		}
		
		/// <summary>Is the left mouse button currently down?</summary>
		public bool leftMouseDown{
			get{
				return UnityEngine.Input.GetMouseButton(0);
			}
		}
		
		/// <summary>Is the right mouse button currently down?</summary>
		public bool rightMouseDown{
			get{
				return UnityEngine.Input.GetMouseButton(1);
			}
		}
		
		public virtual int which{
			get{
				return keyCode;
			}
		}
		
		/// <summary>The HTML document that this event has come from, if any.</summary>
		public HtmlDocument htmlDocument{
			get{
				if(target==null){
					return null;
				}
				
				return (target as Node).document as HtmlDocument;
			}
		}
		
		/// <summary>The WorldUI that this event has come from, if any.</summary>
		public WorldUI worldUI{
			get{
				
				if(target==null){
					return null;
				}
				
				return (target as HtmlElement ).worldUI;
			}
		}
		
		/// <summary>A 0-1 value of where this event occured relative to the target element. 0 is left edge, 1 is right edge.</summary>
		public float relativeX{
			get{
				
				Css.IRenderableNode irn=target as Css.IRenderableNode;
				
				if(irn==null){
					// Unavailable.
					return 0f;
				}
				
				Css.ComputedStyle cs=irn.ComputedStyle;
				
				return (clientX-cs.OffsetLeft) / cs.PixelWidth;
				
			}
		}
		
		/// <summary>A 0-1 value of where this event occured relative to the target element. 0 is top edge, 1 is bottom edge.</summary>
		public float relativeY{
			get{
				
				Css.IRenderableNode irn=target as Css.IRenderableNode;
				
				if(irn==null){
					// Unavailable.
					return 0f;
				}
				
				Css.ComputedStyle cs=irn.ComputedStyle;
				
				return (clientY-cs.OffsetTop) / cs.PixelHeight;
				
			}
		}
		
		/// <summary>Alias for clientX.</summary>
		public double x{
			get{
				return clientX;
			}
		}
		
		/// <summary>Alias for clientY.</summary>
		public double y{
			get{
				return clientY;
			}
		}
		
		/// <summary>X Position relative to the parent.</summary>
		public double offsetX{
			get{
				return localX;
			}
		}
		
		/// <summary>Y Position relative to the parent.</summary>
		public double offsetY{
			get{
				return localY;
			}
		}
		
		/// <summary>The position of the event relative to the top left corner of the target element in pixels.</summary>
		public float localX{
			get{
				Css.ComputedStyle cs=getComputedStyle();
				
				if(cs==null){
					return clientX;
				}
				
				return clientX-cs.OffsetLeft;
				
			}
		}
		
		/// <summary>The position of the event relative to the top left corner of the target element in pixels.</summary>
		public float localY{
			get{
				
				Css.ComputedStyle cs=getComputedStyle();
				
				if(cs==null){
					return clientY;
				}
				
				return clientY-cs.OffsetTop;
				
			}
		}
		
		/// <summary>Gets the target computed style.</summary>
		public Css.ComputedStyle getComputedStyle(){
			
			Css.IRenderableNode irn=target as Css.IRenderableNode;
			
			return irn.ComputedStyle;
			
		}
		
	}
	
}

namespace Dom{
	
	public partial class Event{
		
		/// <summary>Sets up this UIEvent.</summary>
		public void initUIEvent(string type,bool canBubble,bool cancelable,PowerUI.Window view,ulong detail){
			EventType=type;
			bubbles=canBubble;
			this.cancelable=cancelable;
			
		}
		
	}
	
}