using System.Collections;
using UnityEngine;

public class SiloGrillGate : MonoBehaviour
{
	public GateGrillData m_Interface;

	public Animation Target;

	public string ClipName;

	public GameObject NavGate;

	public GameObject StatusLight;

	private void Start()
	{
		if (!m_Interface.UseStatusLight)
		{
			StatusLight.SetActive(false);
		}
		SetGateState(m_Interface.StartOpen, 100f);
	}

	private void Update()
	{
	}

	public void Activate()
	{
		SetGateState(true, 1f);
	}

	public void Deactivate()
	{
		SetGateState(false, 1f);
	}

	public void SetGateState(bool open, float speed)
	{
		if (!open)
		{
			Target[ClipName].time = Target[ClipName].length;
			Target[ClipName].speed = 0f - speed;
			Target.Play(ClipName);
			Container.SendMessage(NavGate, "Deactivate");
			if (m_Interface.UseStatusLight)
			{
				Container.SendMessage(StatusLight, "Activate");
			}
		}
		else
		{
			Target[ClipName].speed = speed;
			Target.Play(ClipName);
			StartCoroutine(DelayNavGateOpening(2f));
			if (m_Interface.UseStatusLight)
			{
				Container.SendMessage(StatusLight, "Deactivate");
			}
		}
	}

	private IEnumerator DelayNavGateOpening(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		Container.SendMessage(NavGate, "Activate");
	}
}
