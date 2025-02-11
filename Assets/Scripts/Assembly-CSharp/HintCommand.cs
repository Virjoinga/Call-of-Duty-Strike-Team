using System.Collections;
using UnityEngine;

public class HintCommand : Command
{
	public HintMessageBox.ImageLayout Layout;

	public string HintTitleKey;

	public string HintTextKey;

	public string NonRetinaResourceFilename;

	public GameObject OnOkCallbackScript;

	public string OnOkCallbackMethod;

	public bool IsSettingsHint;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		MessageBoxController msgs = MessageBoxController.Instance;
		if (msgs != null)
		{
			if (IsSettingsHint)
			{
				if (SystemInfo.supportsGyroscope)
				{
					msgs.DoOptionsHintDialogue(HintTitleKey, HintTextKey, Layout, NonRetinaResourceFilename, this, "EnableGyro", "DisableGyro");
				}
			}
			else
			{
				msgs.DoHintDialogue(HintTitleKey, HintTextKey, Layout, NonRetinaResourceFilename, OnOkCallbackScript, OnOkCallbackMethod);
			}
			while (msgs.IsAnyMessageActive)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		yield return null;
	}

	private void EnableGyro()
	{
		if (!GameSettings.Instance.PlayerGameSettings().FirstPersonGyroscopeEnabled)
		{
			GameSettings.Instance.PlayerGameSettings().FirstPersonGyroscopeEnabled = true;
			SwrveEventsUI.GyroscopeTurnedOn();
		}
	}

	private void DisableGyro()
	{
		if (GameSettings.Instance.PlayerGameSettings().FirstPersonGyroscopeEnabled)
		{
			GameSettings.Instance.PlayerGameSettings().FirstPersonGyroscopeEnabled = false;
			SwrveEventsUI.GyroscopeTurnedOff();
		}
	}
}
