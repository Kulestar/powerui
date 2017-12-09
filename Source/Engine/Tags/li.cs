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

using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a standard list entry element.
	/// </summary>
	
	[Dom.TagName("li")]
	public class HtmlLiElement:HtmlElement{
		
		/// <summary>The type.</summary>
		public string type{
			get{
				return Style.Computed.GetString("list-style-type");
			}
			set{
				Style.Set("list-style-type",value);
			}
		}
		
		/// <summary>The ordinal position of the list element.</summary>
		public long Value{
			get{
				long v;
				long.TryParse(getAttribute("value"),out v);
				return v;
			}
			set{
				setAttribute("value", value.ToString());
			}
		}
		
		/// <summary>Ordinal text for this list element (prefixed).</summary>
		public string Ordinal{
			get{
				return HtmlOListElement.GetOrdinal(Style.Computed,true);
			}
		}
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Normal;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.FramesetOk=false;
				
				int index=lexer.OpenElements.Count-1;
				Element node=lexer.OpenElements[index];
				
				while(true){
					
					if(node is HtmlLiElement){
					
						lexer.GenerateImpliedEndTagsExceptFor("li");
						
						lexer.CloseInclusive("li");
						
						break;
						
					}else if(node.IsSpecial){
						
						string tag=node.Tag;
						
						if(tag!="p" && tag!="address" && tag!="div"){
							break;
						}
						
					}
					
					// Next one:
					index--;
					
					if(index<0){
						break;
					}
					
					node=lexer.OpenElements[index];
				}
				
				// Close paragraphs and append:
				lexer.CloseParagraphButtonScope();
				lexer.Push(this,true);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				if(lexer.IsInListItemScope("li")){
					
					lexer.GenerateImpliedEndTagsExceptFor("li");
					
					lexer.CloseInclusive("li");
					
				}
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}