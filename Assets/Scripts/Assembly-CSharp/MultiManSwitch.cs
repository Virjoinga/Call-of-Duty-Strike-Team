using UnityEngine;

public class MultiManSwitch : MonoBehaviour
{
	public MultiManSwitchData m_Interface;

	private int activeCount;

	private float switchActiveTime;

	private void Start()
	{
		activeCount = 0;
		switchActiveTime = 0f;
	}

	private void Update()
	{
		if (activeCount == m_Interface.RequiredTerminals.Count)
		{
			int num = 0;
			foreach (GameObject item in m_Interface.ObjectsToMessageOnSuccess)
			{
				string message = string.Empty;
				if (num < m_Interface.FunctionsToCallOnSuccess.Count)
				{
					message = m_Interface.FunctionsToCallOnSuccess[num];
				}
				Container.SendMessage(item, message, base.gameObject);
				num++;
			}
			Object.Destroy(this);
		}
		if (activeCount <= 0)
		{
			return;
		}
		if (switchActiveTime > 0f)
		{
			switchActiveTime -= Time.deltaTime;
			return;
		}
		switchActiveTime = 0f;
		activeCount = 0;
		foreach (GameObject requiredTerminal in m_Interface.RequiredTerminals)
		{
			HackableObject componentInChildren = requiredTerminal.GetComponentInChildren<HackableObject>();
			if (componentInChildren != null)
			{
				componentInChildren.FailHackAttempt(false, false);
			}
		}
		int num2 = 0;
		foreach (GameObject item2 in m_Interface.ObjectsToMessageOnFailure)
		{
			string message2 = string.Empty;
			if (num2 < m_Interface.FunctionsToCallOnFailure.Count)
			{
				message2 = m_Interface.FunctionsToCallOnFailure[num2];
			}
			Container.SendMessage(item2, message2, base.gameObject);
			num2++;
		}
	}

	public void Activate()
	{
		switchActiveTime = m_Interface.MaxTimeBetweenActivations;
		activeCount++;
	}
}
