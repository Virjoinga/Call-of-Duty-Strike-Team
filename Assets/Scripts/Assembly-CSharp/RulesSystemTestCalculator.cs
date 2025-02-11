using System.Collections.Generic;
using UnityEngine;

public class RulesSystemTestCalculator : MonoBehaviour
{
	public RulesSystemInterface Attacker;

	public RulesSystemInterface Target;

	public int HitCount;

	public int MissCount;

	public List<float> DamageList = new List<float>();

	public float AverageDamage;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void ResolveProjectileAttacks(int count)
	{
		HitCount = (MissCount = 0);
		DamageList.Clear();
		for (int i = 0; i < count; i++)
		{
			if (RulesSystem.HasHitTargetWithProjectile(Attacker.ProjectileAttackInterface, Target.ProjectileDefenceInterface))
			{
				HitCount++;
				float item = RulesSystem.CalculateProjectileDamage(Attacker.ProjectileAttackDamageInterface, Target.ProjectileDamageDefenceInterface);
				DamageList.Add(item);
			}
			else
			{
				MissCount++;
			}
		}
		AverageDamage = 0f;
		if (DamageList.Count <= 0)
		{
			return;
		}
		foreach (float damage in DamageList)
		{
			float num = damage;
			AverageDamage += num;
		}
		AverageDamage /= DamageList.Count;
	}
}
