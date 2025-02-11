using System.Collections;
using UnityEngine;

public class MenuScreenBlade : MonoBehaviour
{
	public enum BladeOrientation
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3
	}

	public enum BladeTransition
	{
		None = 0,
		On = 1,
		Off = 2,
		Offset = 3
	}

	public delegate void MenuScreenBladeTransitionFinishedDelegate(MenuScreenBlade blade, BladeTransition type);

	public BladeOrientation Orientation = BladeOrientation.Right;

	public bool StartActive;

	public EZScreenPlacement PostArrivalOffset;

	public EaseType PostArrivalOffsetType = EaseType.easeOutSine;

	protected Vector3 mOnScreenOffset = Vector3.zero;

	protected Vector3 mOffScreenRightOffset = new Vector3(35f, 0f, 0f);

	protected Vector3 mOffScreenLeftOffset = new Vector3(-35f, 0f, 0f);

	protected Vector3 mOffScreenTopOffset = new Vector3(0f, 20f, 0f);

	protected Vector3 mOffScreenBottomOffset = new Vector3(0f, -20f, 0f);

	protected EaseType mScreenMoveOnType = EaseType.easeOutSine;

	protected EaseType mScreenMoveOffType = EaseType.easeInSine;

	protected float mScreenMoveTime = 0.25f;

	private MenuScreenBladeTransitionFinishedDelegate mTransitionFinishedDelegate;

	private EZScreenPlacement[] mScreenComponents;

	private AnimateCommonBackgroundBox[] mAnimatedBoxes;

	private BladeTransition mIsTransitioning;

	private Carousel mCarousel;

	private bool mIsActive;

	private bool mScreenPlaced;

	private bool mWasMovedToOffset;

	public bool IsTransitioning
	{
		get
		{
			return mIsTransitioning != BladeTransition.None;
		}
	}

	public bool IsTransitioningOn
	{
		get
		{
			return mIsTransitioning == BladeTransition.On;
		}
	}

	public bool IsTransitioningOff
	{
		get
		{
			return mIsTransitioning == BladeTransition.Off;
		}
	}

	public bool IsActive
	{
		get
		{
			return mIsActive;
		}
		private set
		{
			mIsActive = value;
		}
	}

	public bool IsPlaced
	{
		get
		{
			return mScreenPlaced;
		}
	}

	public float ScreenMoveTime
	{
		get
		{
			return mScreenMoveTime;
		}
		set
		{
			mScreenMoveTime = value;
		}
	}

	public virtual void Awake()
	{
		mScreenComponents = base.transform.GetComponentsInChildren<EZScreenPlacement>();
		mAnimatedBoxes = base.transform.GetComponentsInChildren<AnimateCommonBackgroundBox>();
		mCarousel = base.transform.GetComponentInChildren<Carousel>();
		if (mCarousel != null)
		{
			mCarousel.enabled = false;
		}
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		float num2 = (float)Screen.width * num;
		float num3 = (float)Screen.height * num;
		mOffScreenRightOffset = new Vector3(num2, 0f, 0f);
		mOffScreenLeftOffset = new Vector3(0f - num2, 0f, 0f);
		mOffScreenTopOffset = new Vector3(0f, num3, 0f);
		mOffScreenBottomOffset = new Vector3(0f, 0f - num3, 0f);
		mScreenPlaced = false;
		mWasMovedToOffset = false;
	}

	public virtual void Update()
	{
		if (mScreenComponents != null && !mScreenPlaced)
		{
			bool flag = true;
			EZScreenPlacement[] array = mScreenComponents;
			foreach (EZScreenPlacement eZScreenPlacement in array)
			{
				if (!eZScreenPlacement.Started)
				{
					flag = false;
				}
			}
			if (flag)
			{
				StartScreenPlacement();
				mScreenPlaced = true;
			}
		}
		if (mIsTransitioning == BladeTransition.On)
		{
			bool flag2 = false;
			BladeOrientation orientation = Orientation;
			if ((orientation != 0 && orientation != BladeOrientation.Right) ? Mathf.Approximately(mOnScreenOffset.y, base.gameObject.transform.position.y) : Mathf.Approximately(mOnScreenOffset.x, base.gameObject.transform.position.x))
			{
				mIsTransitioning = BladeTransition.None;
				OnScreen();
			}
		}
		else if (mIsTransitioning == BladeTransition.Off)
		{
			Vector3 offScreenPosition = GetOffScreenPosition();
			bool flag3 = false;
			BladeOrientation orientation = Orientation;
			if ((orientation != 0 && orientation != BladeOrientation.Right) ? Mathf.Approximately(offScreenPosition.y, base.gameObject.transform.position.y) : Mathf.Approximately(offScreenPosition.x, base.gameObject.transform.position.x))
			{
				mIsTransitioning = BladeTransition.None;
				OffScreen();
			}
		}
	}

	public void StartScreenPlacement()
	{
		mIsActive = StartActive;
		if (IsActive)
		{
			base.gameObject.transform.position = mOnScreenOffset;
			return;
		}
		Vector3 offScreenPosition = GetOffScreenPosition();
		base.gameObject.transform.position = offScreenPosition;
	}

	public void Activate(MenuScreenBladeTransitionFinishedDelegate callback, float screenMoveTime)
	{
		mTransitionFinishedDelegate = callback;
		StartCoroutine(Activate(screenMoveTime, 0f));
	}

	public void Activate(MenuScreenBladeTransitionFinishedDelegate callback)
	{
		mTransitionFinishedDelegate = callback;
		StartCoroutine(Activate(mScreenMoveTime, 0f));
	}

	public virtual void Activate()
	{
		StartCoroutine(Activate(mScreenMoveTime, 0f));
	}

	public virtual void DelayedActivate(float delay)
	{
		StartCoroutine(Activate(mScreenMoveTime, delay));
	}

	protected virtual void OnActivate()
	{
	}

	private IEnumerator Activate(float screenMoveTime, float delayActivate)
	{
		mIsTransitioning = BladeTransition.On;
		float delayed = 0f;
		while (delayed < delayActivate)
		{
			delayed += TimeManager.DeltaTime;
			yield return null;
		}
		OnActivate();
		if (!IsActive)
		{
			if (!mWasMovedToOffset)
			{
				base.gameObject.MoveTo(mOnScreenOffset, screenMoveTime, 0f, mScreenMoveOnType);
			}
			else
			{
				base.gameObject.MoveTo(PostArrivalOffset.transform.localPosition, screenMoveTime, 0f, PostArrivalOffsetType);
				mIsTransitioning = BladeTransition.Offset;
			}
			MenuSFX.Instance.MenuBoxExpand.Play2D();
		}
		yield return null;
	}

	public void MoveToOffset(float screenMoveTime)
	{
		if (PostArrivalOffset != null)
		{
			base.gameObject.MoveTo(PostArrivalOffset.transform.localPosition, screenMoveTime, 0f, PostArrivalOffsetType);
			mIsTransitioning = BladeTransition.Offset;
			mWasMovedToOffset = true;
		}
	}

	public void SetOffScreenNow()
	{
		base.transform.position = GetOffScreenPosition();
		mIsTransitioning = BladeTransition.None;
	}

	public void Deactivate()
	{
		StartCoroutine(Deactivate(0f));
	}

	public void DeactivateAndDestroy()
	{
		StartCoroutine(DeactivateAndDestroy(0f));
	}

	public void Deactivate(MenuScreenBladeTransitionFinishedDelegate callback)
	{
		mTransitionFinishedDelegate = callback;
		StartCoroutine(Deactivate(0f));
	}

	public void DelayedDeactivate(float delay)
	{
		StartCoroutine(Deactivate(delay));
	}

	protected virtual void OnDeactivate()
	{
		if (mCarousel != null)
		{
			mCarousel.CancelTransition();
			mCarousel.enabled = false;
		}
	}

	private IEnumerator Deactivate(float delayDeactivate)
	{
		bool wasTransitioningOn = IsTransitioningOn;
		mIsTransitioning = BladeTransition.Off;
		float delayed = 0f;
		while (delayed < delayDeactivate)
		{
			delayed += TimeManager.DeltaTime;
			yield return null;
		}
		OnDeactivate();
		float longestAnimateClosedSequence = mScreenMoveTime;
		AnimateCommonBackgroundBox[] array = mAnimatedBoxes;
		foreach (AnimateCommonBackgroundBox box in array)
		{
			float lengthOfCloseSequence = box.AnimateClosed();
			if (lengthOfCloseSequence > longestAnimateClosedSequence)
			{
				longestAnimateClosedSequence = lengthOfCloseSequence;
			}
		}
		if (IsActive || wasTransitioningOn)
		{
			iTweenExtensions.MoveTo(position: GetOffScreenPosition(), go: base.gameObject, time: longestAnimateClosedSequence, delay: 0f, easeType: mScreenMoveOffType);
			mIsActive = false;
			MenuSFX.Instance.MenuBoxClose.Play2D();
		}
		yield return null;
	}

	public IEnumerator DeactivateAndDestroy(float delayDeactivate)
	{
		mIsTransitioning = BladeTransition.Off;
		float delayed = 0f;
		while (delayed < delayDeactivate)
		{
			delayed += TimeManager.DeltaTime;
			yield return null;
		}
		OnDeactivate();
		float longestAnimateClosedSequence = mScreenMoveTime;
		AnimateCommonBackgroundBox[] array = mAnimatedBoxes;
		foreach (AnimateCommonBackgroundBox box in array)
		{
			float lengthOfCloseSequence = box.AnimateClosed();
			if (lengthOfCloseSequence > longestAnimateClosedSequence)
			{
				longestAnimateClosedSequence = lengthOfCloseSequence;
			}
		}
		if (IsActive)
		{
			iTweenExtensions.MoveTo(position: GetOffScreenPosition(), go: base.gameObject, time: longestAnimateClosedSequence, delay: 0f, easeType: mScreenMoveOffType);
			mIsActive = false;
		}
		float wait = 0f;
		while (wait < longestAnimateClosedSequence)
		{
			wait += TimeManager.DeltaTime;
			yield return null;
		}
		Object.Destroy(base.gameObject);
		yield return null;
	}

	public virtual void OnScreen()
	{
		mIsActive = true;
		AnimateCommonBackgroundBox[] array = mAnimatedBoxes;
		foreach (AnimateCommonBackgroundBox animateCommonBackgroundBox in array)
		{
			animateCommonBackgroundBox.AnimateOpen();
		}
		if (mTransitionFinishedDelegate != null)
		{
			mTransitionFinishedDelegate(this, BladeTransition.On);
			mTransitionFinishedDelegate = null;
		}
		if (mCarousel != null)
		{
			mCarousel.enabled = true;
		}
	}

	public virtual void OffScreen()
	{
		if (mTransitionFinishedDelegate != null)
		{
			mTransitionFinishedDelegate(this, BladeTransition.Off);
			mTransitionFinishedDelegate = null;
		}
	}

	protected void UpdateTransitioningIfNone(MenuScreenBlade toMatch)
	{
		if (mIsTransitioning == BladeTransition.None && toMatch.mIsTransitioning != 0)
		{
			mIsTransitioning = toMatch.mIsTransitioning;
		}
	}

	private Vector3 GetOffScreenPosition()
	{
		Vector3 result = mOffScreenLeftOffset;
		switch (Orientation)
		{
		case BladeOrientation.Left:
			result = mOffScreenLeftOffset;
			break;
		case BladeOrientation.Right:
			result = mOffScreenRightOffset;
			break;
		case BladeOrientation.Top:
			result = mOffScreenTopOffset;
			break;
		case BladeOrientation.Bottom:
			result = mOffScreenBottomOffset;
			break;
		}
		return result;
	}
}
