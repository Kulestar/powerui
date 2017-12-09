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
using UnityEngine;
using Dom;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Represents a HTML Document. UI.document is the main UI document.
	/// Use PowerUI.Document.innerHTML to set it's content.
	/// </summary>

	public partial class HtmlDocument : ReflowDocument{
		
		/// <summary>The HTML element of the document.
		/// This is the outermost tag of the document.</summary>
		public HtmlHtmlElement html;
		
		/// <summary>The body element of the document.
		/// It's contained within the html element.</summary>
		public HtmlBodyElement body;
		
		/// <summary>True if the doc is currently open.</summary>
		internal bool IsOpen;
		/// <summary>The window that this document belongs to.</summary>
		public Window window;
		/// <summary>A unique path for this document. It's usually the document location
		/// unless this document was loaded from an asset file 
		/// (in which case it's a path starting with Assets).</summary>
		private string cachePath_;
		/// <summary>A unique path for this document. It's usually the document location
		/// unless this document was loaded from an asset file 
		/// (in which case it's a path starting with Assets).</summary>
		public string cachePath{
			get{
				if(cachePath_==null){
					if(document.location==null){
						return null;
					}
					
					return document.location.absoluteNoHash;
				}
				
				return cachePath_;
			}
			set{
				cachePath_=value;
			}
		}
		/// <summary>An event called after clear but before the new content is set.
		/// Allows hooking up of various events.</summary>
		public Action<Dom.Event> AfterClearBeforeSet;
		
		/// <summary>Don't use this directly; use location instead.</summary>
		internal override void SetLocation(Location value,bool addHistory){
			
			// Make sure it opens correctly:
			IsOpen=false;
			open();
			
			if(value==null){
				return;
			}
			
			// Clear cache path:
			cachePath=null;
			
			// Update location:
			location_=value;
			
			if(addHistory){
				// Push a history entry:
				window.history.DocumentNavigated();
			}
			
			// Update its parent document:
			value.document=this;
			
			if(AfterClearBeforeSet!=null){
				
				// Invoke during load now:
				AfterClearBeforeSet(createEvent("event","duringload"));
				
			}
			
			// Reset readyState (we don't reset resources loading because we haven't killed any that are currently loading):
			ReadyStateChange(0);
			
			// Load the html:
			DataPackage package=new DataPackage(value.absolute,null);
			
			package.onreadystatechange=delegate(Dom.Event e){
				
				if(package.readyState==4){
					
					if(package.redirectedTo!=null){	
						
						// Set redir path:
						value=package.redirectedTo;
						
						// Update its parent document:
						value.document=this;
						
					}
					
					// Apply now:
					GotDocumentContent(package.responseText,package.statusCode,false);
					
				}
				
			};
			
			// Send now:
			package.send();
			
		}
		
		/// <summary>The documents current target. It's the same as location.hash.</summary>
		public override string Target{
			get{
				return location.hash;
			}
		}
		
		/// <summary>The JS shortcut for setting location as a string.</summary>
		public void set_location(string href){
			
			// Set href:
			location.href=href;
			
		}
		
		/// <summary>Sets location without navigating to it.</summary>
		public void SetRawLocation(Location path){
			location_=path;
			path.document=this;
		}
		
		/// <summary>A custom set domain value.</summary>
		public string domain_;
		/// <summary>True if we're done parsing and setting the innerHTML of the body tag.
		/// Used to guage when the code should be compiled.</summary>
		public bool FinishedParsing;
		/// <summary>The title of the document. This originates from <title> tags.</summary>
		private string CurrentTitle;
		/// <summary>The tooltip of the document. This originates from <.. title="tooltip">. See Document.tooltip.</summary>
		private string CurrentTooltip;
		
		
		/// <summary>Creates a new document which will be rendered with a new renderer.</summary>
		public HtmlDocument():this(null,null,false){}
		
		/// <summary>Creates a new document which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer to use when rendering this document.</param>
		public HtmlDocument(Renderman renderer):this(renderer,null){}
		
		/// <summary>Creates a new document which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer to use when rendering this document.</param>
		/// <param name="parentWindow">The window that will become the parent window. Used in e.g. iframes.</param>
		public HtmlDocument(Renderman renderer,Window parentWindow):this(renderer,parentWindow,false){}
		
		/// <summary>Creates a new document which will be rendered with the given renderer.</summary>
		/// <param name="renderer">The renderer to use when rendering this document.</param>
		/// <param name="parentWindow">The window that will become the parent window. Used in e.g. iframes.</param>
		/// <param name="aot">True if this is a Nitro AOT document (used in the Editor only).</param>
		public HtmlDocument(Renderman renderer,Window parentWindow,bool aot):base(renderer==null?null:renderer.InWorldUI){
			AotDocument=aot;
			
			if(renderer==null){
				renderer=new Renderman(this);
			}
			
			Renderer=renderer;
			
			// Set the XHTML namespace:
			Namespace=Dom.HtmlLexer.XHTMLNamespace;
			
			window=new Window();
			window.SetDocument(this);
			window.parent=parentWindow;
			if(parentWindow!=null){
				window.top=parentWindow.top;
			}else{
				window.top=window;
			}
			
			// Clear style; this loads in the default stylesheet:
			ClearStyle();
			
		}
		
		/// <summary>The root style node.</summary>
		public override Dom.Element documentElement{
			get{
				return html;
			}
		}
		
		/// <summary>The head element if there is one.</summary>
		public HtmlElement head{
			get{
				return html.getElementByTagName("head") as HtmlElement ;
			}
		}
		
		/// <summary>Available width.</summary>
		public int innerWidth{
			get{
				return (int)Viewport.Width;
			}
		}
		
		/// <summary>Available height.</summary>
		public int innerHeight{
			get{
				return (int)Viewport.Height;
			}
		}
		
		/// <summary>Document domain.</summary>
		public string domain{
			get{
				if(domain_==null){
					return location.host;
				}
				
				return domain_;
			}
			set{
				domain_=value;
			}
		}
		
		/// <summary>The document origin.</summary>
		public string origin{
			get{
				return defaultView.location.origin;
			}
		}
		
		/// <summary>Writes the given html to the end of the document.</summary>
		/// <param name="text">The html to write.</param>
		public void write(string text){
			
			if(!IsOpen){
				open();
			}
			
			HtmlElement bd=body;
			
			if(bd==null){
				
				// Create an empty document now:
				HtmlElement root=createElement("html") as HtmlElement ;
				root.innerHTML="<head></head><body></body>";
				appendChild(root);
				bd=root.childNodes[1] as HtmlElement;
				
			}
			
			// Append HTML:
			bd.append(text);
			
		}
		
		/// <summary>Writes the given line to the end of the document.</summary>
		/// <param name="text">The html to write.</param>
		public void writeln(string text){
			write(text+"\n");
		}
		
		/// <summary>Closes the document.</summary>
		public void close(){
			
			if(!IsOpen){
				// Already closed.
				return;
			}
			
			// Mark as closed:
			IsOpen=false;
			
			if(!TryCompile()){
				// We're downloading code.
				// This flag lets the downloader know it needs to also attempt a TryCompile.
				FinishedParsing=true;
			}
			
			// Force a render request as required:
			RequestLayout();
			
			// Setup window:
			window.Ready();
			
			// Dispatch onload (doesn't bubble):
			
			if(body!=null){
				UIEvent e=new UIEvent("load");
				e.SetTrusted(false);
				body.dispatchEvent(e);
			}
			
		}
		
		/// <summary>Opens the document for writing.</summary>
		public void open(){
			
			if(IsOpen){
				// Already open
				return;
			}
			
			// Mark as open:
			IsOpen=true;
			
			// Clear it:
			clear();
			
		}
		
		/// <summary>A window event target if there is one.</summary>
		internal override EventTarget windowTarget{
			get{
				return window;
			}
		}
		
		/// <summary>The parent node as used by EventTarget during capture. Can be null.</summary>
		internal override EventTarget eventTargetParentNode{
			get{
				return window;
			}
		}
		
		/// <summary>Clears the document of all its content, including scripts and styles.</summary>
		public override void clear(){
			
			html=null;
			body=null;
			
			// Clear parsing:
			FinishedParsing=false;
			
			// Run onbeforeunload (always has SetTrusted):
			if(body!=null && !body.dispatchEvent(new BeforeUnloadEvent())){
				return;
			}
			
			// Clear everything:
			base.clear();
			
			// Clear scripts:
			ClearCode();
			
		}
		
		/// <summary>An overriding base target which any links will use if they have no target defined. See the HTML 'base' tag.</summary>
		public string baseTarget;
		
		/// <summary>The character set this document is using.</summary>
		public string characterSet{
			get{
				return "UTF-8";
			}
		}
		
		/// <summary>The character set this document is using.</summary>
		public string charset{
			get{
				return characterSet;
			}
		}
		
		/// <summary>The input encoding set this document is using.</summary>
		public string inputEncoding{
			get{
				return characterSet;
			}
		}
		
		/// <summary>The document background color.</summary>
		public string bgColor{
			get{
				return body.bgColor;
			}
			set{
				body.bgColor=value;
			}
		}
		
		/// <summary>The document foreground (font) color.</summary>
		public string fgColor{
			get{
				return body.fgColor;
			}
			set{
				body.fgColor=value;
			}
		}
		
		/// <summary>All forms on the document.</summary>
		public HTMLCollection forms{
			get{
				
				// Create:
				HTMLCollection set=new HTMLCollection();
				
				// Collect all forms:
				html.getElementsByTagName("form",false,set);
				
				return set;
				
			}
		}
		
		/// <summary>A reference to the window object.</summary>
		public Window defaultView{
			get{
				return window;
			}
		}
		
		/// <summary>All embeds on the document.</summary>
		public HTMLCollection embeds{
			get{
				
				// Create:
				HTMLCollection set=new HTMLCollection();
				
				// Collect all embeds:
				html.getElementsByTagName("embed",false,set);
				
				return set;
				
			}
		}
		
		/// <summary>All applets on the document.</summary>
		public HTMLCollection applets{
			get{
				
				// Create:
				HTMLCollection set=new HTMLCollection();
				
				// Collect all applets:
				html.getElementsByTagName("applet",false,set);
				
				return set;
				
			}
		}
		
		/// <summary>All images on the document.</summary>
		public HTMLCollection images{
			get{
				
				// Create:
				HTMLCollection set=new HTMLCollection();
				
				// Collect all images:
				html.getElementsByTagName("img",false,set);
				
				return set;
				
			}
		}
		
		/// <summary>All links on the document.</summary>
		public HTMLCollection links{
			get{
				
				// Create:
				HTMLCollection set=new HTMLCollection();
				
				// Collect all links:
				html.getElementsByTagName("a",false,set);
				
				return set;
				
			}
		}
		
		/// <summary>All scripts on the document.</summary>
		public HTMLCollection scripts{
			get{
				
				// Create:
				HTMLCollection set=new HTMLCollection();
				
				// Collect all links:
				html.getElementsByTagName("script",false,set);
				
				return set;
				
			}
		}
		
		/// <summary>Document directionality.</summary>
		public string dir{
			get{
				string dirValue=html.getAttribute("dir");
				
				if(string.IsNullOrEmpty(dirValue)){
					return "ltr";
				}
				
				return dirValue;
			}
			set{
				html.setAttribute("dir", value);
			}
		}
		
		/// <summary>Gets or sets design mode (content editable) on the whole document.</summary>
		public bool designMode{
			get{
				return html.contentEditable=="";
			}
			set{
				if(value){
					html.contentEditable="";
				}else{
					html.removeAttribute("contenteditable");
				}
			}
		}
		
		/// <summary>Sets the document content with a status code.
		/// Displays error info if html is blank or ErrorHandlers.CatchAll is set.</summary>
		internal void GotDocumentContent(string html,int status,bool openClose){
			
			if( status!=200 && (string.IsNullOrEmpty(html) || ErrorHandlers.CatchAll) ){
				
				// Build an error message now:
				ErrorInfo error=new ErrorInfo();
				error.document=this;
				error.Url=location;
				error.Custom=html;
				error.StatusCode=status;
				
				// Display:
				ErrorHandlers.Display(error);
				
			}else{
				
				if(openClose){
					// Full open/close cycle:
					innerHTML=html;
				}else{
					// Parse now:
					HtmlLexer lexer=new HtmlLexer(html,this);
					lexer.Parse();
					close();
				}
				
			}
			
			if(resourcesLoading<=0 && readyState!="complete"){
				
				// Fire onload now!
				ReadyStateChange(2);
				
				// Fire event:
				PowerUI.UIEvent de=new PowerUI.UIEvent("load");
				de.SetTrusted(true);
				dispatchEvent(de);
				
			}
			
		}
		
		internal override bool ResourceStatus(EventTarget package,int status){
			if( base.ResourceStatus(package,status) ){
				
				// Run the onload event if we're in an iframe:
				if(window.iframe!=null){
					
					// Dispatch to the element too (don't bubble):
					UIEvent e=new UIEvent("load");
					e.SetTrusted(false);
					window.iframe.dispatchEvent(e);
					
				}
				
				return true;
			}
			
			return false;	
		}
		
		/// <summary>All the links on a page.</summary>
		public HTMLCollection anchors{
			get{
				return links;
			}
		}
		
		/// <summary>The plugins on the page.</summary>
		public HTMLCollection plugins{
			get{
				return embeds;
			}
		}
		
		/// <summary>The location as a string.</summary>
		public string URL{
			get{
				return location.absolute;
			}
		}
		
		/// <summary>The title of the document. This originates from <title> tags.</summary>
		public override string title{
			get{
				return CurrentTitle;
			}
			set{
				CurrentTitle=value;
				Dom.Event e=new Dom.Event("titlechange");
				e.SetTrusted();
				dispatchEvent(e);
			}
		}
		
		/// <summary>The tooltip of the document. This originates from <.. title="tooltip">.
		/// Note that this is set internally.</summary>
		public string tooltip{
			get{
				return CurrentTooltip;
			}
			set{
				if(CurrentTooltip==value){
					return;
				}
				
				CurrentTooltip=value;
				Dom.Event e=new Dom.Event("tooltipchange");
				e.SetTrusted();
				dispatchEvent(e);
			}
		}
		
		/// <summary>Gets or sets the innerHTML of this document.</summary>
		public override string innerML{
			get{
				return innerHTML;
			}
			set{
				innerHTML=value;
			}
		}
		
		public override string ToString(){
			return "[object HtmlDocument]";
		}
		
		/// <summary>Gets or sets the innerHTML of this document.</summary>
		public string innerHTML{
			get{
				System.Text.StringBuilder builder=new System.Text.StringBuilder();
				ToString(builder);
				return builder.ToString();
			}
			set{
				// Open parse and close:
				IsOpen=false;
				open();
				
				if(AfterClearBeforeSet!=null){
					
					// Invoke during load now:
					AfterClearBeforeSet(createEvent("event","duringload"));
					
				}
				
				// Parse now:
				HtmlLexer lexer=new HtmlLexer(value,this);
				lexer.Parse();
				
				// Dom loaded:
				ContentLoadedEvent();
				
				close();
			}
		}
		
	}
	
}