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
using Css;
using Dom;
using System.Text;


namespace PowerUI{

	/// <summary>
	/// Handles the style tag containing inline css.
	/// Use the link tag if you wish to add external css.
	/// </summary>

	[Dom.TagName("style")]
	public class HtmlStyleElement:HtmlElement{
		
		/// <summary>The stylesheet.</summary>
		private Css.StyleSheet StyleSheet_;
		
		
		/// <summary>The media attribute.</summary>
		public string media{
			get{
				return getAttribute("media");
			}
			set{
				setAttribute("media", value);
			}
		}
		
		/// <summary>The type attribute.</summary>
		public string type{
			get{
				return getAttribute("type");
			}
			set{
				setAttribute("type", value);
			}
		}
		
		/// <summary>True if this element is disabled.</summary>
		public bool disabled{
			get{
				return GetBoolAttribute("disabled");
			}
			set{
				SetBoolAttribute("disabled",value);
			}
		}
		
		/// <summary>The stylesheet that this is loading.</summary>
		public Css.StyleSheet sheet{
			get{
				return StyleSheet_;
			}
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & (HtmlTreeMode.InHead | HtmlTreeMode.InBody | HtmlTreeMode.InTemplate | HtmlTreeMode.InTable | HtmlTreeMode.InHeadNoScript))!=0){
				
				// Add as text:
				lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.Rawtext);
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		public override void OnChildrenLoaded(){
			
			// Add to the documents style:
			Node node=firstChild;
			
			if(node!=null){
				StyleSheet_=htmlDocument.AddStyle(this,node.textContent);
			}
			
		}
		
	}
	
}