using System.Collections.Generic;
using UnityEngine;

public class CoverCluster : MonoBehaviour
{
	public enum Type
	{
		Default = 0,
		NearPlayerUnits = 1
	}

	public int[] masks;

	[SerializeField]
	private int[] listOfCoverPoints;

	public NewCoverPointManager myManager;

	public Type type;

	private void Awake()
	{
		MakeArray();
	}

	private void Update()
	{
		if (myManager == null)
		{
			TBFAssert.DoAssert(false, "Cover Cluster " + base.name + " is not associated with a CoverManager. Most likely this is because it's not in the markup layer.");
		}
		if (type == Type.Default)
		{
			base.enabled = false;
			return;
		}
		Clear();
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			CoverPointCore closestCoverPoint = a.awareness.closestCoverPoint;
			if (closestCoverPoint != null)
			{
				int num = closestCoverPoint.neighbours.Length;
				for (int i = 0; i < num; i++)
				{
					Include(CoverNeighbour.coverIndex(closestCoverPoint.neighbours[i]));
				}
			}
		}
	}

	public int[] AsArray()
	{
		if (listOfCoverPoints != null)
		{
			return listOfCoverPoints;
		}
		return new int[0];
	}

	public void Clear()
	{
		for (int i = 0; i < masks.GetLength(0); i++)
		{
			masks[i] = 0;
		}
	}

	public void Include(int index)
	{
		masks[index >> 5] |= 1 << (index & 0x1F);
	}

	public void Exclude(int index)
	{
		masks[index >> 5] &= ~(1 << (index & 0x1F));
	}

	public bool Includes(int index)
	{
		if (index < 0)
		{
			return false;
		}
		return (masks[index >> 5] & (1 << index)) != 0;
	}

	public bool Includes(CoverPointCore cpc)
	{
		int index = cpc.index;
		return (masks[index >> 5] & (1 << (index & 0x1F))) != 0;
	}

	public void RefreshContents()
	{
		int[] array = new int[(myManager.coverPoints.GetLength(0) >> 5) + 1];
		NewCoverPoint[] componentsInChildren = myManager.GetComponentsInChildren<NewCoverPoint>();
		if (masks != null)
		{
			for (int i = 0; i < masks.GetLength(0) * 32; i++)
			{
				if (!Includes(i))
				{
					continue;
				}
				for (int j = 0; j < myManager.coverPoints.GetLength(0); j++)
				{
					if (componentsInChildren[j].index == i)
					{
						array[componentsInChildren[j].core.index >> 5] |= 1 << (componentsInChildren[j].core.index & 0x1F);
					}
				}
			}
		}
		masks = array;
	}

	public CoverPointCore RandomValidCover(Actor a)
	{
		a.awareness.SiftValidCoverFromList(listOfCoverPoints);
		if (AwarenessComponent.usefulCoverCount > 0)
		{
			return AwarenessComponent.usefulCoverArray[Random.Range(0, AwarenessComponent.usefulCoverCount - 1)];
		}
		if (AwarenessComponent.validCoverCount > 0)
		{
			return AwarenessComponent.validCoverArray[Random.Range(0, AwarenessComponent.validCoverCount - 1)];
		}
		return null;
	}

	private void MakeArray()
	{
		if (myManager == null)
		{
			return;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < myManager.coverPoints.Length; i++)
		{
			if (Includes(i))
			{
				list.Add(i);
			}
		}
		listOfCoverPoints = list.ToArray();
	}

	private void OnDrawGizmos()
	{
	}
}
