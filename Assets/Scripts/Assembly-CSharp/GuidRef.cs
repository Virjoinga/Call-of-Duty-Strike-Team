using System;
using UnityEngine;

[Serializable]
public class GuidRef
{
	[SerializeField]
	private GameObject m_Object;

	public string m_ObjectGuid;

	public GameObject theObject
	{
		get
		{
			return m_Object;
		}
		set
		{
			m_Object = value;
			if (value != null && !Application.isPlaying)
			{
				m_ObjectGuid = Container.GetGUIDFromGameObject(m_Object);
				if (m_ObjectGuid == string.Empty)
				{
					m_Object = null;
				}
			}
			else
			{
				m_ObjectGuid = string.Empty;
			}
		}
	}

	public void SetObjectWithGuid(GameObject go)
	{
		m_ObjectGuid = Container.GetGUIDFromGameObject(go);
		if (m_ObjectGuid != string.Empty)
		{
			m_Object = go;
		}
	}

	public void ResolveLink()
	{
		if (m_Object != null)
		{
			if (!Application.isPlaying)
			{
				m_ObjectGuid = Container.GetGUIDFromGameObject(m_Object);
			}
			return;
		}
		GameObject gameObjectFromGuid = Container.GetGameObjectFromGuid(m_ObjectGuid);
		if (gameObjectFromGuid != null)
		{
			m_Object = gameObjectFromGuid;
		}
	}

	public T GetComponent<T>() where T : Component
	{
		if (m_Object == null)
		{
			return (T)null;
		}
		return m_Object.GetComponent<T>();
	}

	public static implicit operator GameObject(GuidRef gr)
	{
		return gr.m_Object;
	}
}
