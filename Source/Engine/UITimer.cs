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
using Dom;


namespace PowerUI{
	
	/// <summary>A delegate used when a timer ticks.</summary>
	public delegate void OnUITimer();
	
	/// <summary>
	/// Provides a way of interrupting a timer event.
	/// Returned by <see cref="PowerUI.UICode.setTimeout"/> and <see cref="PowerUI.UICode.setInterval"/>.
	/// This object can be passed to <see cref="PowerUI.UICode.clearInterval"/> to prevent any further timing events.
	/// </summary>
	
	public class UITimer{
		
		/// <summary>A linked list of active UI timers (interval ones only).</summary>
		private static UITimer FirstActive;
		/// <summary>A linked list of active UI timers (interval ones only).</summary>
		private static UITimer LastActive;
		
		
		/// <summary>Called before the game/scene is unloaded to remove any timers.</summary>
		public static void OnUnload(Document document){
			
			if(FirstActive!=null){
				
				int count=0;
				UITimer current=FirstActive;
				FirstActive=null;
				
				while(current!=null){
					
					if(document==null || current.Document==document){
						
						count++;
						current.Stop();
						
					}
					
					current=current.Next;
				}
				
				if(count!=0){
					Dom.Log.Add("Cleared "+count+" leaked interval timers. This message is harmless but can help discover if you have accidentally left setInterval running (note: marquee uses these timers too).");
				}
				
			}
			
		}
		
		#if UNITY_METRO
		/// <summary>No timers available on Windows 8 so it's got to use the Unity update thread instead.</summary>
		public void Update(){
			UITimer current=FirstActive;
			
			while(current!=null){
				
				current.CurrentTime+=UnityEngine.Time.deltaTime;
				
				if(current.CurrentTime>current.MaxTime){
					// Tick!
					current.CurrentTime=0f;
					current.Elapsed(null);
				}
				
				current=current.Next;
			}
			
		}
		#endif
		
		/// <summary>True if this is a one off timer event and not an interval.</summary>
		public bool OneOff;
		#if UNITY_WP8
		/// <summary>The system timer that will time the callback.</summary>
		public System.Threading.Timer InternalTimer;
		#elif UNITY_METRO
		// No timer appears to be available on Windows 8 :(
		// Use the Unity update loop instead.
		/// <summary>The current ticked time.</summary>
		public float CurrentTime;
		/// <summary>The interval in terms of seconds. When CurrentTime reaches this, it ticks.</summary>
		public float MaxTime;
		#else
		/// <summary>The system timer that will time the callback.</summary>
		public System.Timers.Timer InternalTimer;
		#endif
		
		/// <summary>The interval in MS.</summary>
		public int Interval;
		/// <summary>The linked list of active timers.</summary>
		public UITimer Next;
		/// <summary>The linked list of active timers.</summary>
		public UITimer Previous;
		/// <summary>The document that this timer was created from.</summary>
		public Document Document;
		/// <summary>An alternative to the callback. A delegate called when the time is up.</summary>
		public event OnUITimer OnComplete;
		
		
		/// <summary>Creates a new timer defining how long to wait, the callback to run and if its an interval or not.</summary>
		/// <param name="oneOff">True if this timer is a single event. False will result in the callback being run until it's stopped.</param>
		/// <param name="interval">The time in milliseconds between callbacks.</param>
		/// <param name="callback">The callback (A delegate) to run when the time is up.</param>
		public UITimer(bool oneOff,int interval,OnUITimer callback){
			OnComplete+=callback;
			Setup(oneOff,interval);
		}
		
		private void Setup(bool oneOff,int interval){
			if(interval<=0){
				throw new Exception("Invalid timing interval or callback.");
			}
			
			Interval=interval;
			
			if(OnComplete==null){
				throw new Exception("A callback must be provided for timer methods.");
			}
			
			OneOff=oneOff;
			#if UNITY_WP8
			InternalTimer=new System.Threading.Timer(Elapsed,null,0,interval);
			#elif UNITY_METRO
			MaxTime=(float)Interval/1000f;
			#else
			InternalTimer=new System.Timers.Timer();
			InternalTimer.Elapsed+=Elapsed;
			InternalTimer.Interval=interval;
			InternalTimer.Start();
			#endif
			
			if(!OneOff){
				// Add to active set:
				if(FirstActive==null){
					FirstActive=LastActive=this;
				}else{
					Previous=LastActive;
					LastActive=LastActive.Next=this;
				}
			}
			
		}
		
		/// <summary>Stops this timer from running anymore.</summary>
		public void Stop(){
			
			#if UNITY_WP8
			if(InternalTimer==null){
				return;
			}
			
			InternalTimer.Dispose();
			InternalTimer=null;
			#elif UNITY_METRO
			#else
			if(InternalTimer==null){
				return;
			}
			
			InternalTimer.Enabled=false;
			InternalTimer=null;
			#endif
			
			if(!OneOff){
				
				// Remove from active queue:
				if(Next==null){
					LastActive=Previous;
				}else{
					Next.Previous=Previous;
				}
				
				if(Previous==null){
					FirstActive=Next;
				}else{
					Previous.Next=Next;
				}
				
			}
			
		}
		
		#if UNITY_WP8 || UNITY_METRO
		/// <summary>Windows Phone 8. The method called when the system timer has waited for the specified interval.</summary>
		private void Elapsed(object state){

		#else

		/// <summary>The method called when the system timer has waited for the specified interval.</summary>
		private void Elapsed(object sender,System.Timers.ElapsedEventArgs e){
		
		#endif
			try{
				if(OneOff){
					Stop();
				}
				if(OnComplete!=null){
					OnComplete();
				}
			}catch(Exception er){
				
				Dom.Log.Add("Error in an timed/interval function: "+er.ToString());
				
			}
		}
		
	}
	
}