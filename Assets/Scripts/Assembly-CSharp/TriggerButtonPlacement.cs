using UnityEngine;

public class TriggerButtonPlacement : MonoBehaviour
{
	private void Start()
	{
		Reposition();
	}

	private void Update()
	{
		Reposition();
	}

	private void Reposition()
	{
		GameSettings instance = GameSettings.Instance;
		if (instance != null)
		{
			if (CommonHudController.Instance != null)
			{
				CommonHudController.Instance.TriggerButton.transform.localPosition = new Vector3(instance.PlayerGameSettings().FirstPersonMovableFireButtonXPos, instance.PlayerGameSettings().FirstPersonMovableFireButtonYPos, 0f);
			}
			if (StrategyHudController.Instance != null)
			{
				StrategyHudController.Instance.TriggerButton.transform.localPosition = new Vector3(instance.PlayerGameSettings().FirstPersonMovableFireButtonXPos, instance.PlayerGameSettings().FirstPersonMovableFireButtonYPos, 0f);
			}
		}
	}
}
