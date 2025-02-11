using UnityEngine;

public class InputManager : MonoBehaviour
{
	public delegate bool FingerDownEventHandler(int fingerIndex, Vector2 fingerPos);

	public delegate bool FingerUpEventHandler(int fingerIndex, Vector2 fingerPos, float timeHeldDown);

	public static InputManager mInstance;

	private bool mFingerGesturesEnabled;

	private bool mBlockSingleTap;

	private OrderedEventList<FingerDownEventHandler> mFingerDownEvents;

	private OrderedEventList<FingerUpEventHandler> mFingerUpEvents;

	private OrderedEventList<FingerGestures.FingerTapEventHandler> mFingerTapEvents;

	private OrderedEventList<FingerGestures.FingerTapEventHandler> mFingerDoubleTapEvents;

	private OrderedEventList<FingerGestures.FingerSwipeEventHandler> mFingerSwipeEvents;

	private OrderedEventList<FingerGestures.FingerDragBeginEventHandler> mFingerDragBeginEvents;

	private OrderedEventList<FingerGestures.FingerDragMoveEventHandler> mFingerDragMoveEvents;

	private OrderedEventList<FingerGestures.FingerDragEndEventHandler> mFingerDragEndEvents;

	private OrderedEventList<FingerGestures.DragBeginEventHandler> mTwoFingerDragBeginEvents;

	private OrderedEventList<FingerGestures.DragMoveEventHandler> mTwoFingerDragMoveEvents;

	private OrderedEventList<FingerGestures.DragEndEventHandler> mTwoFingerDragEndEvents;

	private OrderedEventList<FingerGestures.PinchEventHandler> mPinchBeginEvents;

	private OrderedEventList<FingerGestures.PinchMoveEventHandler> mPinchMoveEvents;

	private OrderedEventList<FingerGestures.RotationBeginEventHandler> mRotateBeginEvents;

	private OrderedEventList<FingerGestures.RotationMoveEventHandler> mRotateMoveEvents;

	private OrderedEventList<FingerGestures.RotationEndEventHandler> mRotateEndEvents;

	public static InputManager Instance
	{
		get
		{
			return mInstance;
		}
	}

	public int NumFingersActive
	{
		get
		{
			return FingerGestures.Touches.Count;
		}
	}

	private void Awake()
	{
		TBFAssert.DoAssert(mInstance == null, "can only have one input manager");
		mInstance = this;
		mFingerGesturesEnabled = false;
		mFingerDownEvents = new OrderedEventList<FingerDownEventHandler>();
		mFingerUpEvents = new OrderedEventList<FingerUpEventHandler>();
		mFingerTapEvents = new OrderedEventList<FingerGestures.FingerTapEventHandler>();
		mFingerDoubleTapEvents = new OrderedEventList<FingerGestures.FingerTapEventHandler>();
		mFingerSwipeEvents = new OrderedEventList<FingerGestures.FingerSwipeEventHandler>();
		mFingerDragBeginEvents = new OrderedEventList<FingerGestures.FingerDragBeginEventHandler>();
		mFingerDragMoveEvents = new OrderedEventList<FingerGestures.FingerDragMoveEventHandler>();
		mFingerDragEndEvents = new OrderedEventList<FingerGestures.FingerDragEndEventHandler>();
		mTwoFingerDragBeginEvents = new OrderedEventList<FingerGestures.DragBeginEventHandler>();
		mTwoFingerDragMoveEvents = new OrderedEventList<FingerGestures.DragMoveEventHandler>();
		mTwoFingerDragEndEvents = new OrderedEventList<FingerGestures.DragEndEventHandler>();
		mPinchBeginEvents = new OrderedEventList<FingerGestures.PinchEventHandler>();
		mPinchMoveEvents = new OrderedEventList<FingerGestures.PinchMoveEventHandler>();
		mRotateBeginEvents = new OrderedEventList<FingerGestures.RotationBeginEventHandler>();
		mRotateMoveEvents = new OrderedEventList<FingerGestures.RotationMoveEventHandler>();
		mRotateEndEvents = new OrderedEventList<FingerGestures.RotationEndEventHandler>();
		EnableFingerGestures();
	}

	private void OnDestroy()
	{
		DisableFingerGestures();
		mInstance = null;
	}

	public void SetForContextMenu()
	{
	}

	public void SetForMessageBox()
	{
		DisableFingerGestures();
	}

	public void SetForGamplay()
	{
		EnableFingerGestures();
	}

	public void SetForCutscene()
	{
		DisableFingerGestures();
	}

	private void EnableFingerGestures()
	{
		if (!mFingerGesturesEnabled)
		{
			mFingerGesturesEnabled = true;
			FingerGestures.OnFingerTap += InputManager_OnFingerTap;
			FingerGestures.OnPinchMove += InputManager_OnPinchMove;
			FingerGestures.OnPinchBegin += InputManager_OnPinchBegin;
			FingerGestures.OnTwoFingerDragBegin += InputManager_OnTwoFingerDragBegin;
			FingerGestures.OnTwoFingerDragMove += InputManager_OnTwoFingerDragMove;
			FingerGestures.OnTwoFingerDragEnd += InputManager_OnTwoFingerDragEnd;
			FingerGestures.OnFingerDoubleTap += InputManager_OnFingerDoubleTap;
			FingerGestures.OnFingerSwipe += InputManager_OnFingerSwipe;
			FingerGestures.OnFingerDown += InputManager_OnFingerDown;
			FingerGestures.OnFingerUp += InputManager_OnFingerUp;
			FingerGestures.OnFingerDragBegin += InputManager_OnFingerDragBegin;
			FingerGestures.OnFingerDragEnd += InputManager_OnFingerDragEnd;
			FingerGestures.OnFingerDragMove += InputManager_OnFingerDragMove;
			FingerGestures.OnRotationBegin += InputManager_OnRotationBegin;
			FingerGestures.OnRotationMove += InputManager_OnRotationMove;
			FingerGestures.OnRotationEnd += InputManager_OnRotationEnd;
		}
	}

	private void DisableFingerGestures()
	{
		if (mFingerGesturesEnabled)
		{
			mFingerGesturesEnabled = false;
			FingerGestures.OnFingerTap -= InputManager_OnFingerTap;
			FingerGestures.OnPinchMove -= InputManager_OnPinchMove;
			FingerGestures.OnPinchBegin -= InputManager_OnPinchBegin;
			FingerGestures.OnTwoFingerDragBegin -= InputManager_OnTwoFingerDragBegin;
			FingerGestures.OnTwoFingerDragMove -= InputManager_OnTwoFingerDragMove;
			FingerGestures.OnTwoFingerDragEnd -= InputManager_OnTwoFingerDragEnd;
			FingerGestures.OnFingerDoubleTap -= InputManager_OnFingerDoubleTap;
			FingerGestures.OnFingerSwipe -= InputManager_OnFingerSwipe;
			FingerGestures.OnFingerDown -= InputManager_OnFingerDown;
			FingerGestures.OnFingerUp -= InputManager_OnFingerUp;
			FingerGestures.OnFingerDragBegin -= InputManager_OnFingerDragBegin;
			FingerGestures.OnFingerDragEnd -= InputManager_OnFingerDragEnd;
			FingerGestures.OnFingerDragMove -= InputManager_OnFingerDragMove;
			FingerGestures.OnRotationBegin -= InputManager_OnRotationBegin;
			FingerGestures.OnRotationMove -= InputManager_OnRotationMove;
			FingerGestures.OnRotationEnd -= InputManager_OnRotationEnd;
		}
	}

	private void OnEnable()
	{
		EnableFingerGestures();
	}

	private void OnDisable()
	{
		DisableFingerGestures();
	}

	public void AddOnFingerTapEventHandler(FingerGestures.FingerTapEventHandler func, int priority)
	{
		mFingerTapEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerTapEventHandler(FingerGestures.FingerTapEventHandler func)
	{
		mFingerTapEvents.RemoveEvent(func);
	}

	public void AddOnFingerDownEventHandler(FingerDownEventHandler func, int priority)
	{
		mFingerDownEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerDownEventHandler(FingerDownEventHandler func)
	{
		mFingerDownEvents.RemoveEvent(func);
	}

	public void AddOnFingerUpEventHandler(FingerUpEventHandler func, int priority)
	{
		mFingerUpEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerUpEventHandler(FingerUpEventHandler func)
	{
		mFingerUpEvents.RemoveEvent(func);
	}

	public void AddOnFingerDoubleTapEventHandler(FingerGestures.FingerTapEventHandler func, int priority)
	{
		mFingerDoubleTapEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerDoubleTapEventHandler(FingerGestures.FingerTapEventHandler func)
	{
		mFingerDoubleTapEvents.RemoveEvent(func);
	}

	public void AddOnFingerSwipeEventHandler(FingerGestures.FingerSwipeEventHandler func, int priority)
	{
		mFingerSwipeEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerSwipeEventHandler(FingerGestures.FingerSwipeEventHandler func)
	{
		mFingerSwipeEvents.RemoveEvent(func);
	}

	public void AddOnFingerDragBeginEventHandler(FingerGestures.FingerDragBeginEventHandler func, int priority)
	{
		mFingerDragBeginEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerDragBeginEventHandler(FingerGestures.FingerDragBeginEventHandler func)
	{
		mFingerDragBeginEvents.RemoveEvent(func);
	}

	public void AddOnFingerDragMoveEventHandler(FingerGestures.FingerDragMoveEventHandler func, int priority)
	{
		mFingerDragMoveEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerDragMoveEventHandler(FingerGestures.FingerDragMoveEventHandler func)
	{
		mFingerDragMoveEvents.RemoveEvent(func);
	}

	public void AddOnFingerDragEndEventHandler(FingerGestures.FingerDragEndEventHandler func, int priority)
	{
		mFingerDragEndEvents.AddEvent(priority, func);
	}

	public void RemoveOnFingerDragEndEventHandler(FingerGestures.FingerDragEndEventHandler func)
	{
		mFingerDragEndEvents.RemoveEvent(func);
	}

	public void AddOnTwoFingerDragBeginEventHandler(FingerGestures.DragBeginEventHandler func, int priority)
	{
		mTwoFingerDragBeginEvents.AddEvent(priority, func);
	}

	public void RemoveOnTwoFingerDragBeginEventHandler(FingerGestures.DragBeginEventHandler func)
	{
		mTwoFingerDragBeginEvents.RemoveEvent(func);
	}

	public void AddOnTwoFingerDragMoveEventHandler(FingerGestures.DragMoveEventHandler func, int priority)
	{
		mTwoFingerDragMoveEvents.AddEvent(priority, func);
	}

	public void RemoveOnTwoFingerDragMoveEventHandler(FingerGestures.DragMoveEventHandler func)
	{
		mTwoFingerDragMoveEvents.RemoveEvent(func);
	}

	public void AddOnTwoFingerDragEndEventHandler(FingerGestures.DragEndEventHandler func, int priority)
	{
		mTwoFingerDragEndEvents.AddEvent(priority, func);
	}

	public void RemoveOnTwoFingerDragEndEventHandler(FingerGestures.DragEndEventHandler func)
	{
		mTwoFingerDragEndEvents.RemoveEvent(func);
	}

	public void AddOnPinchBeginEventHandler(FingerGestures.PinchEventHandler func, int priority)
	{
		mPinchBeginEvents.AddEvent(priority, func);
	}

	public void AddOnPinchMoveEventHandler(FingerGestures.PinchMoveEventHandler func, int priority)
	{
		mPinchMoveEvents.AddEvent(priority, func);
	}

	public void RemoveOnPinchBeginEventHandler(FingerGestures.PinchEventHandler func)
	{
		mPinchBeginEvents.RemoveEvent(func);
	}

	public void RemoveOnPinchMoveEventHandler(FingerGestures.PinchMoveEventHandler func)
	{
		mPinchMoveEvents.RemoveEvent(func);
	}

	public void AddOnRotateBeginEventHandler(FingerGestures.RotationBeginEventHandler func, int priority)
	{
		mRotateBeginEvents.AddEvent(priority, func);
	}

	public void RemoveOnRotateBeginEventHandler(FingerGestures.RotationBeginEventHandler func)
	{
		mRotateBeginEvents.RemoveEvent(func);
	}

	public void AddOnRotateMoveEventHandler(FingerGestures.RotationMoveEventHandler func, int priority)
	{
		mRotateMoveEvents.AddEvent(priority, func);
	}

	public void RemoveOnRotateMoveEventHandler(FingerGestures.RotationMoveEventHandler func)
	{
		mRotateMoveEvents.RemoveEvent(func);
	}

	public void AddOnRotateEndEventHandler(FingerGestures.RotationEndEventHandler func, int priority)
	{
		mRotateEndEvents.AddEvent(priority, func);
	}

	public void RemoveOnRotateEndEventHandler(FingerGestures.RotationEndEventHandler func)
	{
		mRotateEndEvents.RemoveEvent(func);
	}

	private void InputManager_OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (mBlockSingleTap)
		{
			return;
		}
		foreach (OrderedEvent<FingerGestures.FingerTapEventHandler> m in mFingerTapEvents.mList)
		{
			m.mFunc(fingerIndex, fingerPos);
		}
	}

	private void InputManager_OnFingerDoubleTap(int fingerIndex, Vector2 fingerPos)
	{
		mBlockSingleTap = true;
		foreach (OrderedEvent<FingerGestures.FingerTapEventHandler> m in mFingerDoubleTapEvents.mList)
		{
			m.mFunc(fingerIndex, fingerPos);
		}
	}

	private void InputManager_OnPinchBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		foreach (OrderedEvent<FingerGestures.PinchEventHandler> m in mPinchBeginEvents.mList)
		{
			m.mFunc(fingerPos1, fingerPos2);
		}
	}

	private void InputManager_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
	{
		foreach (OrderedEvent<FingerGestures.PinchMoveEventHandler> m in mPinchMoveEvents.mList)
		{
			m.mFunc(fingerPos1, fingerPos2, delta);
		}
	}

	private void InputManager_OnTwoFingerDragBegin(Vector2 fingerPos, Vector2 startPos)
	{
		foreach (OrderedEvent<FingerGestures.DragBeginEventHandler> m in mTwoFingerDragBeginEvents.mList)
		{
			m.mFunc(fingerPos, startPos);
		}
	}

	private void InputManager_OnTwoFingerDragMove(Vector2 fingerPos, Vector2 delta)
	{
		foreach (OrderedEvent<FingerGestures.DragMoveEventHandler> m in mTwoFingerDragMoveEvents.mList)
		{
			m.mFunc(fingerPos, delta);
		}
	}

	private void InputManager_OnTwoFingerDragEnd(Vector2 fingerPos)
	{
		foreach (OrderedEvent<FingerGestures.DragEndEventHandler> m in mTwoFingerDragEndEvents.mList)
		{
			m.mFunc(fingerPos);
		}
	}

	private void InputManager_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		foreach (OrderedEvent<FingerDownEventHandler> m in mFingerDownEvents.mList)
		{
			if (!m.mFunc(fingerIndex, fingerPos))
			{
				break;
			}
		}
	}

	private void InputManager_OnFingerUp(int fingerIndex, Vector2 fingerPos, float timeHeldDown)
	{
		foreach (OrderedEvent<FingerUpEventHandler> m in mFingerUpEvents.mList)
		{
			if (!m.mFunc(fingerIndex, fingerPos, timeHeldDown))
			{
				break;
			}
		}
	}

	private void InputManager_OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
	{
		foreach (OrderedEvent<FingerGestures.FingerDragBeginEventHandler> m in mFingerDragBeginEvents.mList)
		{
			m.mFunc(fingerIndex, fingerPos, startPos);
		}
	}

	private void InputManager_OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		foreach (OrderedEvent<FingerGestures.FingerDragMoveEventHandler> m in mFingerDragMoveEvents.mList)
		{
			m.mFunc(fingerIndex, fingerPos, delta);
		}
	}

	private void InputManager_OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
		foreach (OrderedEvent<FingerGestures.FingerDragEndEventHandler> m in mFingerDragEndEvents.mList)
		{
			m.mFunc(fingerIndex, fingerPos);
		}
	}

	private void InputManager_OnFingerSwipe(int fingerIndex, Vector2 startPos, FingerGestures.SwipeDirection direction, float velocity)
	{
		foreach (OrderedEvent<FingerGestures.FingerSwipeEventHandler> m in mFingerSwipeEvents.mList)
		{
			m.mFunc(fingerIndex, startPos, direction, velocity);
		}
	}

	public void InputManager_OnRotationBegin(Vector2 fingerPos1, Vector2 fingerPos2)
	{
		foreach (OrderedEvent<FingerGestures.RotationBeginEventHandler> m in mRotateBeginEvents.mList)
		{
			m.mFunc(fingerPos1, fingerPos2);
		}
	}

	public void InputManager_OnRotationMove(Vector2 fingerPos1, Vector2 fingerPos2, float rotationAngleDelta)
	{
		foreach (OrderedEvent<FingerGestures.RotationMoveEventHandler> m in mRotateMoveEvents.mList)
		{
			m.mFunc(fingerPos1, fingerPos2, rotationAngleDelta);
		}
	}

	public void InputManager_OnRotationEnd(Vector2 fingerPos1, Vector2 fingerPos2, float totalRotationAngle)
	{
		foreach (OrderedEvent<FingerGestures.RotationEndEventHandler> m in mRotateEndEvents.mList)
		{
			m.mFunc(fingerPos1, fingerPos2, totalRotationAngle);
		}
	}

	private void Start()
	{
		mBlockSingleTap = false;
	}

	private void Update()
	{
		mBlockSingleTap = false;
	}
}
