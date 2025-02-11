using UnityEngine;

public class TrackingRobotRealCharacter : RealCharacter
{
	public GameObject Shield;

	public SecurityCameraOverrideData CameraOverrideData;

	public SentryGunOverrideData SentryGunOverrideData;

	private FactionHelper.Category mPreviousFaction;

	private bool mTakingFire;

	private Vector3 mInternalForward;

	public Quaternion mCachedRotation;

	public Transform AimingMuzzle { get; set; }

	public Transform SentryModel { get; set; }

	public bool TakingFire
	{
		get
		{
			return mTakingFire;
		}
	}

	public override void SetupDefaultTask()
	{
		base.SetupDefaultTask();
		if (myActor.awareness.ChDefCharacterType == CharacterType.SentryGun)
		{
			TaskSentryGun taskSentryGun = new TaskSentryGun(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default);
			if (taskSentryGun != null)
			{
				if (SentryGunOverrideData != null)
				{
					myActor.awareness.VisionRange = SentryGunOverrideData.VisionDistance;
					myActor.awareness.FoV = SentryGunOverrideData.VisionFov;
					taskSentryGun.WarmUpTime = SentryGunOverrideData.WarmTime;
					taskSentryGun.CoolDownTime = SentryGunOverrideData.CoolTime;
					taskSentryGun.SweepSpeed = SentryGunOverrideData.Speed;
					taskSentryGun.mZoneInOnTargetTime = SentryGunOverrideData.ZoneInOnTargetTime;
					HackableObject component = myActor.GetComponent<HackableObject>();
					if (component != null && component.m_Interface != null)
					{
						component.m_Interface.TimeToHack = SentryGunOverrideData.TimeToHack;
					}
				}
				mPreviousFaction = myActor.awareness.faction;
				taskSentryGun.CreateScanEffect();
			}
		}
		else
		{
			TaskSecurityCamera taskSecurityCamera = new TaskSecurityCamera(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default);
			if (taskSecurityCamera != null && CameraOverrideData != null)
			{
				myActor.realCharacter.Range = CameraOverrideData.TravelRadius;
				myActor.awareness.VisionRange = CameraOverrideData.ConeRange;
				myActor.awareness.FoV = CameraOverrideData.ConeAngle;
				myActor.awareness.airborne = CameraOverrideData.UseAirborneCover;
				taskSecurityCamera.SweepSpeed = CameraOverrideData.Speed;
				taskSecurityCamera.PauseTimeLeft = CameraOverrideData.PauseTimeLeft;
				taskSecurityCamera.PauseTimeRight = CameraOverrideData.PauseTimeRight;
				taskSecurityCamera.CreateScanEffect();
			}
		}
		mInternalForward = base.gameObject.transform.forward;
		ActorGenerator.SetupCharacterLighting(myActor);
	}

	protected override void Update()
	{
		base.Update();
		TaskSentryGun taskSentryGun = (TaskSentryGun)myActor.tasks.GetRunningTask(typeof(TaskSentryGun));
		if (taskSentryGun != null)
		{
			if (SentryModel != null)
			{
				Quaternion rotation = Quaternion.LookRotation(taskSentryGun.LookDirection);
				Vector3 eulerAngles = new Vector3(rotation.eulerAngles.x + 270f, rotation.eulerAngles.y, 0f);
				rotation.eulerAngles = eulerAngles;
				SentryModel.rotation = rotation;
			}
			if (myActor.awareness.faction != mPreviousFaction)
			{
				myActor.awareness.FoV = 360f;
				mPreviousFaction = myActor.awareness.faction;
			}
			Visualise();
		}
	}

	protected override void SetupAnimationHandles()
	{
	}

	public void SetInternalFacing(Vector3 facing)
	{
		mInternalForward = facing;
	}

	public Vector3 GetInternalForward()
	{
		return mInternalForward;
	}

	public void Visualise()
	{
	}
}
