using UnityEngine;

public class StatsTestBed : MonoBehaviour
{
	public ActorDescriptor[] m_EnemyActors = new ActorDescriptor[2];

	public WeaponDescriptor[] m_EnemyWeapons = new WeaponDescriptor[2];

	public ActorDescriptor[] m_PlayerActors = new ActorDescriptor[4];

	public WeaponDescriptor[] m_PlayerWeapons = new WeaponDescriptor[4];

	private DummyCharacter[] m_PlayerCharacters = new DummyCharacter[4];

	private DummyCharacter[] m_EnemyCharacters = new DummyCharacter[2];

	private string m_FirstPersonId;

	private DummyCharacter CreateDummyCharacter(ActorDescriptor act, WeaponDescriptor weapon, bool isPlayer)
	{
		GameObject gameObject = new GameObject();
		Actor actor = gameObject.AddComponent<Actor>();
		DummyCharacter dummyCharacter = gameObject.AddComponent<DummyCharacter>().Connect(actor) as DummyCharacter;
		dummyCharacter.Id = act.name;
		actor.weapon = gameObject.AddComponent<PlayerWeapon>();
		PlayerWeapon weapon2 = actor.weapon;
		object primaryWeapon;
		if (weapon != null)
		{
			IWeapon weapon3 = weapon.Create(actor.model, 1f, 1f, 1f);
			primaryWeapon = weapon3;
		}
		else
		{
			primaryWeapon = new Weapon_Null();
		}
		object secondaryWeapon;
		if (weapon != null)
		{
			IWeapon weapon3 = weapon.Create(actor.model, 1f, 1f, 1f);
			secondaryWeapon = weapon3;
		}
		else
		{
			secondaryWeapon = new Weapon_Null();
		}
		weapon2.Initialise((IWeapon)primaryWeapon, (IWeapon)secondaryWeapon);
		actor.health = gameObject.AddComponent<HealthComponent>();
		return dummyCharacter;
	}

	public void OnStartTest()
	{
		for (int i = 0; i < m_EnemyActors.Length; i++)
		{
			m_EnemyCharacters[i] = CreateDummyCharacter(m_EnemyActors[i], m_EnemyWeapons[i], false);
		}
		for (int j = 0; j < m_PlayerActors.Length; j++)
		{
			m_PlayerCharacters[j] = CreateDummyCharacter(m_PlayerActors[j], m_PlayerWeapons[j], true);
		}
		MissionListings.eMissionID id = MissionListings.eMissionID.MI_MISSION_ARCTIC;
		DifficultyMode difficultyMode = DifficultyMode.Regular;
		if (Random.Range(0, 2) == 1)
		{
			id = MissionListings.eMissionID.MI_MISSION_AFGHANISTAN;
			difficultyMode = DifficultyMode.Veteran;
		}
		bool flag = false;
		MissionListings.eMissionID id2;
		if (StatsManager.Instance.MissionStats().IsMissionInProgress(out id2))
		{
			flag = true;
			id = id2;
		}
		else
		{
			StatsManager.Instance.BeginSession();
			EventHub.Instance.Report(new Events.StartMission(id, 0, difficultyMode));
		}
		int num = Random.Range(4, 20);
		m_FirstPersonId = m_PlayerCharacters[Random.Range(0, m_PlayerCharacters.Length)].Id;
		for (int k = 0; k < num; k++)
		{
			CreateRandomShot();
		}
		bool success = Random.Range(0, 2) == 0;
		EventHub.Instance.Report(new Events.EndMission(id, 0, difficultyMode, success, false, 0f));
		StatsManager.Instance.EndSession();
		for (int l = 0; l < m_EnemyActors.Length; l++)
		{
			Object.DestroyImmediate(m_EnemyCharacters[l]);
		}
		for (int m = 0; m < m_PlayerActors.Length; m++)
		{
			Object.DestroyImmediate(m_PlayerCharacters[m]);
		}
		if (flag)
		{
			StatsManager.Instance.BeginSession();
			EventHub.Instance.Report(new Events.StartMission(id, 0, difficultyMode));
		}
	}

	public void CreateRandomShot()
	{
		DummyCharacter dummyCharacter = null;
		DummyCharacter dummyCharacter2 = null;
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool longRange = false;
		int num = Random.Range(0, m_EnemyActors.Length);
		int num2 = Random.Range(0, m_PlayerActors.Length);
		if (Random.Range(0, 5) == 0)
		{
			dummyCharacter = m_EnemyCharacters[num];
			dummyCharacter2 = m_PlayerCharacters[num2];
		}
		else
		{
			dummyCharacter = m_PlayerCharacters[num2];
			dummyCharacter2 = m_EnemyCharacters[num];
		}
		dummyCharacter.SetFirstPerson(m_FirstPersonId == dummyCharacter.Id);
		dummyCharacter2.SetFirstPerson(m_FirstPersonId == dummyCharacter2.Id);
		TBFAssert.DoAssert(dummyCharacter2.Id != dummyCharacter.Id, "Actors can't shoot themselves: " + dummyCharacter2.Id);
		flag = Random.Range(0, 2) == 0;
		if (flag)
		{
			flag2 = Random.Range(0, 6) == 0;
			if (flag2)
			{
				flag3 = Random.Range(0, 5) == 0;
				longRange = Random.Range(0, 5) == 0;
			}
		}
		EventHub.Instance.Report(new Events.WeaponFired(dummyCharacter.myActor.EventActor(), dummyCharacter2.myActor.EventActor(), flag));
		if (flag2)
		{
			EventHub.Instance.Report(new Events.Kill(dummyCharacter.myActor.EventActor(), dummyCharacter2.myActor.EventActor(), "shot", flag3, flag3, longRange));
		}
	}
}
