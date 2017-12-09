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
using System.Text;
using System.Collections;
using System.Collections.Generic;


namespace Pico{
	
	/// <summary>
	/// Holds a set of compiler flags.
	/// </summary>
	
	public class FlagSet{
		
		public List<string> Flags=new List<string>();
		
		
		public FlagSet(string[] flags){
			Add(flags);
		}
		
		public int Find(string flag){
			
			for(int i=0;i<Flags.Count;i++){
				
				if(Flags[i]==flag){
					return i;
				}
				
			}
			
			return -1;
		}
		
		public void EditorMode(bool editor){
			
			int index=Find("UNITY_EDITOR");
			
			bool exists=(index!=-1);
			
			if(exists==editor){
				return;
			}
			
			if(!editor){
				Flags.RemoveAt(index);
			}else{
				Unity("EDITOR");
			}
			
		}
		
		public void Add(string[] flags){
			
			for(int i=0;i<flags.Length;i++){
				
				Add(flags[i]);
				
			}
			
		}
		
		public void Add(string flag){
			Flags.Add(flag);
		}
		
		public void Unity(string flag){
			Add("UNITY_"+flag);
		}
		
		public override string ToString(){
			
			StringBuilder result=new StringBuilder();
			
			for(int i=0;i<Flags.Count;i++){
				
				if(i!=0){
					result.Append(" ");
				}
				
				result.Append("/define:"+Flags[i]);
			}
			
			return result.ToString();
			
		}
		
	}
	
}