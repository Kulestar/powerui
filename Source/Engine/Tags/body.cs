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
	/// Represents the body tag. Note that this is added automatically by PowerUI and isn't required.
	/// </summary>
	
	[TagName("body")]
	public class HtmlBodyElement:HtmlElement{
		
		/// <summary>The bgcolor attribute.</summary>
		public string bgColor{
			get{
				return getAttribute("bgcolor");
			}
			set{
				setAttribute("bgcolor", value);
			}
		}
		
		/// <summary>The fgcolor attribute.</summary>
		public string fgColor{
			get{
				return getAttribute("fgcolor");
			}
			set{
				setAttribute("fgcolor", value);
			}
		}
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public override int SetLexerMode(bool last,HtmlLexer lexer){
			
			return HtmlTreeMode.InBody;
			
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
			
			if(mode==HtmlTreeMode.AfterHead){
				
				// Add and push:
				lexer.Push(this,true);
				
				// Clear frames:
				lexer.FramesetOk=false;
				
				// Switch:
				lexer.CurrentMode=HtmlTreeMode.InBody;
				
			}else if(mode==HtmlTreeMode.InBody){
				
				// 2nd element open should be a body tag:
				if(lexer.OpenElements.Count<=1 || lexer.TagCurrentlyOpen("template") || lexer.OpenElements[1].Tag!="body"){
					// Ignore it.
				}else{
					
					// Clear frames ok flag:
					lexer.FramesetOk=false;
					
					// Combine the attributes:
					lexer.CombineInto(this,lexer.OpenElements[1]);
					
				}
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>Cases in which the close tag should be ignored.</summary>
		internal const int IgnoreClose=HtmlTreeMode.InCaption
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow
		| HtmlTreeMode.InCell
		| HtmlTreeMode.InTable;
		
		/// <summary>Called when a close tag of this element has 
		/// been created and is being added to the given lexer.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				// Check if the stack contains elements that aren't allowed to still be open:
				lexer.CheckAfterBodyStack();
				
				// Ok! (It throws a fatal error otherwise)
				// Note that the spec doesn't actually tell us to close the body element.
				// Just change to after body:
				lexer.CurrentMode=HtmlTreeMode.AfterBody;
				
			}else if(mode==HtmlTreeMode.InHead){
				
				// Use anything else method:
				lexer.InHeadElse(null,"body");
				
			}else if(mode==HtmlTreeMode.AfterHead){
				
				// Use anything else method:
				lexer.AfterHeadElse(null,"body");
				
			}else if(mode==HtmlTreeMode.BeforeHtml){
				
				// Allowed to fall through the 'anything else' case:
				lexer.BeforeHtmlElse(null,"body");
				
			}else if((mode & IgnoreClose)!=0){
				
				// Just ignore it/ do nothing.
			
			}else{
				
				return false;
			
			}
			
			return true;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="bgcolor"){
				
				Style.Computed.ChangeTagProperty(
					"background-color",
					new Css.Units.ColourUnit(
						Css.ColourMap.ToSpecialColour(getAttribute(property))
					)
				);
				
				return true;
			}else if(property=="fgcolor"){
				
				Style.Computed.ChangeTagProperty(
					"color",
					new Css.Units.ColourUnit(
						Css.ColourMap.ToSpecialColour(getAttribute(property))
					)
				);
				
				return true;
			}
			
			return false;
		}
		
		public override void OnTagLoaded(){
			HtmlDocument doc=htmlDocument;
			
			if(doc==null || doc.body!=null){
				return;
			}
			
			doc.body=this;
		}
		
	}
	
}