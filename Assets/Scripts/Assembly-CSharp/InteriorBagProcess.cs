using System.Collections.Generic;
using UnityEngine;

public class InteriorBagProcess : BaseBagProcess
{
	private const int kLayer_IgnoreRayCast = 2;

	public GameObject m_InteriorBuilding;

	public Container m_InteriorModel;

	public Container m_ExteriorModel;

	[HideInInspector]
	public Container m_DoorModel;

	public List<GameObject> m_Doors;

	public List<Container> m_DoorModels;

	public override void PreProcess(GameObject obj)
	{
		m_InteriorModel = null;
		m_ExteriorModel = null;
		m_InteriorBuilding = null;
		m_Doors.Clear();
		m_DoorModels.Clear();
		bool flag = false;
		string text = "Errors hooking up:\n";
		Container[] componentsInChildren = obj.GetComponentsInChildren<Container>();
		Container container = null;
		Container container2 = null;
		Container[] array = componentsInChildren;
		foreach (Container container3 in array)
		{
			if (container3.name.ToLower().Contains("int"))
			{
				container = container3;
				if (!container3.name.ToLower().Contains("replace"))
				{
					m_InteriorModel = container;
					continue;
				}
				flag = true;
				text += " -REPLACE: Interior needs replacing \n";
			}
			else if (container3.name.ToLower().Contains("ext"))
			{
				container2 = container3;
				if (!container3.name.ToLower().Contains("replace"))
				{
					m_ExteriorModel = container2;
					continue;
				}
				flag = true;
				text += " -REPLACE: Exterior needs replacing \n";
			}
		}
		if (container == null)
		{
			flag = true;
			text += " -Interior not found - Name must contain 'int' \n";
		}
		if (container2 == null)
		{
			flag = true;
			text += " -Exterior not found - Name must contain 'ext' \n";
		}
		bool flag2 = false;
		Container[] array2 = componentsInChildren;
		foreach (Container container4 in array2)
		{
			if (!container4.name.ToLower().Contains("door"))
			{
				continue;
			}
			if (!container4.name.ToLower().Contains("replace"))
			{
				GameObject gameObject = null;
				int childCount = container4.transform.childCount;
				for (int k = 0; k < childCount; k++)
				{
					GameObject gameObject2 = container4.transform.GetChild(k).gameObject;
					if (gameObject2.name.ToLower().Contains("breachable"))
					{
						gameObject = gameObject2;
					}
				}
				if (gameObject != null)
				{
					m_DoorModels.Add(container4);
					m_Doors.Add(gameObject);
					continue;
				}
				break;
			}
			flag = true;
			flag2 = true;
			text += " -REPLACE: Door needs replacing or removing \n";
			break;
		}
		if ((m_Doors.Count == 0 || m_DoorModels.Count == 0) && !flag2)
		{
			flag = true;
			text += " -Issue with Door\n";
			text += "      Door must contain 'door' \n";
			text += "      Breachable component must contain 'breachable' \n";
		}
		GameObject gameObject3 = null;
		int childCount2 = obj.transform.childCount;
		for (int l = 0; l < childCount2; l++)
		{
			GameObject gameObject4 = obj.transform.GetChild(l).gameObject;
			if (gameObject4.name.ToLower().Contains("interior"))
			{
				gameObject3 = gameObject4;
				if (!gameObject4.name.ToLower().Contains("replace"))
				{
					m_InteriorBuilding = gameObject3;
					continue;
				}
				flag = true;
				text += " -REPLACE: Building interior needs replacing \n";
			}
		}
		if (gameObject3 == null)
		{
			flag = true;
			text += " -Interior building component not found - Name must contain 'interior' \n";
		}
		if (flag)
		{
			Debug.Log(text);
		}
	}

	public override void ApplyProcess(GameObject obj)
	{
		BuildingWithInterior buildingWithInterior = null;
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		if (m_InteriorModel != null)
		{
			gameObject = m_InteriorModel.FindObjectNamesEndsWith("_int");
			if (gameObject == null)
			{
				gameObject = m_InteriorModel.FindObjectNamesEndsWith("_int_geom");
			}
			if (gameObject == null)
			{
				Debug.Log("Coudn't find the interior model from container " + m_InteriorModel.name);
			}
			else
			{
				AddWindowAndDoorComponents(gameObject.transform);
				PairUpDoubleDoors(gameObject);
			}
		}
		if (m_ExteriorModel != null)
		{
			gameObject2 = m_ExteriorModel.FindObjectNamesEndsWith("_ext");
			if (gameObject2 == null)
			{
				gameObject2 = m_ExteriorModel.FindObjectNamesEndsWith("_ext_geom");
			}
			if (gameObject2 == null)
			{
				Debug.Log("Coudn't find the Exterior model from container " + m_ExteriorModel.name);
			}
			else
			{
				AddWindowAndDoorComponents(gameObject2.transform);
				PairUpDoubleDoors(gameObject2);
				GameObject gameObject3 = m_ExteriorModel.FindObjectNamesEndsWith(gameObject2, "_col");
				if (gameObject3 != null)
				{
					gameObject3.layer = LayerMask.NameToLayer("RoofCollider");
				}
			}
		}
		if (m_InteriorBuilding != null)
		{
			buildingWithInterior = m_InteriorBuilding.GetComponentInChildren<BuildingWithInterior>();
			if (buildingWithInterior == null)
			{
				Debug.Log("Coudn't find the interior building from container " + m_InteriorBuilding.name);
			}
		}
		if ((bool)buildingWithInterior)
		{
			buildingWithInterior.m_Interface.Exterior = gameObject2;
			buildingWithInterior.m_Interface.Interior = gameObject;
		}
		if (m_Doors != null)
		{
			for (int i = 0; i < m_Doors.Count; i++)
			{
				GameObject gameObject4 = m_Doors[i];
				Container container = ((i >= m_DoorModels.Count) ? m_DoorModel : m_DoorModels[i]);
				Component[] array = null;
				if (container != null)
				{
					MeshCollider[] componentsInChildren = container.GetComponentsInChildren<MeshCollider>();
					MeshCollider[] array2 = componentsInChildren;
					foreach (MeshCollider meshCollider in array2)
					{
						Object.DestroyImmediate(meshCollider.gameObject);
					}
					array = container.gameObject.GetComponentsInChildren(typeof(MeshFilter));
					if (array.Length == 0)
					{
						Debug.Log("Coudn't find the door mesh from container " + container.name);
					}
				}
				if (gameObject4 != null)
				{
					NavGate componentInChildren = gameObject4.GetComponentInChildren<NavGate>();
					BuildingDoor componentInChildren2 = gameObject4.GetComponentInChildren<BuildingDoor>();
					if ((bool)componentInChildren2)
					{
						componentInChildren2.mAssociatedBuilding = buildingWithInterior;
						if (array != null)
						{
							int num = 0;
							Component[] array3 = array;
							foreach (Component component in array3)
							{
								GameObject gameObject5 = component.gameObject;
								if (gameObject5.GetComponent<NavGate>() == null && !gameObject5.name.Contains("_col") && !gameObject5.name.Contains("_tact"))
								{
									switch (num)
									{
									case 0:
										componentInChildren2.DoorMesh = component as MeshFilter;
										num++;
										break;
									case 1:
										componentInChildren2.AdditionalDoorMesh = component as MeshFilter;
										num++;
										break;
									}
								}
							}
						}
						componentInChildren2.m_NavGate = componentInChildren;
					}
					else
					{
						Debug.Log("Coudn't find the door from container " + gameObject4.name);
					}
				}
				else
				{
					Debug.Log("Coudn't find door " + i + ". They need to be hooked up in the bag object on the IntioerBagProcess");
				}
			}
		}
		if (gameObject2 != null)
		{
			Component[] componentsInChildren2 = gameObject2.GetComponentsInChildren(typeof(BuildingWindow), true);
			Component[] array4 = componentsInChildren2;
			foreach (Component component2 in array4)
			{
				BuildingWindow buildingWindow = component2 as BuildingWindow;
				buildingWindow.mAssociatedBuilding = buildingWithInterior;
			}
		}
	}

	private void AddWindowAndDoorComponents(Transform t)
	{
		if (t.name.ToLowerInvariant().EndsWith("_window") && t.gameObject.GetComponent<BuildingWindow>() == null)
		{
			t.gameObject.AddComponent(typeof(BuildingWindow));
		}
		if (t.name.ToLowerInvariant().EndsWith("_doorac") && t.gameObject.GetComponentsInChildren(typeof(BuildingDoor), true) == null)
		{
			BuildingDoor buildingDoor = t.gameObject.AddComponent<BuildingDoor>();
			buildingDoor.m_Interface.OpeningDirection = BuildingDoor.DoorDirection.ACW;
		}
		if (t.name.ToLowerInvariant().EndsWith("_doorcw") && t.gameObject.GetComponentsInChildren(typeof(BuildingDoor), true) == null)
		{
			BuildingDoor buildingDoor2 = t.gameObject.AddComponent<BuildingDoor>();
			buildingDoor2.m_Interface.OpeningDirection = BuildingDoor.DoorDirection.CW;
		}
		foreach (Transform item in t)
		{
			AddWindowAndDoorComponents(item);
		}
	}

	private void PairUpDoubleDoors(GameObject go)
	{
		BuildingDoor[] componentsInChildren = go.GetComponentsInChildren<BuildingDoor>();
		BuildingDoor[] array = componentsInChildren;
		foreach (BuildingDoor buildingDoor in array)
		{
			string text = buildingDoor.name.ToLowerInvariant();
			int result = text.LastIndexOf("_g");
			if (result >= 0)
			{
				string text2 = text.Substring(result + 1);
				text2 = text2.Split('_')[0];
				text2 = text2.Replace("g", string.Empty);
				text2 = text2.Trim();
				if (!int.TryParse(text2, out result))
				{
					result = -1;
				}
			}
			if (result < 0)
			{
				continue;
			}
			BuildingDoor[] array2 = componentsInChildren;
			foreach (BuildingDoor buildingDoor2 in array2)
			{
				if (buildingDoor.Equals(buildingDoor2))
				{
					continue;
				}
				string text3 = buildingDoor2.name.ToLowerInvariant();
				int result2 = text3.LastIndexOf("_g");
				if (result2 >= 0)
				{
					string text4 = text3.Substring(result2 + 1);
					text4 = text4.Split('_')[0];
					text4 = text4.Replace("g", string.Empty);
					text4 = text4.Trim();
					if (!int.TryParse(text4, out result2))
					{
						result2 = -1;
					}
				}
				if (result == result2)
				{
					buildingDoor.SiblingDoor = buildingDoor2;
					buildingDoor2.SiblingDoor = buildingDoor;
				}
			}
		}
	}
}
