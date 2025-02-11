using System.Collections;
using UnityEngine;

public class MissionSelectController : MenuScreenBlade
{
	public FrontEndButton ChallengesButton;

	public GlobalConflictTracker ConflictTracker;

	public override void Awake()
	{
		base.Awake();
		ChallengesButton.gameObject.SetActive(false);
		CommonBackgroundBox componentInChildren = base.transform.GetComponentInChildren<CommonBackgroundBox>();
		componentInChildren.ForegroundHeightInUnits = 6.125f;
		CommonBackgroundBoxPlacement[] componentsInChildren = ChallengesButton.transform.parent.parent.GetComponentsInChildren<CommonBackgroundBoxPlacement>();
		CommonBackgroundBoxPlacement[] array = componentsInChildren;
		foreach (CommonBackgroundBoxPlacement commonBackgroundBoxPlacement in array)
		{
			commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight -= 0.2f;
			commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight *= 5f;
			commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight /= 4f;
			commonBackgroundBoxPlacement.StartPositionAsPercentageOfBoxHeight += 0.025f;
		}
	}

	public override void OnScreen()
	{
		base.OnScreen();
		MessageBoxController.Instance.CheckActivateDialogue();
		DailyRewards.Instance.CheckDailyReward();
	}

	protected override void OnActivate()
	{
		if (ActivateWatcher.Instance.ActivateUIOpen)
		{
			Time.timeScale = 0f;
		}
		base.OnActivate();
		StartCoroutine(InitiateSwrveTalkEvent());
	}

	protected override void OnDeactivate()
	{
		StopAllCoroutines();
		base.OnDeactivate();
		if (ConflictTracker != null && (ConflictTracker.IsActive || ConflictTracker.IsTransitioningOn))
		{
			ConflictTracker.Deactivate();
		}
	}

	private IEnumerator InitiateSwrveTalkEvent()
	{
		yield return new WaitForSeconds(1f);
		while (MessageBoxController.Instance.IsAnyMessageActive)
		{
			yield return null;
		}
		SwrveEventsUI.SwrveTalkTrigger_Globe();
	}
}
