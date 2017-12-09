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

#if UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	#define UNITY3
#endif

using System;
using UnityEngine;
using System.Threading;
using System.IO;

namespace PowerUI{

	/// <summary>
	/// Helper editor class for dealing with compiler #symbols.
	/// </summary>

	public static class Symbols{
		
		/// <summary>Checks if the given symbol is present in the define symbols set.</summary>
		/// <param name="symbol">The symbol to look for.</param>
		public static bool IsSymbolDefined(string symbol){
			string defineSymbols=GetString();
			if(defineSymbols==symbol){
				return true;
			}
			if(defineSymbols.StartsWith(symbol+";") || defineSymbols.EndsWith(";"+symbol)){
				return true;
			}
			return defineSymbols.Contains(";"+symbol+";");
		}
		
		public static string GetString(){
			#if UNITY_2_6 || UNITY3
			// Read the cscp.rsp file.
			return "";
			#elif UNITY_EDITOR
			return UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup);
			#else
			throw new Exception("Failed to get scripting defines. This usually means you're using PowerUI precompiled without the editor flag.");
			#endif
		}
		
		public static void Set(string sym){
			#if UNITY_2_6 || UNITY3
			// Write the file.
			// Flush asset db.
			#elif UNITY_EDITOR
			UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(UnityEditor.EditorUserBuildSettings.selectedBuildTargetGroup,sym);
			#else
			throw new Exception("Failed to set scripting define. This usually means you're using PowerUI precompiled without the editor flag.");
			#endif
		}
		
		/// <summary>Defines the given symbol in the define symbols set.</summary>
		/// <param name="symbol">The symbol to define.</param>
		public static void DefineSymbol(string symbol){
			if(IsSymbolDefined(symbol)){
				return;
			}
			
			// Get the existing set of symbols:
			string defineSymbols=GetString();
			
			if(string.IsNullOrEmpty(defineSymbols)){
				defineSymbols=symbol;
			}else{
				defineSymbols+=";"+symbol;
			}
			
			// Write it back:
			Set(defineSymbols);
			
		}
		
		/// <summary>Gets all set symbols.</summary>
		public static string[] Get(){
			// Get the existing set of symbols:
			string defineSymbols=GetString();
			return defineSymbols.Split(';');
		}
		
		/// <summary>Removes the given symbol from the define symbols set.</summary>
		/// <param name="symbol">The symbol to remove, if found.</param>
		public static void UndefineSymbol(string symbol){
			if(!IsSymbolDefined(symbol)){
				return;
			}
			
			// Get the existing set of symbols:
			string[] pieces=Get();
			string defineSymbols="";
			
			for(int i=0;i<pieces.Length;i++){
				if(pieces[i]==symbol){
					// This is the symbol we want to strip - skip it.
					continue;
				}
				// Add it to the new string we're making.
				if(defineSymbols!=""){
					defineSymbols+=";";
				}
				defineSymbols+=pieces[i];
			}
			
			// Write it back:
			Set(defineSymbols);
		}
		
	}

}