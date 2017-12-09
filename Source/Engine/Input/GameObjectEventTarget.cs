using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PowerUI;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// An EventTarget automatically added to GameObjects so they can receive your event objects.
	/// Use yourGameObject.dispatchEvent(e) to send a gameObject a W3C event.
	/// To add event listeners, just define methods attached to your 
	/// gameObject (or manually attach them via getEventTarget) such as "OnMouseDown(MouseEvent e)".
	/// </summary>
	public class EventTarget3D : EventTarget{
		
		/// <summary>The 'host' gameobject.</summary>
		public GameObject gameObject;
		
		
		public EventTarget3D(GameObject host){
			gameObject = host;
		}
		
		public override string getTitle(){
			// GameObjects of course don't have a title attribute so we'll need
			// to get a tooltip value by defining something new.
			// You could for example invent a "tooltip" event and fire it to collect the title:
			// dispatchEvent(myTooltipEvent);
			// return myTooltipEvent.tooltip;
			return "(3D Tooltip)";
		}
		
		internal override EventTarget eventTargetParentNode{
			get{
				Transform parent = gameObject.transform.parent;
				
				if(parent==null){
					return null;
				}
				
				return parent.gameObject.getEventTarget();
			}
		}
		
	}
	
	/// <summary>A MonoBehaviour which holds an event target on GameObjects for us.</summary>
	public class GameObjectEventTarget : MonoBehaviour,IEventTarget{
		
		/// <summary>An underlying event target which allows us to addEventListener etc.</summary>
		public EventTarget3D Target;
		
		
		public void Awake(){
			// Apply target now:
			Target=new EventTarget3D(gameObject);
		}
		
		/// <summary>
		/// IEventTarget requires the dispatchEvent method.
		/// It's the same as the standard W3C dispatchEvent.</summary>
		public bool dispatchEvent(Dom.Event e){
			// We received some event!
			// This typically happens when the UI did not handle it, 
			// or because an event was specifically dispatched to this gameObject (via gameObject.dispatchEvent).
			return Target.dispatchEvent(e);
		}
		
		/// <summary>
		/// Uses reflection to collect all methods attached to this gameObject which 
		/// accept exactly one parameter which derives from Dom.Event and starts with 'On'.
		/// It then adds that method as an event handler by lowercasing the name.
		/// "OnMouseDown(MouseEvent e){..}" is the same as 
		/// Target.addEventListener("mousedown",delegate(MouseEvent e){..});</summary>
		public void Setup(){
			
			#if !Input3DManualMode
			
			// Get all scripts:
			List<MonoBehaviour> allScripts = new List<MonoBehaviour>();
			gameObject.GetComponents(allScripts);
			
			// For each one (skip GameObjectEventTarget)..
			foreach(MonoBehaviour script in allScripts){
				
				if(script is GameObjectEventTarget){
					continue;
				}
				
				// Get the methods:
				MethodInfo[] methods = script.GetType().GetMethods();
				
				// For each one..
				for(int i=0;i<methods.Length;i++){
					
					// Get the current method:
					MethodInfo method = methods[i];
					
					// Starts with 'On'?
					if(!method.Name.StartsWith("On")){
						continue;
					}
					
					// Must have exactly one parameter:
					ParameterInfo[] parameters = method.GetParameters();
					
					if(parameters.Length!=1){
						continue;
					}
					
					// The parameter must also be from Dom.Event:
					Type pType = parameters[0].ParameterType;
					
					if(!typeof(Dom.Event).IsAssignableFrom(pType)){
						continue;
					}
					
					// Got one! The delegates type will be (Action<pType>):
					Type delegateType = typeof(Action<>).MakeGenericType(pType);
					
					// The type for our listener:
					Type listenerType = typeof(EventListener<>).MakeGenericType(pType);
					
					// Create our action delegate and hook it up:
					#if NETFX_CORE
					object deleg = method.CreateDelegate(delegateType, script);
					#else
					object deleg = Delegate.CreateDelegate(delegateType, script, method);
					#endif
					
					// Create the listener object:
					EventListener listener = Activator.CreateInstance(listenerType,deleg) as EventListener;
					
					// Add it to the target:
					Target.addEventListener(method.Name.ToLower().Substring(2), listener);
					
				}
				
			}
			
			#endif
		}
		
	}
	
	/// <summary>
	/// Extends GameObject with the 'getEventTarget' and 'dispatchEvent' methods.
	/// Not in the PowerUI namespace so these 'just work' everywhere.
	/// </summary>
	public static class GameObjectExtensions{
	 
		/// <summary>Gets or creates an EventTarget on a GameObject.</summary>
		public static EventTarget getEventTarget(this GameObject go){
			// Get the monobehaviour:
			GameObjectEventTarget monoBehaviour = go.GetComponent<GameObjectEventTarget>();
			
			if(monoBehaviour == null){
				// Add it:
				monoBehaviour = go.AddComponent<GameObjectEventTarget>();
				monoBehaviour.Setup();
			}
			
			// Return the target property:
			return monoBehaviour.Target;
		}

		/// <summary>Dispatches an event to a gameObject.
		/// Depending on the events settings, it may bubble up the scene hierarchy.</summary>
		public static bool dispatchEvent(this GameObject go, Dom.Event e){
			// Get the monobehaviour:
			GameObjectEventTarget monoBehaviour = go.GetComponent<GameObjectEventTarget>();
			
			if(monoBehaviour == null){
				// Build it now:
				monoBehaviour = go.AddComponent<GameObjectEventTarget>();
				monoBehaviour.Setup();
			}
			
			// Dispatch:
			return monoBehaviour.Target.dispatchEvent(e);
		}
		
	}
	
}