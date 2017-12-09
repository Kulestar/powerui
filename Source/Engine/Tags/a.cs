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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using Css;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a clickable link. Note that target is handled internally by the http protocol.
	/// </summary>
	
	[Dom.TagName("a")]
	public class HtmlAnchorElement:HtmlElement{
		
		/// <summary>The target location that should be loaded when clicked.</summary>
		private Location Href_;
		
		
		public HtmlAnchorElement(){
			// Make sure this element is focusable:
			IsFocusable=true;
		}
		
		/// <summary>The hash of the href.</summary>
		public string hash{
			get{
				return GetLocation().hash;
			}
			set{
				GetLocation().hash=value;
			}
		}
		
		/// <summary>The host of the href.</summary>
		public string host{
			get{
				return GetLocation().host;
			}
			set{
				GetLocation().host=value;
			}
		}
		
		/// <summary>The hostname of the href.</summary>
		public string hostname{
			get{
				return GetLocation().hostname;
			}
			set{
				GetLocation().hostname=value;
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
		
		/// <summary>The name attribute.</summary>
		public string name{
			get{
				return getAttribute("name");
			}
			set{
				setAttribute("name", value);
			}
		}
		
		/// <summary>The password from the href.</summary>
		public string password{
			get{
				return GetLocation().password;
			}
			set{
				GetLocation().password=value;
			}
		}
		
		/// <summary>The origin from the href.</summary>
		public string origin{
			get{
				return GetLocation().origin;
			}
		}
		
		/// <summary>The pathname from the href.</summary>
		public string pathname{
			get{
				return GetLocation().pathname;
			}
			set{
				GetLocation().pathname=value;
			}
		}
		
		/// <summary>The port from the href.</summary>
		public string port{
			get{
				return GetLocation().port;
			}
			set{
				GetLocation().port=value;
			}
		}
		
		/// <summary>The protocol from the href.</summary>
		public string protocol{
			get{
				return GetLocation().protocol;
			}
			set{
				GetLocation().protocol=value;
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
		
		/// <summary>The query string of the href.</summary>
		public string search{
			get{
				return GetLocation().search;
			}
			set{
				GetLocation().search=value;
			}
		}
		
		/// <summary>The tabindex of this element.</summary>
		public long tabindex{
			get{
				return tabIndex;
			}
			set{
				tabIndex=(int)value;
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
		
		/// <summary>Alias for textContent.</summary>
		public string text{
			get{
				return textContent;
			}
			set{
				textContent=value;
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
		
		/// <summary>The username from the href.</summary>
		public string username{
			get{
				return GetLocation().username;
			}
			set{
				GetLocation().username=value;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				Element node=lexer.FormattingCurrentlyOpen("a");
				
				if(node!=null){
					
					// (parse error)
					lexer.AdoptionAgencyAlgorithm("a");
					
					lexer.CloseNode(node);
					lexer.FormattingElements.Remove(node);
					
				}
				
				lexer.AddFormattingElement(this);
				
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
				
				lexer.AdoptionAgencyAlgorithm("a");
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="href"){
				Href_=null;
				return true;
			}
			
			return false;
		}
		
		/// <summary>Gets the location now.</summary>
		private Location GetLocation(){
			
			if(Href_!=null){
				return Href_;
			}
			
			// The target:
			string href=getAttribute("href");
			
			#if MOBILE || UNITY_METRO
			// First, look for <source> elements.
			
			// Grab the kids:
			NodeList kids=childNodes_;
			
			if(kids!=null){
				// For each child, grab it's src value. Favours the most suitable protocol for this platform (e.g. market:// on android).
				
				foreach(Node child in kids){
					
					HtmlElement el=child as HtmlElement ;
					
					if(el==null || el.Tag!="source"){
						continue;
					}
					
					// Grab the src:
					string childSrc=el.getAttribute("src");
					
					if(childSrc==null){
						continue;
					}
					
					// Get the optional type - it can be Android,W8,IOS,Blackberry:
					string type=el.getAttribute("type");
					
					if(type!=null){
						type=type.Trim().ToLower();
					}
					
					#if UNITY_ANDROID
						
						if(type=="android" || childSrc.StartsWith("market:")){
							
							href=childSrc;
							
						}
						
					#elif UNITY_WP8 || UNITY_METRO
					
						if(type=="w8" || type=="wp8" || type=="windows" || childSrc.StartsWith("ms-windows-store:")){
							
							href=childSrc;
							
						}
						
					#elif UNITY_IPHONE
						
						if(type=="ios" || childSrc.StartsWith("itms:") || childSrc.StartsWith("itms-apps:")){
							
							href=childSrc;
							
						}
						
						
					#endif
					
				}
				
			}
			
			#endif
			
			if(href==null){
				// Refresh.
				href="";
			}
			
			Href_=new Location(href,document.basepath);
			return Href_;
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			// Time to go to our Href.
			Location path=GetLocation();
			
			// Do we have a file protocol handler available?
			FileProtocol fileProtocol=path.Handler;
			
			if(fileProtocol!=null){
				fileProtocol.OnFollowLink(this,path);
			}
			
		}
		
	}
	
}