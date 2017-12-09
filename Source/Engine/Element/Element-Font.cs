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
using Dom;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// This function is called when a @font-face font is done loading.
	/// </summary>

	public partial class HtmlElement{
		
		/// <summary>Called when a @font-face font is done loading.</summary>
		public void FontLoaded(DynamicFont font){
			
			if(childNodes_==null){
				return;
			}
			
			int count=childNodes_.length;
			
			for(int i=0;i<count;i++){
				IRenderableNode node=(childNodes_[i] as IRenderableNode);
				
				if(node==null){
					continue;
				}
				
				node.FontLoaded(font);
			}
			
		}
		
	}
	
}