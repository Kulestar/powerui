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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
#define PRE_UNITY5
#endif

#if PRE_UNITY5 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
#define PRE_UNITY5_3
#endif

using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.IO;
using Dom;
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// A simple node inspector. Companion of the DOM inspector.
	/// </summary>
	
	public class NodeInspector : EditorWindow{
		
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		
		
		// Add menu item named "Node Inspector" to the PowerUI menu:
		[MenuItem("Window/PowerUI/Node Inspector")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(NodeInspector));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="Node Inspector";
			#else
			GUIContent title=new GUIContent();
			title.text="Node Inspector";
			Window.titleContent=title;
			#endif
			
		}
		
		private Node ComputedDataFor;
		private string ComputedNodeData;
		
		internal void BuildString(Node node){
			
			ComputedDataFor=node;
			string result="";
			
			if(node.Properties!=null && node.Properties.Count>0){
				
				result+="Attributes:\r\n";
				
				foreach(KeyValuePair<string,string> kvp in node.Properties){
					
					result+=kvp.Key+": "+kvp.Value+"\r\n";
					
				}
				
				result+="\r\n";
				
			}
			
			// Is the node renderable?
			IRenderableNode renderable=(node as IRenderableNode);
			
			if(renderable!=null){
				
				result+="Applied Selectors:\r\n";
				
				// Get the computed style:
				ComputedStyle cs=renderable.ComputedStyle;
				
				// All applied styles (without allocating a new list):
				MatchingRoot match=cs.FirstMatch;
				
				while(match!=null){
					
					// Note! These nodes can be participants from other nearby elements.
					// That happens with any combined selector.
					// To filter those ones out, check if this element is the actual target:
					if(match.IsTarget){
						
						// Rule is simply matching.Rule:
						StyleRule rule=match.Rule;
						
						// To get the underlying selector, it's rule.Selector:
						Selector selector=rule.Selector;
						
						// Note that this builds the selector text - avoid calling from OnGUI!
						result+=selector.selectorText+"\r\n";
						
					}
					
					// Next one:
					match=match.NextInStyle;
					
				}
				
				result+="\r\n";
				
				result+="Computed style:\r\n";
				
				// Computed values:
				foreach(KeyValuePair<CssProperty,Css.Value> kvp in cs.Properties){
					
					// kvp.Key (a CSS property) is set to kvp.Value (a CSS value)
					result+=kvp.Key.Name+": ";
					
					if(kvp.Value is Css.Keywords.Inherit){
						
						result+=(kvp.Value as Css.Keywords.Inherit).From+" (inherit)";
						
					}else{
						
						result+=kvp.Value;
						
					}
					
					result+="\r\n";
					
				}
				
				result+="\r\n";
				result+="Computed boxes:\r\n";
				
				// Computed values:
				RenderableData renderData=renderable.RenderData;
				
				if(renderData.FirstBox==null){
					
					result+="There's no boxes. The element isn't visible.\r\n";
					
				}else{
				
					LayoutBox box=renderData.FirstBox;
					
					while(box!=null){
						result+=box.ToString()+" (Scroll: "+box.Scroll.Left+", "+box.Scroll.Top+")\r\n";
						box=box.NextInElement;
					}
					
				}
				
				result+="\r\n";
				
			}
			
			// JS event hooks:
			EventTarget target=(node as EventTarget);
			
			if(target!=null){
				
				result+="Events:\r\n";
				
				if(target.Events==null || target.Events.Handlers==null || target.Events.Handlers.Count==0){
					
					result+="No events hooked up to this element.\r\n";
					
				}else{
					
					// Grab the JS event handlers:
					Dictionary<string,List<EventListener>> allHandlers=target.Events.Handlers;
					
					// Key is e.g. "mousedown"
					// Value is all the things that'll run when the event triggers.
					foreach(KeyValuePair<string,List<EventListener>> kvp in allHandlers){
						
						int count=kvp.Value.Count;
						
						string plural=count==1?"":"s";
						
						result+=kvp.Key+": ("+count+" listener"+plural+")\r\n";
						
					}
					
				}
				
			}
			
			ComputedNodeData=result;
			
		}
		
		void OnGUI(){
			
			// Use the node from the DOM inspector:
			Node node=DomInspector.MouseOverNode;
			
			if(node==null){
				PowerUIEditor.HelpBox("No node selected. Click on a node in the DOM inspector.");
				return;
			}
			
			string name=node.nodeName;
			
			if(name==null){
				name="(Unnamed node)";
			}
			
			bool refresh=GUILayout.Button("Refresh");
			
			GUILayout.Label(name);
			
			if(refresh || ComputedDataFor!=node || ComputedNodeData==null){
				
				try{
					BuildString(node);
				}catch(Exception e){
					ComputedNodeData="<b>Error whilst building node data</b>\r\n"+e.ToString();
				}
				
			}
			
			GUILayout.Label(ComputedNodeData);
			
		}
		
	}
	
}