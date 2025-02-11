using System.Collections.Generic;
using UnityEngine;

public abstract class EventDescriptor : MonoBehaviour
{
	public List<GameObject> ObjectsToCall;

	public List<string> FunctionsToCall;

	public List<GameObject> ObjectParam;

	public bool FireOnlyOnce = true;

	public GameObject BaseObject;

	public virtual void Start()
	{
		if (BaseObject != null)
		{
			Initialise(BaseObject);
		}
	}

	public virtual void FireEvent()
	{
		int num = 0;
		foreach (GameObject item in ObjectsToCall)
		{
			if (num < FunctionsToCall.Count && item != null)
			{
				if (this != null && base.gameObject != null)
				{
					if (num < ObjectParam.Count && ObjectParam[num] != null)
					{
						Container.SendMessageWithParam(item, FunctionsToCall[num], ObjectParam[num], base.gameObject);
					}
					else
					{
						Container.SendMessage(item, FunctionsToCall[num], base.gameObject);
					}
				}
				else
				{
					Container.SendMessage(item, FunctionsToCall[num]);
				}
			}
			num++;
		}
		if (FireOnlyOnce)
		{
			Object.Destroy(this);
		}
	}

	public virtual void Initialise(GameObject gameObj)
	{
	}

	public virtual void DeInitialise()
	{
	}

	public virtual void InitialiseForActor(Actor a)
	{
	}
}
