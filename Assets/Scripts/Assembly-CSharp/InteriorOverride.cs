using System;
using System.Collections.Generic;
using UnityEngine;

public class InteriorOverride : ContainerOverride
{
	public List<ActiveObject> m_ActiveDoors = new List<ActiveObject>();

	public List<ActiveObject> m_ActiveWindows = new List<ActiveObject>();

	public List<ContainerReference> m_HideObjects = new List<ContainerReference>();

	public List<GameObject> m_HideGameObjects = new List<GameObject>();

	public List<GameObject> m_InteriorCameras = new List<GameObject>();

	[NonSerialized]
	public bool m_ShowExterior = true;

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		GameObject bagGameObject = cont.GetBagGameObject();
		GameObject gameObject = null;
		if ((bool)bagGameObject)
		{
			InteriorBagProcess interiorBagProcess = bagGameObject.GetComponentInChildren(typeof(InteriorBagProcess)) as InteriorBagProcess;
			if (interiorBagProcess != null)
			{
				gameObject = interiorBagProcess.m_ExteriorModel.gameObject;
			}
		}
		Component[] componentsInChildren = gameObject.GetComponentsInChildren(typeof(Renderer));
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Renderer renderer = (Renderer)componentsInChildren[i];
			renderer.enabled = true;
		}
		foreach (ContainerReference hideObject in m_HideObjects)
		{
			Component[] componentsInChildren2 = hideObject.GetObject(cont.gameObject).GetComponentsInChildren(typeof(Renderer));
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				Renderer renderer2 = (Renderer)componentsInChildren2[j];
				renderer2.enabled = true;
			}
		}
		foreach (GameObject hideGameObject in m_HideGameObjects)
		{
			Component[] componentsInChildren3 = hideGameObject.GetComponentsInChildren(typeof(Renderer));
			for (int k = 0; k < componentsInChildren3.Length; k++)
			{
				Renderer renderer3 = (Renderer)componentsInChildren3[k];
				renderer3.enabled = true;
			}
		}
		Component[] componentsInChildren4 = cont.GetComponentsInChildren(typeof(BuildingDoor), true);
		for (int l = 0; l < componentsInChildren4.Length; l++)
		{
			BuildingDoor buildingDoor = componentsInChildren4[l] as BuildingDoor;
			if (m_ActiveDoors.Count <= l)
			{
				ActiveObject activeObject = new ActiveObject();
				activeObject.m_Object = buildingDoor.gameObject;
				activeObject.m_Enabled = true;
				m_ActiveDoors.Add(activeObject);
			}
			else
			{
				ActiveObject activeObject2 = m_ActiveDoors[l];
				activeObject2.m_Object = buildingDoor.gameObject;
			}
			m_ActiveDoors[l].m_Object.SetActive(m_ActiveDoors[l].m_Enabled);
		}
		Component[] componentsInChildren5 = cont.GetComponentsInChildren(typeof(BuildingWindow), true);
		for (int m = 0; m < componentsInChildren5.Length; m++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren5[m].gameObject);
		}
		Component[] componentsInChildren6 = cont.GetComponentsInChildren(typeof(CMWindow), true);
		for (int n = 0; n < componentsInChildren6.Length; n++)
		{
			UnityEngine.Object.DestroyImmediate(componentsInChildren6[n].gameObject);
		}
		m_ActiveWindows.Clear();
		BuildingWithInterior buildingWithInterior = cont.GetComponentInChildren(typeof(BuildingWithInterior)) as BuildingWithInterior;
		if (buildingWithInterior != null)
		{
			List<GameObject> list = new List<GameObject>();
			foreach (ContainerReference hideObject2 in m_HideObjects)
			{
				GameObject @object = hideObject2.GetObject(base.gameObject);
				if (@object != null)
				{
					list.Add(@object);
				}
			}
			list.AddRange(m_HideGameObjects);
			buildingWithInterior.m_Interface.ObjectsToHide = list.ToArray();
			List<CMWindow> list2 = new List<CMWindow>();
			for (int num = 0; num < componentsInChildren5.Length; num++)
			{
				if (componentsInChildren5[num] != null)
				{
					CMWindow component = componentsInChildren5[num].gameObject.GetComponent<CMWindow>();
					if (component != null)
					{
						list2.Add(component);
					}
				}
			}
			buildingWithInterior.m_Interface.Windows = list2.ToArray();
			foreach (GameObject interiorCamera in m_InteriorCameras)
			{
				EnemySecurityCameraOverride componentInChildren = interiorCamera.GetComponentInChildren<EnemySecurityCameraOverride>();
				if (componentInChildren != null)
				{
					ActorWrapper componentInChildren2 = interiorCamera.GetComponentInChildren<ActorWrapper>();
					buildingWithInterior.m_Interface.SecurityCameras.Add(componentInChildren2);
				}
			}
			BuildInternalObjectsList(cont, componentsInChildren4);
		}
		else
		{
			Debug.Log("Couldn't find the interior of the building in container " + cont.name);
		}
	}

	public void ToggleExteriorVisibility()
	{
		m_ShowExterior = !m_ShowExterior;
		SetExteriorVisibility(m_ShowExterior);
	}

	public void SetExteriorVisibility(bool bVisible)
	{
	}

	private void BuildInternalObjectsList(Container cont, Component[] doorsToExclude)
	{
		MeshCollider meshCollider = FindCollider(cont.transform);
		if (meshCollider != null)
		{
			BuildingWithInterior interior = cont.GetComponentInChildren(typeof(BuildingWithInterior)) as BuildingWithInterior;
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeof(InterfaceableObject));
			Debug.Log("InteriorOverride: Found this many interfaceable objects " + array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				InterfaceableObject interfaceableObject = array[i] as InterfaceableObject;
				if (IsObjectInside(meshCollider, interfaceableObject.gameObject))
				{
					SetAsInternal(interfaceableObject, interior);
				}
			}
		}
		else
		{
			Debug.Log("Not found a collider on the interior (tag: int_VOL)");
		}
	}

	private void SetAsInternal(InterfaceableObject io, BuildingWithInterior interior)
	{
		io.Interior = interior;
		Debug.Log(string.Concat("InteriorOverride: Setting ", io, "'s interior to ", interior));
	}

	private MeshCollider FindCollider(Transform t)
	{
		if (t.name.EndsWith("_int_vol", StringComparison.OrdinalIgnoreCase))
		{
			return t.gameObject.GetComponentInChildren<MeshCollider>();
		}
		if (t.name.EndsWith("_geom_vol", StringComparison.OrdinalIgnoreCase))
		{
			return t.gameObject.GetComponentInChildren<MeshCollider>();
		}
		foreach (Transform item in t.transform)
		{
			MeshCollider meshCollider = FindCollider(item);
			if (meshCollider != null)
			{
				return meshCollider;
			}
		}
		return null;
	}

	private bool IsObjectInside(MeshCollider interiorVolume, GameObject ob)
	{
		if (ob != null && interiorVolume != null)
		{
			Ray ray = new Ray(ob.transform.position + Vector3.up * 3f, Vector3.down);
			RaycastHit hitInfo;
			if (interiorVolume.Raycast(ray, out hitInfo, 20f))
			{
				BuildingDoor componentInChildren = ob.GetComponentInChildren<BuildingDoor>();
				if ((bool)componentInChildren && !componentInChildren.m_Interface.IsInterior)
				{
					return false;
				}
				Debug.Log(string.Concat("InteriorOverride: ", ob, " raycast successful with ", interiorVolume.gameObject));
				return true;
			}
		}
		return false;
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		string[] array = methodName.Split(' ');
		if (array.Length > 1)
		{
			BuildingDoor[] componentsInChildren = gameObj.GetComponentsInChildren<BuildingDoor>();
			if (componentsInChildren.Length != 0 && int.Parse(array[1]) < componentsInChildren.Length)
			{
				componentsInChildren[int.Parse(array[1])].SendMessage(array[0]);
			}
		}
	}
}
