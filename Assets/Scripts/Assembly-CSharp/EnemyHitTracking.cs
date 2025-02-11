using System.Collections.Generic;

public class EnemyHitTracking
{
	public class EnemyData
	{
		public bool HasHitFriendlyUnit;

		public bool HasIncapacitatedFriendlyUnit;

		public List<string> DamagedBy = new List<string>();

		public List<string> PlayerCharactersHit = new List<string>();
	}

	private Dictionary<string, EnemyData> m_EnemyData = new Dictionary<string, EnemyData>();

	public void Reset()
	{
		m_EnemyData.Clear();
	}

	public EnemyData GetEnemyData(string name)
	{
		EnemyData value = null;
		m_EnemyData.TryGetValue(name, out value);
		return value;
	}

	private EnemyData FindorAddEnemy(string name)
	{
		EnemyData value = null;
		if (!m_EnemyData.TryGetValue(name, out value))
		{
			value = new EnemyData();
			m_EnemyData.Add(name, value);
		}
		return value;
	}

	public void Update(Events.Kill args)
	{
		m_EnemyData.Remove(args.Victim.Name);
	}

	public void Update(Events.WeaponFired args)
	{
		if (args.Attacker == null || args.Victim == null)
		{
			return;
		}
		if (args.Attacker.PlayerControlled)
		{
			EnemyData enemyData = FindorAddEnemy(args.Victim.Name);
			if (enemyData != null && !enemyData.DamagedBy.Contains(args.Attacker.Id))
			{
				enemyData.DamagedBy.Add(args.Attacker.Id);
			}
		}
		else
		{
			if (!args.Victim.PlayerControlled)
			{
				return;
			}
			EnemyData enemyData2 = FindorAddEnemy(args.Attacker.Name);
			if (enemyData2 != null)
			{
				if (!enemyData2.PlayerCharactersHit.Contains(args.Victim.Id))
				{
					enemyData2.PlayerCharactersHit.Add(args.Victim.Id);
				}
				enemyData2.HasIncapacitatedFriendlyUnit = args.Victim.IsMortallyWounded || args.Victim.IsDead;
			}
		}
	}
}
