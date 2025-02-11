using System;
using UnityEngine;

[RequireComponent(typeof(CMPickUp))]
public class PickUpObject : MonoBehaviour
{
	public delegate void OnPickUpEventHandler(object sender);

	public CollectableData m_Interface;

	public int CollectionId = -1;

	private bool mAlreadyPickedUp;

	[HideInInspector]
	public float PickUpRadius = 1.5f;

	public GameObject ObjectiveBlipPrefab;

	private ObjectiveBlip mBlip;

	public event OnPickUpEventHandler OnPickUp;

	private void Start()
	{
		if (!MissionListings.Instance.FlashpointMissionRunning() && PickupManager.Instance.BeenPickedUp(ActStructure.Instance.CurrentMissionID, ActStructure.Instance.CurrentMissionSection, CollectionId))
		{
			if (!m_Interface.TutorialIntel)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			mAlreadyPickedUp = true;
		}
	}

	public void AddEventHandler(OnPickUpEventHandler eventHandler)
	{
		this.OnPickUp = (OnPickUpEventHandler)Delegate.Combine(this.OnPickUp, eventHandler);
	}

	public void RemoveEventHandler(OnPickUpEventHandler eventHandler)
	{
		this.OnPickUp = (OnPickUpEventHandler)Delegate.Remove(this.OnPickUp, eventHandler);
	}

	public void Update()
	{
	}

	public void Collected()
	{
		if (this.OnPickUp != null)
		{
			this.OnPickUp(this);
		}
		if (!MissionListings.Instance.FlashpointMissionRunning() && !mAlreadyPickedUp)
		{
			EventHub.Instance.Report(new Events.IntelCollected());
		}
		mAlreadyPickedUp = true;
		CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get("S_PICKUPINTEL"));
		string message = string.Empty;
		GameObject gameObject = null;
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectsToMessageOnCollection)
		{
			if (num < m_Interface.FunctionsToCallOnCollection.Count)
			{
				message = m_Interface.FunctionsToCallOnCollection[num];
			}
			if (m_Interface.ParamToPass != null && num < m_Interface.ParamToPass.Count)
			{
				gameObject = m_Interface.ParamToPass[num];
			}
			if (gameObject != null)
			{
				Container.SendMessageWithParam(item, message, gameObject, base.gameObject);
			}
			else
			{
				Container.SendMessage(item, message, base.gameObject);
			}
			num++;
		}
		if (!MissionListings.Instance.FlashpointMissionRunning())
		{
			PickupManager.Instance.RegisterPickup(ActStructure.Instance.CurrentMissionID, ActStructure.Instance.CurrentMissionSection, CollectionId);
		}
		if ((bool)mBlip)
		{
			mBlip.SwitchOff();
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void BlipUp()
	{
		if (!mAlreadyPickedUp)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(ObjectiveBlipPrefab) as GameObject;
			mBlip = gameObject.GetComponent<ObjectiveBlip>();
			if ((bool)mBlip)
			{
				mBlip.mObjectiveText = Language.Get("S_FL_INTEL_BLIP");
				GameObject gameObject2 = new GameObject();
				gameObject2.transform.position = base.transform.position + new Vector3(0f, 0.2f, 0f);
				mBlip.Target = gameObject2.transform;
				mBlip.AllowHighlight = false;
				mBlip.ColourBlip(Color.green);
				mBlip.SwitchOn();
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(base.transform.position, PickUpRadius);
	}
}
