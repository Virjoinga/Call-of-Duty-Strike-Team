using System;
using System.Collections.Generic;
using UnityEngine;

public class OverwatchController : MonoBehaviour
{
	public GameObject mOnFinishObj;

	public string mOnFinishMsg;

	public List<GameObject> mGroupObjectToCall;

	public List<string> mGroupFunctionToCall;

	private static OverwatchController smInstance;

	private Weapon_OverwatchMachineGun mMachineGun;

	private StrategyHudController mWaitToHide;

	public static OverwatchController Instance
	{
		get
		{
			return smInstance;
		}
	}

	public float RocketClipReady01
	{
		get
		{
			return mMachineGun.RocketClipReady01;
		}
	}

	public int NumRocketsRemaining
	{
		get
		{
			return mMachineGun.NumRocketsRemaining;
		}
	}

	public bool ReloadingRocket
	{
		get
		{
			return mMachineGun.ReloadingRocket;
		}
	}

	public bool Active { get; set; }

	public float Duration { get; set; }

	public float TargetSpread { get; set; }

	public float CharacterMovementSpeedMultiplier { get; set; }

	public float RocketReloadTime { get; set; }

	public bool TimeoutOverwatch { get; set; }

	public OverWatchCamera OverwatchLC { get; private set; }

	public GameObject Origin { get; set; }

	private void Awake()
	{
		if (smInstance != null)
		{
			throw new Exception("Can not have multiple OverwatchController");
		}
		smInstance = this;
		TargetSpread = 1f;
		CharacterMovementSpeedMultiplier = 0.5f;
		RocketReloadTime = 3f;
	}

	private void Start()
	{
		mMachineGun = new Weapon_OverwatchMachineGun(this);
		if (!TimeoutOverwatch && Duration == 0f)
		{
			Duration = 1f;
		}
	}

	private void OnDestroy()
	{
		smInstance = null;
	}

	private void Update()
	{
		if (OverwatchLC == null)
		{
			OverwatchLC = CameraManager.Instance.gameObject.GetComponentInChildren<OverWatchCamera>();
		}
		if (mWaitToHide != null && mWaitToHide.gameObject.activeInHierarchy)
		{
			mWaitToHide.HideOverwatchShootingHud(true);
			mWaitToHide = null;
		}
		if (!Active)
		{
			return;
		}
		if (Origin != null)
		{
			OverwatchLC.CentrePointObject = Origin;
			OverwatchLC.Start();
			Origin = null;
		}
		TBFAssert.DoAssert(mMachineGun != null);
		mMachineGun.Update(Time.deltaTime, null, null);
		if (TimeoutOverwatch)
		{
			Duration -= Time.deltaTime;
		}
		if (!(Duration <= 0f))
		{
			return;
		}
		Active = false;
		GameController.Instance.ZoomInAvailable = true;
		GameController.Instance.ZoomOutAvailable = true;
		GameController.Instance.OnGameplayViewClick(null, null);
		mWaitToHide = StrategyHudController.Instance;
		mMachineGun.StopLoops();
		Container.SendMessage(mOnFinishObj, mOnFinishMsg, base.gameObject);
		if (mGroupObjectToCall == null || mGroupObjectToCall.Count <= 0)
		{
			return;
		}
		int num = 0;
		foreach (GameObject item in mGroupObjectToCall)
		{
			string message = string.Empty;
			if (mGroupFunctionToCall != null && num < mGroupFunctionToCall.Count)
			{
				message = mGroupFunctionToCall[num];
			}
			Container.SendMessage(item, message, base.gameObject);
			num++;
		}
	}

	public void FireWeapon(bool active)
	{
		if (active)
		{
			mMachineGun.DepressTrigger();
		}
		else
		{
			mMachineGun.ReleaseTrigger();
		}
	}

	public void FireCannon()
	{
		mMachineGun.FireCannon();
	}

	public Vector3 GetPosition()
	{
		if (CameraManager.Instance != null)
		{
			Transform transform = CameraManager.Instance.CurrentCamera.transform;
			return transform.position;
		}
		return Vector3.zero;
	}

	public Vector3 GetTarget()
	{
		if (CameraManager.Instance != null)
		{
			Transform transform = CameraManager.Instance.CurrentCamera.transform;
			return transform.position + transform.forward * 100f;
		}
		return Vector3.zero;
	}
}
