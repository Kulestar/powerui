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
using System.Text;

namespace PowerUI{
	
	/// <summary>
	/// Handles script tags. They should have type="text/javascript" to be handled by PowerUI.
	/// The src="" attribute is also supported if you wish to reuse script by loading it externally.
	/// </summary>
	
	[Dom.TagName("script")]
	public class HtmlScriptElement:HtmlElement{
		
		/// <summary>The script engine in use.</summary>
		public ScriptEngine Engine;
		/// <summary>The external path to this script.</summary>
		public string Src;
		/// <summary>True if the tag is fully loaded; false if it's still being parsed.</summary>
		public bool Loaded;
		/// <summary>Code index is used to place script into the code buffer so it's placed correctly relative to all other code.
		/// This is required if this script takes longer to load than script after it.</summary>
		public int CodeIndex=-1;
		
		
		/// <summary>The charset.</summary>
		public string charset{
			get{
				return getAttribute("charset");
			}
			set{
				setAttribute("charset", value);
			}
		}
		
		/// <summary>The src of the script.</summary>
		public string src{
			get{
				return getAttribute("src");
			}
			set{
				setAttribute("src", value);
			}
		}
		
		/// <summary>The type of the script.</summary>
		public string type{
			get{
				return getAttribute("type");
			}
			set{
				setAttribute("type", value);
			}
		}
		
		/// <summary>The contents of the script.</summary>
		public string text{
			get{
				return textContent;
			}
			set{
				textContent=value;
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
			
			if((mode & (HtmlTreeMode.InHead | HtmlTreeMode.InBody | HtmlTreeMode.InTemplate | HtmlTreeMode.InSelectInTable | HtmlTreeMode.InSelect | HtmlTreeMode.InTable))!=0){
				
				// Append it:
				lexer.Push(this,true);
				
				// Switch to Script data:
				lexer.PreviousMode = lexer.CurrentMode;
				lexer.CurrentMode = HtmlTreeMode.Text;
				lexer.State = HtmlParseMode.Script;
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				lexer.AfterHeadHeadTag(this);
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.Text){
				
				lexer.CloseCurrentNode();
				lexer.CurrentMode=lexer.PreviousMode;
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		public override void OnChildrenLoaded(){
			
			// Add to the Document's code but only if this tag shouldn't be dumped:
			string type=getAttribute("type");
			
			if(string.IsNullOrEmpty(type)){
				// JS is the default.
				type="text/javascript";
			}
			
			Engine=htmlDocument.GetScriptEngine(type);
			
			if(Engine!=null){
				
				Node first=firstChild;
				
				if(first!=null){
					Engine.AddCode(first.textContent);
				}
				
				// If its got an SRC, load it now:
				if(!string.IsNullOrEmpty(Src)){
					
					LoadContent();
					
				}
			}
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="type"){
				return true;
			}else if(property=="src"){
				Src=getAttribute("src");
				
				if(Engine!=null){
					LoadContent();
				}
				
				return true;
			}
			
			return false;
		}
		
		/// <summary>Sends a request off to get the content of this tag if it's external (i.e. has an src attribute).</summary>
		private void LoadContent(){
			if(!Loaded || string.IsNullOrEmpty(Src)){
				return;
			}
			
			if(Engine==null){
				return;
			}
			
			// The code index makes sure that this script is loaded into this position relative to other script on the page:
			CodeIndex=Engine.GetCodeIndex();
			
			
			DataPackage package=new DataPackage(Src,document.basepath);
			
			package.onload=delegate(UIEvent e){
				
				if(document_==null || CodeIndex<0){
					return;
				}
				// The element is still somewhere on the UI.
				string code=package.responseText;
				
				if(Engine!=null){
					Engine.AddCode(code,CodeIndex);
				}
				
				CodeIndex=-1;
				
				// We might also be the last code to download - attempt a compile now, but only if the document is done parsing.
				// If the doc isn't done parsing, it might not have added all the script to the buffer yet (and will try compile itself).
				if(htmlDocument.FinishedParsing){
					htmlDocument.TryCompile();
				}
				
			};
			
			package.send();
			
		}
		
		public override void OnTagLoaded(){
			Loaded=true;
			LoadContent();
			base.OnTagLoaded();
		}
		
	}
	
}