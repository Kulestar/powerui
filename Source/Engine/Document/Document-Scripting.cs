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


namespace PowerUI{
	
	public partial class HtmlDocument{
		
		/// <summary>True if this is a Nitro AOT compilation document.</summary>
		public bool AotDocument;
		/// <summary>Only used by Nitro AOT. The location of the html file for error reporting.</summary>
		public string ScriptLocation;
		/// <summary>The available script engines for this document, indexed by script type.</summary>
		public Dictionary<string,ScriptEngine> Engines;
		
		
		/// <summary>Clears all code.</summary>
		public void ClearCode(){
			Engines=null;
			FinishedParsing=false;
			
			// Clear any running intervals:
			UITimer.OnUnload(this);
		}
		
		/// <summary>Gets or creates a script engine of the given type.</summary>
		public ScriptEngine GetScriptEngine(string type){
			
			type=type.ToLower().Trim();
			
			if(Engines==null){
				// Create:
				Engines=new Dictionary<string,ScriptEngine>();
			}
			
			ScriptEngine engine;
			if(Engines.TryGetValue(type,out engine)){
				return engine;
			}
			
			// Try and instance it now! Get the global engine:
			engine=ScriptEngines.Get(type);
			
			if(engine==null){
				// Don't know how to handle this type of script.
				Dom.Log.Add("Warning: Some script has been ignored due to its type ('"+type+"'). Did you mean 'text/javascript'?");
				return null;
			}
			
			// Instance it:
			engine=engine.Instance(this);
			
			if(engine==null){
				// Security problem (it would've logged it for us).
				return null;
			}
			
			// Set doc:
			engine.Document=this;
			
			// Hook it up for each of this engines types:
			string[] types=engine.GetTypes();
			
			for(int i=0;i<types.Length;i++){
				Engines[types[i].ToLower().Trim()]=engine;
			}
			
			Dom.Event e=new Dom.Event("scriptengineload");
			dispatchEvent(e);
			
			return engine;
			
		}
		
		/// <summary>Attempts to compile all code.</summary>
		public bool TryCompile(){
			
			if(Engines==null){
				return false;
			}
			
			// TC each engine:
			foreach(KeyValuePair<string,ScriptEngine> kvp in Engines){
				
				if(!kvp.Value.TryCompile()){
					return false;
				}
				
			}
			
			// Clear FP:
			FinishedParsing=false;
			
			// Ok!
			return true;
			
		}
		
		/// <summary>Gets the existing JS engine for this document.</summary>
		public JavaScriptEngine JavascriptEngine{
			get{
				
				if(Engines==null){
					return null;
				}
				
				ScriptEngine engine;
				
				if(Engines.TryGetValue("text/javascript",out engine)){
					return engine as JavaScriptEngine;
				}
				
				return null;
				
			}
		}
		
		/// <summary>Gets or sets script variable values.</summary>
		/// <param name="index">The name of the variable.</param>
		/// <returns>The variable value.</returns>
		public object getJsVariable(string global){
			JavaScriptEngine nse=JavascriptEngine;
			
			if(nse!=null){
				return nse[global];
			}
			
			return null;
		}
		
		/// <summary>Gets or sets script variable values.</summary>
		/// <param name="index">The name of the variable.</param>
		/// <returns>The variable value.</returns>
		public void setJsVariable(string global,object value){
			JavaScriptEngine nse=JavascriptEngine;
			
			if(nse!=null){
				nse[global]=value;
			}
		}
		
		/// <summary>The global scope.</summary>
		public override object GlobalScope{
			get{
				return window;
			}
		}
		
		/// <summary>Runs a nitro function by name with optional arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">Optional arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object Run(string name,params object[] args){
			return RunLiteral(name,GlobalScope,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object[] args){
			return RunLiteral(name,GlobalScope,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="context">The context to use for the 'this' value.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object context,object[] args){
			return RunLiteral(name,context,args,false);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object[] args,bool optional){
			return RunLiteral(name,GlobalScope,args,optional);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="context">The context to use for the 'this' value.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunLiteral(string name,object context,object[] args,bool optional){
			JavaScriptEngine jse=JavascriptEngine;
			
			if(jse!=null){
				var obj = jse[name] as Jint.Native.JsValue;
				if(obj == null){
					if(optional){
						return null;
					}
					throw new Exception("The method '"+name+"' does not exist in your Javascript global scope.");
				}
				return jse.Run(obj,context,args);
			}
			
			return null;
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="context">The context to use for the 'this' value.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunOptionally(string name,object context,params object[] args){
			return RunLiteral(name,context,args,true);
		}
		
		/// <summary>Runs a nitro function by name with a set of arguments only if the method exists.</summary>
		/// <param name="name">The name of the function in lowercase.</param>
		/// <param name="optional">True if the method call is optional. No exception is thrown if not found.</param>
		/// <param name="args">The set of arguments to use when calling the function.</param>
		/// <returns>The value that the called function returned, if any.</returns>
		public object RunOptionally(string name,params object[] args){
			return RunLiteral(name,GlobalScope,args,true);
		}
		
		/// <summary>Attempts to execute the given code segment.</summary>
		public object Execute(string code, object scope){
			JavaScriptEngine nse=JavascriptEngine;
			return nse.Compile(code);
		}
		
	}
	
}