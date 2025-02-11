using System.Collections;
using UnityEngine;

public class FlashpointEndCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		GameObject mh2 = GameObject.Find("Music_Danger");
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Deactivate");
		}
		mh2 = GameObject.Find("Music_Combat");
		if ((bool)mh2)
		{
			Container.SendMessage(mh2, "Deactivate");
		}
		foreach (Grenade grenade in Grenade.GlobalPoolCache)
		{
			if (grenade != null)
			{
				grenade.MarkForDelete = true;
			}
		}
		yield return new WaitForEndOfFrame();
		CommonHudController.Instance.ShowGMGResults(true);
		if (HUDMessenger.Instance != null)
		{
			HUDMessenger.Instance.PushMessage(Language.Get("S_FL_WIN"), string.Empty, string.Empty, false);
		}
		GMGSFX.Instance.WaveComplete.Play2D();
		yield return new WaitForSeconds(2f);
		CommonHudController.Instance.ShowWave(false);
	}
}
