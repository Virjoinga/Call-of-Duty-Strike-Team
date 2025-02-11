using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UnitsOfInterestTriggerPoint : MonoBehaviour
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

	public Collider TriggerVolume;

	public int NumberInVolumeToTrigger = 1;

	public GameObject[] ObjectsToPassUnitsTo;

	public ActorWrapper[] Actors;

	public bool DestroyOnPass;

	private UnitData[] mUnitsToCheck;

	public void Start()
	{
		if (TriggerVolume == null)
		{
			TriggerVolume = base.gameObject.GetComponent<Collider>();
		}
		mUnitsToCheck = new UnitData[Actors.Length];
		ActorWrapper[] actors = Actors;
		foreach (ActorWrapper actorWrapper in actors)
		{
			actorWrapper.TargetActorChanged += Setup;
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

	private void OnDestroy()
	{
		ActorWrapper[] actors = Actors;
		foreach (ActorWrapper actorWrapper in actors)
		{
			actorWrapper.TargetActorChanged -= Setup;
		}
	}

	private void PassCheck()
	{
		int num = 0;
		List<GameObject> list = new List<GameObject>();
		UnitData[] array = mUnitsToCheck;
		foreach (UnitData unitData in array)
		{
			if (unitData != null && unitData.mIsInArea && unitData.IsAlive())
			{
				num++;
				list.Add(unitData.mUnit);
			}
		}
		if (num < NumberInVolumeToTrigger)
		{
			return;
		}
		GameObject[] objectsToPassUnitsTo = ObjectsToPassUnitsTo;
		foreach (GameObject gameObject in objectsToPassUnitsTo)
		{
			MissionObjective component = gameObject.GetComponent<MissionObjective>();
			if (component != null)
			{
				component.SetupFromSetPieceOverride(list.ToArray());
			}
		}
		if (DestroyOnPass)
		{
			Object.Destroy(base.gameObject);
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
}
