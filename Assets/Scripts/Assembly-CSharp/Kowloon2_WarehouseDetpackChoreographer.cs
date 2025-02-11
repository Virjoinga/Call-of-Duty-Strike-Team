using UnityEngine;

public class Kowloon2_WarehouseDetpackChoreographer : MonoBehaviour
{
	public GameObject[] DPR1;

	public GameObject[] DPR2;

	private bool DP1;

	private bool DP2;

	private int Counter;

	public GameObject ToDialogueManager;

	private void DP1Destroyed()
	{
		Counter++;
		CheckCounter();
		DP1 = true;
		if (!DP2)
		{
			GameObject[] dPR = DPR1;
			foreach (GameObject target in dPR)
			{
				Container.SendMessage(target, "Activate", base.gameObject);
				Debug.Log("Activate message sent!");
			}
		}
	}

	private void DP2Destroyed()
	{
		Counter++;
		CheckCounter();
		DP2 = true;
		if (!DP1)
		{
			GameObject[] dPR = DPR2;
			foreach (GameObject target in dPR)
			{
				Container.SendMessage(target, "Activate", base.gameObject);
				Debug.Log("Activate message sent!");
			}
		}
	}

	private void CheckCounter()
	{
		if (Counter == 2)
		{
			Container.SendMessage(ToDialogueManager, "GetClearDialogue", base.gameObject);
		}
	}
}
