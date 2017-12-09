// ---------------------------
//   Standard Module System
//      MIT Licensed
// (Extend and use as you wish)
// ---------------------------

using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;


namespace Modular{
	
	/// <summary>
	/// A starter info class.
	/// Passed to all starting modules.
	/// </summary>
	[Values.Preserve]
	public class StartInfo{
		
		/// <summary>A globally shared scanner.</summary>
		internal static AssemblyScanner SharedScanner;
		
		/// <summary>True if scanners should be shared.
		/// That basically means those scanners will be applied to all loading assemblies.</summary>
		public bool ShareScanners=true;
		/// <summary>A shared scanner.</summray>
		internal AssemblyScanner Scanner_;
		
		/// <summary>A scanner for this start info.</summary>
		public AssemblyScanner Scanner{
			get{
				if(Scanner_==null){
					Scanner_=new AssemblyScanner();
				}
				
				return Scanner_;
			}
		}
		
		/// <summary>Searches modules for subclasses of the given type (includes the type itself).
		/// Runs the found event for each one it discovers.</summary>
		public void FindAllSubTypes(Type type,OnFoundTypeEvent found){
			Scanner.FindAllSubTypes(type,found);
		}
		
		/// <summary>Called right after all modules in the given assembly are done starting.</summary>
		public virtual void Done(Assembly asm){
			
			// Got a scanner?
			// Scan now if so:
			if(Scanner_!=null){
				
				// Got a scanner!
				
				// Apply those new ones to all loaded assemblies, aside from asm:
				foreach(KeyValuePair<Assembly,StartInfo> kvp in Modular.Start.Started){
					
					if(kvp.Key==asm){
						continue;
					}
					
					// Scan an already loaded assembly with the new scanners only:
					Scanner_.Scan(kvp.Key);
					
				}
				
				// Merge new scanners into the shared one:
				if(ShareScanners){
					
					if(SharedScanner==null){
						SharedScanner=Scanner_;
					}else{
						// Merge in:
						Scanner_.MergeInto(SharedScanner);
						Scanner_=SharedScanner;
					}
					
				}
				
				// Scan asm with *all* scanners:
				Scanner_.Scan(asm);
				
			}
			
		}
		
	}
	
}