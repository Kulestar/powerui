using Dom;
using System;
using Css;
using UnityEngine;


namespace PowerUI{
	
	/// <summary>
	/// An input pointer.
	/// These trigger touch events and mouse events.
	/// There's one set of pointers for the overall project (InputPointer.All).
	/// If there's a pointer on the screen at all times (i.e. desktops) then the set is always at least 1 in size.
	/// </summary>
	public class InputPointer{
		
		// Various states of InputPointer.DragStatus
		
		/// <summary>Not tested if a pointer is in the dragging state yet.</summary>
		public const int DRAG_UNKNOWN=0;
		/// <summary>Tried to drag something that isn't actually draggable.</summary>
		public const int DRAG_NOT_AVAILABLE=1;
		/// <summary>Dragging something.</summary>
		public const int DRAGGING=2;
		/// <summary>Selecting something.</summary>
		public const int SELECTING=4;
		
		/// <summary>A globally unique ID.</summary>
		private static long GlobalID=0;
		
		/// <summary>To avoid resizing this array repeatedly, we track how many are actually in use.</summary>
		internal static int PointerCount;
		/// <summary>The raw set of all available input pointers. PointerCount is how many indices are actually in use.</summary>
		internal static InputPointer[] AllRaw=new InputPointer[1];
		
		/// <summary>Removes the pointers marked for removal.</summary>
		internal static void Tidy(){
			
			int tidyIndex=0;
			
			for(int i=0;i<PointerCount;i++){
				
				// Get the pointer:
				InputPointer pointer=AllRaw[i];
				
				if(pointer.Removed){
					continue;
				}
				
				// Transfer:
				if(tidyIndex!=i){
					AllRaw[tidyIndex]=pointer;
				}
				
				// Move index up:
				tidyIndex++;
				
			}
			
			// Update count:
			PointerCount=tidyIndex;
			
		}
		
		/// <summary>Doubles the size of the pointer stack, AllRaw.</summary>
		private static void ResizePointerStack(){
			
			InputPointer[] newStack=new InputPointer[AllRaw.Length*2];
			Array.Copy(AllRaw,0,newStack,0,AllRaw.Length);
			AllRaw=newStack;
			
		}
		
		/// <summary>The set of all input pointers that are currently actively doing something.
		/// Note that this array gets built when you request this property;
		/// If you want extra speed, access AllRaw and use PointerCount instead.</summary>
		public static InputPointer[] All{
			get{
				
				// Create:
				InputPointer[] arr=new InputPointer[PointerCount];
				
				// Copy:
				Array.Copy(AllRaw,0,arr,0,PointerCount);
				
				return arr;
				
			}
		}
		
		/// <summary>Used by pointers that don't stick around (fingers and styluses).
		/// They're marked as still alive until they don't show up in the touch set anymore.
		/// It's always unfortunate that we have to do things like this;
		/// the underlying API's are event based but Unity would rather we poll instead.</summary>
		public bool StillAlive=true;
		/// <summary>A unique ID for this pointer.</summary>
		public int ID=-1;
		/// <summary>True if this pointer should fire touch events.</summary>
		public bool FireTouchEvents;
		/// <summary>The current X coordinate on the screen.</summary>
		public float ScreenX;
		/// <summary>The current Y coordinate on the screen.</summary>
		public float ScreenY;
		/// <summary>The current X coordinate on the document that ActiveOver is in.</summary>
		public float DocumentX;
		/// <summary>The current Y coordinate on the document that ActiveOver is in.</summary>
		public float DocumentY;
		/// <summary>The current drag status. See e.g. DRAG_UNKNOWN.</summary>
		public int DragStatus=DRAG_UNKNOWN;
		/// <summary>The X coordinate when the pointer went down (clicked).</summary>
		public float DownDocumentX;
		/// <summary>The Y coordinate when the pointer went down (clicked).</summary>
		public float DownDocumentY;
		/// <summary>This occurs with touch pointers. They get marked as removed when the finger is no longer on the screen.</summary>
		public bool Removed;
		/// <summary>The current minimum drag distance.</summary>
		public float MinDragDistance;
		/// <summary>Used by e.g. dragging. The target that is "pressed" can be different from the one being actually dragged.</summary>
		public EventTarget ActiveUpdatingTarget;
		/// <summary>Same as ActiveOver, only this can also include GameObjects. See also ActiveOverGameObject.
		/// It'll be an EventTarget3D if that's the case. The object that this pointer is currently over.</summary>
		public EventTarget ActiveOverTarget;
		/// <summary>The element that this pointer last pressed/ clicked on.</summary>
		public EventTarget ActivePressedTarget;
		
		/// <summary>The element that this pointer is currently over.</summary>
		public Element ActiveOver{
			get{
				return ActiveOverTarget as Element;
			}
		}
		
		/// <summary>Used by e.g. dragging. The element that is "pressed" can be different from the one being actually dragged.</summary>
		public Element ActiveUpdating{
			get{
				return ActiveUpdatingTarget as Element;
			}
		}
		
		/// <summary>The element that this pointer last pressed/ clicked on.</summary>
		public Element ActivePressed{
			get{
				return ActivePressedTarget as Element;
			}
		}
		
		/// <summary>The latest pressure.</summary>
		public float Pressure;
		/// <summary>The latest button that went down.</summary>
		public int ButtonID;
		/// <summary>True if LatestRayHit is valid (because it hit something).</summary>
		public bool LatestHitSuccess;
		/// <summary>If WorldUI's receive input, a ray must be fired from CameraFor3DInput to attempt input.
		/// This is the lastest ray result. UI.MouseOver updates this immediately; it's updated at the UI rate otherwise.</summary>
		public UnityEngine.RaycastHit LatestHit;
		/// <summary>A globally unique ID.</summary>
		public long pointerId=0;
		/// <summary>Is this the primary pointer? They all are by default in PowerUI
		/// (everything receives the 'legacy' mouse events).</summary>
		public bool isPrimary=true;
		
		
		/// <summary>The gameObject this pointer is currently over (if any).</summary>
		public UnityEngine.GameObject ActiveOverGameObject{
			get{
				EventTarget3D et3D = ActiveOverTarget as EventTarget3D;
				
				if(et3D==null){
					return null;
				}
				
				return et3D.gameObject;
			}
		}
		
		/// <summary>The type of input pointer.</summary>
		public virtual string pointerType{
			get{
				return "";
			}
		}
		
		/// <summary>The 'barrel' pressure when available.</summary>
		public virtual float tangentialPressure{
			get{
				return 0f;
			}
		}
		
		/// <summary>The width of the active pointer area in CSS pixels.</summary>
		public virtual double width{
			get{
				return 1;
			}
		}
		
		/// <summary>The height of the active pointer area in CSS pixels.</summary>
		public virtual double height{
			get{
				return 1;
			}
		}
		
		/// <summary>The x tilt of the pointer.</summary>
		public virtual float tiltX{
			get{
				return 0f;
			}
		}
		
		/// <summary>The y tilt of the pointer.</summary>
		public virtual float tiltY{
			get{
				return 0f;
			}
		}
		
		/// <summary>A convenience method which looks out for mousedown/mouseup events and runs them as Click() and Release().
		/// You'd use this from an OnGUI method.</summary>
		/// <returns>True if it did something with the event.</returns>
		public bool HandleEvent(){
			return HandleEvent(UnityEngine.Event.current);
		}
		
		/// <summary>A convenience method which looks out for mousedown/mouseup events and runs them as Click() and Release().
		/// You'd use this from an OnGUI method.</summary>
		/// <param name='current'>An optional event - usually just Event.current.</param>
		/// <returns>True if it did something with the event.</returns>
		public virtual bool HandleEvent(UnityEngine.Event current){
			
			UnityEngine.EventType type=current.type;
			
			// Look out for mouse events:
			if(type==UnityEngine.EventType.MouseUp){
				
				// Release it:
				SetPressure(current.button,0f);
				
			}else if(type==UnityEngine.EventType.MouseDown){
				
				// Press it down:
				SetPressure(current.button,1f);
				
			}else{
				return false;
			}
			
			return true;
		}
		
		/// <summary>The rotation angle of an input when available.</summary>
		public virtual float twist{
			get{
				return 0f;
			}
		}
		
		/// <summary>True if this input is currently down.</summary>
		public bool IsDown{
			get{
				return Pressure!=0f;
			}
		}
		
		/// <summary>The X coordinate when the pointer went down (clicked). Same as DownDocumentX</summary>
		public float startX{
			get{
				return DownDocumentX;
			}
		}
		
		/// <summary>The Y coordinate when the pointer went down (clicked). Same as DownDocumentY</summary>
		public float startY{
			get{
				return DownDocumentY;
			}
		}
		
		/// <summary>True if this pointers position has changed.
		/// On the next input update it will recompute the element it's over.</summary>
		protected bool HasMoved;
		/// <summary>The next position to move to.</summary>
		private Vector2 NextPosition_;
		
		/// <summary>Sets the ScreenX/ScreenY coordinates.
		/// If it has changed then on the next input update 
		/// it will recompute the element it's over. Top left corner is 0,0.</summary>
		public Vector2 Position{
			get{
				return new Vector2(ScreenX,ScreenY);
			}
			set{
				NextPosition_=value;
				
				if(value.x==ScreenX && value.y==ScreenY){
					return;
				}
				
				HasMoved=true;
			}
		}
		
		
		public InputPointer(){
			pointerId=GlobalID++;
		}
		
		/// <summary>Makes it so this pointer will get removed on the next input update.</summary>
		public void Remove(){
			Removed=true;
		}
		
		/// <summary>Adds this pointer to the available set so it'll get updated.
		public void Add(){
			
			if(PointerCount==AllRaw.Length){
				
				// Resize:
				ResizePointerStack();
				
			}
			
			// Add it:
			AllRaw[PointerCount++]=this;
			
		}
		
		/// <summary>Called when the input system wants to fire a raycast to figure out if this pointer is
		/// 'over' any WorldUI's.</summary>
		public virtual bool Raycast(out RaycastHit hit,Camera cam,Vector2 screenPoint){
			
			// Cast into the scene now:
			Ray ray=cam.ScreenPointToRay(screenPoint);
			return Physics.Raycast(ray,out hit);
			
		}
		
		/// <summary>Best used from the Relocate method. Sets ScreenX/ScreenY to the given position
		/// and returns true if it changed.</summary>
		public bool TryChangePosition(Vector2 position,out Vector2 delta){
			return TryChangePosition(position,false,out delta);
		}
		
		/// <summary>Best used from the Relocate method. Sets ScreenX/ScreenY to the given position
		/// and returns true if it changed. Optionally invert the y coordinate.</summary>
		public bool TryChangePosition(Vector2 position, bool invertY,out Vector2 delta){
			if(invertY){
				// Invert the y value:
				position.y=ScreenInfo.ScreenY-1f-position.y;
			}
			
			// Moved?
			if(position.x==ScreenX && position.y==ScreenY){
				// Nope!
				delta=Vector2.zero;
				return false;
			}
			
			// Delta:
			delta=new Vector2(
				position.x - ScreenX,
				position.y - ScreenY
			);
			
			// Apply the new position:
			ScreenX=position.x;
			ScreenY=position.y;
			
			return true;
		}
		
		/// <summary>Finds the minimum drag distance. Always greater than zero.</summary>
		public float GetMinDragDistance(){
			
			// If we've got a 'draggable' element, that is preferred:
			Element draggable=( (ActiveUpdatingTarget==null)?ActivePressedTarget : ActiveUpdatingTarget ) as Element;
			
			if(draggable==null){
				return Input.MinimumDragStartDistance;
			}
			
			float distance=draggable.DragStartDistance;
			
			if(distance==0f){
				// Default, (after checking for a mindrag attribute):
				string minDrag=draggable.getAttribute("mindrag");
				
				if(minDrag==null){
					
					// Unspecified.
					// Default depends if we're actually a 'draggable' or not:
					if(ActiveUpdatingTarget==null){
						distance=Input.MinimumDragStartDistance;
					}else{
						// 1:
						distance=1f;
					}
					
				}else if(!float.TryParse(minDrag,out distance) || distance<=0f){
					
					// Default:
					distance=Input.MinimumDragStartDistance;
					
				}
				
			}
			
			return distance;
			
		}
		
		/// <summary>Checks if the delta between DocumentX/Y and DownDocumentX/Y is bigger than our min drag size.</summary>
		public bool MovedBeyondDragDistance{
			get{
				
				// If we've got a 'draggable' element, that is preferred:
				EventTarget draggable=(ActiveUpdatingTarget==null) ? ActivePressedTarget : ActiveUpdatingTarget;
				
				if(draggable==null){
					return false;
				}
				
				float d=DownDocumentX-DocumentX;
			
				if(d<=-MinDragDistance || d>=MinDragDistance){
					return true;
				}
				
				d=DownDocumentY-DocumentY;
				
				return (d<=-MinDragDistance || d>=MinDragDistance);	
			}
		}
		
		/// <summary>Force the pointer to invalidate itself (which makes it recompute which element is under it).</summary>
		public void ForceInvalidate(){
			HasMoved=true;
		}
		
		/// <summary>Update ScreenX/ScreenY.</summary>
		/// <returns>True if it moved.</returns>
		public virtual bool Relocate(out UnityEngine.Vector2 delta){
			
			if(HasMoved){
				// Reset:
				HasMoved=false;
				
				delta=new Vector2(
					NextPosition_.x - ScreenX,
					NextPosition_.y - ScreenY
				);
				
				// Update position:
				ScreenX=NextPosition_.x;
				ScreenY=NextPosition_.y;
				
				return true;
			}
			
			delta=Vector2.zero;
			return false;
		}
		
		/// <summary>Sets ButtonID mapping from the Unity ID to the W3C ones.</summary>
		public void SetButton(int unityButtonID){
			
			switch(unityButtonID){
				case 0:
					ButtonID=0;
				break;
				case 1:
					// Right:
					ButtonID=2;
				break;
				case 2:
					// Middle:
					ButtonID=1;
				break;
				default:
					ButtonID=unityButtonID;
				break;
			}
			
		}
		
		/// <summary>This is the same as Down(button) then Up(button).</summary>
		public void Click(int unityButtonID){
			Down(unityButtonID);
			Up(unityButtonID);
		}
		
		/// <summary>Mouseup (right button). The same as Up(1).</summary>
		public void RightUp(){
			SetPressure(1,0f);
		}
		
		/// <summary>Mousedown (right button). The same as Down(1).</summary>
		public void RightDown(){
			SetPressure(1,1f);
		}
		
		/// <summary>Mouseup (left button). The same as Up(0).</summary>
		public void LeftUp(){
			SetPressure(0,0f);
		}
		
		/// <summary>Mousedown (left button). The same as Down(0).</summary>
		public void LeftDown(){
			SetPressure(0,1f);
		}
		
		/// <summary>Presses this pointer down. Essentially sets the pressure to 1.</summary>
		public void Down(int unityButtonID){
			SetPressure(unityButtonID,1f);
		}
		
		/// <summary>Releases this pointer (same as Release). Essentially sets the pressure to 0.</summary>
		public void Up(int unityButtonID){
			SetPressure(unityButtonID,0f);
		}
		
		/// <summary>Releases this pointer (same as Up). Essentially sets the pressure to 0.</summary>
		public void Release(int unityButtonID){
			SetPressure(unityButtonID,0f);
		}
		
		/// <summary>Sets the current button and the pressure.</summary>
		public void SetPressure(int unityButtonID,float pressure){
			SetButton(unityButtonID);
			SetPressure(pressure);
		}
		
		/// <summary>Attempts to resolve LatestHit to an event target. Prefers to use Input.TargetResolver but falls back searching for 
		/// a script on the gameobject which implements it. Will search up the hierarchy too.</summary>
		public IEventTarget ResolveTarget(){
			
			return Input.ResolveTarget(LatestHit.transform.gameObject);
			
		}
		
		/// <summary>Sets the pressure level.</summary>
		public void SetPressure(float v){
			
			// Was it up before?
			bool wasUp=(Pressure==0f);
			
			// Set pressure:
			Pressure=v;
			
			// If it's non-zero then we'll need to grab the clicked object:
			if(v==0f){
				
				if(wasUp){
					// No change.
				}else{
					
					// It's up now. Clear:
					EventTarget oldActivePressed=ActivePressedTarget;
					
					// Clear:
					ActivePressedTarget=null;
					
					if(oldActivePressed!=null){
						// Refresh CSS (active; applies to parents too):
						EventTarget current = oldActivePressed;
						
						while(current!=null){
							
							// Get it as a renderable node:
							IRenderableNode irn = (current as IRenderableNode);
							
							if(irn!=null){
								irn.ComputedStyle.RefreshLocal();
							}
							
							current=current.eventTargetParentNode;
						}
						
					}
					
					// Trigger up event.
					MouseEvent e=new MouseEvent(DocumentX,DocumentY,ButtonID,false);
					e.trigger=this;
					e.SetModifiers();
					e.EventType="mouseup";
					
					if(oldActivePressed==null){
						Input.Unhandled.dispatchEvent(e);
					}else{
						oldActivePressed.dispatchEvent(e);
					}
					
					// Click if needed:
					if(oldActivePressed==ActiveOverTarget && DragStatus==0){
						
						// Click!
						e.Reset();
						e.trigger=this;
						e.SetModifiers();
						e.EventType="click";
						
						if(oldActivePressed==null){
							Input.Unhandled.dispatchEvent(e);
						}else if(oldActivePressed.dispatchEvent(e)){
							
							// Clear the selection if necessary:
							HtmlElement h=(oldActivePressed as HtmlElement);
							
							if(h!=null){
								// Clear selection if there is one:
								(h.document as HtmlDocument).clearSelection();
							}
							
						}
						
					}
					
					if(FireTouchEvents){
						
						// Trigger a touchend event too:
						TouchEvent te=new TouchEvent("touchend");
						te.trigger=this;
						te.SetModifiers();
						te.SetTrusted();
						te.clientX=DocumentX;
						te.clientY=DocumentY;
						
						if(oldActivePressed==null){
							Input.Unhandled.dispatchEvent(te);
						}else{
							oldActivePressed.dispatchEvent(te);
						}
						
					}
					
					if(DragStatus==DRAGGING){
						
						// Trigger dragend:
						DragEvent de=new DragEvent("dragend");
						de.trigger=this;
						de.SetModifiers();
						de.SetTrusted();
						de.clientX=ScreenX;
						de.clientY=ScreenY;
						
						if(oldActivePressed.dispatchEvent(de)){
							
							// Trigger a drop event next:
							de.Reset();
							de.EventType="drop";
							if(ActiveOverTarget!=null && ActiveOverTarget.dispatchEvent(de)){
								
								// Proceed to try and drop it into the dropzone (ActiveOver).
								
							}
							
						}
						
					}else if(DragStatus==SELECTING){
						
						// Finished selection - trigger selectionend:
						Dom.Event sc=new Dom.Event("selectionend");
						sc.SetTrusted();
						
						// Dispatch on the element:
						oldActivePressed.dispatchEvent(sc);
						
					}
					
					// Always clear drag status:
					DragStatus=0;
					MinDragDistance=0f;
					
				}
				
			}else if(wasUp){
				
				// It was up and it's now just gone down.
				
				// Cache position:
				DownDocumentX=DocumentX;
				DownDocumentY=DocumentY;
				
				// Cache down:
				ActivePressedTarget=ActiveOverTarget;
				
				// Trigger down event.
				
				if(ActivePressedTarget!=null){
					// Refresh CSS (active; applies to parents too):
					
					EventTarget current = ActivePressedTarget;
					
					while(current!=null){
						
						// Get it as a renderable node:
						IRenderableNode irn = (current as IRenderableNode);
						
						if(irn!=null){
							irn.ComputedStyle.RefreshLocal();
						}
						
						current=current.eventTargetParentNode;
					}
					
				}
				
				// Trigger down event.
				MouseEvent e=new MouseEvent(DocumentX,DocumentY,ButtonID,true);
				e.trigger=this;
				e.EventType="mousedown";
				e.SetModifiers();
				
				if(ActivePressedTarget==null){
					Input.Unhandled.dispatchEvent(e);
				}else{
					ActivePressedTarget.dispatchEvent(e);
				}
				
				if(FireTouchEvents){
					
					// Trigger a touchend event too:
					TouchEvent te=new TouchEvent("touchstart");
					te.trigger=this;
					te.clientX=DocumentX;
					te.clientY=DocumentY;
					te.SetTrusted();
					te.SetModifiers();
					
					if(ActivePressedTarget==null){
						Input.Unhandled.dispatchEvent(te);
					}else{
						ActivePressedTarget.dispatchEvent(te);
					}
					
				}
				
			}
			
		}
		
	}
	
}