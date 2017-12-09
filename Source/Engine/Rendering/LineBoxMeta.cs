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
using System.Collections;
using System.Collections.Generic;
using Css;
using InfiniText;


namespace PowerUI{
	
	/// <summary>
	/// Stores the information used whilst laying out boxes during a reflow.
	/// <summary>

	public class LineBoxMeta{
		
		/// <summary>The "host" block box.</summary>
		public BlockBoxMeta HostBlock;
		/// <summary>The height of the current line being processed.</summary>
		public float LineHeight;
		/// <summary>A linked list of elements on a line are kept. This is the last element on the current line.</summary>
		internal LayoutBox LastOnLine;
		/// <summary>A linked list of elements on a line are kept. This is the first element on the current line.</summary>
		internal LayoutBox FirstOnLine;
		/// <summary>A linked list of elements on a line are kept. This is the last element on the current out of flow line.</summary>
		internal LayoutBox LastOutOfFlow;
		/// <summary>A linked list of elements on a line are kept. This is the first element on the current out of flow line.</summary>
		internal LayoutBox FirstOutOfFlow;
		/// <summary>The last line start. Tracked for alignment.</summary>
		internal LayoutBox LastLineStart;
		/// <summary>The first line start. Tracked for alignment.</summary>
		internal LayoutBox FirstLineStart;
		/// <summary>This boxes whitespace mode.</summary>
		internal int WhiteSpace;
		/// <summary>The current 'clear zone'. Added to PenY when something is added to the current line.</summary>
		internal float ClearY_;
		/// <summary>The set of active floated elements for the current line being rendered.</summary>
		internal FloatingElements Floats;
		/// <summary>The current x location of the renderer in screen pixels from the left.</summary>
		internal float PenX;
		/// <summary>The value for horizontal-align. If it's not required, this is just 0.</summary>
		public int HorizontalAlign;
		/// <summary>The value for vertical-align.</summary>
		public int VerticalAlign;
		/// <summary>Vertical-align offset from the baseline.</summary>
		public float VerticalAlignOffset;
		/// <summary>The current box being worked on.</summary>
		internal LayoutBox CurrentBox;
		/// <summary>The next box in the hierarchy.</summary>
		public LineBoxMeta Parent;
		/// <summary>The inline element.</summary>
		public RenderableData RenderData;
		/// <summary>An offset to apply to MaxX.</summary>
		public float MaxOffset;
		/// <summary>True if the current line contains bi-directional text. I.e. Any box with a non-zero "UnicodeBidi" value.</summary>
		public bool ContainsBidirectional;
		
		
		public LineBoxMeta(LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData){
			
			Parent=parent;
			CurrentBox=firstBox;
			RenderData=renderData;
			
		}
		
		/// <summary>Removes the given box from this line. Must not be the first one.</summary>
		public void RemoveFromLine(LayoutBox box){
			
			if(FirstOnLine==null){
				return;
			}
			
			LayoutBox prev=null;
			LayoutBox current=FirstOnLine;
			
			while(current!=null){
				
				if(current==box){
					
					// Got it!
					if(prev==null){
						throw new Exception("Can't remove first box from a line.");
					}
					
					prev.NextOnLine=current.NextOnLine;
					
					if(current.NextOnLine==null){
						LastOnLine=prev;
					}
					
					break;
					
				}
				
				prev=current;
				current=current.NextOnLine;
				
			}
			
		}
		
		/// <summary>Attempts to break a line for a parent inline node.</summary>
		public void TryBreakParent(){
			
			// If the parent is an inline node, 'this' is the first element on its line.
			if(Parent==null){
				return;
			}
			
			if(Parent.FirstOnLine==CurrentBox){
				
				// Go another level up if we can:
				if(Parent is BlockBoxMeta){
					return;
				}
				
				Parent.TryBreakParent();
				return;
				
			}
			
			// We're not the first on the line (but we'll always be the last). Safety check:
			if(Parent.LastOnLine!=CurrentBox){
				return;
			}
			
			// Remove current box from the line:
			Parent.RemoveFromLine(CurrentBox);
			
			// Complete it:
			Parent.CompleteLine(LineBreakMode.Normal);
			
			// Re-add:
			Parent.AddToLine(CurrentBox);
			
			// Clear offset:
			MaxOffset=0f;
			
		}
		
		/// <summary>The value of the CSS line-height property.</summary>
		public float CssLineHeight;
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public virtual float PenY{
			get{
				return 0f;
			}
			set{
			}
		}
		
		/// <summary>The length of the longest line so far. Used for the content width.</summary>
		public virtual float LargestLineWidth{
			get{
				return HostBlock.LargestLineWidth_;
			}
			set{
				HostBlock.LargestLineWidth_=value;
			}
		}
		
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public virtual bool GoingLeftwards{
			get{
				return HostBlock.GoingLeftwards_;
			}
			set{
				HostBlock.GoingLeftwards_=value;
			}
		}
		
		/// <summary>The start position of a line.</summary>
		public virtual float LineStart{
			get{
				return HostBlock.LineStart_;
			}
			set{
				HostBlock.LineStart_=value;
			}
		}
		
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public virtual float MaxX{
			get{
				return HostBlock.MaxX_ - MaxOffset;
			}
			set{
				HostBlock.MaxX_=value;
			}
		}
		
		/// <summary>Ensures the given amount of space is available by 
		/// completing the line if needed and potentially clearing floats too.</summary>
		/// <returns>True if it broke.</returns>
		public int GetLineSpace(float width,float initialOffset){
			
			// If the box no longer fits on the current line..
			float space=(MaxX-PenX);
			
			if((initialOffset+width) <= space){
				return 0;
			}
			
			// Does it fit on a newline?
			if(width <= (MaxX-LineStart)){
				return 1;
			}
			
			// Still nope! If we've got any floats, try clearing them:
			while(TryClearFloat()){
				
				if(width <= (MaxX-LineStart)){
					
					// Great, it'll fit on a newline!
					return 1;
					
				}
				
			}
			
			// Ok! This one's special because it doesn't actually fit on the line.
			// Important for words which may need to break internally.
			return 2;
			
		}
		
		/// <summary>Adds the given style to the current line.</summary>
		/// <param name="style">The style to add.</param>
		internal void AddToLine(LayoutBox styleBox){
			
			// Make sure it's safe:
			styleBox.Parent=CurrentBox;
			styleBox.NextLineStart=null;
			styleBox.NextOnLine=null;
			
			if((styleBox.PositionMode & PositionMode.InFlow)==0){
				
				// Out of flow - add it to a special line:
				if(FirstOutOfFlow==null){
					FirstOutOfFlow=LastOutOfFlow=styleBox;
				}else{
					LastOutOfFlow=LastOutOfFlow.NextOnLine=styleBox;
				}
				
				styleBox.ParentOffsetLeft=PenX+styleBox.Margin.Left;
				styleBox.ParentOffsetTop=PenY+styleBox.Margin.Top;
				
				return;
			}
			
			// If currentBox is overriding bidi..
			if((CurrentBox.UnicodeBidi & UnicodeBidiMode.Override)!=0){
				
				// Override the value now.
				
				// If it was mirrored, keep it so:
				if((styleBox.UnicodeBidi & UnicodeBidiMode.Mirrored)!=0){
					
					if(GoingLeftwards){
						styleBox.UnicodeBidi=UnicodeBidiMode.LeftwardsMirrored;
					}else{
						styleBox.UnicodeBidi=UnicodeBidiMode.RightwardsMirrored;
					}
					
				}else if(GoingLeftwards){
					styleBox.UnicodeBidi=UnicodeBidiMode.LeftwardsNormal;
				}else{
					styleBox.UnicodeBidi=UnicodeBidiMode.RightwardsNormal;
				}
				
			}
			
			// Only leftwards (mirrored or normal) matters here:
			if((styleBox.UnicodeBidi & UnicodeBidiMode.Leftwards)!=0){
				ContainsBidirectional=true;
			}
			
			int floatMode=styleBox.FloatMode;
			
			if(floatMode==FloatMode.None){
				
				// In flow - add to line:
				
				// Add the clear zone:
				if(ClearY_!=0f){
					PenY+=ClearY_;
					ClearY_=0f;
				}
				
				if(FirstOnLine==null){
					FirstOnLine=styleBox;
					LastOnLine=styleBox;
					
					if(FirstLineStart==null){
						
						// First child element. Update parent if we've got one:
						if(Parent!=null && Parent.CurrentBox!=null){
							
							Parent.CurrentBox.FirstChild=styleBox;
							
						}
						
						FirstLineStart=LastLineStart=styleBox;
					}else{
						LastLineStart=LastLineStart.NextLineStart=styleBox;
					}
					
				}else{
					LastOnLine.NextOnLine=styleBox;
					LastOnLine=styleBox;
				}
				
			}else{
				
				// Adding a float - is this an inline element?
				if(this is InlineBoxMeta){
					
					// Add to nearest block:
					HostBlock.AddToLine(styleBox);
					return;
					
				}
				
				// Going left?
				if(GoingLeftwards){
					// Invert:
					floatMode=(floatMode==FloatMode.Right)?FloatMode.Left : FloatMode.Right;
				}
				
				// The total width is..
				float totalWidth=styleBox.TotalWidth;
				
				// Got enough space for this float?
				if((MaxX-PenX)<totalWidth){
					
					// Not enough space on the line for this floating block.
					bool cls=true;
					
					if(PenX!=LineStart){
						
						// Line break:
						CompleteLine(LineBreakMode.Normal);
						
						// Clear line space only if the test is still true:
						cls=((MaxX-PenX)<totalWidth);
						
					}
					
					if(cls){
						
						// Try clearing space for it:
						if(ClearLineSpace(floatMode)){
							
							// Test again:
							if((MaxX-PenX)<totalWidth){
								
								// Potentially clear the other side this time:
								if(!ClearLineSpace(floatMode) || (MaxX-PenX)<totalWidth){
									
									// Line break.
									CompleteLine(LineBreakMode.Normal);
									
								}
								
							}
							
						}else{
							
							// Line break.
							CompleteLine(LineBreakMode.Normal);
							
						}
						
					}
					
				}
				
				if(Floats==null){
					Floats=new FloatingElements();
				}
				
				if(floatMode==FloatMode.Right){
					
					// Push down onto the FR stack:
					styleBox.NextOnLine=Floats.Right;
					Floats.Right=styleBox;
					
					// Special case for any inline-block which isn't in block mode.
					// MaxX is *not* the final position for it.
					// We'll revisit it when we know how much of a 'gap' there is.
					styleBox.ParentOffsetLeft=MaxX-totalWidth+styleBox.Margin.Left;
					
					if(styleBox.ParentOffsetLeft<0f){
						styleBox.ParentOffsetLeft=0f;
					}
					
					// Reduce maxX:
					MaxX-=totalWidth;
					
				}else{
					
					// Push down onto the FL stack:
					styleBox.NextOnLine=Floats.Left;
					Floats.Left=styleBox;
					
					// Update left offset:
					styleBox.ParentOffsetLeft=LineStart+styleBox.Margin.Left;
					
					// Push over where lines start at:
					LineStart+=totalWidth;
					
					// Push over all the elements before this on the line.
					// (Note: A float will only join a line if it fits on it; 
					// this won't push something beyond the end).
					LayoutBox currentLine=FirstOnLine;
					
					while(currentLine!=null){
						
						// Move it:
						currentLine.ParentOffsetLeft+=totalWidth;
						
						// Next one:
						currentLine=currentLine.NextOnLine;
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>The float 'clearance' on the left/right. It's basically the bottom of left/right floats.</summary>
		public float FloatClearance(bool left){
			
			if(Floats==null){
				return 0f;
			}
			
			LayoutBox activeFloat=left ? Floats.Left : Floats.Right;
			
			float max=0f;
			
			while(activeFloat!=null){
				
				// Yes - how far down must we go?
				float requiredClear=(activeFloat.ParentOffsetTop + activeFloat.Height);
				
				if(requiredClear>max){
					max=requiredClear;
				}
				
				// Go left:
				activeFloat=activeFloat.NextOnLine;
				
			}
			
			return max;
			
		}
		
		/// <summary>Tries to clear a left/right float (whichever is shortest first).</summary>
		/// <returns>True if either side was cleared.</returns>
		public bool TryClearFloat(){
			
			if(Floats==null){
				return false;
			}
			
			if(Floats.Left==null){
				
				if(Floats.Right==null){
					return false;
				}
				
				// Clear right:
				ClearFloat(FloatMode.Right);
				
				return true;
				
			}
			
			if(Floats.Right==null){
				
				// Clear left:
				ClearFloat(FloatMode.Left);
				
				return true;
				
			}
			
			// Clear shortest:
			float clearanceL=FloatClearance(true);
			float clearanceR=FloatClearance(false);
			
			if(clearanceL>clearanceR){
				
				// R first.
				ClearFloat(FloatMode.Right);
				
			}else{
				
				// L first.
				ClearFloat(FloatMode.Left);
				
			}
			
			return true;
			
		}
		
		/// <summary>Clears left/right/both floats.</summary>
		public void ClearFloat(int mode){
			
			if(Floats==null){
				return;
			}
			
			LayoutBox activeFloat;
			float penY=PenY;
			
			if((mode & FloatMode.Left)!=0){
				
				// Clear left.
				activeFloat=Floats.Left;
				Floats.Left=null;
				
				while(activeFloat!=null){
					
					// Yes - how far down must we go?
					float requiredClear=(activeFloat.ParentOffsetTop + activeFloat.Height + activeFloat.Margin.Bottom);
					
					if((penY+ClearY_)<requiredClear){
						// Clear over it now:
						ClearY_=requiredClear-penY;
					}
					
					// Decrease LineStart:
					LineStart-=activeFloat.TotalWidth;
					
					// Go left:
					activeFloat=activeFloat.NextOnLine;
					
				}
				
				if((mode & FloatMode.Right)==0){
					
					// Check if the right side has been cleared too.
					
					// Test clear right:
					activeFloat=Floats.Right;
					
					while(activeFloat!=null){
						
						// Is the current render point now higher than this floating object?
						// If so, we must reduce LineStart/ increase MaxX depending on which type of float it is.
						
						if((PenY+ClearY_)>=(activeFloat.ParentOffsetTop + activeFloat.Height + activeFloat.Margin.Bottom)){
							
							// Clear!
							
							// Pop:
							Floats.Right=activeFloat.NextOnLine;
							
							// Increase max x:
							MaxX+=activeFloat.TotalWidth;
							
						}else{
							
							// Didn't clear - stop there.
							// (We don't want to clear any further over to the right).
							break;
							
						}
						
						activeFloat=activeFloat.NextOnLine;
					}
					
				}
				
				// Reset PenX:
				PenX=LineStart;
				
			}
			
			if((mode & FloatMode.Right)!=0){
				
				// Clear right.
				activeFloat=Floats.Right;
				Floats.Right=null;
				
				while(activeFloat!=null){
					
					// Yes - how far down must we go?
					float requiredClear=(activeFloat.ParentOffsetTop + activeFloat.Height + activeFloat.Margin.Bottom);
					
					if((penY+ClearY_)<requiredClear){
						// Clear over it now:
						ClearY_=requiredClear-penY;
					}
					
					// Increase max x:
					MaxX+=activeFloat.TotalWidth;
					
					// Go right:
					activeFloat=activeFloat.NextOnLine;
					
				}
				
				if((mode & FloatMode.Left)==0){
					
					// Check if any left floats were cleared.
					activeFloat=Floats.Left;
					
					while(activeFloat!=null){
						
						// Is the current render point now higher than this floating object?
						// If so, we must reduce LineStart/ increase MaxX depending on which type of float it is.
						
						if((PenY+ClearY_)>=(activeFloat.ParentOffsetTop + activeFloat.Height + activeFloat.Margin.Bottom)){
							
							// Clear!
							
							// Pop:
							Floats.Left=activeFloat.NextOnLine;
							
							// Decrease LineStart:
							LineStart-=activeFloat.TotalWidth;
							
							// Must reset PenX:
							PenX=LineStart;
							
						}else{
							
							// Didn't clear - stop there.
							// (We don't want to clear any further over to the left).
							break;
							
						}
						
						activeFloat=activeFloat.NextOnLine;
					}
					
				}
				
			}
			
		}
		
		/// <summary>Horizontally aligns a line based on alignment settings.</summary>
		/// <param name="currentBox">The style of the first element on the line.</param>
		/// <param name="lineSpace">The amount of space available to the line.</param>
		/// <param name="elementCount">The number of elements on this line.</param>
		/// <param name="lineLength">The width of the line in pixels.</param>
		/// <param name="parentBox">The style which defines the alignment.</param>
		private void AlignHorizontally(LayoutBox currentBox,LayoutBox lastBox,float lineSpace,int elementCount,float lineLength,int align){
			
			if(elementCount==0){
				return;
			}
			
			if(align!=HorizontalAlignMode.Left){
				// Does the last element on the line end with a space? If so, act like the space isn't there by reducing line length by it.
				lineLength-=lastBox.EndSpaceSize;
			}
			
			// How many pixels each element will be moved over:
			float offsetBy=0f;
			// How many pixels we add to offsetBy each time we shift an element over:
			float justifyDelta=0f;
			// True if we ignore block elements.
			bool ignoreBlock=true;
			
			if((align & HorizontalAlignMode.EitherCenter)!=0){
				// We're centering - shift by half the 'spare' pixels on this row.
				
				// How many pixels of space this line has left / 2:
				offsetBy=(lineSpace-lineLength)/2f;
				
				// Don't ignore block for -moz-center:
				ignoreBlock=(align==HorizontalAlignMode.Center);
				
			}else if(align==HorizontalAlignMode.Right){
				// How many pixels of space this line has left:
				offsetBy=(lineSpace-lineLength);
				
			}else if(align==HorizontalAlignMode.Justify){
				
				// Justify. This is where the total spare space on the line gets shared out evenly
				// between the elements on this line.
				// So, we take the spare space and divide it up by the elements on this line:
				justifyDelta=(lineSpace-lineLength)/(float)elementCount;
				
			}
			
			while(currentBox!=null){
				
				if(ignoreBlock){
					
					// If it's display:block, ignore it.
					if(currentBox.DisplayMode==DisplayMode.Block){
						
						// Skip!
						currentBox=currentBox.NextOnLine;
						continue;
						
					}
					
				}
				
				// Shift the element over by the offset.
				currentBox.ParentOffsetLeft+=offsetBy;
				
				offsetBy+=justifyDelta;
				
				currentBox=currentBox.NextOnLine;
			}
			
		}
		
		/// <summary>Completes a line, optionally breaking it.</summary>
		/// <param name="settings">Values from LineBreakMode. You'd usually pass Normal.</param>
		public void CompleteLine(int settings){
			
			float lineHeight=LineHeight;
			
			// Note: Doesn't matter about last here.
			if(settings!=0){
				
				// Horizontally + Vertically align all elements on the current line and reset it:
				LayoutBox currentBox=FirstOnLine;
				LayoutBox first=currentBox;
				LayoutBox last=LastOnLine;
				
				FirstOnLine=null;
				LastOnLine=null;
				
				// Baseline is default:
				int elementCount=0;
				int verticalAlignMode=VerticalAlign;
				float baseOffset=VerticalAlignOffset;
				float lineLength=0f;
				
				// Is the nearest block parent a table cell?
				bool tableCell=(CurrentBox.DisplayMode==DisplayMode.TableCell);
				
				if(tableCell){
					// Don't apply any v-align yet:
					verticalAlignMode=0;
				}
				
				while(currentBox!=null){
					// Calculate the offset to where the top left corner is (of the complete box, margin included):
					
					// Must be positioned such that the boxes sit on this lines baseline.
					// the baseline is by default at half the line-height but moves up whenever 
					// an inline-block element with padding/border/margin is added.
					
					lineLength+=currentBox.Width;
					float delta=-(currentBox.Height+currentBox.Margin.Bottom);
					
					bool inline=(currentBox.DisplayMode & DisplayMode.OutsideInline)!=0;
					
					if(currentBox.DisplayMode==DisplayMode.Inline){
						
						// Must also move it down by padding and border:
						delta+=currentBox.Border.Bottom + currentBox.Padding.Bottom;
						
					}
					
					if((verticalAlignMode & VerticalAlignMode.BaselineRelative)!=0){
						
						if(inline){
							
							// Bump the elements so they all sit neatly on the baseline:
							float baselineShift=(CurrentBox.Baseline-currentBox.Baseline)+baseOffset;
							
							switch(verticalAlignMode){
								case VerticalAlignMode.Super:
									baselineShift+=(currentBox.InnerHeight * 0.75f);
								break;
								case VerticalAlignMode.Sub:
									baselineShift-=(currentBox.InnerHeight * 0.75f);
								break;
								case VerticalAlignMode.Middle:
									
									// Align the middle of the element with the baseline:
									baselineShift-=(currentBox.InnerHeight * 0.5f);
									
									// Plus half of the parent x-height:
									baselineShift+=(CurrentBox.FontFace.ExHeight * CurrentBox.FontSize * 0.5f);
									
								break;
								case VerticalAlignMode.TextTop:
									
									// Align the top of the text with the baseline:
									// (add its x-height):
									baselineShift-=(CurrentBox.FontFace.ExHeight * CurrentBox.FontSize);
									
								break;
								case VerticalAlignMode.TextBottom:
									
									// Align the top of the text with the baseline:
									// (remove its x-height):
									baselineShift+=(CurrentBox.FontFace.ExHeight * CurrentBox.FontSize);
									
								break;
							}
							
							delta-=baselineShift;
							
							// May need to update the line height:
							
							if(baselineShift>0){
								
								// (This is where gaps come from below inline images):
								
								if(currentBox.DisplayMode==DisplayMode.Inline){
									
									// Line height next:
									baselineShift+=currentBox.InnerHeight;
									
								}else{
									
									// E.g. inline-block:
									baselineShift+=currentBox.TotalHeight;
								}
								
								if(baselineShift>LineHeight){
									
									LineHeight=baselineShift;
									lineHeight=baselineShift;
									
									// Stalled!
									
									// - This happens because we've just found out that an element sitting on the baseline
									//   has generated a gap and ended up making the line get taller.
									//   Elements after this one can affect the baseline so we can't "pre test" this condition.
									//   Line height is important for positioning elements, so we'll need to go again
									//   on the elements that we've already vertically aligned.
									
									// Halt and try again:
									currentBox=first;
									elementCount=0;
									lineLength=0f;
									goto Stall;
									
								}
								
							}
							
						}
						
					}
					
					elementCount++;
					currentBox.ParentOffsetTop=PenY+delta+lineHeight;
					
					// Hop to the next one:
					currentBox=currentBox.NextOnLine;
					
					Stall:
						continue;
					
				}
				
				if(this is BlockBoxMeta){
					
					// Horizontal align:
					int hAlign=HorizontalAlign;
					
					if((settings&LineBreakMode.Last)!=0 && RenderData!=null){
						
						// Use H-Align last:
						int lastAlign=RenderData.computedStyle.HorizontalAlignLastX;
						
						if(lastAlign==HorizontalAlignMode.Auto){
							
							// Pick an alignment based on HorizontalAlign:
							lastAlign=hAlign;
							
							if(lastAlign==HorizontalAlignMode.Justify){
								// Left or right:
								if(GoingLeftwards){
									lastAlign=HorizontalAlignMode.Right;
								}else{
									lastAlign=HorizontalAlignMode.Left;
								}
							}
							
						}
						
						hAlign=lastAlign;
					}
					
					if(hAlign!=0){
						AlignHorizontally(first,last,MaxX-LineStart,elementCount,lineLength,hAlign);
					}
					
					// If this is the last line and vAlign is a table mode..
					if(
						(settings & LineBreakMode.Last)!=0 && 
						(settings & LineBreakMode.Break)==0 && 
						(tableCell || (verticalAlignMode & VerticalAlignMode.TableMode)!=0) 
					){
						
						if(tableCell){
							
							// Adjust the meaning of valign on this element:
							switch(VerticalAlign){
								case VerticalAlignMode.Top:
									verticalAlignMode=VerticalAlignMode.TableTop;
								break;
								case VerticalAlignMode.Middle:
									verticalAlignMode=VerticalAlignMode.TableMiddle;
								break;
								case VerticalAlignMode.Bottom:
									verticalAlignMode=VerticalAlignMode.TableBottom;
								break;
							}
							
						}
						
						// Aligning everything vertically (tables):
						float vDelta=0;
						
						// Vertical alignment:
						switch(verticalAlignMode){
							
							case VerticalAlignMode.TableMiddle:
								
								// Similar to below - we find the gap, then add *half* of that onto OffsetTop.
								vDelta=(CurrentBox.InnerHeight-(PenY+lineHeight)) / 2f;
								
							break;
							case VerticalAlignMode.TableBottom:
							
								// Find the gap; it's parent height-contentHeight.
								vDelta=CurrentBox.InnerHeight-(PenY+lineHeight);
								
							break;
							// case VerticalAlignMode.TableTop:
								// Default - do nothing
							// break;
						}
						
						if(vDelta!=0){
							// Move everything down by vDelta.
							
							// For each line..
							LayoutBox currentLine = FirstLineStart;
							
							while(currentLine!=null){
								// For each box on the line..
								LayoutBox currentOnLine = currentLine;
								
								while(currentOnLine!=null){
									// Move it:
									currentOnLine.ParentOffsetTop+=vDelta;
									currentOnLine=currentOnLine.NextOnLine;
								}
								
								currentLine = currentLine.NextLineStart;
							}
							
						}
						
					}
				
				}
				
				// Got bidi text and the bidi algo is enabled:
				if(ContainsBidirectional && (CurrentBox.UnicodeBidi!=UnicodeBidiMode.Plaintext)){
					
					ContainsBidirectional=false;
					// - Unicode Bi-Directionality -
					
					currentBox=first;
					LayoutBox startBidi=null;
					LayoutBox previousBidi=null;
					
					while(currentBox!=null){
						
						int bidiMode=currentBox.UnicodeBidi;
						
						if(startBidi==null){
							
							// If we hit a leftwards box, mark it as the start:
							if((bidiMode & UnicodeBidiMode.Leftwards)!=0){
								
								startBidi=currentBox;
								
							}
							
						}else if((bidiMode & UnicodeBidiMode.Rightwards)!=0){
							
							// Hit a rightwards node! The previous box was the last one.
							if(previousBidi!=startBidi){
								
								// Do nothing otherwise - it was only 1 box.
								
								// Rightwards align startBidi to previousBidi.
								// The "line length" is simply the gap between them:
								float bidiLineMax=(previousBidi.ParentOffsetLeft + previousBidi.Width) - startBidi.ParentOffsetLeft;
								
								// Align!
								RightwardsAlign(startBidi,previousBidi,bidiLineMax);
								
							}
							
						}
						
						previousBidi=currentBox;
						currentBox=currentBox.NextOnLine;
					}
					
					// If start is not null (and it has something after it)..
					if(startBidi!=null && startBidi.NextOnLine!=null){
						
						// Align from startBidi -> EOL.
						RightwardsAlign(startBidi,null,CurrentBox.InnerWidth - startBidi.ParentOffsetLeft);
						
					}
					
				}
				
				currentBox=FirstOutOfFlow;
				FirstOutOfFlow=null;
				LastOutOfFlow=null;
				
				while(currentBox!=null){
					// Calculate the offset to where the top left corner is (of the complete box, margin included):
					
					float delta=-currentBox.Margin.Bottom;
					
					if((currentBox.DisplayMode & DisplayMode.OutsideInline)!=0){
						
						// Must also move it down by padding and border:
						delta+=currentBox.Border.Bottom + currentBox.Padding.Bottom;
						
					}else if((currentBox.DisplayMode & DisplayMode.OutsideBlock)!=0){
						
						// Clear x:
						currentBox.ParentOffsetLeft=LineStart;
						
					}
					
					currentBox.ParentOffsetTop=PenY+delta;
					
					// Hop to the next one:
					currentBox=currentBox.NextOnLine;
				}
				
			}
			
			// Recurse down to the nearest flow root element.
			
			if(this is InlineBoxMeta){
				
				// Apply valid width/height:
				LayoutBox box=CurrentBox;
				
				bool inFlow=((box.PositionMode & PositionMode.InFlow)!=0);
				
				// Update line height and baseline:
				if(inFlow){
					
					if(lineHeight>Parent.LineHeight){
						Parent.LineHeight=lineHeight;
					}
					
					if(CurrentBox.Baseline>Parent.CurrentBox.Baseline){
						Parent.CurrentBox.Baseline=CurrentBox.Baseline;
					}
					
				}
				
				// Otherwise it explicitly defined them ("inline replaced").
				if(box.OrdinaryInline){
					
					box.InnerHeight=lineHeight;
					box.InnerWidth=PenX-LineStart;
					box.SetDimensions(false,false);
					
					// Update content w/h:
					box.ContentHeight=box.InnerHeight;
					box.ContentWidth=box.InnerWidth;
					
				}
				
				if(inFlow){
					// Update dim's:
					Parent.AdvancePen(box);
				}
				
				if(inFlow && ((settings & LineBreakMode.Break)!=0)){
					
					// Linebreak the parent:
					Parent.CompleteLine(LineBreakMode.Break);
					
					// Create a new box!
					// (And add it to the parent)
					LayoutBox styleBox=new LayoutBox();
					styleBox.Border=box.Border;
					styleBox.Padding=box.Padding;
					styleBox.Margin=box.Margin;
					styleBox.NextInElement=null;
					
					// No left margin:
					styleBox.Margin.Left=0f;
					
					styleBox.DisplayMode=box.DisplayMode;
					styleBox.PositionMode=box.PositionMode;
					
					CurrentBox=styleBox;
					
					// Add to the inline element's render data:
					RenderData.LastBox.NextInElement=styleBox;
					RenderData.LastBox=styleBox;
					
					// Add to line next:
					Parent.AddToLine(styleBox);
					
				}
				
			}else{
				
				// Done recursing downwards - we're at the block!
				
				if(settings!=0){
					
					// Update largest line (excludes float right which is actually what we want!):
					if(PenX>LargestLineWidth){
						LargestLineWidth=PenX;
					}
					
					// Move the pen down to the following line:
					PenY+=lineHeight;
					
					// Are any floats now cleared?
					if(Floats!=null){
						
						LayoutBox activeFloat=Floats.Left;
						
						while(activeFloat!=null){
							
							// Is the current render point now higher than this floating object?
							// If so, we must reduce LineStart/ increase MaxX depending on which type of float it is.
							if((PenY+ClearY_)>=(activeFloat.ParentOffsetTop + activeFloat.Height + activeFloat.Margin.Bottom)){
								
								// Clear!
								
								// Pop:
								Floats.Left=activeFloat.NextOnLine;
								
								// Decrease LineStart:
								LineStart-=activeFloat.TotalWidth;
								
								// Must reset PenX:
								PenX=LineStart;
								
							}else{
								
								// Didn't clear - stop there.
								// (We don't want to clear any further over to the left).
								break;
								
							}
							
							activeFloat=activeFloat.NextOnLine;
						}
						
						// Test clear right:
						activeFloat=Floats.Right;
						
						while(activeFloat!=null){
							
							// Is the current render point now higher than this floating object?
							// If so, we must reduce LineStart/ increase MaxX depending on which type of float it is.
							
							if((PenY+ClearY_)>=(activeFloat.ParentOffsetTop + activeFloat.Height + activeFloat.Margin.Bottom)){
								
								// Clear!
								
								// Pop:
								Floats.Right=activeFloat.NextOnLine;
								
								// Increase max x:
								MaxX+=activeFloat.TotalWidth;
								
							}else{
								
								// Didn't clear - stop there.
								// (We don't want to clear any further over to the right).
								break;
								
							}
							
							activeFloat=activeFloat.NextOnLine;
						}
						
					}
					
				}
				
			}
			
			if((settings & LineBreakMode.Break)!=0){
				
				// Finally, reset the pen 
				// (this is after the recursion call, so we've cleared floats etc):
				MaxOffset=0f;
				PenX=LineStart;
				LineHeight=0f;
				
			}
			
		}
		
		/// <summary>Part of the bi-directional algorithm. Converts leftwards boxes to rightwards ones.</summary>
		private void RightwardsAlign(LayoutBox currentBox,LayoutBox to,float lineMax){
			
			/*
			// Note that word order can be confused without this.
			while(currentBox!=null && currentBox!=to){
			
				// Update its offset left:
				currentBox.ParentOffsetLeft=lineMax - (currentBox.Width + currentBox.ParentOffsetLeft);
				
				// Next on line:
				currentBox=currentBox.NextOnLine;
				
			}
			*/
			
		}
		
		/// <summary>Attempts to clear left or right. If they're both the same height
		/// then it will clear the given side.</summary>
		private bool ClearLineSpace(int floatMode){
			
			if(Floats!=null){
				
				if(Floats.Right!=null){
					
					if(Floats.Left==null){
						
						// Clear right:
						ClearFloat(FloatMode.Right);
						
					}else{
						
						// Clear shortest:
						float clearanceL=FloatClearance(true);
						float clearanceR=FloatClearance(false);
						
						if(clearanceL>clearanceR){
							
							// R first.
							ClearFloat(FloatMode.Right);
							
						}else if(clearanceR>clearanceL){
							
							// L first.
							ClearFloat(FloatMode.Left);
							
						}else{
							
							// They're the same. Clear *this* side:
							ClearFloat(floatMode);
							
						}
						
					}
					
				}else if(Floats.Left!=null){
					
					// Clear left:
					ClearFloat(FloatMode.Left);
					
				}else{
					return false;
				}
				
			}else{
				return false;
			}
			
			return true;
		}
		
		/// <summary>Advances the pen now.</summary>
		public void AdvancePen(LayoutBox styleBox){
			
			int floatMode=styleBox.FloatMode;
			
			if(floatMode!=FloatMode.None && this is BlockBoxMeta){
				
				// Float (block/inline-block only):
				BlockBoxMeta bbm=this as BlockBoxMeta;
				
				// Always apply top here (no vertical-align and must be after the above clear):
				styleBox.ParentOffsetTop=bbm.PenY_ + bbm.ClearY_ + styleBox.Margin.Top;
				
				// Pen only advances for left:
				if(
					(floatMode==FloatMode.Left && !GoingLeftwards) || 
					(floatMode==FloatMode.Right && GoingLeftwards)
				){
					
					// Same as below (but x only)
					PenX+=styleBox.Margin.Left+styleBox.Width+styleBox.Margin.Right;
					
				}
				
			}else{
				
				PenX+=styleBox.Margin.Left;
				styleBox.ParentOffsetLeft=PenX;
				PenX+=styleBox.Width+styleBox.Margin.Right;
				
				// If it's not a flow root then don't use total height.
				// If it's a word then we don't check it at all.
				float effectiveHeight;
				
				if(styleBox.DisplayMode==DisplayMode.Inline){
				
					effectiveHeight=styleBox.InnerHeight;
				}else{
					effectiveHeight=styleBox.TotalHeight;
				}
				
				if(effectiveHeight>LineHeight){
					LineHeight=effectiveHeight;
				}
				
				float baseline=styleBox.Baseline;
				
				if(baseline>CurrentBox.Baseline){
					CurrentBox.Baseline=baseline;
				}
				
			}
			
		}
		
	}
	
	public class BlockBoxMeta : LineBoxMeta{
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		internal float PenY_;
		/// <summary>The point at which lines begin at.</summary>
		internal float LineStart_;
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		internal bool GoingLeftwards_;
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		internal float MaxX_;
		/// <summary>The previous block margin (margin-bottom). Used for margin collapsing.</summary>
		public float PreviousMargin;
		/// <summary>The length of the longest line so far.</summary>
		public float LargestLineWidth_;
		
		
		public BlockBoxMeta(LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData):base(parent,firstBox,renderData){}
		
		/// <summary>The current y location of the renderer in screen pixels from the top.</summary>
		public override float PenY{
			get{
				return PenY_;
			}
			set{
				PenY_=value;
			}
		}
		
		/// <summary>True if the rendering direction is left. This originates from the direction: css property.</summary>
		public override bool GoingLeftwards{
			get{
				return GoingLeftwards_;
			}
			set{
				GoingLeftwards_=value;
			}
		}
		
		/// <summary>The x value that must not be exceeded by elements on a line. Used if the parent has fixed width.</summary>
		public override float MaxX{
			get{
				return MaxX_;
			}
			set{
				MaxX_=value;
			}
		}
		
		/// <summary>The starting line point.</summary>
		public override float LineStart{
			get{
				return LineStart_;
			}
			set{
				LineStart_=value;
			}
		}
		
		/// <summary>The length of the longest line so far.</summary>
		public override float LargestLineWidth{
			get{
				return LargestLineWidth_;
			}
			set{
				LargestLineWidth_=value;
			}
		}
		
	}
	
	public class InlineBoxMeta : LineBoxMeta{
		
		public InlineBoxMeta(BlockBoxMeta block,LineBoxMeta parent,LayoutBox firstBox,RenderableData renderData):base(parent,firstBox,renderData){
			
			MaxOffset=parent.PenX + firstBox.InlineStyleOffsetLeft;
			HostBlock=block;
			
		}
		
	}
	
}