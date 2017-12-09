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
	/// Represents a keypress event.
	/// </summary>
	
	public class KeyboardEvent : UIEvent{
		
		public const ulong DOM_KEY_LOCATION_STANDARD=0;
		public const ulong DOM_KEY_LOCATION_LEFT=1;
		public const ulong DOM_KEY_LOCATION_RIGHT=2;
		public const ulong DOM_KEY_LOCATION_NUMPAD=3;
		// Dropped:
		// public const int DOM_KEY_LOCATION_MOBILE=4;
		// public const int DOM_KEY_LOCATION_JOYSTICK=5;
		
		public KeyboardEvent(int keyCode,char character,bool down):base(keyCode,character,down){}
		
		public KeyboardEvent(string type,object init):base(type,init){}
		
		/// <summary>Partial location implementation.</summary>
		public ulong location{
			get{
				return DOM_KEY_LOCATION_STANDARD;
			}
		}
		
		// isComposing
		
	}
	
	public partial class UIEvent{
		
		/// <summary>True if this event is repeating.</summary>
		public bool repeat{
			get{
				return heldDown;
			}
		}
		
		/// <summary>The keycode as text.</summary>
		public string code{
			get{
				return ""+character;
			}
		}
		
		/// <summary>The keycode.</summary>
		public string key{
			get{
				return ""+character;
			}
		}
		
		private bool GetModifier(uint mask){
			return (Modifiers&mask)==mask;
		}
		
		/// <summary>Gets the modifier state for the given key.</summary>
		public bool getModifierState(string keyArg){
			
			if(keyArg==null){
				return false;
			}
			
			keyArg=keyArg.Trim().ToLower();
			
			switch(keyArg){
				
				case "accel":
					// Now depreciated
					return ctrlKey || metaKey;
				case "alt":
					return altKey;
				case "altgraph":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_ALT_GRAPH) || (altKey && ctrlKey);
				case "capslock":
					return capsLock;
				case "control":
					return ctrlKey;
				case "fn":
					return modifierFn;
				case "fnlock":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_FN_LOCK); 
				case "hyper":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_HYPER); 
				case "meta":
					return metaKey;
				case "numlock":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_NUM_LOCK);
				case "os":
					return metaKey;
				case "scrolllock":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_SCROLL_LOCK);
				case "shift":
					return shiftKey;
				case "super":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_SUPER);
				case "symbol":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_SYMBOL); 
				case "symbollock":
					return GetModifier(EventModifierInit.MODIFIER_SHIFT_SYMBOL_LOCK); 
				
			}
			
			return false;
			
		}
		
		
		
		/// <summary>Is this an fn button?</summary>
		public bool modifierFn{
			get{
				return GetModifier(EventModifierInit.MODIFIER_SHIFT_FN);
			}
		}
		
		/// <summary>Is caps lock on?</summary>
		public bool capsLock{
			get{
				return GetModifier(EventModifierInit.MODIFIER_SHIFT_CAPS_LOCK);
			}
		}
		
		/// <summary>Is a meta key (Windows key/ the Mac key etc) down?</summary>
		public bool metaKey{
			get{
				return GetModifier(EventModifierInit.MODIFIER_SHIFT_META);
			}
		}
		
		/// <summary>Is an alt key down?</summary>
		public bool altKey{
			get{
				return GetModifier(EventModifierInit.MODIFIER_SHIFT_ALT);
			}
		}
		
		/// <summary>Is a control key down?</summary>
		public bool ctrlKey{
			get{
				return GetModifier(EventModifierInit.MODIFIER_SHIFT_CTRL);
			}
		}
		
		/// <summary>Is a shift key down?</summary>
		public bool shiftKey{
			get{
				return GetModifier(EventModifierInit.MODIFIER_SHIFT_SHIFT);
			}
		}
		
	}
	
}