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
	/// A simple DOM inspector.
	/// </summary>
	
	public class DomInspector : EditorWindow{
		
		/// <summary>The node with the mouse over it.</summary>
		public static Node MouseOverNode;
		/// <summary>The document entry being viewed.</summary>
		public static DocumentEntry Entry;
		/// <summary>The last opened window.</summary>
		public static EditorWindow Window;
		/// <summary>All unfolded nodes.</summary>
		public static Dictionary<Node,bool> ActiveUnfolded;
		/// <summary>Cached names for all documents.</summary>
		private static string[] AllDocumentNames_;
		/// <summary>All available docs.</summary>
		private static List<DocumentEntry> AllDocuments_;
		
		
		/// <summary>Makes a path safe for displaying on the Editor UI.</summary>
		public static string ListableName(string path){
			
			if(path!=null){
				path=path.Trim();
			}
			
			if(string.IsNullOrEmpty(path)){
				return "(No suitable name)";
			}
			
			return path.Replace("/","\u2044");
		}
		
		/// <summary>True if any docs are available.</summary>
		public static bool DocumentsAvailable{
			get{
				return AllDocuments_!=null && AllDocuments_.Count!=0;
			}
		}
		
		/// <summary>Gets a document by its unique ID.</summary>
		public static Document GetByUniqueID(uint uniqueID){
			
			// First, search UI.document:
			Document result=SearchForID(UI.document,uniqueID);
			
			if(result!=null){
				return result;
			}
			
			// Search every WorldUI:
			WorldUI current=UI.LastWorldUI;
			
			while(current!=null){
				
				// Search it too:
				result=SearchForID(current.document,uniqueID);
				
				if(result!=null){
					return result;
				}
				
				current=current.UIBefore;
			}
			
			return null;
		}
		
		/// <summary>Collects all available documents from the main UI and any WorldUI's.</summary>
		public static List<DocumentEntry> GetDocuments(bool refresh){
			
			if(AllDocuments_!=null && !refresh){
				return AllDocuments_;
			}
			
			List<DocumentEntry> results=new List<DocumentEntry>();
			AllDocuments_=results;
			
			// First, search UI.document:
			Search(UI.document,"Main UI",results);
			
			// Search every WorldUI:
			WorldUI current=UI.LastWorldUI;
			
			while(current!=null){
				
				// Search it too:
				Search(current.document,current.Name,results);
				
				current=current.UIBefore;
			}
			
			// Cache names:
			AllDocumentNames_=new string[results.Count];
			
			for(int i=0;i<results.Count;i++){
				
				results[i].Index=i;
				
				string name=results[i].Name;
				
				if(string.IsNullOrEmpty(name)){
					
					// Using doc.location:
					Dom.Location loc=results[i].Document.location;
					
					if(loc==null){
						name="about:blank";
					}else{
						name=loc.absolute;
					}
					
				}
				
				name=ListableName(name);
				
				// Set into names:
				AllDocumentNames_[i]=name;
				
			}
			
			return results;
			
		}
		
		/// <summary>Draws a document dropdown (using GetDocuments).</summary>
		public static DocumentEntry DocumentDropdown(DocumentEntry selected){
			
			if(AllDocumentNames_==null){
				// Load docs:
				GetDocuments(true);
			}
			
			if(AllDocuments_.Count==0){
				
				// Waiting for valid doc.
				PowerUIEditor.HelpBox("Waiting for documents (click this window after hitting play).");
				return null;
				
			}
			
			// Draw a refresh button:
			if(GUILayout.Button("Refresh documents")){
				
				// Refresh now:
				GetDocuments(true);
				
			}
			
			// Get index:
			int index=(selected==null) ? 0 : selected.Index;
			
			// Draw dropdown list now!
			int selectedIndex=EditorGUILayout.Popup(index,AllDocumentNames_);
			
			// Must always pull from all docs (as a refresh may result in the object changing).
			return AllDocuments_[selectedIndex];
			
		}
		
		/// <summary>Searches a document for any sub-documents (inside iframes).</summary>
		public static Document SearchForID(Document doc,uint id){
			
			if(doc==null){
				return null;
			}
			
			if(doc.uniqueID==id){
				return doc;
			}
			
			// Search for iframe's:
			Dom.HTMLCollection set=doc.getElementsByTagName("iframe");
			
			foreach(Element node in set){
				
				// Get as a HTML element:
				HtmlElement htmlElement=node as HtmlElement;
				
				// Double check it's not some evil iframe twin:
				if(htmlElement!=null){
					
					// Search content doc:
					Document result=SearchForID(htmlElement.contentDocument,id);
					
					if(result!=null){
						return result;
					}
					
				}
				
			}
			
			return null;
		}
		
		/// <summary>Searches a document for any sub-documents (inside iframes).</summary>
		public static void Search(Document doc,string name,List<DocumentEntry> results){
			
			if(doc==null){
				return;
			}
			
			// Add it:
			results.Add(new DocumentEntry(doc,name));
			
			// Search for iframe's:
			Dom.HTMLCollection set=doc.getElementsByTagName("iframe");
			
			foreach(Element node in set){
				
				// Get as a HTML element:
				HtmlElement htmlElement=node as HtmlElement;
				
				// Double check it's not some evil iframe twin:
				if(htmlElement!=null){
					
					string src=htmlElement.getAttribute("src");
					
					// Search content doc:
					Search(htmlElement.contentDocument,src,results);
					
				}
				
			}
			
		}
		
		// Add menu item named "DOM Inspector" to the PowerUI menu:
		[MenuItem("Window/PowerUI/DOM Inspector")]
		public static void ShowWindow(){
			// Show existing window instance. If one doesn't exist, make one.
			Window=EditorWindow.GetWindow(typeof(DomInspector));

			// Give it a title:
			#if PRE_UNITY5_3
			Window.title="DOM Inspector";
			#else
			GUIContent title=new GUIContent();
			title.text="DOM Inspector";
			Window.titleContent=title;
			#endif
			
		}
		
		// Called at 100fps.
		void Update(){
			
			if(!DocumentsAvailable){
				
				// Reload:
				GetDocuments(true);
				
			}
			
		}
		
		/// <summary>Redraws the node inspector.</summary>
		void RedrawNodeInspector(){
			
			if(NodeInspector.Window!=null){
				
				NodeInspector.Window.Repaint();
				
			}
			
		}
		
		void DrawNode(Node node){
			
			string name=(node is DocumentType)?"#doctype":node.nodeName;
			
			if(name==null){
				name="(Unnamed node)";
			}
			
			string idAttr=node.getAttribute("id");
			
			if(idAttr!=null){
				name+=" id='"+idAttr+"'";
			}
			
			Rect zone;
			
			if(node.childCount==0){
				
				// Leaf node of the DOM:
				GUILayout.BeginHorizontal();
				GUILayout.Space(EditorGUI.indentLevel * 20);
				GUILayout.Label(name);
				
				zone=GUILayoutUtility.GetLastRect();
				
				if(zone.Contains(UnityEngine.Event.current.mousePosition)){
					
					// Mark as the node with the mouse over it:
					MouseOverNode=node;
					RedrawNodeInspector();
					
				}
				
				
				GUILayout.EndHorizontal();
				return;
				
			}
			
			bool unfolded=ActiveUnfolded.ContainsKey(node);
			bool status=EditorGUILayout.Foldout(unfolded, name);
			
			// Check mouse (Doesn't work unfortunately!):
			zone=GUILayoutUtility.GetLastRect();
			
			if(zone.Contains(UnityEngine.Event.current.mousePosition)){
				
				// Mark as the node with the mouse over it:
				MouseOverNode=node;
				RedrawNodeInspector();
				
			}
			
			if(status!=unfolded){
				
				// Changed - update active:
				if(status){
					ActiveUnfolded[node]=true;
				}else{
					ActiveUnfolded.Remove(node);
				}
				
				MouseOverNode=node;
				
				RedrawNodeInspector();
				
			}
			
			if(status){
				
				// It's open - draw kids:
				NodeList kids=node.childNodes;
				if(kids!=null){
					
					int indent=EditorGUI.indentLevel+1;
					
					for(int i=0;i<kids.length;i++){
						
						// Get the child:
						Node child=kids[i];
						
						EditorGUI.indentLevel=indent;
						
						// Draw it too:
						DrawNode(child);
						
					}
					
				}
				
			}
			
		}
		
		void OnGUI(){
			
			// Doc dropdown:
			Entry=DocumentDropdown(Entry);
			
			if(Entry==null){
				return;
			}
			
			if(ActiveUnfolded==null){
				ActiveUnfolded=new Dictionary<Node,bool>();
			}
			
			EditorGUI.indentLevel=0;
			
			// For each node in the document..
			DrawNode(Entry.Document);
			
		}
		
	}
	
	public class DocumentEntry{
		
		public int Index;
		public Document Document;
		public string Name;
		
		
		public DocumentEntry(Document doc,string name){
			Document=doc;
			Name=name;
		}
		
	}
	
}