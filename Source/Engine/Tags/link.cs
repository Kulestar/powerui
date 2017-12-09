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
	/// Handles the link tag commonly used for linking external style sheets.
	/// Note that this isn't for clickable links - that's the a tag as defined in html.
	/// The href must end in .css, or either rel="stylesheet" or type="text/css" must be defined.
	/// Otherwise, this tag is ignored by PowerUI.
	/// </summary>
	
	[Dom.TagName("link")]
	public class HtmlLinkElement:HtmlElement{
		
		/// <summary>True if this links to CSS.</summary>
		public bool IsCSS;
		/// <summary>The external path to the CSS.</summary>
		public string Href;
		/// <summary>The stylesheet that this is loading.</summary>
		public Css.StyleSheet styleSheet=null;
		
		
		/// <summary>Represents whether the link is disabled.</summary>
		public bool disabled{
			get{
				return GetBoolAttribute("disabled");
			}
			set{
				SetBoolAttribute("disabled",value);
			}
		}
		
		/// <summary>The href attribute.</summary>
		public string href{
			get{
				return getAttribute("href");
			}
			set{
				setAttribute("href", value);
			}
		}
		
		/// <summary>The media attribute.</summary>
		public string media{
			get{
				return getAttribute("media");
			}
			set{
				setAttribute("media", value);
			}
		}
		
		/// <summary>The referrerpolicy attribute.</summary>
		public string referrerPolicy{
			get{
				return getAttribute("referrerpolicy");
			}
			set{
				setAttribute("referrerpolicy", value);
			}
		}
		
		/// <summary>The rel attribute.</summary>
		public string rel{
			get{
				return getAttribute("rel");
			}
			set{
				setAttribute("rel", value);
			}
		}
		
		/// <summary>The set of rel values.</summary>
		public DOMTokenList relList{
			get{
				return new DOMTokenList(this,"rel");
			}
		}
		
		/*
		public DOMSettableTokenList sizes{
			get{
				return new DOMSettableTokenList(this,"sizes");
			}
		}
		*/
		
		/// <summary>The stylesheet that this is loading.</summary>
		public Css.StyleSheet sheet{
			get{
				return styleSheet;
			}
		}
		
		/// <summary>The target attribute.</summary>
		public string target{
			get{
				return getAttribute("target");
			}
			set{
				setAttribute("target", value);
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
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & (HtmlTreeMode.InHead | HtmlTreeMode.InBody | HtmlTreeMode.InTemplate | HtmlTreeMode.InHeadNoScript))!=0){
				
				// Append it. DO NOT push to the stack:
				lexer.Push(this,false);
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="rel"){
				string rel=getAttribute("rel");
				if(rel!=null){
					if((rel.Trim().ToLower())=="stylesheet"){
						IsCSS=true;
					}
				}
				LoadContent();
			}else if(property=="type"){
				string type=getAttribute("type");
				if(type!=null){
					if((type.Trim().ToLower())=="text/css"){
						IsCSS=true;
					}
				}
				LoadContent();
			}else if(property=="href"){
				Href=getAttribute("href");
				if(!IsCSS){
					IsCSS=Href.ToLower().EndsWith(".css");
				}
				LoadContent();
			}
			
			return false;
		}
		
		/// <summary>Loads external CSS if a href is available and it's known to be css.</summary>
		public void LoadContent(){
			if(!IsCSS || string.IsNullOrEmpty(Href) || styleSheet!=null){
				return;
			}
			
			// Let's go get it!
			styleSheet=htmlDocument.AddStyle(this,null);
			
			DataPackage package=new DataPackage(Href,document.basepath);
			styleSheet.Location=package.location;
			
			package.onload=delegate(UIEvent e){
				
				if(document==null || styleSheet==null){
					return;
				}
				
				// The element is still somewhere on the UI.
				
				// Load it now:
				styleSheet.ParseCss(package.responseText);
				
				// Redraw:
				htmlDocument.RequestLayout();
				
			};
			
			package.send();
			
		}
		
	}
	
}