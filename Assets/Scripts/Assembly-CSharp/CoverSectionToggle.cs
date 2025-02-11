using System.Collections.Generic;
using UnityEngine;

public class CoverSectionToggle : MonoBehaviour
{
	public List<string> turnOnNames;

	public List<string> turnOffNames;

	private List<CoverSection> turnOn;

	private List<CoverSection> turnOff;

	private void Awake()
	{
		CoverSection[] array = Object.FindObjectsOfType(typeof(CoverSection)) as CoverSection[];
		turnOn = new List<CoverSection>();
		turnOff = new List<CoverSection>();
		CoverSection[] array2 = array;
		foreach (CoverSection coverSection in array2)
		{
			if (turnOnNames.Contains(coverSection.gameObject.name))
			{
				turnOn.Add(coverSection);
			}
			if (turnOffNames.Contains(coverSection.gameObject.name))
			{
				turnOff.Add(coverSection);
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (component == null || !component.behaviour.PlayerControlled)
		{
			return;
		}
		foreach (CoverSection item in turnOff)
		{
			item.DisableCover();
		}
		foreach (CoverSection item2 in turnOn)
		{
			item2.EnableCover();
		}
	}
}
