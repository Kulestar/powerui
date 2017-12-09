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

using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;


namespace Loonim{
	
	public delegate void TickboxChangeEvent(SettingTickbox box);
	
	/// <summary>
	/// Represents a checkbox option which toggles a defined symbol.
	/// It uses defines symbols for performance purposes; it can short out pieces of code that aren't needed.
	/// Used mainly by general settings.
	/// </summary>
	
	public class SettingTickbox{
		
		/// <summary>True if its checked.</summary>
		public bool Active;
		/// <summary>The English name, e.g. "Custom Input".</summary>
		public string Name;
		/// <summary>The help text.</summary>
		public string Help;
		/// <summary>Called when the tickbox changes.</summary>
		public TickboxChangeEvent OnChanged;
		
		
		public SettingTickbox(string name,string help,TickboxChangeEvent onChanged){
			Name=name;
			Help=help;
			OnChanged=onChanged;
		}
		
		/// <summary>Shows the UI.</summary>
		public void OnGUI(){
			
			bool previous=Active;
			Active=EditorGUILayout.Toggle(Name,previous);
			
			#if !PRE_UNITY3_5
			UnityEditor.EditorGUILayout.HelpBox(Help,MessageType.Info);
			#endif
			
			if(previous!=Active){
				
				if(OnChanged!=null){
					OnChanged(this);
				}
				
			}
			
		}
		
	}

}