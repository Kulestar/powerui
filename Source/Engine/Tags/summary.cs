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
	/// Represents a HTML5 summary element.
	/// </summary>
	
	[Dom.TagName("summary")]
	public class HtmlSummaryElement:HtmlElement{
		
		/// <summary>The details element this affects when clicked.</summary>
		public HtmlElement Details;
		
		
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
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.CloseParagraphThenAdd(this);
				
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
				lexer.BlockClose("summary");
			}else{
				return false;
			}
			
			return true;
			
		}
		
		public override void OnClickEvent(MouseEvent clickEvent){
			
			if(Details==null){
				return;
			}
				
			// Hide/show the details element.
			
			// Grab the details computed style:
			ComputedStyle computed=Details.Style.Computed;
			
			// The display it's going to:
			string display;
			
			// Is it currently visible?
			if(computed.DisplayX==DisplayMode.None){
				// Nope! Show it.
				display="block";
			}else{
				// Yep - hide it.
				display="none";
			}
			
			// Change its display:
			computed.ChangeTagProperty("display",display);
			
		}
		
	}
	
}