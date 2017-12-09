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
	
	/// <summary>This delegate represents a method to call on the main thread.</summary>
	public delegate void MainThreadDelegate();
	
	/// <summary>
	/// Callbacks can be used to make sure certain things run on Unity's main thread.
	/// To use them, call Callback.MainThread(delegate(){ main thread code in here });
	/// Note that order is not necessarily retained if you happen to call it on the main thread anyway - it will run immediately.
	/// </summary>
	
	public class Callback{
		
		/// <summary>Queues the given delegate in order to run it on the main Unity thread.</summary>
		public static void MainThread(MainThreadDelegate toRun){
			
			if(Thread.CurrentThread==Callbacks.MainThread){
				toRun();
				return;
			}
			
			// Enqueue:
			Callbacks.Add(new Callback(toRun));
			
		}
		
		/// <summary>Stored as a linked list - this is the next callback.</summary>
		public Callback NextCallback;
		/// <summary>The delegate to run.</summary>
		public MainThreadDelegate ToRun;
		
		
		public Callback(MainThreadDelegate mtd){
			
			if(mtd==null){
				throw new ArgumentNullException("Callbacks must have a method to run.");
			}
			
			ToRun=mtd;
		}
		
		/// <summary>True if callbacks will run immediately with no delay.</summary>
		public static bool WillRunImmediately{
			get{
				return (Thread.CurrentThread==Callbacks.MainThread);
			}
		}
		
		/// <summary>True if callbacks will be delayed until the next callback run.</summary>
		public static bool WillDelay{
			get{
				
				return (Thread.CurrentThread!=Callbacks.MainThread);
				
			}
		}
		
	}
	
}