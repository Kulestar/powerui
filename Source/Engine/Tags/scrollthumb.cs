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
	/// Handles the thumb of a scrollbar.
	/// </summary>
	
	[Dom.TagName("scrollthumb")]
	public class HtmlScrollThumbElement:HtmlElement{
		
		/// <summary>True if the thumb should recalc its size during the next layout.</summary>
		public bool RecalculateNext;
		/// <summary>The starting X/Y coordinate that the thumb was located at when it was clicked on.</summary>
		public float Start;
		/// <summary>True if this is a vertical thumb.</summary>
		public bool IsVertical;
		/// <summary>The scrollbar this thumb belongs to.</summary>
		public HtmlScrollbarElement ScrollBar;
		
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>The distance the input pointer must move in order to start dragging this element.</summary>
		public override float DragStartDistance{
			get{
				// Scrollbars start dragging immediately:
				return 1f;
			}
		}
		
		public override void OnComputeBox(Renderman renderer,Css.LayoutBox thumbBox,ref bool widthUndefined,ref bool heightUndefined){
			
			if(IsVertical){
				
				if(thumbBox.Position.Top==float.MaxValue){
					
					// Initial push over the first arrow:
					thumbBox.Position.Top=StartArrowSize;
					
				}
				
			}else if(thumbBox.Position.Left==float.MaxValue){
				
				// Initial push over the first arrow:
				thumbBox.Position.Left=StartArrowSize;
				
			}
			
		}
		
		public override void OnRender(Renderman renderer){
			
			// Get the target:
			HtmlElement target=ScrollBar.scrollTarget;
			
			if(target==null){
				return;
			}
			
			ComputedStyle computed=target.style.Computed;
			LayoutBox box=computed.FirstBox;
			
			if(box==null){
				// Not visible or hasn't been drawn yet.
				return;
			}
			
			int overflowMode=IsVertical?box.OverflowY : box.OverflowX;
			float visible=IsVertical?box.VisiblePercentageY() : box.VisiblePercentageX();
			
			if(visible>1f){
				visible=1f;
			}else if(visible<0f){
				visible=0f;
			}
			
			// Handle auto next:
			if(overflowMode==VisibilityMode.Auto && visible==1f){
				
				// Hide it:
				ScrollBar.Style.display="none";
				
				// Make sure it's not scrolled:
				if(IsVertical && box.Scroll.Top!=0f){
					
					if(box.Scroll.Top!=0f){
						// Clear it:
						computed.ChangeTagProperty("scroll-top",new Css.Units.DecimalUnit(0f));
					}
					
				}else if(box.Scroll.Left!=0f){
					
					computed.ChangeTagProperty("scroll-left",new Css.Units.DecimalUnit(0f));
					
				}
				
				// Mark as hidden so the informer can watch out for it:
				ScrollBar.Hidden=true;
				
				return;
				
			}
			
			ApplyTabSize(visible,Style.Computed.FirstBox);
			
		}
		
		/// <summary>Makes the thumb a percentage size relative to the length of the bar.</summary>
		/// <param name="percentSize">A value from 0->1 that represents how visible the content
		/// is and as a result how long the thumb is.</param>
		public void ApplyTabSize(float percentSize,LayoutBox box){
			
			// How wide is the border/padding of the thumb?
			float styleSize=StyleSize;
			
			// How big should the new thumb be?
			float newSize=(percentSize*BarSize)-styleSize;
			
			if(newSize==ThumbSize){
				// It didn't change.
				return;
			}
			
			// Apply the new thumb size:
			if(IsVertical){
				
				// For this pass:
				box.InnerHeight=newSize;
				
				// Future passes:
				style.Computed.ChangeTagProperty("height",new Css.Units.DecimalUnit(newSize),false);
				
			}else{
				
				// This pass:
				box.InnerWidth=newSize;
				
				// Future passes:
				style.Computed.ChangeTagProperty("width",new Css.Units.DecimalUnit(newSize),false);
				
			}
			
		}
		
		protected override bool HandleLocalEvent(Dom.Event e,bool bubblePhase){
			
			if(e.type=="dragstart"){
				
				// Track where the bar started off:
				Start=Position;
			
			}
			
			// Handle locally:
			return base.HandleLocalEvent(e,bubblePhase);
			
		}
		
		/// <summary>The size (either width or height) of the element before the scroll thumb.</summary>
		public float StartArrowSize{
			get{
				if(ScrollBar==null){
					return 0;
				}
				
				// Grab the first child:
				Node firstChild=ScrollBar.firstChild;
				
				if(firstChild==this){
					// No button before:
					return 0;
				}
				
				// Get its box:
				LayoutBox box=(firstChild as HtmlElement).style.Computed.FirstBox;
				
				return IsVertical ? box.Height : box.Width;
			}
		}
		
		/// <summary>The size (either width or height) of the element after the scroll thumb.</summary>
		public float EndArrowSize{
			get{
				if(ScrollBar==null){
					return 0;
				}
				
				// Grab the last child:
				Node lastChild=ScrollBar.lastChild;
				
				if(lastChild==this){
					// No button after:
					return 0;
				}
				
				// Get its box:
				LayoutBox box=(lastChild as HtmlElement).style.Computed.FirstBox;
				
				return IsVertical ? box.Height : box.Width;
			}
		}
		
		/// <summary>The total border/padding size on this axis of the thumb itself.</summary>
		public float StyleSize{
			get{
				// Grab the box:
				LayoutBox box=RenderData.FirstBox;
				
				// Return the h/v style size. That's:
				return IsVertical ? box.BorderedHeight-box.InnerHeight : box.BorderedWidth-box.InnerWidth;
			}
		}
		
		/// <summary>Called when the element has been scrolled.</summary>
		/// <param name="progress">A value from 0->1 which denotes how much the content has been scrolled by.</param>
		public void ElementScrolled(float progress){
			
			// Get the position the thumb should be at (Compensating for the arrow button):
			float scrollPoint=(progress*BarSize)+StartArrowSize;
			
			// Make it relative to the current location:
			scrollPoint-=Position;
			
			// Scroll the thumb only:
			ScrollBy(scrollPoint,true,false);
		}
		
		/// <summary>Called when the thumb is being dragged.</summary>
		public override bool OnDrag(PowerUI.DragEvent mouseEvent){
			
			float delta;
			
			if(IsVertical){
				delta=mouseEvent.clientY-mouseEvent.trigger.DownDocumentY;
			}else{
				delta=mouseEvent.clientX-mouseEvent.trigger.DownDocumentX;
			}
			
			if(delta==0){
				return false;
			}
			
			ScrollBy(delta,false,true);
			
			return false;
		}
		
		public override void OnChildrenLoaded(){
			
			// We're always draggable:
			setAttribute("draggable", "1");
			
			// Get the scroll bar:
			ScrollBar=parentElement as HtmlScrollbarElement;
			
			if(ScrollBar==null){
				return;
			}
			
			// Set orient attrib for CSS:
			setAttribute("orient", ScrollBar.getAttribute("orient"));
			
			IsVertical=(ScrollBar.IsVertical);
			
		}
		
		/// <summary>Gets the length of the bar in pixels, minus arrow buttons.</summary>
		/// <returns>The pixel length of the scrollbar.</returns>
		public float BarSize{
			get{
				if(ScrollBar==null){
					return 0f;
				}
				
				LayoutBox barBox=ScrollBar.RenderData.FirstBox;
				
				if(barBox==null){
					// Not visible.
					return 0f;
				}
				
				float result=IsVertical ? barBox.InnerHeight : barBox.InnerWidth;
				
				float arrowSizes=EndArrowSize;
				
				if(arrowSizes==0f){
					// Use start twice (we haven't computed the box of the end arrow yet).
					arrowSizes=StartArrowSize * 2f;
				}else{
					arrowSizes+=StartArrowSize;
				}
				
				return result-arrowSizes;
			}
		}
		
		/// <summary>The current position of the thumb (in pixels).
		/// Note that width of elements before the thumb like buttons are included in this.</summary>
		public float Position{
			get{
				BoxStyle box=RenderData.FirstBox.Position;
				return IsVertical ? box.Top : box.Left;
			}
		}
		
		/// <summary>Gets this thumb's 0-1 progress along the scrollbar, 
		/// taking into account the size of the thumb itself.</summary>
		public float Progress{
			get{
				// Get the size of the track, accounting for the thumb itself:
				float trackSize=(BarSize-ThumbSize);
				
				// Get the thumb position:
				float position=(Position-StartArrowSize);
				
				return position / trackSize;
			}
		}
		
		/// <summary>Scrolls this scrollbar by the given number of pixels.
		/// Note that this may fail if the scrollbar cannot scroll any further.</summary>
		/// <param name="pixels">The number of pixels to scroll this bar by.</param>
		public void ScrollBy(float pixels){
			ScrollBy(pixels,true,true);
		}
		
		/// <summary>Scrolls this scrollbar by the given number of pixels, optionally relative to a fixed point on the bar.
		/// Note that this may fail if the scrollbar cannot scroll any further.</summary>
		/// <param name="pixels">The number of pixels to scroll this bar by.</param>
		/// <param name="fromCurrent">True if pixels is relative to where the thumb currently is. False if pixels is relative
		/// to where the bar was when the mouse was clicked. See e.g. <see cref="PowerUI.VScrollTabTag.StartY"/>.</param>
		/// <param name="scrollTarget">True if the target should also be scrolled.</param>
		public void ScrollBy(float pixels,bool fromCurrent,bool scrollTarget){
			
			// Scroll it by pixels from Start.
			float newLocation=pixels;
			float currentPosition=Position;
			
			LayoutBox box=RenderData.FirstBox;
			
			if(fromCurrent){
				newLocation+=currentPosition;
			}else{
				newLocation+=Start;
			}
			
			// Get the size of the button before the tab:
			float sizeBefore=StartArrowSize;
			
			float barSize=BarSize;
			
			float max=barSize+sizeBefore;
			
			if(IsVertical){
				max-=box.Height;
			}else{
				max-=box.Width;
			}
			
			if(newLocation<sizeBefore){
				newLocation=sizeBefore;
			}else if(newLocation>max){
				newLocation=max;
			}
			
			if(newLocation==currentPosition){
				return;
			}
			
			if(IsVertical){
				Style.top=newLocation+"fpx";
			}else{
				Style.left=newLocation+"fpx";
			}
			
			// Fire a change event on the scrollbar:
			Dom.Event e=new Dom.Event("change");
			e.SetTrusted();
			ScrollBar.dispatchEvent(e);
			
			if(scrollTarget){
				
				// Get info for the target:
				HtmlElement target=ScrollBar.scrollTarget;
				ComputedStyle targetCs=target.style.Computed;
				box=targetCs.FirstBox;
				
				if(target!=null && box!=null){
					
					// Get the progress:
					float progress=(newLocation-sizeBefore)/barSize;
					
					// Update CSS:
					if(IsVertical){
						targetCs.ChangeTagProperty("scroll-top",new Css.Units.DecimalUnit(progress * box.ContentHeight));
					}else{
						targetCs.ChangeTagProperty("scroll-left",new Css.Units.DecimalUnit(progress * box.ContentWidth));
					}
					
					// And request a redraw:
					target.htmlDocument.RequestLayout();
					
				}
				
			}
			
		}
		
		/// <summary>Scrolls this thumb to the specific percentage along the bar. 0-1. Optionally scrolls the target element.</summary>
		public void ScrollToPoint(float percent,bool scrollTarget){
			ScrollTo(percent * BarSize,scrollTarget);
		}
		
		/// <summary>Scrolls this thumb to the specific position along the bar. Optionally scrolls the target element.</summary>
		public void ScrollTo(float location,bool scrollTarget){
			Start=0;
			ScrollBy(location,false,scrollTarget);
		}
		
		/// <summary>Gets the thumbs size in pixels.</summary>
		/// <returns>The size of the thumb in pixels.</returns>
		public float ThumbSize{
			get{
				return IsVertical ? RenderData.FirstBox.InnerHeight : RenderData.FirstBox.InnerWidth;
			}
		}
		
	}
	
}