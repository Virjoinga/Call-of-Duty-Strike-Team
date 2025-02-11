using System.Collections.Generic;
using UnityEngine;

public class HiddenObjectsManager : MonoBehaviour
{
	public List<GameObject> HiddenObjects = new List<GameObject>();

	public List<Renderer> HiddenRenderers = new List<Renderer>();

	public List<Terrain> HiddenTerrains = new List<Terrain>();

	public HashSet<GameObject> HiddenObjectsSet
	{
		get
		{
			return new HashSet<GameObject>(HiddenObjects);
		}
		set
		{
			HiddenObjects = new List<GameObject>(value);
		}
	}

	public HashSet<Renderer> HiddenRenderersSet
	{
		get
		{
			return new HashSet<Renderer>(HiddenRenderers);
		}
		set
		{
			HiddenRenderers = new List<Renderer>(value);
		}
	}

	public HashSet<Terrain> HiddenTerrainsSet
	{
		get
		{
			return new HashSet<Terrain>(HiddenTerrains);
		}
		set
		{
			HiddenTerrains = new List<Terrain>(value);
		}
	}

	public static HiddenObjectsManager Instance
	{
		get
		{
			HiddenObjectsManager hiddenObjectsManager = (HiddenObjectsManager)Object.FindObjectOfType(typeof(HiddenObjectsManager));
			if ((bool)hiddenObjectsManager)
			{
				return hiddenObjectsManager;
			}
			GameObject gameObject = new GameObject("HiddenObjectsManager");
			gameObject.hideFlags = HideFlags.HideInHierarchy;
			return gameObject.AddComponent<HiddenObjectsManager>();
		}
	}

	public bool IsThereHiddenObjects
	{
		get
		{
			return HiddenObjects.Count != 0;
		}
	}

	public void Start()
	{
		ShowAll();
	}

	public void ShowAll()
	{
		foreach (Renderer hiddenRenderer in HiddenRenderers)
		{
			if ((bool)hiddenRenderer)
			{
				hiddenRenderer.enabled = true;
			}
		}
		foreach (Terrain hiddenTerrain in HiddenTerrains)
		{
			if ((bool)hiddenTerrain)
			{
				hiddenTerrain.enabled = true;
			}
		}
		foreach (GameObject hiddenObject in HiddenObjects)
		{
			if ((bool)hiddenObject)
			{
				hiddenObject.hideFlags = HideFlags.None;
				hiddenObject.SetActive(!hiddenObject.activeSelf);
				hiddenObject.SetActive(!hiddenObject.activeSelf);
			}
		}
		Object.DestroyImmediate(base.gameObject);
	}
}
