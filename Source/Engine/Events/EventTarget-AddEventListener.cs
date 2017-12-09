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
using PowerUI;


namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		// All event-specific addEventListener overloads (except for SVG).
		// This avoids needing to manually create e.g. a EventListener<KeyboardEvent> object.
		
		public void addEventListener(string name,Action<Dom.Event> method){
			addEventListener(name,new EventListener<Dom.Event>(method));
		}
		
		public void addEventListener(string name,Action<AnimationEvent> method){
			addEventListener(name,new EventListener<AnimationEvent>(method));
		}

		public void addEventListener(string name,Action<AudioProcessingEvent> method){
			addEventListener(name,new EventListener<AudioProcessingEvent>(method));
		}

		public void addEventListener(string name,Action<BeforeInputEvent> method){
			addEventListener(name,new EventListener<BeforeInputEvent>(method));
		}

		public void addEventListener(string name,Action<BeforeUnloadEvent> method){
			addEventListener(name,new EventListener<BeforeUnloadEvent>(method));
		}

		public void addEventListener(string name,Action<BlobEvent> method){
			addEventListener(name,new EventListener<BlobEvent>(method));
		}

		public void addEventListener(string name,Action<ClipboardEvent> method){
			addEventListener(name,new EventListener<ClipboardEvent>(method));
		}

		public void addEventListener(string name,Action<CloseEvent> method){
			addEventListener(name,new EventListener<CloseEvent>(method));
		}

		public void addEventListener(string name,Action<CompositionEvent> method){
			addEventListener(name,new EventListener<CompositionEvent>(method));
		}

		public void addEventListener(string name,Action<CustomEvent> method){
			addEventListener(name,new EventListener<CustomEvent>(method));
		}

		public void addEventListener(string name,Action<CSSFontFaceLoadEvent> method){
			addEventListener(name,new EventListener<CSSFontFaceLoadEvent>(method));
		}

		public void addEventListener(string name,Action<DeviceLightEvent> method){
			addEventListener(name,new EventListener<DeviceLightEvent>(method));
		}

		public void addEventListener(string name,Action<DeviceMotionEvent> method){
			addEventListener(name,new EventListener<DeviceMotionEvent>(method));
		}

		public void addEventListener(string name,Action<DeviceOrientationEvent> method){
			addEventListener(name,new EventListener<DeviceOrientationEvent>(method));
		}

		public void addEventListener(string name,Action<DeviceProximityEvent> method){
			addEventListener(name,new EventListener<DeviceProximityEvent>(method));
		}
		
		public void addEventListener(string name,Action<DOMTransactionEvent> method){
			addEventListener(name,new EventListener<DOMTransactionEvent>(method));
		}

		public void addEventListener(string name,Action<DragEvent> method){
			addEventListener(name,new EventListener<DragEvent>(method));
		}

		public void addEventListener(string name,Action<EditingBeforeInputEvent> method){
			addEventListener(name,new EventListener<EditingBeforeInputEvent>(method));
		}

		public void addEventListener(string name,Action<ErrorEvent> method){
			addEventListener(name,new EventListener<ErrorEvent>(method));
		}

		public void addEventListener(string name,Action<FetchEvent> method){
			addEventListener(name,new EventListener<FetchEvent>(method));
		}

		public void addEventListener(string name,Action<FocusEvent> method){
			addEventListener(name,new EventListener<FocusEvent>(method));
		}

		public void addEventListener(string name,Action<GamepadEvent> method){
			addEventListener(name,new EventListener<GamepadEvent>(method));
		}

		public void addEventListener(string name,Action<HashChangeEvent> method){
			addEventListener(name,new EventListener<HashChangeEvent>(method));
		}

		public void addEventListener(string name,Action<IDBVersionChangeEvent> method){
			addEventListener(name,new EventListener<IDBVersionChangeEvent>(method));
		}

		public void addEventListener(string name,Action<InputEvent> method){
			addEventListener(name,new EventListener<InputEvent>(method));
		}

		public void addEventListener(string name,Action<KeyboardEvent> method){
			addEventListener(name,new EventListener<KeyboardEvent>(method));
		}

		public void addEventListener(string name,Action<MediaStreamEvent> method){
			addEventListener(name,new EventListener<MediaStreamEvent>(method));
		}

		public void addEventListener(string name,Action<MessageEvent> method){
			addEventListener(name,new EventListener<MessageEvent>(method));
		}

		public void addEventListener(string name,Action<MouseEvent> method){
			addEventListener(name,new EventListener<MouseEvent>(method));
		}

		public void addEventListener(string name,Action<MutationEvent> method){
			addEventListener(name,new EventListener<MutationEvent>(method));
		}

		public void addEventListener(string name,Action<OfflineAudioCompletionEvent> method){
			addEventListener(name,new EventListener<OfflineAudioCompletionEvent>(method));
		}

		public void addEventListener(string name,Action<PageTransitionEvent> method){
			addEventListener(name,new EventListener<PageTransitionEvent>(method));
		}

		public void addEventListener(string name,Action<PointerEvent> method){
			addEventListener(name,new EventListener<PointerEvent>(method));
		}

		public void addEventListener(string name,Action<PopStateEvent> method){
			addEventListener(name,new EventListener<PopStateEvent>(method));
		}

		public void addEventListener(string name,Action<ProgressEvent> method){
			addEventListener(name,new EventListener<ProgressEvent>(method));
		}

		public void addEventListener(string name,Action<RelatedEvent> method){
			addEventListener(name,new EventListener<RelatedEvent>(method));
		}

		public void addEventListener(string name,Action<RTCDataChannelEvent> method){
			addEventListener(name,new EventListener<RTCDataChannelEvent>(method));
		}

		public void addEventListener(string name,Action<RTCIdentityErrorEvent> method){
			addEventListener(name,new EventListener<RTCIdentityErrorEvent>(method));
		}

		public void addEventListener(string name,Action<RTCIdentityEvent> method){
			addEventListener(name,new EventListener<RTCIdentityEvent>(method));
		}

		public void addEventListener(string name,Action<RTCPeerConnectionIceEvent> method){
			addEventListener(name,new EventListener<RTCPeerConnectionIceEvent>(method));
		}

		public void addEventListener(string name,Action<SensorEvent> method){
			addEventListener(name,new EventListener<SensorEvent>(method));
		}

		public void addEventListener(string name,Action<StorageEvent> method){
			addEventListener(name,new EventListener<StorageEvent>(method));
		}

		public void addEventListener(string name,Action<TextEvent> method){
			addEventListener(name,new EventListener<TextEvent>(method));
		}

		public void addEventListener(string name,Action<TimeEvent> method){
			addEventListener(name,new EventListener<TimeEvent>(method));
		}

		public void addEventListener(string name,Action<TouchEvent> method){
			addEventListener(name,new EventListener<TouchEvent>(method));
		}

		public void addEventListener(string name,Action<TrackEvent> method){
			addEventListener(name,new EventListener<TrackEvent>(method));
		}

		public void addEventListener(string name,Action<TransitionEvent> method){
			addEventListener(name,new EventListener<TransitionEvent>(method));
		}

		public void addEventListener(string name,Action<UIEvent> method){
			addEventListener(name,new EventListener<UIEvent>(method));
		}

		public void addEventListener(string name,Action<UserProximityEvent> method){
			addEventListener(name,new EventListener<UserProximityEvent>(method));
		}

		public void addEventListener(string name,Action<WebGLContextEvent> method){
			addEventListener(name,new EventListener<WebGLContextEvent>(method));
		}

		public void addEventListener(string name,Action<WheelEvent> method){
			addEventListener(name,new EventListener<WheelEvent>(method));
		}

	}
	
}