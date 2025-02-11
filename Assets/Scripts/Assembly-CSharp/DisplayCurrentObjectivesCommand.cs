using System.Collections;
using UnityEngine;

public class DisplayCurrentObjectivesCommand : Command
{
	public override bool Blocking()
	{
		return false;
	}

	public override IEnumerator Execute()
	{
		ObjectiveManager om2 = null;
		om2 = ((!(GlobalObjectiveManager.Instance != null)) ? (Object.FindObjectOfType(typeof(ObjectiveManager)) as ObjectiveManager) : GlobalObjectiveManager.Instance.CurrentObjectiveManager);
		if (!(om2 != null))
		{
			yield break;
		}
		string[] objectives = om2.GetVisibleActiveObjectiveStrings();
		int objIdx = 0;
		if (objectives != null && objectives.Length > 0)
		{
			while (objIdx < objectives.Length)
			{
				string translated = AutoLocalize.Get(objectives[objIdx]);
				CommonHudController.Instance.AddToMessageLog(translated);
				objIdx++;
				yield return new WaitForSeconds(0.5f);
			}
		}
	}
}
