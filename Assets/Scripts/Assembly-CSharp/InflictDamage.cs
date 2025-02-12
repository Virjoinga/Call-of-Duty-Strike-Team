using System.Collections.Generic;
using UnityEngine;

public class InflictDamage : MonoBehaviour
{
	public InflictDamageData m_Interface;

	public List<ActorWrapper> actorWrappers;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Activate()
	{
		if (m_Interface.ForceCriticalInjured)
		{
			foreach (ActorWrapper actorWrapper in actorWrappers)
			{
				actorWrapper.GetActor().health.ForceCriticallyInjured();
			}
		}
		else if (m_Interface.RandomDamageMax > 0f)
		{
			foreach (ActorWrapper actorWrapper2 in actorWrappers)
			{
				float amount = Random.Range(m_Interface.RandomDamageMin, m_Interface.RandomDamageMax);
				Vector3 direction = new Vector3(Random.Range(-1, 1), 0.5f, Random.Range(-1, 1));
				actorWrapper2.GetActor().health.ModifyHealth(null, amount, "ActOfGod", direction, false);
				if (actorWrapper2.GetActor().health.HealthEmpty && m_Interface.ReactionVars.ApplyReactionOnDeath)
				{
					DoRagdollResponse(actorWrapper2);
				}
			}
		}
		else
		{
			int num = 0;
			foreach (ActorWrapper actorWrapper3 in actorWrappers)
			{
				float num2 = ((num < m_Interface.DamageToApply.Count) ? m_Interface.DamageToApply[num] : m_Interface.DamageToApply[0]);
				actorWrapper3.GetActor().health.ModifyHealth(null, 0f - num2, "ActOfGod", Vector3.one, false);
				if (actorWrapper3.GetActor().health.HealthEmpty && m_Interface.ReactionVars.ApplyReactionOnDeath)
				{
					DoRagdollResponse(actorWrapper3);
				}
				num++;
			}
		}
		if (m_Interface.OneShot)
		{
			Object.Destroy(this);
		}
	}

	public void DoRagdollResponse(ActorWrapper aw)
	{
		HitLocation[] bones = aw.GetActor().realCharacter.Ragdoll.Bones;
		ReactionModifier.RandomPreset randomReaction = m_Interface.ReactionVars.RandomReaction;
		Transform transform = aw.GetActor().transform;
		Vector3 vector = Vector3.zero;
		float explosionForce = 0f;
		float explosionRadius = 14f;
		float upwardsModifier = 0f;
		switch (randomReaction)
		{
		case ReactionModifier.RandomPreset.None:
			vector = transform.forward * m_Interface.ReactionVars.ForwardModifier;
			vector += transform.right * m_Interface.ReactionVars.RightModifier;
			explosionForce = m_Interface.ReactionVars.ImpactForce;
			upwardsModifier = m_Interface.ReactionVars.Upforce;
			break;
		case ReactionModifier.RandomPreset.FallForward:
			vector = transform.forward * Random.Range(0.5f, 1f);
			vector += transform.right * Random.Range(-0.5f, 0.5f);
			m_Interface.ReactionVars.ImpactHeight = Random.Range(2.3f, 3.4f);
			explosionForce = Random.Range(35.5f, 42.5f);
			upwardsModifier = Random.Range(0.5f, 1.5f);
			break;
		case ReactionModifier.RandomPreset.ShotInChestFront:
			vector = transform.forward * Random.Range(-1f, -0.5f);
			vector += transform.right * Random.Range(-0.5f, 0.5f);
			m_Interface.ReactionVars.ImpactHeight = Random.Range(2.3f, 3.4f);
			explosionForce = Random.Range(35.5f, 42.5f);
			upwardsModifier = Random.Range(0.5f, 1.5f);
			break;
		case ReactionModifier.RandomPreset.Grenade:
			vector = transform.forward * Random.Range(-1f, 1f);
			vector += transform.right * Random.Range(-1f, 1f);
			m_Interface.ReactionVars.ImpactHeight = Random.Range(0.1f, 0.3f);
			explosionForce = Random.Range(35.5f, 62.5f);
			upwardsModifier = Random.Range(20.5f, 45.5f);
			break;
		case ReactionModifier.RandomPreset.LargeBlast:
			vector = transform.forward * Random.Range(-1f, 1f);
			vector += transform.right * Random.Range(-1f, 1f);
			m_Interface.ReactionVars.ImpactHeight = Random.Range(0.1f, 0.3f);
			explosionForce = Random.Range(65.5f, 92.5f);
			upwardsModifier = Random.Range(45.5f, 75.5f);
			break;
		}
		Vector3 explosionPosition = transform.position - vector + new Vector3(0f, m_Interface.ReactionVars.ImpactHeight, 0f);
		HitLocation[] array = bones;
		foreach (HitLocation hitLocation in array)
		{
			if (!hitLocation.GetComponent<Rigidbody>().isKinematic)
			{
				hitLocation.GetComponent<Rigidbody>().AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, ForceMode.Impulse);
			}
		}
	}

	public void Deactivate()
	{
		Object.Destroy(this);
	}

	public void Enable()
	{
		Enable();
	}

	public void Disable()
	{
		Disable();
	}
}
