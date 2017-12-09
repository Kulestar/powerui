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


namespace PowerUI{
	
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
		/// <summary>The defines symbol that gets defined.</summary>
		public string DefinesSymbol;
		
		
		public SettingTickbox(string name,string defines,string help){
			Name=name;
			Help=help;
			DefinesSymbol=defines;
			Update();
		}
		
		/// <summary>Checks if the symbol has been defined manually.</summary>
		public void Update(){
			Active=Symbols.IsSymbolDefined(DefinesSymbol);
		}
		
		/// <summary>Shows the UI.</summary>
		public void OnGUI(){
			
			bool previous=Active;
			Active=EditorGUILayout.Toggle(Name,previous);
			
			PowerUIEditor.HelpBox(Help);
			
			if(previous!=Active){
				Changed();
			}
			
		}
		
		/// <summary>Called when the setting changes.</summary>
		public void Changed(){
			
			if(Active){
				Symbols.DefineSymbol(DefinesSymbol);
			}else{
				Symbols.UndefineSymbol(DefinesSymbol);
			}
			
		}
		
	}

}