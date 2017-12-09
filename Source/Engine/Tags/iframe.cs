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

using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Handles iframes.
	/// Supports the src="" attribute.
	/// </summary>
	
	[Dom.TagName("iframe")]
	public class HtmlIframeElement:HtmlElement{
		
		/// <summary>The src of the page this iframe points to.</summary>
		public string Src;
		/// <summary>True if the tag for this iframe has been loaded.</summary>
		public bool Loaded;
		/// <summary>The document in this iframe.</summary>
		public HtmlDocument ContentDocument;
		
		
		/// <summary>The align attribute.</summary>
		public string align{
			get{
				return getAttribute("align");
			}
			set{
				setAttribute("align", value);
			}
		}
		
		/// <summary>The content window.</summary.
		public Window contentWindow{
			get{
				return ContentDocument.window;
			}
		}
		
		/// <summary>The height attribute.</summary>
		public string height{
			get{
				return getAttribute("height");
			}
			set{
				setAttribute("height", value);
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
		
		/// <summary>The frameborder attribute.</summary>
		public string frameBorder{
			get{
				return getAttribute("frameborder");
			}
			set{
				setAttribute("frameborder", value);
			}
		}
		
		/// <summary>The marginheight attribute.</summary>
		public string marginHeight{
			get{
				return getAttribute("marginheight");
			}
			set{
				setAttribute("marginheight", value);
			}
		}
		
		/// <summary>The marginwidth attribute.</summary>
		public string marginWidth{
			get{
				return getAttribute("marginwidth");
			}
			set{
				setAttribute("marginwidth", value);
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
		
		/// <summary>The src attribute.</summary>
		public string src{
			get{
				return getAttribute("src");
			}
			set{
				setAttribute("src", value);
			}
		}
		
		/// <summary>The width attribute.</summary>
		public string width{
			get{
				return getAttribute("width");
			}
			set{
				setAttribute("width", value);
			}
		}
		
		/*
		public DOMSettableTokenList sandbox{
			get{
				return new DOMSettableTokenList(this,"sandbox");
			}
		}
		*/
		
		/// <summary>Reloads the iframe.</summary>
		public void reload(){
			contentWindow.location.reload();
		}
		
		/// <summary>Indicates whether it's possible to navigate backwards</summary>
		public bool getCanGoBack(){
			return contentWindow.history.canGoBack;
		}
		
		/// <summary>Goes to the previous location in its browsing history.</summary>
		public void goBack(){
			contentWindow.history.back();
		}
		
		/// <summary>Indicates whether it's possible to navigate forwards</summary>
		public bool getCanGoForward(){
			return contentWindow.history.canGoForward;
		}
		
		/// <summary>Goes to the next location in its browsing history.</summary>
		public void goForward(){
			contentWindow.history.forward();
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.FramesetOk=false;
				
				lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.Rawtext);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				Src=getAttribute("src");
				LoadContent();
			}else{
				return false;
			}
			
			return true;
		}
		
		/// <summary>Loads the content of this iframe now.</summary>
		private void LoadContent(){
			if(!Loaded || string.IsNullOrEmpty(Src)){
				return;
			}
			
			Location parentLocation=null;
			
			if(parentNode!=null){
				parentLocation=document.basepath;
			}
			
			// Load now:
			ContentDocument.location=new Location(Src,parentLocation);
			
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
			// Update viewport:
			if(ContentDocument==null){
				return;
			}
			
			ContentDocument.Viewport.Height=box.InnerHeight;
			ContentDocument.Viewport.Width=box.InnerWidth;
			
		}
		
		public float InnerWidth{
			get{
				
				return Style.Computed.ResolveDecimal(Css.Properties.Width.GlobalProperty);
				
			}
		}
		
		public float InnerHeight{
			get{
				
				return Style.Computed.ResolveDecimal(Css.Properties.Height.GlobalProperty);
				
			}
		}
		
		public override void OnChildrenLoaded(){
			Loaded=true;
			
			// Iframes generate a new document object for isolation purposes:
			ContentDocument=new HtmlDocument(htmlDocument.Renderer,htmlDocument.window);
			
			// Setup the iframe ref:
			ContentDocument.window.iframe=this;
			
			// Append the document as a child of the iframe:
			appendChild(ContentDocument);
			
			LoadContent();
			
			// And handle style/ other defaults:
			base.OnTagLoaded();
		}
		
	}
	
}