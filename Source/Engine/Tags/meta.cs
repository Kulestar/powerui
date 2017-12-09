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
	/// Handles the meta tag. These are essentially just ignored by PowerUI.
	/// </summary>
	
	[Dom.TagName("meta")]
	public class HtmlMetaElement:HtmlElement{
	
		/// <summary>The meta-data attribute.</summary>
		public string content{
			get{
				return getAttribute("meta-data");
			}
			set{
				setAttribute("meta-data", value);
			}
		}
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override void OnTagLoaded(){
			
			string name=getAttribute("name");
			string content;
			
			if(name=="languages"){
				
				// Localization tag incoming!
				
				// Get the document.languages API:
				Languages languages=htmlDocument.languages;
				
				// Got a custom location?
				string location=getAttribute("src");
				
				if(location!=null){
					languages.location=location;
				}
				
				// Available languages:
				content=getAttribute("content");
				
				if(content!=null){
					
					// Create each language now:
					string[] codes=content.Split(',');
					
					Language[] set=new Language[codes.Length];
					
					for(int i=0;i<codes.Length;i++){
						
						// Get it from the API:
						set[i]=languages.get(codes[i]);
						
					}
					
					// Apply to the all available var:
					languages.all_=set;
					
				}
				
			}
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & (HtmlTreeMode.InHead | HtmlTreeMode.InBody | HtmlTreeMode.InTemplate | HtmlTreeMode.InHeadNoScript))!=0){
				
				// Append it. DO NOT push to the stack:
				lexer.Push(this,false);
				
				// Should check for encoding here.
				// http://w3c.github.io/html/syntax.html#the-in-head-insertion-mode
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
	}
	
}