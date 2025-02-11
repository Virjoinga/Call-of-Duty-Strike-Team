public class Player : ISaveLoad
{
	public const float DEFAULT_SOUND_MUSIC = 1f;

	public const float DEFAULT_SOUND_SFX = 1f;

	private const bool DEFAULT_GESTURES_ENABLED = false;

	private const bool DEFAULT_GYRO_ENABLED = false;

	private const float DEFAULT_GYROSCOPE = 0.1f;

	private const float DEFAULT_X_SENSITIVITY = 0.3f;

	private const float DEFAULT_Y_SENSITIVITY = 0.25f;

	private const float DEFAULT_GAMEPAD_SENSITIVITY = 0.2f;

	private const float DEFAULT_AIM_ASSIST = 0.75f;

	private const bool DEFAULT_INVERT_GYRO_X = false;

	private const bool DEFAULT_INVERT_GYRO_Y = false;

	private const bool DEFAULT_INVERT_TOUCH_X = false;

	private const bool DEFAULT_INVERT_TOUCH_Y = false;

	private const bool DEFAULT_INVERT_GAMEPAD_X = false;

	private const bool DEFAULT_INVERT_GAMEPAD_Y = false;

	private const bool DEFAULT_SINGLE_TAP_SHOOT = false;

	private const bool DEFAULT_DOUBLE_TAP_SHOOT = false;

	private const bool DEFAULT_ADS_AUTO_LOCK_ON = true;

	private const bool DEFAULT_MOVEABLE_FIRE_BUTTON = false;

	private const float DEFAULT_MOVEABLE_FIRE_BUTTON_X = 0f;

	private const float DEFAULT_MOVEABLE_FIRE_BUTTON_Y = 0f;

	private const bool DEFAULT_NOTIFICATION_SALE = true;

	private const bool DEFAULT_NOTIFICATION_FRIEND = true;

	private const bool DEFAULT_NOTIFICATION_CHALLENGE = true;

	private const string SoundMusicKey = "SoundMusicVolume";

	private const string SoundSFXKey = "SoundSFXVolume";

	private const string GesturesEnabledKey = "GesturesEnabled";

	private const string GyroscopeEnabledKey = "GyroscopeEnabled";

	private const string GyroscopeKey = "Gyroscope";

	private const string XSensitivityKey = "XSensitivity";

	private const string YSensitivityKey = "YSensitivity";

	private const string GamepadSensitivityKey = "GamepadSensitivity";

	private const string AimAssistKey = "AimAssist";

	private const string InvertGyroXKey = "InvertGyroX";

	private const string InvertGyroYKey = "InvertGyroY";

	private const string InvertTouchXKey = "InvertTouchX";

	private const string InvertTouchYKey = "InvertTouchY";

	private const string InvertGamepadXKey = "InvertGamepadX";

	private const string InvertGamepadYKey = "InvertGamepadY";

	private const string SingleTapShootKey = "SingleTapShoot";

	private const string DoubleTapShootKey = "DoubleTapShoot";

	private const string ADSAutoLockOnKey = "ADSAutoLockOn";

	private const string NotificationsSaleKey = "NotificationsSale";

	private const string NotificationsFriendKey = "NotificationsFriend";

	private const string NotificationsChallengeKey = "NotificationsChallenge";

	private const string MovableFireButtonKey = "MovableFireButton";

	private const string MovableFireButtonXPositionKey = "MovableFireButtonXPos";

	private const string MovableFireButtonYPositionKey = "MovableFireButtonYPos";

	private bool mGesturesEnabled;

	private bool mGyroEnabled;

	private float mGyroscope = 0.1f;

	private float mXSensitivity = 0.3f;

	private float mYSensitivity = 0.25f;

	private float mGamepadSensitivity = 0.2f;

	private float mAimAssist = 0.75f;

	private bool mInvertGyroX;

	private bool mInvertGyroY;

	private bool mInvertTouchX;

	private bool mInvertTouchY;

	private bool mInvertGamepadX;

	private bool mInvertGamepadY;

	private bool mSingleTapShoot;

	private bool mDoubleTapShoot;

	private bool mADSAutoLockOn = true;

	private bool mMovableFireButton;

	private float mMovableFireButtonX;

	private float mMovableFireButtonY;

	private bool mNotificationsSale = true;

	private bool mNotificationsFriend = true;

	private bool mNotificationsChallenge = true;

	public bool FirstPersonGesturesEnabled
	{
		get
		{
			return mGesturesEnabled;
		}
		set
		{
			mGesturesEnabled = value;
		}
	}

	public bool FirstPersonGyroscopeEnabled
	{
		get
		{
			return mGyroEnabled;
		}
		set
		{
			mGyroEnabled = value;
		}
	}

	public float FirstPersonGyroscope
	{
		get
		{
			return mGyroscope;
		}
		set
		{
			mGyroscope = value;
		}
	}

	public float FirstPersonXSensitivity
	{
		get
		{
			return mXSensitivity;
		}
		set
		{
			mXSensitivity = value;
		}
	}

	public float FirstPersonYSensitivity
	{
		get
		{
			return mYSensitivity;
		}
		set
		{
			mYSensitivity = value;
		}
	}

	public float FirstPersonGamepadSensitivity
	{
		get
		{
			return mGamepadSensitivity;
		}
		set
		{
			mGamepadSensitivity = value;
		}
	}

	public float FirstPersonAimAssist
	{
		get
		{
			return mAimAssist;
		}
		set
		{
			mAimAssist = value;
		}
	}

	public bool FirstPersonInvertGyroscopeX
	{
		get
		{
			return mInvertGyroX;
		}
		set
		{
			mInvertGyroX = value;
		}
	}

	public bool FirstPersonInvertGyroscopeY
	{
		get
		{
			return mInvertGyroY;
		}
		set
		{
			mInvertGyroY = value;
		}
	}

	public bool FirstPersonInvertTouchX
	{
		get
		{
			return mInvertTouchX;
		}
		set
		{
			mInvertTouchX = value;
		}
	}

	public bool FirstPersonInvertTouchY
	{
		get
		{
			return mInvertTouchY;
		}
		set
		{
			mInvertTouchY = value;
		}
	}

	public bool FirstPersonInvertGamepadX
	{
		get
		{
			return mInvertGamepadX;
		}
		set
		{
			mInvertGamepadX = value;
		}
	}

	public bool FirstPersonInvertGamepadY
	{
		get
		{
			return mInvertGamepadY;
		}
		set
		{
			mInvertGamepadY = value;
		}
	}

	public bool FirstPersonMovableFireButton
	{
		get
		{
			return mMovableFireButton;
		}
		set
		{
			mMovableFireButton = value;
		}
	}

	public float FirstPersonMovableFireButtonXPos
	{
		get
		{
			return mMovableFireButtonX;
		}
		set
		{
			mMovableFireButtonX = value;
		}
	}

	public float FirstPersonMovableFireButtonYPos
	{
		get
		{
			return mMovableFireButtonY;
		}
		set
		{
			mMovableFireButtonY = value;
		}
	}

	public bool SingleTapShoot
	{
		get
		{
			return mSingleTapShoot;
		}
		set
		{
			mSingleTapShoot = value;
			if (mSingleTapShoot)
			{
				DoubleTapShoot = false;
			}
		}
	}

	public bool DoubleTapShoot
	{
		get
		{
			return mDoubleTapShoot;
		}
		set
		{
			mDoubleTapShoot = value;
			if (mDoubleTapShoot)
			{
				SingleTapShoot = false;
			}
		}
	}

	public bool ADSAutoLockOn
	{
		get
		{
			return mADSAutoLockOn;
		}
		set
		{
			mADSAutoLockOn = value;
		}
	}

	public bool NotificationsSale
	{
		get
		{
			return mNotificationsSale;
		}
		set
		{
			mNotificationsSale = value;
		}
	}

	public bool NotificationsFriend
	{
		get
		{
			return mNotificationsFriend;
		}
		set
		{
			mNotificationsFriend = value;
		}
	}

	public bool NotificationsChallenge
	{
		get
		{
			return mNotificationsChallenge;
		}
		set
		{
			mNotificationsChallenge = value;
		}
	}

	public void Save()
	{
		SecureStorage.Instance.SetBool("GesturesEnabled", mGesturesEnabled);
		SecureStorage.Instance.SetBool("GyroscopeEnabled", mGyroEnabled);
		SecureStorage.Instance.SetFloat("Gyroscope", mGyroscope);
		SecureStorage.Instance.SetFloat("XSensitivity", mXSensitivity);
		SecureStorage.Instance.SetFloat("YSensitivity", mYSensitivity);
		SecureStorage.Instance.SetFloat("GamepadSensitivity", mGamepadSensitivity);
		SecureStorage.Instance.SetFloat("AimAssist", mAimAssist);
		SecureStorage.Instance.SetBool("InvertGyroX", mInvertGyroX);
		SecureStorage.Instance.SetBool("InvertGyroY", mInvertGyroY);
		SecureStorage.Instance.SetBool("InvertTouchX", mInvertTouchX);
		SecureStorage.Instance.SetBool("InvertTouchY", mInvertTouchY);
		SecureStorage.Instance.SetBool("InvertGamepadX", mInvertGamepadX);
		SecureStorage.Instance.SetBool("InvertGamepadY", mInvertGamepadY);
		SecureStorage.Instance.SetBool("SingleTapShoot", mSingleTapShoot);
		SecureStorage.Instance.SetBool("DoubleTapShoot", mDoubleTapShoot);
		SecureStorage.Instance.SetBool("ADSAutoLockOn", mADSAutoLockOn);
		SecureStorage.Instance.SetBool("NotificationsSale", mNotificationsSale);
		SecureStorage.Instance.SetBool("NotificationsFriend", mNotificationsFriend);
		SecureStorage.Instance.SetBool("NotificationsChallenge", mNotificationsChallenge);
		SecureStorage.Instance.SetBool("MovableFireButton", mMovableFireButton);
		SecureStorage.Instance.SetFloat("MovableFireButtonXPos", mMovableFireButtonX);
		SecureStorage.Instance.SetFloat("MovableFireButtonYPos", mMovableFireButtonY);
	}

	public void Load()
	{
		SecureStorage.Instance.GetBool("GesturesEnabled", ref mGesturesEnabled);
		SecureStorage.Instance.GetBool("GyroscopeEnabled", ref mGyroEnabled);
		SecureStorage.Instance.GetFloat("Gyroscope", ref mGyroscope);
		SecureStorage.Instance.GetFloat("XSensitivity", ref mXSensitivity);
		SecureStorage.Instance.GetFloat("YSensitivity", ref mYSensitivity);
		SecureStorage.Instance.GetFloat("GamepadSensitivity", ref mGamepadSensitivity);
		SecureStorage.Instance.GetFloat("AimAssist", ref mAimAssist);
		SecureStorage.Instance.GetBool("InvertGyroX", ref mInvertGyroX);
		SecureStorage.Instance.GetBool("InvertGyroY", ref mInvertGyroY);
		SecureStorage.Instance.GetBool("InvertTouchX", ref mInvertTouchX);
		SecureStorage.Instance.GetBool("InvertTouchY", ref mInvertTouchY);
		SecureStorage.Instance.GetBool("InvertGamepadX", ref mInvertGamepadX);
		SecureStorage.Instance.GetBool("InvertGamepadY", ref mInvertGamepadY);
		SecureStorage.Instance.GetBool("SingleTapShoot", ref mSingleTapShoot);
		SecureStorage.Instance.GetBool("DoubleTapShoot", ref mDoubleTapShoot);
		SecureStorage.Instance.GetBool("ADSAutoLockOn", ref mADSAutoLockOn);
		SecureStorage.Instance.GetBool("NotificationsSale", ref mNotificationsSale);
		SecureStorage.Instance.GetBool("NotificationsFriend", ref mNotificationsFriend);
		SecureStorage.Instance.GetBool("NotificationsChallenge", ref mNotificationsChallenge);
		SecureStorage.Instance.GetBool("MovableFireButton", ref mMovableFireButton);
		SecureStorage.Instance.GetFloat("MovableFireButtonXPos", ref mMovableFireButtonX);
		SecureStorage.Instance.GetFloat("MovableFireButtonYPos", ref mMovableFireButtonY);
	}

	public void Reset()
	{
		mGesturesEnabled = false;
		mGyroEnabled = false;
		mGyroscope = 0.1f;
		mXSensitivity = 0.3f;
		mYSensitivity = 0.25f;
		mGamepadSensitivity = 0.2f;
		mAimAssist = 0.75f;
		mInvertGyroX = false;
		mInvertGyroY = false;
		mInvertTouchX = false;
		mInvertTouchY = false;
		mInvertGamepadX = false;
		mInvertGamepadY = false;
		mSingleTapShoot = false;
		mDoubleTapShoot = false;
		mADSAutoLockOn = true;
		mNotificationsSale = true;
		mNotificationsFriend = true;
		mNotificationsChallenge = true;
		mMovableFireButton = false;
		mMovableFireButtonX = 0f;
		mMovableFireButtonY = 0f;
	}

	public void ResetAndSaveSoundSettings()
	{
		SecureStorage.Instance.MusicVolume = 1f;
		SecureStorage.Instance.SoundFXVolume = 1f;
	}
}
