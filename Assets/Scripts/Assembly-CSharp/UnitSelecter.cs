using System.Collections.Generic;
using UnityEngine;

public class UnitSelecter : MonoBehaviour
{
	public UnitSelectButton[] Units;

	private bool[] mUnitButtonInUse;

	private List<KeyValuePair<Actor, int>> mBufferedStartupActorList = new List<KeyValuePair<Actor, int>>();

	private void Start()
	{
		mUnitButtonInUse = new bool[Units.Length];
		for (int i = 0; i < mUnitButtonInUse.Length; i++)
		{
			Units[i].gameObject.BroadcastMessage("Hide", true, SendMessageOptions.DontRequireReceiver);
			mUnitButtonInUse[i] = false;
		}
		if (mBufferedStartupActorList == null)
		{
			return;
		}
		foreach (KeyValuePair<Actor, int> mBufferedStartupActor in mBufferedStartupActorList)
		{
			AddUnit(mBufferedStartupActor.Key, mBufferedStartupActor.Value);
		}
		mBufferedStartupActorList = null;
	}

	private void Update()
	{
		for (int i = 0; i < mUnitButtonInUse.Length; i++)
		{
			if (mUnitButtonInUse[i])
			{
				Units[i].UpdateForUnit();
			}
		}
	}

	public void AddUnit(Actor actor, int index)
	{
		if (index >= 4)
		{
			return;
		}
		if (mUnitButtonInUse == null)
		{
			mBufferedStartupActorList.Add(new KeyValuePair<Actor, int>(actor, index));
			return;
		}
		for (int i = 0; i < mUnitButtonInUse.Length; i++)
		{
			if (!mUnitButtonInUse[i])
			{
				Units[i].gameObject.BroadcastMessage("Hide", false, SendMessageOptions.DontRequireReceiver);
				Units[i].SetupForUnit(actor, index);
				mUnitButtonInUse[i] = true;
				return;
			}
			if (Units[i].MyActor == actor)
			{
				return;
			}
		}
		Debug.LogError("run ouf of unit select buttons?");
	}

	public void HideUnitSelecters(bool hide)
	{
		if (mUnitButtonInUse == null)
		{
			return;
		}
		for (int i = 0; i < mUnitButtonInUse.Length; i++)
		{
			if (mUnitButtonInUse[i])
			{
				Units[i].gameObject.BroadcastMessage("Hide", hide, SendMessageOptions.DontRequireReceiver);
				Units[i].UpdateForUnit();
			}
		}
	}

	public void DisableUnitCameo(string name)
	{
		if (mUnitButtonInUse == null)
		{
			return;
		}
		for (int i = 0; i < mUnitButtonInUse.Length; i++)
		{
			if (mUnitButtonInUse[i] && Units[i].UnitName.Text == name)
			{
				Units[i].gameObject.SetActive(false);
				mUnitButtonInUse[i] = false;
				break;
			}
		}
	}

	public void ReEnableUnitCameo(string name)
	{
		if (mUnitButtonInUse == null)
		{
			return;
		}
		for (int i = 0; i < mUnitButtonInUse.Length; i++)
		{
			if (!mUnitButtonInUse[i] && Units[i] != null && Units[i].UnitName.Text == name)
			{
				Units[i].gameObject.SetActive(true);
				Units[i].gameObject.BroadcastMessage("Hide", false, SendMessageOptions.DontRequireReceiver);
				Units[i].UpdateForUnit();
				mUnitButtonInUse[i] = true;
				break;
			}
		}
	}
}
