using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuDebugController : MenuScreenBlade
{
	public ToggleEditOption DebugGodMode;

	public PercentageEditOption DebugSlowMo;

	public ToggleEditOption DebugSuperSprint;

	public ToggleEditOption BuildInfoDisplay;

	public ToggleEditOption ViewConeToggle;

	public ToggleEditOption InfiniteAmmoToggle;

	public ToggleEditOption ShowCoverPointsToggle;

	public ToggleEditOption FirstPersonPenaltyToggle;

	public ToggleEditOption BalanceTweaksToggle;

	public ToggleEditOption TapToShootToggle;

	public ToggleEditOption GhostModeToggle;

	public ToggleEditOption EnablePerkToggle;

	private HudFps mHudFpsDisplay;

	private BuildInfoDisplay mBuildInfoDisplayText;

	private List<Actor> mDebugPlayerActorList;

	public override void Activate()
	{
		base.Activate();
		mDebugPlayerActorList = new List<Actor>();
		Actor[] array = Object.FindObjectsOfType(typeof(Actor)) as Actor[];
		Actor[] array2 = array;
		foreach (Actor actor in array2)
		{
			if (actor.behaviour.PlayerControlled)
			{
				mDebugPlayerActorList.Add(actor);
			}
		}
		Refresh();
	}

	protected override void OnActivate()
	{
		base.OnActivate();
		PercentageEditOption[] componentsInChildren = GetComponentsInChildren<PercentageEditOption>();
		PercentageEditOption[] array = componentsInChildren;
		foreach (PercentageEditOption percentageEditOption in array)
		{
			percentageEditOption.Enable();
		}
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();
		mDebugPlayerActorList.Clear();
		PercentageEditOption[] componentsInChildren = GetComponentsInChildren<PercentageEditOption>();
		PercentageEditOption[] array = componentsInChildren;
		foreach (PercentageEditOption percentageEditOption in array)
		{
			percentageEditOption.Disable();
		}
	}

	private void Refresh()
	{
	}

	private void ShowDebugLog()
	{
	}

	private void ChangedDebugGodMode()
	{
	}

	private void ChangedDebugSlowMo()
	{
	}

	private void ChangedSuperSpeed()
	{
		if (DebugSuperSprint.Value)
		{
			BaseCharacter.m_sDebugGaitOverride = 8f;
		}
		else
		{
			BaseCharacter.m_sDebugGaitOverride = 0f;
		}
	}

	private void ChangedBuildInfoDisplay()
	{
	}

	private void CancelSlowMo()
	{
	}

	private void GotoNextSection()
	{
	}

	private IEnumerator Timer()
	{
		while (FrontEndController.Instance.IsBusy)
		{
			yield return null;
		}
		SectionManager sectMan = SectionManager.GetSectionManager();
		if (sectMan != null)
		{
			Spawner.ms_bDebugTeleport = true;
			sectMan.LoadNextSection(-1);
			yield return new WaitForSeconds(5f);
			sectMan.ActivateSection();
		}
	}

	private void ToggleViewCones()
	{
	}

	private void ToggleTapToShoot()
	{
	}

	private void ToggleInfiniteAmmo()
	{
	}

	private void ToggleShowCoverPoints()
	{
	}

	private void ToggleFirstPersonPenalty()
	{
	}

	private void ToggleBalanceTweaks()
	{
	}

	private bool AnyInvunerablePlayerControlledActors()
	{
		bool result = false;
		foreach (Actor mDebugPlayerActor in mDebugPlayerActorList)
		{
			if (mDebugPlayerActor.health.Invulnerable)
			{
				result = true;
			}
		}
		return result;
	}

	private void ToggleGhostMode()
	{
	}

	private void TogglePerks()
	{
	}
}
