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
using UnityEngine;
using Dom;


namespace PowerUI{
	
	public class AnimationEvent : Dom.Event{
		
		public string animationName;
		public float elapsedTime;
		public string psuedoElement;
		
		
		public AnimationEvent(string type):base(type){}
		public AnimationEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
			// string init.animationName 
			// float init.elapsedTime
			// string init.psuedoElement
			
		}
		
	}
	
	public class AudioProcessingEvent : Dom.Event{
		
		public AudioBuffer input;
		public AudioBuffer output;
		public float playbackTime;
		
		
		public AudioProcessingEvent(){}
		public AudioProcessingEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
			// AudioBuffer init.input 
			// AudioBuffer init.output
			// float init.playbackTime
			
		}
		
	}
	
	public class BeforeInputEvent : Dom.Event{
		
		public BeforeInputEvent(){}
		public BeforeInputEvent(string type,object init):base(type,init){}
		
	}
	
	public class BeforeUnloadEvent : Dom.Event{
		
		public string returnValue;
		
		public BeforeUnloadEvent():base("beforeunload"){
			SetTrusted();
			cancelable=false;
		}
		
	}
	
	public class BlobEvent : Dom.Event{
		
		public byte[] data;
		
		public BlobEvent(){}
		public BlobEvent(object init):base(null,init){}
		
		public BlobEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
			// byte[] init.data
			
		}
		
	}
	
	public class ClipboardEvent : Dom.Event{
		
		/// <summary>The raw clipboard data. Always non-null.</summary>
		private DataTransfer _ClipboardData;
		
		
		public DataTransfer clipboardData{
			get{
				return _ClipboardData;
			}
		}
		
		/// <summary>A shortcut to textual clipboard data. Null if it's not textual.</summary>
		public string text{
			get{
				return _ClipboardData.getData("text/plain");
			}
		}
		
		public ClipboardEvent(){}
		
		public ClipboardEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			// string init.data
			// string init.dataType
			_ClipboardData=new DataTransfer();
			
		}
		
	}
	
	public class CloseEvent : Dom.Event{
		
		public CloseEvent(){}
		public CloseEvent(string type,object init):base(type,init){}
		
	}
	
	public class CompositionEvent : UIEvent{
		
		public CompositionEvent(){}
		public CompositionEvent(string type,object init):base(type,init){}
		
	}
	
	public class CustomEvent : Dom.Event{
		
		public object detail;
		
		public CustomEvent(){}
		public CustomEvent(string type,object init):base(type,init){}
		
	}
	
	public class CSSFontFaceLoadEvent : Dom.Event{
		
		public CSSFontFaceLoadEvent(){}
		public CSSFontFaceLoadEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceLightEvent : Dom.Event{
		
		public DeviceLightEvent(){}
		public DeviceLightEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceMotionEvent : Dom.Event{
		
		public DeviceMotionEvent(){}
		public DeviceMotionEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceOrientationEvent : Dom.Event{
		
		public bool absolute;
		public float alpha;
		public float beta;
		public float gamma;
		
		public DeviceOrientationEvent(string type):base(type){}
		public DeviceOrientationEvent(string type,object init):base(type,init){}
		
	}
	
	public class DeviceProximityEvent : Dom.Event{
		
		public DeviceProximityEvent(){}
		public DeviceProximityEvent(string type,object init):base(type,init){}
		
	}
	
	public class DOMTransactionEvent : Dom.Event{
		
		public DOMTransactionEvent(){}
		public DOMTransactionEvent(string type,object init):base(type,init){}
		
	}
	
	public class DragEvent : UIEvent{
		
		/// <summary>The delta X value.</summary>
		public float deltaX;
		/// <summary>The delta Y value.</summary>
		public float deltaY;
		
		
		public DragEvent(string type):base(type){}
		public DragEvent(string type,object init):base(type,init){}
		
	}
	
	public class EditingBeforeInputEvent : Dom.Event{
		
		public EditingBeforeInputEvent(){}
		public EditingBeforeInputEvent(string type,object init):base(type,init){}
		
	}
	
	public class ErrorEvent : Dom.Event{
		
		public ErrorEvent(){}
		public ErrorEvent(string type,object init):base(type,init){}
		
	}
	
	public class FetchEvent : Dom.Event{
		
		public FetchEvent(){}
		public FetchEvent(string type,object init):base(type,init){}
		
	}
	
	public class FocusEvent : UIEvent{
		
		/// <summary>The element being focused. May be null.</summary>
		public Element focusing;
		
		public FocusEvent(string type):base(type){}
		public FocusEvent(string type,object init):base(type,init){}
		
	}
	
	public class GamepadEvent : Dom.Event{
		
		public GamepadEvent(){}
		public GamepadEvent(string type,object init):base(type,init){}
		
	}
	
	public class IDBVersionChangeEvent : Dom.Event{
		
		public IDBVersionChangeEvent():base("versionchange"){}
		public IDBVersionChangeEvent(string type,object init):base(type,init){}
		
	}
	
	public class InputEvent : UIEvent{
		
		public InputEvent():base("input"){}
		public InputEvent(string type,object init):base(type,init){}
		
	}
	
	public class MediaStreamEvent : Dom.Event{
		
		public MediaStreamEvent():base("mediastream"){}
		public MediaStreamEvent(string type,object init):base(type,init){}
		
	}
	
	public class MessageEvent : Dom.Event{
		
		public object data;
		public string origin;
		
		
		public MessageEvent():base("message"){}
		
		public MessageEvent(object message,string origin):base("message"){
			data=message;
			this.origin=origin;
		}
		
		public MessageEvent(string type,object init):base(type,init){}
		
	}
	
	public class MouseEvent : UIEvent{
		
		/// <summary>True if rayHit was successful and hit something.</summary>
		public bool raySuccess{
			get{
				return trigger.LatestHitSuccess;
			}
		}
		
		/// <summary>The raycast hit that was sent into the scene whilst looking for WorldUI's.
		/// It's default(RaycastHit) if it didn't hit anything (see raySuccess).</summary>
		public RaycastHit rayHit{
			get{
				return trigger.LatestHit;
			}
		}
		
		public MouseEvent(){}
		
		public MouseEvent(float x,float y,int button,bool down):base(x,y,down){
			keyCode=button;
		}
		
		public MouseEvent(string type,object init):base(type,init){}
		
		public override int which{
			get{
				return button;
			}
		}
		
		/// <summary>Screen x coordinate.</summary>
		public float screenX{
			get{
				return clientX;
			}
		}
		
		/// <summary>Screen y coordinate.</summary>
		public float screenY{
			get{
				return clientY;
			}
		}
		
		/// <summary>X coordinate relative to the page (accounts for scrolling too).</summary>
		public float pageX{
			get{
				
				// Get the document:
				Document doc=document;
				
				if(doc==null){
					return clientX;
				}
				
				return (doc.documentElement as HtmlElement).scrollLeft + clientX;
			}
		}
		
		/// <summary>Y coordinate relative to the page (accounts for scrolling too).</summary>
		public float pageY{
			get{
				
				// Get the document:
				Document doc=document;
				
				if(doc==null){
					return clientY;
				}
				
				return (doc.documentElement as HtmlElement).scrollTop + clientY;
			}
		}
		
	}
	
	public class MutationEvent : UIEvent{
		
		public MutationEvent(){}
		
		public MutationEvent(string type,object init):base(type,init){}
		
	}
	
	public class OfflineAudioCompletionEvent : Dom.Event{
		
		public OfflineAudioCompletionEvent(){}
		
		public OfflineAudioCompletionEvent(string type,object init):base(type,init){}
		
	}
	
	public class PageTransitionEvent : Dom.Event{
		
		public PageTransitionEvent(){}
		
		public PageTransitionEvent(string type,object init):base(type,init){}
		
	}
	
	public class PointerEvent : MouseEvent{
		
		/// <summary>A unique pointer ID.</summary>
		public long pointerId{
			get{
				return trigger.pointerId;
			}
		}
		
		/// <summary>The width of the pointer area.</summary>
		public double width{
			get{
				return trigger.width;
			}
		}
		
		/// <summary>The height of the pointer area.</summary>
		public double height{
			get{
				return trigger.height;
			}
		}
		
		/// <summary>The triggers pressure.</summary>
		public float pressure{
			get{
				return trigger.Pressure;
			}
		}
		
		/// <summary>Tangential pressure.</summary>
		public float tangentialPressure{
			get{
				return trigger.tangentialPressure;
			}
		}
		
		/// <summary>X tilt of the trigger.</summary>
		public float tiltX{
			get{
				return trigger.tiltX;
			}
		}
		
		/// <summary>Y tilt of the trigger.</summary>
		public float tiltY{
			get{
				return trigger.tiltY;
			}
		}
		
		/// <summary>Twist of the trigger.</summary>
		public float twist{
			get{
				return trigger.twist;
			}
		}
		
		/// <summary>The type of this pointer.</summary>
		public string pointerType{
			get{
				return trigger.pointerType;
			}
		}
		
		/// <summary>True if this is the primary pointer.</summary>
		public bool isPrimary{
			get{
				return trigger.isPrimary;
			}
		}
		
		public PointerEvent(){}
		
		public PointerEvent(string type,object init):base(type,init){}
		
	}
	
	public class PopStateEvent : Dom.Event{
		
		public object state;
		
		
		public PopStateEvent():base("popstate"){}
		
		public PopStateEvent(string type,object init):base(type,init){}
		
	}
	
	public class ProgressEvent : Dom.Event{
		
		public ProgressEvent(){}
		
		public ProgressEvent(string type,object init):base(type,init){}
		
	}
	
	public class RelatedEvent : Dom.Event{
		
		public RelatedEvent(){}
		
		public RelatedEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCDataChannelEvent : Dom.Event{
		
		public RTCDataChannelEvent(){}
		
		public RTCDataChannelEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCIdentityErrorEvent : Dom.Event{
		
		public RTCIdentityErrorEvent(){}
		
		public RTCIdentityErrorEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCIdentityEvent : Dom.Event{
		
		public RTCIdentityEvent(){}
		
		public RTCIdentityEvent(string type,object init):base(type,init){}
		
	}
	
	public class RTCPeerConnectionIceEvent : Dom.Event{
		
		public RTCPeerConnectionIceEvent(){}
		
		public RTCPeerConnectionIceEvent(string type,object init):base(type,init){}
		
	}
	
	public class SensorEvent : Dom.Event{
		
		public SensorEvent(){}
		
		public SensorEvent(string type,object init):base(type,init){}
		
	}
	
	public class StorageEvent : Dom.Event{
		
		public StorageEvent(){}
		
		public StorageEvent(string type,object init):base(type,init){}
		
	}
	
	public class TextEvent : Dom.Event{
		
		public TextEvent(){}
		
		public TextEvent(string type,object init):base(type,init){}
		
	}
	
	public class TimeEvent : Dom.Event{
		
		public TimeEvent(){}
		
		public TimeEvent(string type,object init):base(type,init){}
		
	}
	
	public class TouchEvent : UIEvent{
		
		/// <summary>A unique ID.</summary>
		public long identifier{
			get{
				return trigger.pointerId;
			}
		}
		
		/// <summary>Cached touches that pressed down on the element that is the target of this event.</summary>
		private TouchList targetTouches_;
		/// <summary>Cached touches regardless of which element they're over.</summary>
		private TouchList touches_;
		/// <summary>Cached changed touches.</summary>
		private TouchList changedTouches_;
		
		/// <summary>Touches which changed that have changed/ became active since the last event.</summary>
		public TouchList changedTouches{
			get{
				
				if(changedTouches_==null){
					
					// Create it:
					changedTouches_=new TouchList();
					
					// Always only one - specifically the originating pointer:
					changedTouches_.push(trigger as TouchPointer);
					
				}
				
				return changedTouches_;
			}
		}
		
		/// <summary>All touches that pressed down on the element that is the target of this event.</summary>
		public TouchList targetTouches{
			get{
				
				if(targetTouches_==null){
					
					// Create the list:
					targetTouches_=new TouchList();
					
					// For each available input, add it to the list if it's a finger touch:
					for(int i=0;i<InputPointer.PointerCount;i++){
						
						InputPointer pointer=InputPointer.AllRaw[i];
						
						if(pointer.ActivePressedTarget!=target){
							continue;
						}
						
						if(pointer is TouchPointer){
							
							// Add:
							targetTouches_.push(pointer as TouchPointer);
							
						}
						
					}
					
				}
				
				return targetTouches_;
				
			}
		}
		
		/// <summary>All touches regardless of which element they're over.</summary>
		public TouchList touches{
			get{
				
				if(touches_==null){
					
					// Create the list:
					touches_=new TouchList();
					
					// For each available input, add it to the list if it's a finger touch:
					for(int i=0;i<InputPointer.PointerCount;i++){
						
						InputPointer pointer=InputPointer.AllRaw[i];
						
						if(pointer is TouchPointer){
							
							// Add:
							touches_.push(pointer as TouchPointer);
							
						}
						
					}
					
				}
				
				return touches_;
				
			}
		}
		
		public TouchEvent(string type):base(type){}
		
		public TouchEvent(string type,object init):base(type,init){}
		
	}
	
	public class TrackEvent : Dom.Event{
		
		public TrackEvent(){}
		
		public TrackEvent(string type,object init):base(type,init){}
		
	}
	
	public class TransitionEvent : Dom.Event{
		
		public TransitionEvent(){}
		
		public TransitionEvent(string type,object init):base(type,init){}
		
	}
	
	public class UserProximityEvent : Dom.Event{
		
		public UserProximityEvent(){}
		
		public UserProximityEvent(string type,object init):base(type,init){}
		
	}
	
	public class WebGLContextEvent : Dom.Event{
		
		public WebGLContextEvent(){}
		
		public WebGLContextEvent(string type,object init):base(type,init){}
		
	}
	
	public class WheelEvent : UIEvent{
		
		public const ushort DOM_DELTA_PIXEL=0;
		public const ushort DOM_DELTA_LINE=1;
		public const ushort DOM_DELTA_PAGE=2;
		
		public float deltaX;
		public float deltaY;
		public float deltaZ;
		/// <summary>Always pixel here.</summary>
		public int deltaMode=DOM_DELTA_PIXEL;
		
		
		public WheelEvent():base("wheel"){}
		
		public WheelEvent(string type,object init):base(type,init){}
		
	}
	
}