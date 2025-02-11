using UnityEngine;

public class LevelLayer : MonoBehaviour
{
	public bool m_Editable = true;

	public bool m_Locked;

	public bool m_DeepUnlock;

	public bool m_DeleteOnLoad;

	public LayerItem.LayerType m_Type;

	public bool IsEditable
	{
		get
		{
			return m_Editable;
		}
		set
		{
			if (value && m_Type == LayerItem.LayerType.External_Layer)
			{
				return;
			}
			Transform[] layerContents = GetLayerContents();
			if (layerContents != null)
			{
				for (int i = 0; i < layerContents.Length; i++)
				{
					if (value)
					{
						layerContents[i].hideFlags &= ~(HideFlags.HideInHierarchy | HideFlags.NotEditable);
					}
					else
					{
						layerContents[i].hideFlags |= HideFlags.HideInHierarchy | HideFlags.NotEditable;
					}
				}
			}
			if (value)
			{
				base.gameObject.transform.hideFlags &= ~HideFlags.NotEditable;
			}
			else
			{
				base.gameObject.transform.hideFlags |= HideFlags.NotEditable;
			}
			m_Editable = value;
		}
	}

	public bool IsHidden
	{
		get
		{
			Transform[] layerContents = GetLayerContents();
			if (layerContents != null)
			{
				Transform[] array = layerContents;
				int num = 0;
				if (num < array.Length)
				{
					Transform transform = array[num];
					return !transform.gameObject.activeInHierarchy;
				}
			}
			return false;
		}
		set
		{
			Transform[] layerContents = GetLayerContents();
			if (layerContents != null)
			{
				Transform[] array = layerContents;
				foreach (Transform transform in array)
				{
					transform.gameObject.SetActive(!value);
				}
			}
		}
	}

	public bool IsDeepUnlock
	{
		get
		{
			return m_DeepUnlock;
		}
		set
		{
			Container[] componentsInChildren = base.gameObject.GetComponentsInChildren<Container>(true);
			foreach (Container container in componentsInChildren)
			{
				container.IsEditable = false;
				container.IsViewable = value;
			}
			m_DeepUnlock = value;
		}
	}

	public bool IsLocked
	{
		get
		{
			return m_Locked;
		}
		set
		{
			m_Locked = value;
		}
	}

	public bool MarkToKeep { get; set; }

	private Transform[] GetLayerContents()
	{
		if (base.transform.childCount > 0)
		{
			Transform[] array = new Transform[base.transform.childCount];
			for (int i = 0; i < base.transform.childCount; i++)
			{
				array[i] = base.transform.GetChild(i);
			}
			return array;
		}
		return null;
	}

	public void RefreshContainerGuids()
	{
		Object[] array = Object.FindObjectsOfType(typeof(Container));
		for (int i = 0; i < array.Length; i++)
		{
			Container container = (Container)array[i];
			if (container.m_Guid == string.Empty)
			{
				container.GenerateNewGuid();
			}
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
