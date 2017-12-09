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
	/// Handles the rb tag.
	/// </summary>

	[Dom.TagName("rb")]
	public class HtmlRbElement:HtmlElement{
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				if(lexer.IsInScope("ruby")){
					
					lexer.GenerateImpliedEndTags();
					
				}
				
				lexer.Push(this,true);
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>True if an implicit end is allowed.</summary>
		public override ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.Normal;
			}
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public override bool OkToBeOpenAfterBody{
			get{
				return true;
			}
		}
		
	}
	
}