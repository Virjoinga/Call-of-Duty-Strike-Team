using System.Collections.Generic;
using UnityEngine;

public class FakeCharacter : BaseCharacter
{
	private void Awake()
	{
		Settings = Settings ?? new CharacterSettings();
		mWander = 0f;
		mWanderRate = 0.1f;
		mRootBone = null;
		mNextFrameNavMesh = false;
		mForceOffscreen = false;
		lastPosition = base.transform.position;
		SetUpTransForm();
	}

	private void Start()
	{
		myActor.OnScreen = false;
		SetupAnimationHandles();
		new TaskRoutine(myActor.tasks, TaskManager.Priority.LONG_TERM, Task.Config.Default, new List<TaskDescriptor>(), true);
		mIdleTimer = 0f;
		mAimBone = myActor.model.transform.FindInHierarchy("Bone027");
		mImposedLookDirectionValid = LookType.None;
		mIsMoving = false;
		mStance = Stance.Standing;
		mMovementStyle = MovementStyle.Walk;
		mCarried = null;
		myActor.awareness.LookDirection = new Vector3(1f, 0f, 0f);
		CachePositionGproj();
	}

	private void Update()
	{
		NavMeshAgent component = GetComponent<NavMeshAgent>();
		component.enabled = mNextFrameNavMesh;
		if (component.isOnOffMeshLink)
		{
			component.CompleteOffMeshLink();
		}
		Vector3 newVel = base.transform.position - lastPosition;
		if (component != null && component.isOnOffMeshLink)
		{
			newVel.Set(0f, 0f, 0f);
		}
		newVel.y = 0f;
		Vector3 destination = component.destination;
		float magnitude = newVel.magnitude;
		string newStateStr = "Walk";
		if (magnitude <= 0.2f * Time.deltaTime)
		{
			newStateStr = "Stand";
		}
		if (!IsUsingNavMesh())
		{
			newStateStr = "Puppet";
		}
		Vector3 vector = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(base.transform.position);
		if (CameraManager.Instance.ActiveCamera != CameraManager.ActiveCameraType.StrategyCamera && (GameController.Instance.IsFirstPerson || (vector.x >= -0.05f && vector.x <= 1.05f && vector.y >= -0.1f && vector.y <= 1f)))
		{
			myActor.OnScreen = !mForceOffscreen;
		}
		else
		{
			myActor.OnScreen = false;
		}
		bool expensiveTick = (Time.frameCount & 1) == (roundRobinIndex & 1);
		myActor.Pose.UpdatePose(destination, base.transform.position, newVel, myActor.awareness.LookDirection, ref newStateStr, expensiveTick);
		if (!IsUsingNavMesh() && !IsBeingMovedManually())
		{
			myActor.Pose.Puppet();
		}
		else
		{
			myActor.Pose.PostModuleUpdate();
		}
		if (mCarried != null)
		{
			mCarried.SetPosition(myActor.GetPosition());
			mCarried.transform.forward = myActor.model.transform.forward;
			mCarried.realCharacter.myActor.awareness.LookDirection = myActor.model.transform.forward;
		}
		lastPosition = base.transform.position;
		forcedCrouch = false;
		UpdateWeapons();
	}
}
