using System.Collections.Generic;
using UnityEngine;

public class DropZoneController : MonoBehaviour
{
	private static DropZoneController instance;

	public List<DropZoneArea> DropZoneAreas = new List<DropZoneArea>();

	public static DropZoneController Instance()
	{
		return instance;
	}

	public void Awake()
	{
		Object[] array = Object.FindObjectsOfType(typeof(DropZoneArea));
		for (int i = 0; i < array.Length; i++)
		{
			DropZoneArea item = (DropZoneArea)array[i];
			DropZoneAreas.Add(item);
		}
		instance = this;
	}

	public void ActivateDropZoneAreas(bool bActivate)
	{
		for (int i = 0; i < DropZoneAreas.Count; i++)
		{
			DropZoneAreas[i].ActivateArea(bActivate);
		}
	}
}
