using System.Collections.Generic;
using UnityEngine;

public class UnitsNotInVolumeObjective : MissionObjective
{
	private class UnitData
	{
		public GameObject mUnit;

		public HealthComponent mHealth;

		public bool mIsInArea;

		public UnitData(GameObject go)
		{
			mUnit = go;
			mHealth = mUnit.GetComponent<HealthComponent>();
			mIsInArea = false;
		}

		public bool IsAlive()
		{
			if (mHealth == null)
			{
				return true;
			}
			if (!mHealth.HealthEmpty)
			{
				return true;
			}
			return false;
		}
	}

	public UnitsNotInVolData m_UnitsVolInterface;

	[HideInInspector]
	public List<ActorWrapper> Actors;

	private UnitData[] mUnitsToCheck;

	public override void Start()
	{
		base.Start();
		if (m_UnitsVolInterface.TriggerVolume == null)
		{
			m_UnitsVolInterface.TriggerVolume = base.gameObject.GetComponent<Collider>();
		}
		mUnitsToCheck = new UnitData[Actors.Count];
		foreach (ActorWrapper actor in Actors)
		{
			actor.TargetActorChanged += Setup;
		}
	}

	private void Setup(object sender)
	{
		GameObject gameObject = sender as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		for (int i = 0; i < mUnitsToCheck.Length; i++)
		{
			if (mUnitsToCheck[i] == null)
			{
				mUnitsToCheck[i] = new UnitData(gameObject);
				break;
			}
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (ActorWrapper actor in Actors)
		{
			actor.TargetActorChanged -= Setup;
		}
	}

	private void PassCheck()
	{
		int num = 0;
		int num2 = 0;
		UnitData[] array = mUnitsToCheck;
		foreach (UnitData unitData in array)
		{
			if (unitData != null)
			{
				if (unitData.mIsInArea)
				{
					num++;
				}
				if (unitData.IsAlive())
				{
					num2++;
				}
			}
		}
		switch (m_UnitsVolInterface.Check)
		{
		case UnitsInVolumeObjective.TriggerCheck.Any:
			if (num < mUnitsToCheck.Length)
			{
				Pass();
			}
			break;
		case UnitsInVolumeObjective.TriggerCheck.All_Alive:
			if (num == 0)
			{
				Pass();
			}
			break;
		case UnitsInVolumeObjective.TriggerCheck.Specific_Number:
			if (mUnitsToCheck.Length - num >= m_UnitsVolInterface.SpecificNumber)
			{
				Pass();
			}
			if (m_UnitsVolInterface.SpecificNumber > num2)
			{
				Fail();
			}
			break;
		default:
			TBFAssert.DoAssert(false, "unknown check type");
			break;
		}
	}

	private UnitData IsUnitOfInterest(GameObject unitToCheck)
	{
		UnitData[] array = mUnitsToCheck;
		foreach (UnitData unitData in array)
		{
			if (unitData != null && unitData.mUnit.Equals(unitToCheck))
			{
				return unitData;
			}
		}
		return null;
	}

	public void OnTriggerEnter(Collider other)
	{
		UnitData unitData = IsUnitOfInterest(other.gameObject);
		if (unitData != null)
		{
			unitData.mIsInArea = true;
			PassCheck();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		UnitData unitData = IsUnitOfInterest(other.gameObject);
		if (unitData != null)
		{
			unitData.mIsInArea = false;
			PassCheck();
		}
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.blue.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
