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
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Dom;

namespace PowerUI{
	
	/// <summary>
	/// Manages all current file protocols://.
	/// File protocols such as http, cache, dynamic, scene etc enable PowerUI to load files in
	/// custom ways - for example if your game uses a specialised cdn, you may easily implement
	/// it as a new FileProtocol.
	/// </summary>
	
	public static class FileProtocols{
		
		/// <summary>Http is used for any unrecognised protocols.
		/// This is useful with e.g. links to apps, such as ms-windows-store://
		/// When a link like that occurs, and it's not overriden with a custom handler, http will deal with it (and subsequently pop it up externally).</summary>
		public static string UnrecognisedProtocolHandler="http";
		
		/// <summary>The set of available protocols. Use get to access.</summary>
		public static Dictionary<string,FileProtocol> Protocols;
		
		
		/// <summary>Adds the given file protocol to the global set for use.
		/// Note that you do not need to call this manually; Just deriving from
		/// FileProtocol is all that is required.</summary>
		/// <param name="protocolType">The type of the protocol to add.</param>
		public static void Add(Type protocolType){
			
			if(Protocols==null){
				Protocols=new Dictionary<string,FileProtocol>();
			}
			
			// Instance it:
			FileProtocol protocol=(FileProtocol)Activator.CreateInstance(protocolType);
			
			// Get the names:
			string[] nameSet=protocol.GetNames();
			
			if(nameSet==null){
				return;
			}
			
			foreach(string name in nameSet){
				Protocols[name.ToLower()]=protocol;
			}
			
		}
		
		/// <summary>Gets a protocol by the given name.</summary>
		/// <param name="protocol">The name of the protocol, e.g. "http".</param>
		/// <returns>A FileProtocol if found; null otherwise.</returns>
		public static FileProtocol Get(string protocol){
			
			if(Protocols==null){
				throw new Exception("No file protocols available!");
			}
			
			if(protocol==null){
				protocol="";
			}
			
			FileProtocol result=null;
			if(!Protocols.TryGetValue(protocol.ToLower(),out result)){
				
				// Get the unrecognised protocol handler:
				Protocols.TryGetValue(UnrecognisedProtocolHandler,out result);
				
			}
			
			
			return result;
		}
		
	}
	
}