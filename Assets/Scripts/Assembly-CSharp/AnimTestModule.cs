using System.Collections.Generic;
using UnityEngine;

public class AnimTestModule : MonoBehaviour
{
	public AnimDirector Director;

	public List<ActionTest> MovementActions;

	public float AnimSpeed;

	public bool AnimTimeOverride;

	public float AnimTime;

	public float AimAmount;

	public bool Shooting;

	public bool Crouch;

	public bool Run;

	public bool Point;

	public bool SetPiece;

	public bool Reload;

	private bool LastReload;

	private int MoveCategoryHandle;

	private int AimCategoryHandle;

	private int ShootingCategoryHandle;

	private int CrouchOverrideHandle;

	private int RunOverrideHandle;

	private int PointOverrideHandle;

	private int SetPieceCategoryHandle;

	private float LastEventTime;

	private int CurrentTestIndex;

	private void Start()
	{
		MoveCategoryHandle = Director.GetCategoryHandle("Movement");
		if (MovementActions != null)
		{
			foreach (ActionTest movementAction in MovementActions)
			{
				movementAction.SetHandle(Director.GetActionIndex(MoveCategoryHandle, movementAction.Name));
			}
		}
		LastEventTime = Time.time;
		CurrentTestIndex = 0;
		AnimSpeed = 1f;
		LastReload = (Reload = false);
		AimCategoryHandle = Director.GetCategoryHandle("Aiming");
		Director.PlayAction(AimCategoryHandle, 0);
		Director.SetCategorySpeed(AimCategoryHandle, 0f);
		ShootingCategoryHandle = Director.GetCategoryHandle("Shooting");
		Director.PlayAction(ShootingCategoryHandle, 0);
		Director.SetCategorySpeed(ShootingCategoryHandle, 1f);
		CrouchOverrideHandle = Director.GetOverrideHandle("Crouch");
		RunOverrideHandle = Director.GetOverrideHandle("Run");
		PointOverrideHandle = Director.GetOverrideHandle("Point");
	}

	private void Update()
	{
		if (CurrentTestIndex < MovementActions.Count && Time.time - LastEventTime > MovementActions[CurrentTestIndex].Duration)
		{
			Director.PlayAction(MoveCategoryHandle, MovementActions[CurrentTestIndex].GetHandle());
			LastEventTime = Time.time;
			CurrentTestIndex++;
			if (CurrentTestIndex >= MovementActions.Count)
			{
				CurrentTestIndex = 0;
			}
		}
		Director.EnableOverride(CrouchOverrideHandle, Crouch);
		Director.EnableOverride(RunOverrideHandle, Run);
		if (Point)
		{
			Director.EnableOverride(PointOverrideHandle, Point);
		}
		else
		{
			Director.EnableOverride(PointOverrideHandle, Point);
		}
		if (AnimTimeOverride)
		{
			Director.SetCategoryTime(MoveCategoryHandle, AnimTime);
		}
		if (AimAmount < 0f)
		{
			AimAmount = 0f;
		}
		if (AimAmount > 1f)
		{
			AimAmount = 1f;
		}
		if (LastReload != Reload)
		{
			if (Reload)
			{
				Director.EnableCategory(ShootingCategoryHandle, true, 0.25f);
				Director.PlayAction(ShootingCategoryHandle, 1);
			}
			else
			{
				Director.EnableCategory(ShootingCategoryHandle, false, 0.25f);
			}
			LastReload = Reload;
		}
		Director.PlayAction(AimCategoryHandle, 0);
		Director.SetCategoryTime(AimCategoryHandle, AimAmount);
		Director.SetCategorySpeed(MoveCategoryHandle, AnimSpeed);
	}
}
