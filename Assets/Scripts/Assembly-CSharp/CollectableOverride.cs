using System.Collections.Generic;
using UnityEngine;

public class CollectableOverride : ContainerOverride
{
	public CollectableData m_OverrideData = new CollectableData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
		SectionManager sectionManager = SectionManager.GetSectionManager();
		if ((bool)sectionManager)
		{
			int sectionIndex = SectionManager.GetSectionIndex();
			sectionManager.m_Sections[sectionIndex].m_IntelCount = 0;
			CollectableOverride[] array = Object.FindObjectsOfType(typeof(CollectableOverride)) as CollectableOverride[];
			if (array != null)
			{
				sectionManager.m_Sections[sectionIndex].m_IntelCount = array.Length;
			}
			MissionListings.SetIntelForMissionSection(sectionIndex, array.Length);
			sectionManager.ApplyToPrefab();
		}
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		PickUpObject pickUpObject = cont.FindComponentOfType(typeof(PickUpObject)) as PickUpObject;
		if (pickUpObject != null)
		{
			pickUpObject.m_Interface = m_OverrideData;
		}
		PickUpObject[] array = Object.FindObjectsOfType(typeof(PickUpObject)) as PickUpObject[];
		if (array == null)
		{
			return;
		}
		int num = 0;
		List<int> intelIds = GetIntelIds(array);
		PickUpObject[] array2 = array;
		foreach (PickUpObject pickUpObject2 in array2)
		{
			if (pickUpObject2.CollectionId == -1)
			{
				pickUpObject2.CollectionId = num;
			}
			int num2 = array.Length;
			while (IsThisADuplicate(pickUpObject2.CollectionId, intelIds, num))
			{
				pickUpObject2.CollectionId = num2;
				intelIds[num] = num2;
				num2++;
			}
			num++;
		}
	}

	private List<int> GetIntelIds(PickUpObject[] obj)
	{
		List<int> list = new List<int>();
		foreach (PickUpObject pickUpObject in obj)
		{
			list.Add(pickUpObject.CollectionId);
		}
		return list;
	}

	private bool IsThisADuplicate(int id, List<int> allIds, int thisIdx)
	{
		int num = 0;
		foreach (int allId in allIds)
		{
			if (thisIdx != num && allId == id)
			{
				return true;
			}
			num++;
		}
		return false;
	}
}
