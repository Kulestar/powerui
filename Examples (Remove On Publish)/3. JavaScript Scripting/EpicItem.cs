using System;
using UnityEngine;

namespace JavascriptExample {
	
	/// <summary>
	/// This class is used by the Nitro Example.
	/// It shows that nitro can easily work with objects of your own design.
	/// </summary>

	public class EpicItem{
		
		public string ID;
		private int _Quantity;
		
		public EpicItem(string id,int quantity){
			ID=id;
			_Quantity=quantity;
		}
		
		/// <summary>Shows Nitro works with properties.</summary>
		public int Quantity{
			get{
				return _Quantity;
			}
			
			set{
				if(value<0){
					value=0;
				}
				_Quantity=value;
			}
		}
		
		/// <summary>Shows nitro works with methods. Adds one to the quantity and returns it.</summary>
		public int AddOne(){
			_Quantity++;
			return _Quantity;
		}
		
	}

}