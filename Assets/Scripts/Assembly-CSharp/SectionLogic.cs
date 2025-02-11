using System.Collections.Generic;
using UnityEngine;

public class SectionLogic : MonoBehaviour
{
	public List<SectionLogicElement> m_SectionOptions = new List<SectionLogicElement>();

	private void Awake()
	{
		SectionManager sectionManager = SectionManager.GetSectionManager();
		if (sectionManager != null)
		{
			sectionManager.RegisterSectionLogic(this);
		}
	}

	public void StartSection(int sectionIndex)
	{
		if (m_SectionOptions.Count > sectionIndex)
		{
			foreach (Transform item in base.gameObject.transform)
			{
				item.gameObject.SetActive(true);
			}
			SectionLogicElement sectionLogicElement = m_SectionOptions[sectionIndex];
			{
				foreach (GameObject disableObject in sectionLogicElement.m_DisableObjects)
				{
					disableObject.SetActive(false);
				}
				return;
			}
		}
		Debug.Log("The section logic on " + base.gameObject.name + " doesn't have any options for section index: " + sectionIndex);
	}
}
