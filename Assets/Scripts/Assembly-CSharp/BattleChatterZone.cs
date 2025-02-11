using UnityEngine;

public class BattleChatterZone : KillZone
{
	private int mNumSquadInside;

	public void Awake()
	{
	}

	public void OnDestroy()
	{
	}

	public void Update()
	{
	}

	public override void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null) && !Contains(component))
		{
			m_ActorsWithin.Add(component);
			if (component.behaviour.PlayerControlled)
			{
				mNumSquadInside++;
				InformSpeechIfAnyBattleChatterZoneActive();
			}
		}
	}

	public override void OnTriggerExit(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (!(component == null) && Contains(component))
		{
			m_ActorsWithin.Remove(component);
			if (component.behaviour.PlayerControlled)
			{
				mNumSquadInside--;
				InformSpeechIfAnyBattleChatterZoneActive();
			}
		}
	}

	private void InformSpeechIfAnyBattleChatterZoneActive()
	{
	}
}
