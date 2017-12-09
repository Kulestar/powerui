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
using System.Threading;


namespace PowerUI{
	
	/// <summary>A class which manages callbacks running on Unity's main thread.
	/// Use Callback.MainThread to run some code on the main thread.</summary>
	
	internal static class Callbacks{
	
	
		/// <summary>Unity's main thread.</summary>
		internal static Thread MainThread;
		
		/// <summary>The main callback queue. Stored as a linked list - this is the tail of the queue.</summary>
		internal static Callback LastToRun;
		/// <summary>The main callback queue. Stored as a linked list - this is the head of the queue.</summary>
		internal static Callback FirstToRun;
		
		
		/// <summary>Sets up the callback system. Always called on Unity's main thread.</summary>
		internal static void Start(){
			
			MainThread=Thread.CurrentThread;
			
		}
		
		/// <summary>Runs all callbacks in the queue.</summary>
		internal static void RunAll(){
			
			// Grab the one at the front:
			Callback current=FirstToRun;
			
			// Clear the queue:
			FirstToRun=null;
			LastToRun=null;
			
			while(current!=null){
				
				try{
					// Run it now:
					current.ToRun();
					
				}catch(Exception e){
					Dom.Log.Add("Callback error: "+e.ToString());
				}
				
				// Hop to the next one:
				current=current.NextCallback;
			}
			
		}
		
		/// <summary>Don't call this directly. Use callback.Go() instead. Adds the callback to the main queue.</summary>
		internal static void Add(Callback callback){
			
			if(FirstToRun==null){
				FirstToRun=LastToRun=callback;
				return;
			}
			
			if(LastToRun!=null){
				// Add to the end:
				LastToRun.NextCallback=callback;
			}
			
			LastToRun=callback;
			
		}
	
	}
	
}