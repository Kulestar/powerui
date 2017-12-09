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


namespace PowerUI{
	
	public class TouchList : IEnumerable<TouchPointer>{
		
		List<TouchPointer> values=new List<TouchPointer>();
		
		/// <summary>Removes the given index.</summary>
		public void removeAt(int index){
			values.RemoveAt(index);
		}
		
		/// <summary>Removes the given node.</summary>
		public void remove(TouchPointer touch){
			values.Remove(touch);
		}
		
		/// <summary>Insert at the given index.</summary>
		public void insert(int index,TouchPointer touch){
			values.Insert(index,touch);
		}
		
		/// <summary>Adds the given touch to the list.</summary>
		public void push(TouchPointer touch){
			values.Add(touch);
		}
		
		/// <summary>The number of nodes in the list.</summary>
		public int length{
			get{
				return values.Count;
			}
		}
		
		/// <summary>Gets a touch at a particular index.</summary>
		public TouchPointer item(int index){
			return values[index];
		}
		
		/// <summary>Gets an element at the specified index.</summary>
		public TouchPointer this[int index]{
			get{
				return values[index];
			}
			internal set{
				values[index]=value;
			}
		}
		
		public IEnumerator<TouchPointer> GetEnumerator(){
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
}