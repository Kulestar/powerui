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
	
	public class EventInit{
		
		public bool bubbles;
		public bool cancelable;
		public bool composable;
		
		internal virtual void Setup(Dom.Event e){}
		
	}
	
	public class CustomEventInit:EventInit{
		
		public object detail;
		
	}
	
	public class UIEventInit:EventInit{
		
		public Window view;
		public long detail;
		
	}
	
	public class EventModifierInit:UIEventInit{
		
		/// <summary>The location in ExtraModifiers.</summary>
		/// Note that this block is consistent with EventModifiers so we can just straight cast it to an integer from the UnityEvent
		/// (but with the bonus of being able to add the extra ones).
		public const uint MODIFIER_SHIFT_SHIFT=1<<0;
		public const uint MODIFIER_SHIFT_CTRL=1<<1;
		public const uint MODIFIER_SHIFT_ALT=1<<2;
		public const uint MODIFIER_SHIFT_META=1<<3;
		public const uint MODIFIER_SHIFT_NUM_LOCK=1<<4;
		public const uint MODIFIER_SHIFT_CAPS_LOCK=1<<5;
		public const uint MODIFIER_SHIFT_FN=1<<6;
		
		public const uint MODIFIER_SHIFT_ALT_GRAPH=1<<7;
		public const uint MODIFIER_SHIFT_FN_LOCK=1<<8;
		public const uint MODIFIER_SHIFT_HYPER=1<<9;
		public const uint MODIFIER_SHIFT_SCROLL_LOCK=1<<10;
		public const uint MODIFIER_SHIFT_SUPER=1<<11;
		public const uint MODIFIER_SHIFT_SYMBOL=1<<12;
		public const uint MODIFIER_SHIFT_SYMBOL_LOCK=1<<13;
		
		public uint Modifiers;
		
		public bool ctrlKey{
			get{
				return Get(MODIFIER_SHIFT_CTRL);
			}
			set{
				Set(MODIFIER_SHIFT_CTRL,value);
			}
		}
		
		public bool shiftKey{
			get{
				return Get(MODIFIER_SHIFT_SHIFT);
			}
			set{
				Set(MODIFIER_SHIFT_SHIFT,value);
			}
		}
		
		public bool altKey{
			get{
				return Get(MODIFIER_SHIFT_ALT);
			}
			set{
				Set(MODIFIER_SHIFT_ALT,value);
			}
		}
		
		public bool metaKey{
			get{
				return Get(MODIFIER_SHIFT_META);
			}
			set{
				Set(MODIFIER_SHIFT_META,value);
			}
		}
		
		public EventModifierInit(){}
		public EventModifierInit(UnityEngine.Event e){
			
			// Pull modifiers:
			Modifiers=(uint)e.modifiers;
			
		}
		
		private bool Get(uint mask){
			return (Modifiers&mask)==mask;
		}
		
		private void Set(uint mask,bool value){
			if(value){
				Modifiers|=mask;
			}else{
				// Clear:
				Modifiers&=~mask;
			}
		}
		
		public bool modifierAltGraph{
			get{
				return Get(MODIFIER_SHIFT_ALT_GRAPH);
			}
			set{
				Set(MODIFIER_SHIFT_ALT_GRAPH,value);
			}
		}
		
		public bool modifierCapsLock{
			get{
				return Get(MODIFIER_SHIFT_CAPS_LOCK);
			}
			set{
				Set(MODIFIER_SHIFT_CAPS_LOCK,value);
			}
		}
		
		public bool modifierFn{
			get{
				return Get(MODIFIER_SHIFT_FN);
			}
			set{
				Set(MODIFIER_SHIFT_FN,value);
			}
		}
		
		public bool modifierFnLock{
			get{
				return Get(MODIFIER_SHIFT_FN_LOCK);
			}
			set{
				Set(MODIFIER_SHIFT_FN_LOCK,value);
			}
		}
		
		public bool modifierHyper{
			get{
				return Get(MODIFIER_SHIFT_HYPER);
			}
			set{
				Set(MODIFIER_SHIFT_HYPER,value);
			}
		}
		
		public bool modifierNumLock{
			get{
				return Get(MODIFIER_SHIFT_NUM_LOCK);
			}
			set{
				Set(MODIFIER_SHIFT_NUM_LOCK,value);
			}
		}
		
		public bool modifierScrollLock{
			get{
				return Get(MODIFIER_SHIFT_SCROLL_LOCK);
			}
			set{
				Set(MODIFIER_SHIFT_SCROLL_LOCK,value);
			}
		}
		
		public bool modifierSuper{
			get{
				return Get(MODIFIER_SHIFT_SUPER);
			}
			set{
				Set(MODIFIER_SHIFT_SUPER,value);
			}
		}
		
		public bool modifierSymbol{
			get{
				return Get(MODIFIER_SHIFT_SYMBOL);
			}
			set{
				Set(MODIFIER_SHIFT_SYMBOL,value);
			}
		}
		
		public bool modifierSymbolLock{
			get{
				return Get(MODIFIER_SHIFT_SYMBOL_LOCK);
			}
			set{
				Set(MODIFIER_SHIFT_SYMBOL_LOCK,value);
			}
		}
		
	}
	
}