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
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// Represents a non-standard marquee tag.
	/// </summary>
	
	[Dom.TagName("marquee")]
	public class HtmlMarqueeElement:HtmlElement{
		
		/// <summary>True when this marquee is active.</summary>
		public bool Active;
		/// <summary>The amount of times this will loop.</summary>
		public int Loop=-1;
		/// <summary>The underlying timer which causes the scrolling.</summary>
		private UITimer Timer;
		/// <summary>The amount of pixels a scroll will cause.</summary>
		public float ScrollAmount=6;
		/// <summary>The time in milliseconds between scrolls.</summary>
		public int ScrollDelay=85;
		/// <summary>How this marquee scrolls.</summary>
		public MarqueeBehaviour Behaviour=MarqueeBehaviour.Scroll;
		/// <summary>The direction of the marquee.</summary>
		public MarqueeDirection Direction=MarqueeDirection.Left;
		
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>Call this to begin a marquee.</summary>
		public void Start(){
			
			if(Active){
				return;
			}
			
			Active=true;
			
			// Doesn't bubble:
			Dom.Event e=new Dom.Event("start");
			e.SetTrusted(false);
			
			if(dispatchEvent(e)){
				
				// Start our timer:
				Timer=new UITimer(false,ScrollDelay,OnTick);
				Timer.Document=document_;
				
			}
			
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.</summary>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.InBody){
				
				lexer.AddMarkedFormattingElement(this);
				
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
				
				lexer.CloseMarkedFormattingElement("marquee");
				
			}else{
			
				return false;
			
			}
			
			return true;
			
		}
		
		/// <summary>Called when the element is removed from the DOM.</summary>
		public override void OnRemovedFromDOM(){
			Stop();
		}
		
		/// <summary>Call this to stop a scrolling marquee.</summary>
		public void Stop(){
			
			if(!Active){
				return;
			}
			
			// Doesn't bubble:
			Dom.Event e=new Dom.Event("stop");
			e.SetTrusted(false);
			
			if(dispatchEvent(e)){
				
				Active=false;
				
				// Stop and clear the timer:
				Timer.Stop();
				
				Timer=null;
				
			}
			
		}
		
		private void OnTick(){
			
			// Grab the computed style:
			ComputedStyle style=Style.Computed;
			
			float amount=ScrollAmount;
			
			// Is it odd? If so, it travels in the inverted direction.
			if(((int)Direction&1)==1){
				
				// Invert the direction:
				amount=-ScrollAmount;
				
			}
			
			float scrollLeft=style.ScrollLeft;
			float scrollTop=style.ScrollTop;
			
			if((int)Direction<=2){
				
				// Vertical scroll:
				scrollTop+=amount;
			
				// Grab the content height:
				float contentHeight=style.ContentHeight;
				
				// Grab the parent height:
				float height=style.InnerHeight;
				
				switch(Behaviour){
					
					case MarqueeBehaviour.Scroll:
						
						if(scrollTop<-height){
							
							// Wrap:
							scrollTop=contentHeight;
							
							Wrapped();
							
						}else if(scrollTop>contentHeight){
							
							// Wrap:
							scrollTop=-height;
							
							Wrapped();
							
						}
						
					break;
					
					case MarqueeBehaviour.Alternate:
						
						float minimum=-(height-contentHeight);
						
						if(minimum>=0){
							
							// No space to bounce anyway.
							return;
							
						}
						
						if(scrollTop>0f){
							
							// Reset:
							scrollTop=0f;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Up){
								
								Direction=MarqueeDirection.Down;
								
							}else{
								
								Direction=MarqueeDirection.Up;
								
							}
							
							Bounced();
							
						}else if(scrollTop<minimum){
							
							scrollTop=minimum;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Up){
								
								Direction=MarqueeDirection.Down;
								
							}else{
								
								Direction=MarqueeDirection.Up;
								
							}
							
							Bounced();
							
						}
						
					break;
					
				}
				
			}else{
				
				// Horizontal scroll:
				scrollLeft+=amount;
				
				// Grab the content width:
				float contentWidth=style.ContentWidth;
				
				// Grab the parent width:
				float width=style.InnerWidth;
				
				switch(Behaviour){
					
					case MarqueeBehaviour.Scroll:
						
						if(scrollLeft<-width){
							
							// Wrap:
							scrollLeft=contentWidth;
							
							Wrapped();
							
						}else if(scrollLeft>contentWidth){
							
							// Wrap:
							scrollLeft=-width;
							
							Wrapped();
							
						}
						
					break;
					
					case MarqueeBehaviour.Alternate:
						
						float minimum=-(width-contentWidth);
						
						if(minimum>=0f){
							
							// No space to bounce anyway.
							return;
							
						}
						
						if(scrollLeft>0f){
							
							// Reset:
							scrollLeft=0f;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Left){
								
								Direction=MarqueeDirection.Right;
								
							}else{
								
								Direction=MarqueeDirection.Left;
								
							}
							
							Bounced();
							
						}else if(scrollLeft<minimum){
							
							scrollLeft=minimum;
							
							// Flip the direction.
							if(Direction==MarqueeDirection.Left){
								
								Direction=MarqueeDirection.Right;
								
							}else{
								
								Direction=MarqueeDirection.Left;
								
							}
							
							Bounced();
							
						}
						
					break;
					
				}
				
			}
			
			// Scroll now:
			scrollTo(scrollLeft,scrollTop);
			
			// Request a redraw:
			style.RequestLayout();
			
		}
		
		/// <summary>Called when the marquee bounces the content.</summary>
		private void Bounced(){
			
			// Trigger:
			Dom.Event e=new Dom.Event("bounce");
			e.SetTrusted(false);
			dispatchEvent(e);
			
			// Consider looping too:
			Wrapped();
			
		}
		
		/// <summary>Called when the marquee wraps.</summary>
		private void Wrapped(){
			
			if(Loop==-1){
				return;
			}
			
			Loop--;
			
			if(Loop==0){
				
				// Stop the marquee:
				Stop();
				
				// Fire the finish event:
				
				// Doesn't bubble:
				Dom.Event e=new Dom.Event("finish");
				e.SetTrusted(false);
				dispatchEvent(e);
				
			}
			
		}
		
		public override bool OnAttributeChange(string property){
			
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="loop"){
				
				Loop=int.Parse(getAttribute("loop"));
				
				if(Loop==0){
					Loop=1;
				}else if(Loop<0){
					Loop=-1;
				}
				
			}else if(property=="scrollamount"){
				
				ScrollAmount=int.Parse(getAttribute("scrollamount"));
				
			}else if(property=="scrolldelay"){
				
				ScrollDelay=int.Parse(getAttribute("scrolldelay"));
				
				if(ScrollDelay<50){
					
					// No super fast scrolling - it's too distracting. Use animate for effects like that.
					
					ScrollDelay=50;
					
				}
				
			}else if(property=="behaviour"){
				
				ApplyBehaviour(getAttribute("behaviour"));
				
			}else if(property=="behavior"){
				
				ApplyBehaviour(getAttribute("behavior"));
				
			}else if(property=="direction"){
				
				// Grab the direction:
				string direction=getAttribute("direction");
				
				switch(direction){
					
					case "left":
						Direction=MarqueeDirection.Left;
					break;
					
					case "right":
						Direction=MarqueeDirection.Right;
					break;
					
					case "up":
						Direction=MarqueeDirection.Up;
					break;
					
					case "down":
						Direction=MarqueeDirection.Down;
					break;
					
				}
				
			}else{
				
				return false;
				
			}
			
			return true;
			
		}
		
		private void ApplyBehaviour(string behaviour){
			
			switch(behaviour){
				
				case "scroll":
					Behaviour=MarqueeBehaviour.Scroll;
				break;
				
				case "slide":
					Behaviour=MarqueeBehaviour.Slide;
				break;
				
				case "alternate":
					Behaviour=MarqueeBehaviour.Alternate;
				break;
				
			}
			
		}
		
		internal override void RemovedFromDOM(){
			base.RemovedFromDOM();
			
			// Clear the timer:
			if(Timer!=null){
				Timer.Stop();
				Timer=null;
			}
			
		}
		
		public override void OnChildrenLoaded(){
			
			Start();
			
			// Base:
			base.OnChildrenLoaded();
			
		}
		
	}
	
}