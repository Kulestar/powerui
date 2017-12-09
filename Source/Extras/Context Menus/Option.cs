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


namespace ContextMenus{
	
	/// <summary>An event occuring on options.</summary>
	public delegate void OptionEventMethod(Option sender);
	
	/// <summary>
	/// An option.
	/// </summary>
	
	public class Option : OptionList{
		
		/// <summary>The index of this option in the parent list.</summary>
		public int index;
		/// <summary>The HTML for this option.</summary>
		public string markup;
		/// <summary>The latest element that represents 'this' option.
		/// Note that this is different from 'element' (which is potentially a submenu).
		/// Optionally set so e.g. submenu's can know where to go.</summary>
		public PowerUI.HtmlElement buttonElement;
		/// <summary>The method to run.</summary>
		public OptionEventMethod method;
		
		
		public Option(){}
		
		public Option(string markup,OptionEventMethod method){
			
			this.markup=markup;
			this.method=method;
			
		}
		
		/// <summary>Two attributes (optindex='index' and onclick='OptionList.ResolveOptionFromClick') which make an option clickable.</summary>
		public string mouseRef{
			get{
				return "optindex='"+index+"' onclick='ContextMenus.OptionList.ResolveOptionFromClick'";
			}
		}
		
		/// <summary>Closes the whole menu.</summary>
		public void closeMenu(){
			parent.close(true);
		}
		
		/// <summary>Run this option.</summary>
		public override void run(){
			
			// If there's no delegate, check if we've got some sub-option to run instead.
			if(method!=null){
				
				// Run the method:
				method(this);
				
				// Close the menu:
				closeMenu();
				
			}else{
				
				// Run option 0:
				base.run();
				
			}
			
		}
		
	}
	
}