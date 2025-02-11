using UnityEngine;

public class DropZoneArea : MonoBehaviour
{
	public DropZoneData m_Interface;

	private DropZoneObject[] mDropZoneObjects;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public DropZoneObject[] DropZoneObjects
	{
		get
		{
			return mDropZoneObjects;
		}
	}

	public void ActivateArea(bool bActivate)
	{
		CMDropZoneArea component = base.gameObject.GetComponent<CMDropZoneArea>();
		component.enabled = bActivate;
		component.ShowBlip(bActivate);
	}

	public void Awake()
	{
		mDropZoneObjects = GetComponentsInChildren<DropZoneObject>();
		DropZoneObject[] array = mDropZoneObjects;
		foreach (DropZoneObject dropZoneObject in array)
		{
			dropZoneObject.gameObject.SetActive(false);
		}
	}

	public void Start()
	{
		if (!m_Interface.Locked)
		{
			CMDropZoneArea component = base.gameObject.GetComponent<CMDropZoneArea>();
			component.DropZoneBlipObj.SetUnlocked();
		}
	}

	public void Update()
	{
		if (!m_Interface.Locked)
		{
			return;
		}
		Vector3 position = base.gameObject.transform.position;
		position.y = 0f;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Vector3 position2 = a.GetPosition();
			if (Vector3.SqrMagnitude(position2 - position) < m_Interface.RadToUnlock * m_Interface.RadToUnlock)
			{
				m_Interface.Locked = false;
				CMDropZoneArea component = base.gameObject.GetComponent<CMDropZoneArea>();
				component.DropZoneBlipObj.SetUnlocked();
				if (CommonHudController.Instance != null && LoadingProgressMarker.Instance == null)
				{
					CommonHudController.Instance.AddToMessageLog(AutoLocalize.Get("S_DROPZONEACTIVATED"));
				}
				break;
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(base.transform.position, m_Interface.RadToUnlock);
	}
}
