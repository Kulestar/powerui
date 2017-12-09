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
	/// Called when a type which derives from some other type was found.
	/// </summary>
	public delegate void OnFoundTypeEvent(Type type);
	
	/// <summary>
	/// A scanner method which adds the types to scan for to the given scanner.
	/// </summary>
	public delegate void AssemblyScannerMethod(AssemblyScanner scanner);
	
	/// <summary>
	/// Scans all the types in an assembly, checking if they're any of the types we're after.
	/// </summary>
	
	[Values.Preserve]
	public class AssemblyScanner{
		
		/// <summary>Finds all subtypes and registers the handler 
		/// for any newly loaded assemblies.</summary>
		public static void FindAllSubTypesNow(Type type,OnFoundTypeEvent mtd){
			
			// Only adds the one:
			ScanAllNow(delegate(AssemblyScanner scanner){
				
				// Find:
				scanner.FindAllSubTypes(type,mtd);
				
			});
			
		}
		
		/// <summary>
		/// Scans the assembly containing the given method 
		/// which adds the types to look for.</summary>
		public static void ScanThisNow(AssemblyScannerMethod mtd){
			
			// Create an assembly scanner:
			Modular.AssemblyScanner scanner=new Modular.AssemblyScanner();
			
			// Add the checkers:
			mtd(scanner);
			
			// Scan "this" assembly now:
			scanner.Scan(mtd.GetType());
			
		}
		
		/// <summary>
		/// Scans all loaded assemblies. The given method provides the types to look for.
		/// Stores the scanner globally so any newly loaded assemblies will get scanned too.
		/// </summary>
		public static void ScanAllNow(AssemblyScannerMethod mtd){
			ScanAllNow(mtd,true);
		}
		
		/// <summary>
		/// Scans all loaded assemblies. The given method provides the types to look for.
		/// </summary>
		/// <param name="sharedScanner">True if you'd like the scanners to be stored
		/// globally. They'll then be used on any newly loaded assemblies.</param>
		public static void ScanAllNow(AssemblyScannerMethod mtd,bool sharedScanner){
			
			// Create an assembly scanner:
			Modular.AssemblyScanner scanner=new Modular.AssemblyScanner();
			
			// Add the checkers:
			mtd(scanner);
			
			// Scan all assemblies now:
			scanner.ScanAll();
			
			// Merge now:
			if(sharedScanner){
				
				if(StartInfo.SharedScanner==null){
					StartInfo.SharedScanner=scanner;
				}else{
					// merge!
					scanner.MergeInto(StartInfo.SharedScanner);
					scanner=StartInfo.SharedScanner;
				}
				
			}
			
		}
		
		/// <summary>All the types to look out for, sorted by pass number.</summary>
		private SortedDictionary<int,List<TypeToFind>> ToFind;
		
		
		public AssemblyScanner(){
			ToFind=new SortedDictionary<int,List<TypeToFind>>();
		}
		
		/// <summary>Merges the scanners from this into the given one.</summary>
		public void MergeInto(AssemblyScanner scanner){
			
			// For each pass..
			foreach(KeyValuePair<int,List<TypeToFind>> kvp in ToFind){
				
				int tcCount=kvp.Value.Count;
				
				// For each type checker..
				for(int tc=0;tc<tcCount;tc++){
					
					// Add it to scanner:
					scanner.Add(kvp.Key,kvp.Value[tc]);
					
				}
				
			}
			
		}
		
		/// <summary>Scans all loaded assemblies.</summary>
		public void ScanAll(){
			
			if(Modular.Start.Started.Count==0){
				
				#if NETFX_CORE
				// This typically happens in the Editor.
				Modular.Start.Now(GetType());
				#else
				
				// Start all assemblies:
				Assembly[] all=System.AppDomain.CurrentDomain.GetAssemblies();
				
				foreach(Assembly a in all){
					Modular.Start.Now(a);
				}
				
				#endif
				
			}
			
			// For each started module..
			foreach(KeyValuePair<Assembly,StartInfo> kvp in Modular.Start.Started){
				
				// Scan it:
				Scan(kvp.Key);
				
			}
			
		}
		
		/// <summary>Checks for the given type.</summary>
		public void FindAllSubTypes(Type type,OnFoundTypeEvent found){
			FindAllSubTypes(0,type,found,false);
		}
		
		/// <summary>Checks for the given type. Optionally allow multiple passes
		/// (e.g. if searching for a type requires other types to be ready to go).</summary>
		public void FindAllSubTypes(int pass,Type type,OnFoundTypeEvent found){
			FindAllSubTypes(pass,type,found,false);
		}
		
		/// <summary>Checks for the given type. Optionally allow multiple passes
		/// (e.g. if searching for a type requires other types to be ready to go).</summary>
		public void FindAllSubTypes(int pass,Type type,OnFoundTypeEvent found,bool allowGeneric){
			
			// Create:
			TypeToFind ttf=new TypeToFind(type,found);
			ttf.AllowGeneric=allowGeneric;
			
			// Add:
			Add(pass,ttf);
			
		}
		
		/// <summary>Adds a type to find to the given pass.</summary>
		private void Add(int pass,TypeToFind ttf){
			
			// Create the set if it's needed:
			List<TypeToFind> set;
			if(!ToFind.TryGetValue(pass,out set)){
				set=new List<TypeToFind>();
				ToFind[pass]=set;
			}
			
			// Add:
			set.Add(ttf);
			
		}
		
		/// <summary>Scans the assembly containing the given type now.</summary>
		public void Scan(Type type){
			
			#if NETFX_CORE
			Scan(type.GetTypeInfo().Assembly);
			#else
			Scan(type.Assembly);
			#endif
			
		}
		
		/// <summary>Scans the given assembly now.</summary>
		public void Scan(Assembly asm){
			
			// For each pass..
			foreach(KeyValuePair<int,List<TypeToFind>> kvp in ToFind){
				
				// Pass now:
				ScanPass(asm,kvp.Value);
				
			}
			
		}
		
		/// <summary>Performs one of the scan passes.</summary>
		private void ScanPass(Assembly asm,List<TypeToFind> set){
			
			int tcCount=set.Count;
			
			#if NETFX_CORE
			
			// For each type..
			foreach(TypeInfo type in asm.DefinedTypes){
				
				// Is it any of the types?
				
				bool isGeneric=type.IsGenericType;
				
				// For each type checker..
				for(int tc=0;tc<tcCount;tc++){
					
					Type toFind=set[tc].Type;
					
					if(isGeneric && !set[tc].AllowGeneric){
						continue;
					}
					
					// Is it a 'toFind' class?
					if( type.IsSubclassOf(toFind) ){
						
						// Yes it is - run the callback:
						set[tc].Found(type.AsType());
						
					}
					
				}
				
			}
			
			#else
			
			// Get all types:
			Type[] allTypes=asm.GetTypes();
			
			// For each type..
			for(int i=allTypes.Length-1;i>=0;i--){
				Type type=allTypes[i];
				
				bool isGeneric=type.IsGenericType;
				
				// For each type checker..
				for(int tc=0;tc<tcCount;tc++){
					
					Type toFind=set[tc].Type;
					
					if(isGeneric && !set[tc].AllowGeneric){
						continue;
					}
					
					// Is it a 'toFind' class?
					if( type.IsSubclassOf(toFind) ){
						
						// Yes it is - run the callback:
						set[tc].Found(type);
						
					}
					
				}
				
			}
			
			#endif
			
		}
		
	}
	
	/// <summary>
	/// A type to search for subclasses of.
	/// E.g. used when looking for custom tags, file handlers etc.
	/// </summary>
	internal class TypeToFind{
		
		/// <summary>The type itself.</summary>
		internal Type Type;
		/// <summary>True if it should allow generics through. False by default.</summary>
		internal bool AllowGeneric;
		/// <summary>The callback to run when it's found.</summary>
		internal OnFoundTypeEvent Found;
		
		
		internal TypeToFind(Type type,OnFoundTypeEvent found){
			Type=type;
			Found=found;
		}
		
	}
	
}