using UnityEngine;

public class HackObjective : MissionObjective
{
	public HackObjectiveData m_hackInterface;

	[HideInInspector]
	public GameObject AssociatedHackableTerminal;

	public override void Start()
	{
		base.Start();
		HackableObject component = AssociatedHackableTerminal.GetComponent<HackableObject>();
		if (component != null)
		{
			component.AssociatedHackObjective = this;
			base.gameObject.transform.position = component.gameObject.transform.position;
			UpdateBlipTarget();
		}
		UpdateBlipTarget();
	}

	public void OnSuccessfullHack()
	{
		bool flag = true;
		HackableObject component = AssociatedHackableTerminal.GetComponent<HackableObject>();
		if (component != null && component.HackState != HackableObject.State.HackSucessful)
		{
			flag = false;
		}
		if (flag)
		{
			Pass();
		}
	}
}
