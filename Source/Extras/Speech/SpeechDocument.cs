using System;
using System.Collections;
using Dom;
using Css;

namespace Speech{
	
	/// <summary>
	/// A Speech markup document is used when speech is generated standalone.
	/// </summary>
	
	public class SpeechDocument : ReflowDocument{
		
		/// <summary>Cached reference for the Speech namespace.</summary>
		private static MLNamespace _SpeechNamespace;
		
		/// <summary>The XML namespace for speech markup language.</summary>
		public static MLNamespace SpeechNamespace{
			get{
				if(_SpeechNamespace==null){
					
					// Setup the namespace (Doesn't request the URL; see XML namespaces for more info):
					_SpeechNamespace=Dom.MLNamespaces.Get("http://www.w3.org/2001/10/synthesis","ssml","application/ssml+xml");
					
				}
				
				return _SpeechNamespace;
			}
		}
		
		private bool IsOpen=true;
		private string CurrentTitle;
		
		
		public override string title{
			get{
				return CurrentTitle;
			}
			set{
				CurrentTitle=value;
			}
		}
		
		/// <summary>The root speak element.</summary>
		public SpeechSpeakElement speak;
		
		
		public SpeechDocument():base(null){
			
			// Apply namespace:
			Namespace=SpeechNamespace;
			
		}
		
		/// <summary>The root style node.</summary>
		public override Dom.Element documentElement{
			get{
				return speak;
			}
		}
		
		/// <summary>Gets or sets the innerSSML of this document.</summary>
		public string innerSSML{
			get{
				return innerML;
			}
			set{
				innerML=value;
			}
		}
		
		/// <summary>Gets or sets the innerSSML of this document.</summary>
		public override string innerML{
			get{
				System.Text.StringBuilder builder=new System.Text.StringBuilder();
				ToString(builder);
				return builder.ToString();
			}
			set{
				// Open parse and close:
				IsOpen=false;
				open();
				
				// Parse now:
				HtmlLexer lexer=new HtmlLexer(value,this);
				lexer.Parse();
				
				// Dom loaded:
				ContentLoadedEvent();
				
				close();
			}
		}
		
		/// <summary>Closes the document.</summary>
		public void close(){
			
			if(!IsOpen){
				// Already closed.
				return;
			}
			
			// Mark as closed:
			IsOpen=false;
			
			// Force a render request as required:
			RequestLayout();
			
		}
		
		/// <summary>Opens the document for writing.</summary>
		public void open(){
			
			if(IsOpen){
				// Already open
				return;
			}
			
			// Mark as open:
			IsOpen=true;
			
			// Clear it:
			clear();
			
		}
		
		/// <summary>Clears the document of all it's content, including scripts and styles.</summary>
		public override void clear(){
			
			speak=null;
			
			// Gracefully clear the innerHTML:
			base.clear();
			
		}
		
	}
	
}