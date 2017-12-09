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

namespace PowerUI{
	
	/// <summary>
	/// Represents the bgslice element. It's used by border-image as a virtual element.
	/// It essentially takes an image and "slices" it into 9 parts. Internally their just divs
	/// each focused on different parts of the image.
	/// </summary>
	
	[Dom.TagName("bgslice")]
	public class HtmlBgSliceElement:HtmlElement{
		
		/// <summary>The image in use.</summary>
		public string Image;
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="src"){
				
				// Image src to use.
				string src=getAttribute("src");
				
				if(src==null){
					src="";
				}
				
				Image=src;
				
				if(childNodes_!=null){
					
					// Kids have already been created.
					
					// For each of the 9 'cells':
					for(int i=0;i<9;i++){
						
						// Get the child:
						HtmlElement child=childNodes_[i] as HtmlElement ;
						
						if(child==null){
							continue;
						}
						
						// Change the image:
						child.style.backgroundImage="url(\""+src.Replace("\"","\\\"")+"\")";
						
					}
					
				}
				
				return true;
			}
			
			return false;
		}
		
		public override void OnChildrenLoaded(){
			
			// Let's create the 9 grid cells now.
			// Each one targets a different region of the image.
			RecreateCells();
			
			// Base:
			base.OnChildrenLoaded();
			
		}
		
		/// <summary>Rebuilds the underlying set of 9 cells.</summary>
		public void RecreateCells(){
		
			// This creates 9 cells. Their just inline-block divs. All share the following:
			string image="display:inline-block;background-image:url(\""+Image.Replace("\"","\\\"")+"\");";
			
			string cells="";
			
			for(int i=0;i<9;i++){
				
				// Create the cells HTML:
				cells+="<div style='"+image+"'></div>";
				
			}
			
			innerHTML=cells;
			
		}
		
	}
	
}