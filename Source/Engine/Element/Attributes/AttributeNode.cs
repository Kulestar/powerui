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
using Dom;


namespace Dom{
	
	/// <summary>
	/// Represents an attribute on an element.
	/// Only ever used on request; not used internally.
	/// (You can actually delete this class and it's associated functions, which are in the folder alongside it).
	/// </summary>
	
	public class AttributeNode:Node{
		
		/// <summary>The name of the attribute.</summary>
		private string UnprefixedName;
		/// <summary>The value of the attribute.</summary>
		public string value;
		/// <summary>True if this is an ID node.</summary>
		public bool isId;
		/// <summary>Was this attribute specified?</summary>
		public bool specified;
		/// <summary>Owning element.</summary>
		public Element ownerElement;
		
		
		public AttributeNode(Node owner,string name,string val){
			UnprefixedName=name;
			ownerElement=owner as Element;
			specified=(val!=null);
			value=val;
		}
		
		public AttributeNode(Node owner,string name,string val,string ns){
			UnprefixedName=name;
			Namespace=MLNamespaces.GetPrefix(ns);
			ownerElement=owner as Element;
			specified=(val!=null);
			value=val;
		}
		
		/// <summary>The name without the prefix.</summary>
		public string name{
			get{
				return UnprefixedName;
			}
		}
		
		/// <summary>The name without the prefix.</summary>
		public override string localName{
			get{
				return UnprefixedName;
			}
		}
		
		/// <summary>The nodes full name, accounting for namespace.</summary>
		public override string nodeName{
			get{
				if(Namespace==null){
					return UnprefixedName;
				}
				
				return Namespace.Prefix+":"+UnprefixedName;
			}
		}
		
		/// <summary>The node type.</summary>
		public override ushort nodeType{
			get{
				return 2;
			}
		}
		
		/// <summary>The value.</summary>
		public override string nodeValue{
			get{
				return value;
			}
			set{
				this.value=value;
			}
		}
		
		/// <summary>The value.</summary>
		public override string textContent{
			get{
				return value;
			}
			set{
				this.value=value;
			}
		}
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public override bool isEqualNode(Node other){
			if(other==this){
				return true;
			}
			AttributeNode t=other as AttributeNode;
			return t!=null && t.name==name && t.nodeValue==nodeValue;
		}
		
	}
	
}