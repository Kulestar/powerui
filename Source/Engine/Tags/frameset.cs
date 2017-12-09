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
	/// Handles the frameset element.
	/// </summary>
	
	[Dom.TagName("frameset")]
	public class HtmlFramesetElement:HtmlElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,Dom.HtmlLexer lexer){
			
			return Dom.HtmlTreeMode.InFrameset;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.AfterHead){
				
				// Add and push:
				lexer.Push(this,true);
				
				// Switch:
				lexer.CurrentMode=HtmlTreeMode.InFrameset;
			
			}else if(mode==HtmlTreeMode.InFrameset){
				
				lexer.Push(this,true);
				
			}else if(mode==HtmlTreeMode.InBody){
				
				if(lexer.OpenElements.Count<=1 || !lexer.FramesetOk || lexer.OpenElements[1].Tag!="body"){
					// Ignore it.
				}else{
					
					// Remove the body element from its parent node.
					Element body=lexer.OpenElements[1];
					
					if(body.parentNode_!=null){
						body.parentNode_.removeChild(body);
					}
					
					// Pop all but the html tag.
					while (lexer.OpenElements.Count > 1){
						lexer.CloseCurrentNode();
					}
					
					// Add and switch:
					lexer.Push(this,true);
					
					lexer.CurrentMode = HtmlTreeMode.InFrameset;
					
				}
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InFrameset){
				
				if(lexer.CurrentElement.Tag!="html"){
					// Ignore otherwise
					
					lexer.CloseCurrentNode();
					
					if(lexer.CurrentElement.Tag!="frameset"){
						
						lexer.CurrentMode=HtmlTreeMode.AfterFrameset;
						
					}
					
				}
				
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
	}
	
}