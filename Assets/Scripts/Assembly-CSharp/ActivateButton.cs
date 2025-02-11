using System;
using UnityEngine;

public class ActivateButton : MonoBehaviour
{
	public enum State
	{
		Idle = 0,
		Pulse = 1,
		Disabled = 2
	}

	private const float PULSE_MOVE_TIME = 1f;

	private const float ROTATION_SPEED = 20f;

	private const int NUM_FLASHES = 4;

	private const int NUM_FLASHES_ON = 3;

	private UIButton mButton;

	private PackedSprite mGlow;

	private PackedSprite mHighlight;

	private PackedSprite mExclamation;

	private float mLastFrameTime;

	private float mPulseTime;

	private State mCurrentState;

	private int mNumPulses;

	private bool mUp;

	public bool Alert { get; set; }

	public State CurrentState
	{
		get
		{
			return mCurrentState;
		}
		set
		{
			if (mCurrentState != value)
			{
				mCurrentState = value;
			}
		}
	}

	public void UpdateStatus()
	{
		if (FrontEndController.Instance.IsInFrontend)
		{
			CurrentState = ((Bedrock.getUserConnectionStatus() == Bedrock.brUserConnectionStatus.BR_LOGGED_OUT) ? State.Pulse : State.Idle);
		}
		else
		{
			CurrentState = State.Disabled;
		}
		Alert = ActivateFriendInviteWatcher.Instance.friendInviteCount != 0;
	}

	private void Awake()
	{
		mButton = base.gameObject.GetComponentInChildren<UIButton>();
		Transform transform = base.transform.FindChild("Exclamation");
		if (transform != null)
		{
			mExclamation = transform.GetComponent<PackedSprite>();
		}
		Transform transform2 = base.transform.FindChild("Glow");
		if (transform2 != null)
		{
			mGlow = transform2.GetComponent<PackedSprite>();
		}
		Transform transform3 = base.transform.FindChild("Highlight");
		if (transform3 != null)
		{
			mHighlight = transform3.GetComponent<PackedSprite>();
		}
		mCurrentState = State.Pulse;
		mNumPulses = 0;
		mPulseTime = 0f;
		mLastFrameTime = 0f;
	}

	private void OnEnable()
	{
		Bedrock.UserConnectionStatusChanged += OnlineStatusChanged;
		ActivateFriendInviteWatcher.FriendInviteCountUpdated += OnFriendInviteCountUpdated;
		Alert = ActivateFriendInviteWatcher.Instance.friendInviteCount != 0;
	}

	private void OnDisable()
	{
		Bedrock.UserConnectionStatusChanged -= OnlineStatusChanged;
		ActivateFriendInviteWatcher.FriendInviteCountUpdated -= OnFriendInviteCountUpdated;
	}

	private void Update()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		mPulseTime += realtimeSinceStartup - mLastFrameTime;
		if (mPulseTime >= 1f)
		{
			mPulseTime = 0f;
			mUp = !mUp;
			if (mNumPulses++ > 7)
			{
				mNumPulses = 0;
			}
		}
		float num = ((!mUp) ? (1f - mPulseTime / 1f) : (mPulseTime / 1f));
		if (mCurrentState == State.Idle)
		{
			if (mGlow != null && mGlow.IsHidden())
			{
				mGlow.Hide(false);
			}
			if (mButton != null && mButton.controlState != 0)
			{
				mButton.SetControlState(UIButton.CONTROL_STATE.NORMAL, true);
			}
			if (mExclamation != null)
			{
				bool flag = num > 0.5f || mNumPulses > 4;
				mExclamation.Hide(!Alert || !flag);
			}
			if (mHighlight != null)
			{
				mHighlight.Hide(false);
				mHighlight.transform.Rotate(new Vector3(0f, 0f, 20f * (realtimeSinceStartup - mLastFrameTime)));
			}
		}
		else if (mCurrentState == State.Disabled)
		{
			if (mGlow != null && !mGlow.IsHidden())
			{
				mGlow.Hide(true);
			}
			if (mButton != null && mButton.controlState != UIButton.CONTROL_STATE.DISABLED)
			{
				mButton.SetControlState(UIButton.CONTROL_STATE.DISABLED, true);
			}
			if (mExclamation != null && !mExclamation.IsHidden())
			{
				mExclamation.Hide(true);
			}
			if (mHighlight != null && !mHighlight.IsHidden())
			{
				mHighlight.Hide(true);
			}
		}
		else if (mCurrentState == State.Pulse)
		{
			if (mGlow != null)
			{
				Color white = Color.white;
				white.a = num;
				mGlow.Hide(false);
				mGlow.SetColor(white);
			}
			if (mButton != null && mButton.controlState != 0)
			{
				mButton.SetControlState(UIButton.CONTROL_STATE.NORMAL, true);
			}
			if (mExclamation != null && !mExclamation.IsHidden())
			{
				mExclamation.Hide(true);
			}
			if (mHighlight != null)
			{
				mHighlight.Hide(false);
				mHighlight.transform.Rotate(new Vector3(0f, 0f, 20f * (realtimeSinceStartup - mLastFrameTime)));
			}
		}
		mLastFrameTime = realtimeSinceStartup;
	}

	private void OnlineStatusChanged(object sender, EventArgs e)
	{
		UpdateStatus();
	}

	private void OnFriendInviteCountUpdated(object sender, FriendInviteCountEventArgs e)
	{
		UpdateStatus();
	}
}
