// ---------------------------
//   Standard Module System
//      MIT Licensed
// (Extend and use as you wish)
// ---------------------------

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace Modular{
	
	/// <summary>
	/// This searches for modules which 
	/// conform to the standard module interface in the given assemblies.
	/// It'll then invoke their start methods in their defined order.
	/// As they start, they'll also optionally add to an AssemblyScanner
	/// which is then used to e.g. discover custom tags etc.
	/// </summary>
	
	[Values.Preserve]
	public static class Start{
		
		/// <summary>
		/// All started modules.
		/// </summary>
		public static Dictionary<Assembly,StartInfo> Started=new Dictionary<Assembly,StartInfo>();
		
		/// <summary>
		/// Starts all modules within the assembly that contains the given type.
		/// If it's already been started, this does nothing. It will:
		/// - Runs all methods in a Modular.Main class.
		/// - Run any new scanners on already loaded modules (The assembly contains a module which scans for types)
		/// - Adds those new ones to a global set of scanners
		/// - Run *all* scanners on the new module (Searches for properties, units, tags,..)
		/// </summary>
		public static void Now(Type type){
			
			#if NETFX_CORE
			Now(type.GetTypeInfo().Assembly);
			#else
			Now(type.Assembly);
			#endif
			
		}
		
		/// <summary>
		/// Starts all modules within the given assembly.
		/// </summary>
		public static void Now(Assembly asm){
			Now(asm,true);
		}
		
		/// <summary>
		/// Starts all modules within the given assembly.
		/// </summary>
		public static void Now(Assembly asm,bool sharedScanner){
			
			// Already started?
			if(Started.ContainsKey(asm)){
				return;
			}
			
			// Create the basic starter info:
			StartInfo info=new StartInfo();
			info.ShareScanners=sharedScanner;
			
			// Start now:
			Now(asm,info);
			
		}
		
		/// <summary>
		/// Starts all modules within the given assembly.
		/// </summary>
		/// <param name='info'>
		/// An information object passed to each starting module.
		/// </param>
		public static void Now(Assembly asm,StartInfo info){
			
			// Already started?
			if(Started.ContainsKey(asm)){
				return;
			}
			
			// Add:
			Started[asm]=info;
			
			// First, find the Modular.Main class.
			// Each module simply adds a method to it (via partial classes).
			// Method is of the form Start_moduleName(StartInfo info);
			Type type=asm.GetType("Modular.Main");
			
			if(type==null){
				// No modules in this assembly.
				info.Done(asm);
				return;
			}
			
			// Ok great - Get each method and obtain 
			// any other starter settings:
			MethodInfo[] methods=type.GetMethods(
				BindingFlags.Static | BindingFlags.Public
			);
			
			if(methods==null || methods.Length==0){
				// No modules in this assembly.
				info.Done(asm);
				return;
			}
			
			// Sort them by priority next (if they declare it).
			SortedDictionary<int,StarterGroup> byPriority=new SortedDictionary<int,StarterGroup>();
			
			// For each method, add it to the sorted set:
			for(int i=0;i<methods.Length;i++){
				
				// Get the meta attribute:
				#if NETFX_CORE
				Meta metaAttribute=methods[i].GetCustomAttribute(typeof(Meta),false) as Meta;
				#else
				Meta metaAttribute=Attribute.GetCustomAttribute(methods[i],typeof(Meta),false) as Meta;
				#endif
				
				int priority=0;
				
				if(metaAttribute!=null){
					// Get the priority:
					priority=metaAttribute.Priority;
				}
				
				// Get the group for that priority:
				StarterGroup group;
				if(!byPriority.TryGetValue(priority,out group)){
					
					// Create it:
					group=new StarterGroup();
					byPriority[priority]=group;
					
				}
				
				// Add the method to the group:
				group.Methods.Add(methods[i]);
				
			}
			
			// - Start them now, in the correct order! -
			
			// Create the params:
			object[] parameters=new object[]{info};
			
			// Start:
			foreach(KeyValuePair<int,StarterGroup> kvp in byPriority){
				
				foreach(MethodInfo method in kvp.Value.Methods){
					
					// Invoke it now, passing in the set:
					try{
						
						// Invoke it:
						method.Invoke(null,parameters);
						
					}catch(Exception e){
						
						// Failed to start a module!
						UnityEngine.Debug.LogError("Module failed to start ("+method.Name+"): "+e);
						
					}
					
				}
				
			}
			
			info.Done(asm);
			
		}
		
	}
	
}