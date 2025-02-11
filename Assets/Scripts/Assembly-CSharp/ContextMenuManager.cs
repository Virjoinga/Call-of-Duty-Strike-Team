using System;
using UnityEngine;

public class ContextMenuManager : MonoBehaviour
{
	[Serializable]
	public class InterfaceableObjSettings
	{
		public InterfaceableObject IObj;

		public bool EnabledAtStart;
	}

	public InterfaceableObjSettings[] ContextObjectsSettings;

	private void Start()
	{
		InterfaceableObjSettings[] contextObjectsSettings = ContextObjectsSettings;
		foreach (InterfaceableObjSettings interfaceableObjSettings in contextObjectsSettings)
		{
			if (interfaceableObjSettings.IObj != null)
			{
				interfaceableObjSettings.IObj.enabled = interfaceableObjSettings.EnabledAtStart;
			}
		}
	}
}
