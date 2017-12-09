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
using Dom;
using Css;


namespace PowerUI{

	/// <summary>
	/// A html element which is created when using &variables;.
	/// They are tracked in this way so their content can be replaced
	/// if the language of the UI changes.
	/// </summary>
	
	[Dom.TagName("LangNode")]
	public class VariableElement:HtmlSpanElement,Dom.ILangNode{
		
		/// <summary>The &Name; used.</summary>
		internal string Name;
		/// <summary>The set of arguments that can be passed into a variable at runtime. &Name(a0,a1,a2..);
		/// Arguments are accessed simply with &arg.X;</summary>
		internal string[] Arguments;
		
		
		/// <summary>Changes the name of the variable. Thats the text used &Here;.</summary>
		public void SetVariableName(string name){
			Name=name;
			
		}
		
		/// <summary>Reloads the content of this variable if it's name matches the given one.</summary>
		/// <param name="name">The name of the variable to reset.</param>
		internal override void ResetVariable(string name){
			if(Name==name){
				LoadNow();
			}
			
			// Allow resetting internal ones:
			base.ResetVariable(name);
		}
		
		/// <summary>Reloads the content of this variable element. This should
		/// be used when the language of the UI changes.</summary>
		internal override void ResetAllVariables(){
			LoadNow();
		}
		
		/// <summary>Reads the runtime argument at the given index. Used with &arg.X; where x starts from 0.
		/// The arguments themselves originate from e.g. &variableName(arg0,arg1,arg2..);</summary>
		public string GetArgument(int id){
			if(Arguments==null || id>=Arguments.Length || id<0){
				throw new Exception("Argument ID ("+id+") out of range for this variable.");
			}
			return Arguments[id];
		}
		
		/// <summary>Sets the runtime argument set. See the Arguments variable.</summary>
		/// <param name="arguments">The new arguments.</param>
		public void SetArguments(string[] arguments){
			Arguments=arguments;
		}
		
		/// <summary>The internal text content of the lang node.</summary>
		public override string textContent{
			get{
				NodeList kids=childNodes_;
				
				if(kids==null){
					return "";
				}
				
				string res="";
				
				for(int i=0;i<kids.length;i++){
					res+=kids[i].textContent;
				}
				
				return res;
			}
			set{}
		}
		
		public override void ToString(System.Text.StringBuilder builder){
			
			// HTML of this is just its arg:
			builder.Append('&');
			builder.Append(Name);
			builder.Append(';');
			
		}
		
		/// <summary>Loads the content of this variable element by looking up the
		/// content for the variables name.</summary>
		public void LoadNow(){
			
			if(Name.StartsWith("arg.")){
				
				// Load the argument from the parent variable.
				string id=Name.Substring(4).Trim();
				innerHTML=((VariableElement)parentNode).GetArgument(int.Parse(id));
				
			}else{
				
				// Use a variable lookup:
				UI.Variables.GetValue(Name,document_,delegate(string value){
					innerHTML=value;
				});
				
			}
			
		}
		
		/// <summary>Gets this elements content as text.</summary>
		/// <returns>A text string of the elements content.</returns>
		public override string ToString(){
			return "&"+Name+";";
		}
		
		/// <summary>Gets or sets the innerHTML of this element.</summary>
		public override string innerHTML{
			get{
				
				System.Text.StringBuilder result=new System.Text.StringBuilder();
				
				if(childNodes_!=null){
					
					for(int i=0;i<childNodes_.length;i++){
						childNodes_[i].ToString(result);
					}
					
				}
				
				return result.ToString();
			}
			set{
				
				if(childNodes_!=null){
					empty();
				}
				
				if(!string.IsNullOrEmpty(value)){
					
					// Parse now:
					HtmlLexer lexer=new HtmlLexer(value,this);
					
					// PCData is fine here.
					
					// Ok!
					lexer.Parse();
				
				}
				
			}
		}
		
	}
	
}