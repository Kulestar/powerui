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


using Css;
using UnityEngine;
using Dom;
using Widgets;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// This widget:// or window:// protocol enables a link to pop open or close a widget.
	/// E.g. href="window://floating/bank" will open a 'floating' type window and load 'Resources/bank/index.html' into it.
	/// </summary>
	
	public class WidgetProtocol:FileProtocol{
		
		public override string[] GetNames(){
			return new string[]{"window","widget"};
		}
		
		public override void OnFollowLink(HtmlElement linkElement,Location path){
			
			// First get the widget type by name:
			
			// Get the doc:
			HtmlDocument doc=linkElement.htmlDocument;
			
			// The 'domain' is the widget type:
			string widgetType=path.host;
			
			// The rest of the path is the URL (e.g. '/bank'):
			string url=path.pathname;
			
			// Watch out for e.g. /bank/
			if(url!="" && url[0]=='/'){
				// Make it just e.g. 'bank':
				url=url.Substring(1);
			}
			
			if(url!="" && url[url.Length-1]=='/'){
				url=url.Substring(0,url.Length-1);
			}
			
			// Any query string is passed in as extras:
			Dictionary<string,string> searchParams=path.searchParams;
			
			Dictionary<string,object> globals=new Dictionary<string,object>();
			
			// Remap them to string/object pairs:
			if(searchParams!=null){
				
				foreach(KeyValuePair<string,string> kvp in searchParams){
					
					globals.Add(kvp.Key,kvp.Value);
					
				}
				
			}
			
			// Add the anchor:
			globals["-spark-anchor"]=linkElement;
			
			// Cycle the widget (closes it if it's open):
			doc.widgets.cycle(widgetType,url,globals);
			
		}
		
	}
	
}