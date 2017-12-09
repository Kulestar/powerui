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

using Dom;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// Represents a meter element.
	/// </summary>
	
	[Dom.TagName("meter")]
	public class HtmlMeterElement:HtmlElement{
		
		/// <summary>Progress bar priority (it's a virtual div).</summary>
		public const int Priority=Css.VirtualElements.DURING_ZONE+90;
		
		/// <summary>The min.</summary>
		private double Min_=0;
		/// <summary>The max.</summary>
		private double Max_=1.0;
		/// <summary>The low bound.</summary>
		private double Low_=double.MinValue;
		/// <summary>The high bound.</summary>
		private double High_=double.MaxValue;
		/// <summary>The current value.</summary>
		private double Value_=0;
		/// <summary>The internal bar.</summary>
		private HtmlDivElement Bar_;
		
		
		/// <summary>Can this element have a label?</summary>
		internal override bool IsFormLabelable{
			get{
				return true;
			}
		}
		
		/// <summary>All labels targeting this select element.</summary>
		public NodeList labels{
			get{
				return HtmlLabelElement.FindAll(this);
			}
		}
		
		/// <summary>The current position.</summary>
		public double position{
			get{
				return (Value_-Min_)/(Max_-Min_);
			}
		}
		
		/// <summary>The high bound.</summary>
		public double high{
			get{
				if(High_==double.MaxValue){
					return Max_;
				}
				return High_;
			}
			set{
				High_=value;
				setAttribute("high", value.ToString());
			}
		}
		
		/// <summary>The low bound.</summary>
		public double low{
			get{
				if(Low_==double.MinValue){
					return Min_;
				}
				return Low_;
			}
			set{
				Low_=value;
				setAttribute("low", value.ToString());
			}
		}
		
		/// <summary>The min.</summary>
		public double min{
			get{
				return Min_;
			}
			set{
				Min_=value;
				setAttribute("min", value.ToString());
			}
		}
		
		/// <summary>The max.</summary>
		public double max{
			get{
				return Max_;
			}
			set{
				Max_=value;
				setAttribute("max", value.ToString());
			}
		}
		
		/// <summary>The current value.</summary>
		public double Value{
			get{
				return Value_;
			}
			set{
				Value_=value;
				setAttribute("value", value.ToString());
			}
		}
		
		/// <summary>The optimum numeric value.</summary>
		public double optimum{
			get{
				double v;
				double.TryParse(getAttribute("optimum"),out v);
				return v;
			}
			set{
				setAttribute("optimum", value.ToString());
			}
		}
		
		/// <summary>Updates the progress of the bar.</summary>
		private void Refresh(){
			
			if(Bar_==null){
				return;
			}
			
			double pos=position;
			
			if(pos<0.0){
				pos=0;
			}else if(pos>1.0){
				pos=1;
			}
			
			// Update any additional range attributes:
			
			
			// Update the virtual bar:
			Bar_.style.width=(pos * 100.0)+"%";
			
		}
		
		public override void OnTagLoaded(){
			
			// Append the internal green bar:
			ComputedStyle computed=Style.Computed;
			Bar_=computed.GetOrCreateVirtual(Priority,"div") as HtmlDivElement;
			
		}
		
		public override bool OnAttributeChange(string property){
			if(base.OnAttributeChange(property)){
				return true;
			}
			
			if(property=="min"){
			
				if(!double.TryParse(getAttribute("min"),out Min_)){
					Min_=0;
				}
				
				// Update the bar:
				Refresh();
				
				return true;
				
			}else if(property=="max"){
				
				if(!double.TryParse(getAttribute("max"),out Max_)){
					Max_=1.0;
				}
				
				// Update the bar:
				Refresh();
				
				return true;
				
			}else if(property=="high"){
				
				if(!double.TryParse(getAttribute("high"),out High_)){
					High_=double.MaxValue;
				}
				
				// Update the bar:
				Refresh();
				
				return true;
				
			}else if(property=="low"){
				
				if(!double.TryParse(getAttribute("low"),out Low_)){
					Low_=double.MinValue;
				}
				
				// Update the bar:
				Refresh();
				
				return true;
				
			}else if(property=="value"){
				
				if(!double.TryParse(getAttribute("value"),out Value_)){
					Value_=0;
				}
				
				// Update the bar:
				Refresh();
				
				return true;
				
			}
			
			return false;
			
		}
		
	}
	
}