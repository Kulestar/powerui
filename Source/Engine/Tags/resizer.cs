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
	/// Handles the resizer tab (appears when two scrollbars are visible, but can also be manually added).
	/// </summary>
	
	[Dom.TagName("resizer")]
	public class HtmlResizerElement:HtmlElement{
		
		/// <summary>Originates from the resize CSS property.</summary>
		private bool AllowX;
		/// <summary>Originates from the resize CSS property.</summary>
		private bool AllowY;
		/// <summary>The starting mouse values.</summary>
		public float PositionX;
		/// <summary>The starting mouse values.</summary>
		public float PositionY;
		/// <summary>A custom element being resized when this is dragged.</summary>
		public HtmlElement ToResize;
		/// <summary>The actual being resized when this is dragged.</summary>
		private HtmlElement ToResize_;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>The distance the input pointer must move in order to start dragging this element.</summary>
		public override float DragStartDistance{
			get{
				// Resizers start dragging immediately:
				return 1f;
			}
		}
		
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(base.HandleLocalEvent(e,bubblePhase)){
				// It was blocked. Don't run the default.
				return true;
			}
			
			if(e.type=="dragstart"){
				
				if(ToResize==null){
					ToResize_=parentElement as HtmlElement;
				}else{
					ToResize_=ToResize;
				}
				
				// Get the CSS resize property:
				if(ToResize_!=null){
					
					// Obtain its values:
					Css.Properties.Resize.Compute(ToResize_.ComputedStyle,out AllowX,out AllowY);
					
					// Does it explicitly change the resize target?
					while(ToResize_!=null){
						
						// Get the target attrib:
						string attr=ToResize_.getAttribute("resize-target");
						
						if(attr!=null){
							
							if(attr=="parent"){
								
								// Update to resize:
								ToResize_=ToResize_.parentElement as HtmlElement;
								
								// Loop again; that might also specify a target.
								continue;
								
							}
						}
						
						break;
						
					}
					
				}
				
			}
			
			return false;
		}
		
		/// <summary>Called when the thumb is being dragged.</summary>
		public override bool OnDrag(PowerUI.DragEvent mouseEvent){
			
			// Get the amount of pixels the pointer moved by:
			float deltaX=AllowX ? mouseEvent.deltaX : 0f;
			float deltaY=AllowY ? mouseEvent.deltaY : 0f;
			
			if(deltaX==0f && deltaY==0f){
				return false;
			}
			
			// Resize now!
			ComputedStyle cs=ToResize_.Style.Computed;
			
			if(deltaX!=0f){
				
				// Width is..
				Css.Value width=cs[Css.Properties.Width.GlobalProperty];
				
				// Update it:
				deltaX+=width.GetDecimal(ToResize_.RenderData,Css.Properties.Width.GlobalProperty);
				
				// Write it back out:
				cs.ChangeProperty(Css.Properties.Width.GlobalProperty,new Css.Units.DecimalUnit(deltaX));
				
			}
			
			if(deltaY!=0f){
				
				// Height is..
				Css.Value height=cs[Css.Properties.Height.GlobalProperty];
				
				// Update it:
				deltaY+=height.GetDecimal(ToResize_.RenderData,Css.Properties.Height.GlobalProperty);
				
				// Write it back out:
				cs.ChangeProperty(Css.Properties.Height.GlobalProperty,new Css.Units.DecimalUnit(deltaY));
				
			}
			
			return false;
			
		}
		
		public override void OnChildrenLoaded(){
			
			// We're always draggable:
			setAttribute("draggable", "1");
			
		}
		
	}
	
}