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
	/// Represents a HTML area element.
	/// </summary>
	
	[Dom.TagName("area")]
	public class HtmlAreaElement:HtmlElement{
		
		/// <summary>The target location that should be loaded when clicked.</summary>
		private Location Href_;
		
		
		/// <summary>Gets the location now.</summary>
		private Location GetLocation(){
			
			if(Href_!=null){
				return Href_;
			}
			
			// The target:
			string href=getAttribute("href");
			
			if(href==null){
				// Refresh.
				href="";
			}
			
			Href_=new Location(href,document.basepath);
			return Href_;
		}
		
		/// <summary>The accessKey attribute.</summary>
		public string accessKey{
			get{
				return getAttribute("accesskey");
			}
			set{
				setAttribute("accesskey", value);
			}
		}
		
		/// <summary>The alt attribute.</summary>
		public string alt{
			get{
				return getAttribute("alt");
			}
			set{
				setAttribute("alt", value);
			}
		}
		
		/// <summary>The coords attribute.</summary>
		public string coords{
			get{
				return getAttribute("coords");
			}
			set{
				setAttribute("coords", value);
			}
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
		
		/// <summary>The hreflang attribute.</summary>
		public string hreflang{
			get{
				return getAttribute("hreflang");
			}
			set{
				setAttribute("hreflang", value);
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
		
		/// <summary>The query string of the href.</summary>
		public string search{
			get{
				return GetLocation().search;
			}
			set{
				GetLocation().search=value;
			}
		}
		
		/// <summary>The shape attribute.</summary>
		public string shape{
			get{
				return getAttribute("shape");
			}
			set{
				setAttribute("shape", value);
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
				
				// Always self closing:
				lexer.Push(this,false);
				
				// Also blocks framesets though:
				lexer.FramesetOk=false;
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>True if this element closes itself.</summary>
		public override bool IsSelfClosing{
			get{
				return true;
			}
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
			
			if(property=="href"){
				Href_=null;
				return true;
			}
			
			return false;
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