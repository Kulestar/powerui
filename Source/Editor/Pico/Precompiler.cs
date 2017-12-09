#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	#define PRE_UNITY5
#endif

#if UNITY_4_6 || UNITY_4_7 || !PRE_UNITY5
	#define UNITY_UI_EXISTS
#endif

//--------------------------------------
//                Pico
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Reflection;


namespace Pico{

	/// <summary>
	/// Precompiles modules/ libraries so they don't get rebuilt by Unity every time any source file changes.
	/// This can be used by any of your modules too, so do make use of this side feature to really speed up your build times!
	/// </summary>

	public static class Precompiler{
		
		/// <summary>Builds the given set of files into the given module using standard compiler args.</summary>
		public static bool Build(Module module){
			
			// Get the target path:
			string target=module.DllPath;
			
			// Get the defines:
			string defines=GetFlags(module.EditorMode);
			
			// Build the options:
			string options="";
			foreach(string srcPath in module.SourceFolders){
				
				options+=" /recurse:\""+System.IO.Path.GetFullPath(srcPath)+"/*.cs.preco\"";
				
			}
			
			// Get a provider:
			CSharpCodeProvider compiler=new CSharpCodeProvider();
			
			CompilerParameters parameters=new CompilerParameters();
			parameters.GenerateInMemory=false;
			parameters.GenerateExecutable=true;
			parameters.OutputAssembly=target;
			parameters.CompilerOptions=defines+" /target:library"+options;
			
			// Reference Unity:
			AddUnityDllPaths(module.EditorMode,parameters.ReferencedAssemblies);
			
			// Build now:
			CompilerResults results = compiler.CompileAssemblyFromSource(parameters,"");
			
			if(results.Errors.Count>0){
				// We had errors! May just be warnings though, so let's check:
				
				foreach(CompilerError ce in results.Errors){
					if(ce.IsWarning){
						Debug.LogWarning(ce.ToString());
					}else{
						Debug.LogError(ce.ToString());
					}
				}
				
				if(!File.Exists(target)){
					
					Debug.LogError("Error precompiling "+target+".");
					return false;
					
				}
				
			}
			
			return true;
			
		}
		
		/// <summary>Gets the Unity provided set of compiler flags.</summary>
		public static string GetFlags(bool editor){
			
			string[] flags=EditorUserBuildSettings.activeScriptCompilationDefines;
			
			FlagSet set=new FlagSet(flags);
			
			set.EditorMode(editor);
			
			return set.ToString();
			
		}
		
		/// <summary>Adds the paths to the UnityEngine/Editor dlls to the given collection.</summary>
		public static void AddUnityDllPaths(bool editor,StringCollection references){
			
			// Time to go find the UnityEngine and UnityEditor dlls.
			// We can cheat a little here though - we know for a fact that both are already loaded and in use :)
			
			if(editor){
				references.Add(typeof(Editor).Assembly.Location);
			}
			
			references.Add(typeof(GameObject).Assembly.Location);
			
			#if UNITY_UI_EXISTS
			references.Add(typeof(UnityEngine.UI.RawImage).Assembly.Location);
			#endif
			
		}
		
	}
	
}