using System;
using Dom;


namespace PowerUI{
	
	public partial class HtmlDocument{
		
		public Event createEvent(string type){
			return createEvent(type,null,null);
		}
		
		public Event createEvent(string type,string evtType){
			return createEvent(type,evtType,null);
		}
		
		public Event createEvent(string type,string evtType,object init){
			
			if(type==null){
				return null;
			}
			
			type=type.Trim().ToLower();
			
			switch(type){
				
				case "uievent":
				case "uievents":
					
					return new UIEvent(evtType,init);
					
				case "mouseevent":
				case "mouseevents":
					
					return new MouseEvent(evtType,init);
				
				case "focusevent":
					
					return new FocusEvent(evtType,init);
					
				case "event":
				case "events":
				case "htmlevent":
				case "htmlevents":
					
					return new Event(evtType,init);
				
				case "textevent":
					
					return new TextEvent(evtType,init);
				
				case "keyboardevent":
					
					return new KeyboardEvent(evtType,init);
					
				case "customevent":
					
					return new CustomEvent(evtType,init);
				
			}
			
			// htmlevent, event etc:
			return new Event(evtType,init);
			
		}
		
	}
	
}