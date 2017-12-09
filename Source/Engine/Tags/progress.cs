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
	/// Represents a progress element.
	/// Select the contents with progress > div.
	/// </summary>
	
	[Dom.TagName("progress")]
	public class HtmlProgressElement:HtmlElement{
		
		/// <summary>Progress bar priority (it's a virtual div).</summary>
		public const int Priority=Css.VirtualElements.DURING_ZONE+90;
		
		/// <summary>The max.</summary>
		private double Max_=1.0;
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
				return Value_/Max_;
			}
		}
		
		/// <summary>The max.</summary>
		public double max{
			get{
				return Max_;
			}
			set{
				Max_ = value;
				setAttribute("max", value.ToString());
			}
		}
		
		/// <summary>The current value.</summary>
		public double Value{
			get{
				return Value_;
			}
			set{
				Value_ = value;
				setAttribute("value", value.ToString());
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
			
			if(property=="max"){
				
				if(!double.TryParse(getAttribute("max"),out Max_)){
					Max_=1.0;
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