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
using Dom;


namespace PowerUI{
	
	
	public partial class HtmlElement{
		
		
		/// <summary>Runs the given function held in the named attribute (e.g. onkeydown) and checks if that function blocked
		/// the event. In the case of a blocked event, no default action should occur. Note that this is called by dispatchEvent
		/// and the attribute functions run before handlers do (same as Firefox).</summary>
		/// <param name="e">A standard DOM Event containing e.g. key/mouse information.</param>
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(bubblePhase){
				
				// Run the function:
				object result=Run("on"+e.type,e);
				
				if(result!=null && result is bool){
					// It returned true/false - was it false?
					
					if(!(bool)result){
						// Explicitly returned false - Blocked it.
						return true;
					}
					
				}
				
			}
			
			if(base.HandleLocalEvent(e,bubblePhase)){
				// Blocked it - stop there.
				return true;
			}
			
			// Main defaults occur in here:
			if(bubblePhase && e is MouseEvent && e.type=="click"){
				OnClickEvent((MouseEvent)e);
			}
			
			return false;
		}
		
		/// <summary>Runs a nitro function whos name is held in the given attribute.</summary>
		/// <param name="attribute">The name of the attribute in lowercase, e.g. "onmousedown".</param>
		/// <param name="args">Additional parameters you would like to pass to your function.</param>
		/// <returns>The value returned by the function.</returns>
		/// <exception cref="NullReferenceException">Thrown if the function does not exist.</exception>
		public object Run(string attribute,params object[] args){
			return RunLiteral(attribute,args);
		}
		
		/// <summary>Runs a nitro function whos name is held in the given attribute with a fixed block of arguments.</summary>
		/// <param name="attribute">The name of the attribute in lowercase, e.g. "onmousedown".</param>
		/// <param name="args">Additional parameters you would like to pass to your function.</param>
		/// <returns>The value returned by the function.</returns>
		/// <exception cref="NullReferenceException">Thrown if the function does not exist.</exception>
		public object RunLiteral(string attribute,object[] args){
			string methodName=getAttribute(attribute);
			
			if(methodName==null){
				return null;
			}
			
			if(methodName.IndexOf('(') != -1 || methodName.IndexOf('=') != -1){
				// Eval it.
				var engine = (document as HtmlDocument).JavascriptEngine;
				if(engine == null){
					// Silent ignore
					return null;
				}
				var method = engine.Compile("(function(event){\r\n" + methodName + "\r\n})") as Jint.Native.JsValue;
				if(method == null){
					return null;
				}
				return method.Invoke(engine.Map(this), engine.Map(args));
			}
			
			int index=methodName.LastIndexOf('.');
			
			if(index!=-1){
				
				// C# or UnityJS method.
				
				// Grab the class name:
				string className=methodName.Substring(0,index);
				
				// Go get the type:
				Type type=JavaScript.CodeReference.GetFirstType(className);
				
				if(type==null){
					Dom.Log.Add("Type not found: "+className);
					return null;
				}
				
				// Update the method name:
				methodName=methodName.Substring(index+1);
				
				// Grab the method info:
				try{
					#if NETFX_CORE
					MethodInfo method=type.GetTypeInfo().GetDeclaredMethod(methodName);
					#else
					MethodInfo method=type.GetMethod(methodName);
					#endif
					// Invoke it:
					return method.Invoke(null,args);
				}catch(Exception e){
					Dom.Log.Add("Calling method "+className+"."+methodName+"(..) errored: "+e);
					return null;
				}
			}
			
			return htmlDocument.RunLiteral(methodName,this,args);
		}
		
		
	}
	
}