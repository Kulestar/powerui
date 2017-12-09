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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Css;
using Dom;


namespace PowerUI{
	
	/// <summary>
	/// One input grid per renderer.
	/// </summary>
	public class InputGrid{
		
		/// <summary>The number of cells on x.</summary> 
		public int Width;
		/// <summary>The number of cells on y.</summary>
		public int Height;
		/// <summary>The cell size.</summary>
		public float CellSize;
		/// <summary>The raw grid cells.</summary>
		public InputGridCell[] Grid;
		/// <summary>The head of the grid entry pool.</summary>
		private InputGridEntry PooledCell_;
		
		
		/// <summary>Pushes the given renderable data into the grid now.</summary>
		public void Push(RenderableData renderData){
			
			// Node must be an element:
			if(renderData==null || !(renderData.Node is Dom.Element)){
				return;
			}
			
			// Get the region:
			ScreenRegion region=renderData.OnScreenRegion;
			
			if(region==null){
				return;
			}
			
			// Get the bounds:
			int minX=(int)(region.ScreenMinX/CellSize);
			int minY=(int)(region.ScreenMinY/CellSize);
			
			if(minX>=Width || minY>=Height){
				// ..It's offscreen?
				return;
			}
			
			if(minX<0){
				minX=0;
			}
			
			if(minY<0){
				minY=0;
			}
			
			// Max (inclusive):
			int maxX=(int)(region.ScreenMaxX/CellSize);
			int maxY=(int)(region.ScreenMaxY/CellSize);
			
			if(maxX<0 || maxY<0){
				// ..It's offscreen?
				return;
			}
			
			if(maxX>=Width){
				maxX=Width-1;
			}
			
			if(maxY>=Height){
				maxY=Height-1;
			}
			
			// Generating a bunch of entries now!
			int cellIndex=minY * Width;
			
			// Only having one box means we don't need to do multiline testing.
			// It also means we can optimise "central" boxes too (see below about that).
			bool oneBox=(renderData.FirstBox!=null && renderData.FirstBox.NextInElement==null);
			
			for(int y=minY;y<=maxY;y++){
				
				// Central boxes are on *both* middle X and middle Y
				// The content in the cell already is being completely overlapped
				// so we can simply empty it out (and recycle the entries too).
				bool centralBox=(oneBox && y!=minY && y!=maxY);
				
				for(int x=minX;x<=maxX;x++){
					
					// Get the target cell:
					InputGridCell cell=Grid[cellIndex+x];
					
					if(centralBox && x!=minX && x!=maxX){
						
						// Overwrite! Pool the cell:
						if(cell.Back!=null){
							
							// Pool:
							cell.Back.Previous=PooledCell_;
							PooledCell_=cell.Front;
							
							// Clear:
							cell.Back=null;
							cell.Front=null;
							
						}
						
					}
					
					// Get an entry (typically from the pool):
					InputGridEntry ige=GetEntry();
					
					// Set it up:
					ige.RenderData=renderData;
					
					if(cell.Front==null){
						cell.Back=ige;
						cell.Front=ige;
					}else{
						// Stack on the front:
						ige.Previous=cell.Front;
						cell.Front=ige;
					}
					
				}
				
				cellIndex+=Width;
				
			}
			
		}
		
		/// <summary>Gets a grid entry for the given 
		private InputGridEntry GetEntry(){
			
			InputGridEntry ige=PooledCell_;
			
			if(ige==null){
				return new InputGridEntry();
			}
			
			// Update pool:
			PooledCell_=ige.Previous;
			ige.Previous=null;
			
			return ige;
		}
		
		/// <summary>Empties the grid into the pool.</summary>
		public void Empty(){
			
			if(Grid==null){
				return;
			}
			
			// For each grid cell..
			for(int i=0;i<Grid.Length;i++){
				
				InputGridCell cell=Grid[i];
				
				if(cell.Back!=null){
					
					// Pool:
					cell.Back.Previous=PooledCell_;
					PooledCell_=cell.Front;
					
					// Clear:
					cell.Back=null;
					cell.Front=null;
					
				}
				
			}
			
		}
		
		/// <summary>Empties out and resizes the grid (if it requires it).</summary>
		public void Clean(float sWidth,float sHeight){
			
			float cellSize=InputGridCell.Size;
			CellSize=cellSize;
			
			// Get the number of cells on x:
			int width=(int)Math.Ceiling( sWidth/cellSize );
			int height=(int)Math.Ceiling( sHeight/cellSize );
			
			if(Grid==null){
				
				Width=width;
				Height=height;
				Grid=new InputGridCell[width * height];
				
				// Create the cells:
				for(int i=0;i<Grid.Length;i++){
					Grid[i]=new InputGridCell();
				}
				
			}else if(Width!=width || Height!=height){
				
				// Pool the nodes:
				Empty();
				
				Width=width;
				Height=height;
				InputGridCell[] newGrid=new InputGridCell[width * height];
				
				// Copy over the cells into the new array, and create any that are required.
				int max=(newGrid.Length < Grid.Length) ? newGrid.Length : Grid.Length;
				
				// Copy that many:
				Array.Copy(Grid,0,newGrid,0,max);
				
				// Fill the rest:
				for(int i=max;i<newGrid.Length;i++){
					
					newGrid[i]=new InputGridCell();
					
				}
				
				Grid=newGrid;
				
			}
			
		}
		
		/// <summary>gets a cell at the given indices.</summary>
		public InputGridCell this[int x,int y]{
			get{
				if(Grid==null || x<0 || x>=Width || y<0 || y>=Height){
					return null;
				}
				
				return Grid[(y*Width)+x];
			}
		}
		
	}
	
	/// <summary>
	/// A single cell in an input grid.
	/// </summary>
	public class InputGridCell{
		
		/// <summary>The common input grid cell size. Based on DPI and is one "inch" by default.</summary>
		public static float Size{
			get{
				
				// One inch squares is a reasonable input size in most instances.
				// -> If they're smaller, then reflow will be slower creating more cells
				// (and it wouldn't really gain anything because UI elements aren't typically tiny anyway)
				
				// -> If they're bigger, you end up with faster reflow but resolving input slows down
				// (as more elements end up in the same cell)
				
				return (float)ScreenInfo.Dpi;
				
			}
		}
		
		/// <summary>The first entry (most towards the back).</summary>
		public InputGridEntry Back;
		/// <summary>The last entry (most towards the front).</summary>
		public InputGridEntry Front;
		
		
		/// <summary>Gets the renderable data at the given point (may be null).</summary>
		public RenderableData Get(float x,float y){
			
			InputGridEntry ige=Front;
			
			while(ige!=null){
				
				// Get the render data:
				RenderableData renderData=ige.RenderData;
				
				// Get the zone:
				ScreenRegion screenBox=renderData.OnScreenRegion;
				
				// Is this node visible and is the point within it?
				if(screenBox!=null && screenBox.Contains(x,y)){
					
					// At this point, the mouse could still be outside it.
					// This happens with inline elements - their clipping boundary can contain multiple sub-boxes.
					// So, time to check how many boxes it has, then the individual boxes if we've got more than one.
					
					LayoutBox box=renderData.FirstBox;
					
					if(box!=null && box.NextInElement!=null){
						
						// Multiple boxes. Must be contained in one of them to win.
						
						while(box!=null){
							
							if(box.Contains(x,y)){
								// Ok!
								return renderData;
							}
							
							// Advance to the next one:
							box=box.NextInElement;
							
						}
						
					}else{
						
						// Yep!
						return renderData;
						
					}
					
				}
				
				ige=ige.Previous;
			}
			
			return null;
			
		}
		
		public void Pool(){}
		
	}
	
	/// <summary>
	/// A single entry in the input grid.
	/// There's one grid per Renderman and it's built during reflow.
	/// </summary>
	public class InputGridEntry{
		
		/// <summary>The one behind this one.</summary>
		public InputGridEntry Previous;
		/// <summary>The renderable data to test.</summary>
		public RenderableData RenderData;
		
	}
	
}