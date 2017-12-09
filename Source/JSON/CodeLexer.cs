//--------------------------------------
//         Nitro Script Engine
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         nitro.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using Dom;

namespace Json{

	/// <summary>
	/// Provides a wrapper for reading characters from a string of code.
	/// It strips comments and other junk internally such as tabs and spaces.
	/// </summary>

	public class CodeLexer:StringReader{
		
		/// <summary>Set this true if no junk should be stripped out.</summary>
		public bool Literal;
		/// <summary>The current line the lexer is on.</summary>
		public int LineNumber=1;
		/// <summary>True if the lexer just read some junk.</summary>
		public bool DidReadJunk;
		
		/// <summary>Creates a new code lexer with the given code text.</summary>
		/// <param name="code">The code to parse.</param>
		public CodeLexer(string code):base(code){
			// Skip any initial junk:
			while(ReadJunk()){}
		}
		
		/// <summary>Reads a character from the string.</summary>
		/// <returns>The character read.</returns>
		public override char Read(){
			char read=base.Read();
			DidReadJunk=false;
			if(!Literal){
				while(ReadJunk()){
					DidReadJunk=true;
				}
			}else{
				LineCheck(read);
			}
			return read;
		}
		
		/// <summary>Increases the line count if the given character is a newline.</summary>
		private void LineCheck(char c){
			if(c=='\n'){
				LineNumber++;
			}
		}
		
		/// <summary>Finds if the current location is followed by junk. Note that the junk has already been read off.</summary>
		/// <returns>True if it is, false otherwise.</returns>
		public bool PeekJunk(){
			return DidReadJunk;
		}
		
		/// <summary>Skips junk in the text such as tabs or comments.</summary>
		/// <returns>True if junk was read off.</returns>
		public bool ReadJunk(){
			char c=Peek();
			if(c==StringReader.NULL){
				// End of string
				return false;
			}
			if(c=='/'){
				if(Peek(1)=='*'){
					// Block comment
					Advance();
					Advance();
					c=Peek();
					while(c!=StringReader.NULL){
						if(c=='*'&&Peek(1)=='/'){
							Advance();
							Advance();
							break;
						}
						LineCheck(c);
						Advance();
						c=Peek();
					}
					return (c!=StringReader.NULL);
				}else if(Peek(1)=='/'){
					char peek=Peek();
					while(peek!=StringReader.NULL&&peek!='\n'&&peek!='\r'){
						Advance();
						peek=Peek();
					}
					return true;
				}
			}else if(c==' '||c=='\n'||c=='\r'||c=='\t'){
				LineCheck(c);
				Advance();
				
				if(Peek()=='#' && Peek(1)=='l'){
					Advance();
					Advance();
					
					// Line number next:
					System.Text.StringBuilder lineNumber=new System.Text.StringBuilder();
					
					char peek=Peek();
					
					while(peek!=StringReader.NULL&&peek!='\n'&&peek!='\r'){
						lineNumber.Append(peek);
						Advance();
						peek=Peek();
					}
					
					// Parse it now:
					LineNumber=int.Parse(lineNumber.ToString());
				}
				
				return true;
			}
			return false;
		}
		
	}
	
}