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

using PowerUI;


namespace Dom{
	
	public partial class NodeList{
			
		public HtmlElement htmlItem(int index){
			return values[index] as HtmlElement;
		}
		
	}
	
	public partial class HTMLCollection{
			
		public HtmlElement htmlItem(int index){
			return values[index] as HtmlElement;
		}
		
	}
	
}