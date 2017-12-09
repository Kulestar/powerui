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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	
	public partial class HtmlElement{
		
		/// <summary>The union of all client rects.</summary>
		public BoxRegion getBoundingClientRect(){
			
			// Get the first box:
			LayoutBox box=RenderData.FirstBox;
			
			// Get the first rect:
			float x=box.X;
			float y=box.Y;
			float maxX=box.MaxX;
			float maxY=box.MaxY;
			
			// Go to next one:
			box=box.NextInElement;
			
			while(box!=null){
				
				// Combine it in:
				if(box.X<x){
					x=box.X;
				}
				
				if(box.Y<y){
					y=box.Y;
				}
				
				if(box.MaxX>maxX){
					maxX=box.MaxX;
				}
				
				if(box.MaxY>maxY){
					maxY=box.MaxY;
				}
				
				// Go to next one:
				box=box.NextInElement;
				
			}
			
			return new BoxRegion(x,y,maxX-x,maxY-y);
			
		}
		
		/// <summary>This elements client rects.</summary>
		public LayoutBox[] getClientRects(){
			
			// Create the set:
			int count=RenderData.BoxCount;
			LayoutBox[] set=new LayoutBox[count];
			
			// Get the first box:
			LayoutBox box=RenderData.FirstBox;
			int i=0;
			
			while(box!=null){
				
				// Add:
				set[i++]=box;
				
				// Go to next one:
				box=box.NextInElement;
				
			}
			
			return set;
			
		}
		
	}
	
}