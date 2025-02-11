using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DiscoverableObject : MonoBehaviour
{
	private class UnitData
	{
		public GameObject mUnit;

		public Actor unitActor;

		public bool mIsInArea;

		public UnitData(GameObject go)
		{
			mUnit = go;
			unitActor = mUnit.GetComponent<Actor>();
			mIsInArea = false;
		}

		public bool IsAlive()
		{
			if (unitActor.health == null)
			{
				return true;
			}
			if (!unitActor.health.HealthEmpty)
			{
				return true;
			}
			return false;
		}
	}

	public delegate void OnDiscoveredEventHandler(object sender);

	public Collider TriggerVolume;

	public Collider RayCastVolume;

	public ActorWrapper[] Actors;

	private UnitData[] mUnitsToCheck;

	public bool NoRaycast;

	public float RaycastRadius = 1.5f;

	private int mViewModelMask;

	public event OnDiscoveredEventHandler OnDiscovered;

	public void AddEventHandler(OnDiscoveredEventHandler eventHandler)
	{
		this.OnDiscovered = (OnDiscoveredEventHandler)Delegate.Combine(this.OnDiscovered, eventHandler);
	}

	public void RemoveEventHandler(OnDiscoveredEventHandler eventHandler)
	{
		this.OnDiscovered = (OnDiscoveredEventHandler)Delegate.Remove(this.OnDiscovered, eventHandler);
	}

	public void Start()
	{
		if (TriggerVolume == null)
		{
			TriggerVolume = base.gameObject.GetComponent<Collider>();
		}
		if (RayCastVolume == null)
		{
			RayCastVolume = base.gameObject.GetComponent<Collider>();
		}
		mUnitsToCheck = new UnitData[Actors.Length];
		ActorWrapper[] actors = Actors;
		foreach (ActorWrapper actorWrapper in actors)
		{
			actorWrapper.TargetActorChanged += Setup;
		}
		if (RayCastVolume != null)
		{
			mViewModelMask = 1 << RayCastVolume.gameObject.layer;
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
		UnitData[] array = mUnitsToCheck;
		foreach (UnitData unitData in array)
		{
			if (unitData != null && unitData.mIsInArea && unitData.IsAlive())
			{
				num++;
				break;
			}
		}
		if (num > 0)
		{
			Discovered();
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

	public void Update()
	{
		bool flag = false;
		if (GameController.Instance.IsFirstPerson)
		{
			flag = true;
		}
		else
		{
			for (int i = 0; i < mUnitsToCheck.Length; i++)
			{
				if (mUnitsToCheck[i] != null && mUnitsToCheck[i].unitActor.tasks.IsRunningTask(typeof(TaskPeekaboo)))
				{
					flag = true;
					break;
				}
			}
		}
		if (!NoRaycast && flag && RayCastVolume != null)
		{
			Transform transform = CameraManager.Instance.PlayCamera.transform;
			Ray ray = new Ray(transform.position, transform.forward);
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(ray, out hitInfo, 100f, mViewModelMask) && hitInfo.collider == RayCastVolume)
			{
				Discovered();
			}
		}
	}

	private void Discovered()
	{
		CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get("S_INTELDISCOVERED"));
		if (this.OnDiscovered != null)
		{
			this.OnDiscovered(this);
		}
		UnityEngine.Object.Destroy(this);
	}
}
