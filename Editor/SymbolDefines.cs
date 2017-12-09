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

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{

	/// <summary>
	/// Defines and undefines useful symbols, like #PowerUI
	/// </summary>
	
	[InitializeOnLoad]
	public class SymbolDefines{
		
		private static bool Changed;
		public static Dictionary<string,bool> Defines;
		
		
		static SymbolDefines(){
			
			#if !PowerUI && !UNITY_2_6 && !UNITY3
			Setup();
			#endif
			
		}
		
		private static void Setup(){
			LoadDefines();
			
			Define("PowerUI");
			
			Apply();
		}
		
		public static void Apply(){
			
			if(!Changed){
				return;
			}
			
			string result="";
			
			foreach(KeyValuePair<string,bool> kvp in Defines){
				
				if(result!=""){
					result+=";";
				}
				
				result+=kvp.Key;
			}
			
			Symbols.Set(result);
			
		}
		
		public static void LoadDefines(){
			
			Defines=new Dictionary<string,bool>();
			
			string[] set=Symbols.Get();
			
			for(int i=0;i<set.Length;i++){
				Define(set[i]);
			}
			
			Changed=false;
			
		}
		
		public static void Define(string symbol){
			Changed=true;
			Defines[symbol]=true;
		}
		
		public static void Undefine(string symbol){
			if(Defines.Remove(symbol)){
				Changed=true;
			}
		}
		
	}
	
}	