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

using System;
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Useful helpers for iterating through all child nodes (i.e. through all kids too).
	/// </summary>

	public partial class Node{
		
		/// <summary>All child nodes of this node.</summary>
		public IEnumerable<Node> all{
			get{
				
				if(childNodes_!=null){
					
					foreach(Node child in childNodes_){
						
						// Return the child itself:
						yield return child;
						
						if(child.childNodes_!=null){
							
							foreach(Node subChild in child.all){
								
								// Return its kids too:
								yield return subChild;
								
							}
							
						}
						
					}
					
				}
				
			}
		}
		
		/// <summary>All text nodes that are children of this element.</summary>
		public IEnumerable<TextNode> allText{
			get{
				foreach(Node child in all){
					if(child is TextNode){
						yield return child as TextNode;
					}
				}
			}
		}
		
		/// <summary>All elements that are children of this element (not like element.all, which is all nodes including text).</summary>
		public IEnumerable<Element> allElements{
			get{
				foreach(Node child in all){
					if(child is Element){
						yield return child as Element;
					}
				}
			}
		}
	
		/// <summary>All HTML elements that are children of this element (not like element.all, which is all nodes including text).</summary>
		public IEnumerable<PowerUI.HtmlElement> allHtml{
			get{
				foreach(Node child in all){
					if(child is PowerUI.HtmlElement){
						yield return child as PowerUI.HtmlElement;
					}
				}
			}
		}
		
	}
	
}