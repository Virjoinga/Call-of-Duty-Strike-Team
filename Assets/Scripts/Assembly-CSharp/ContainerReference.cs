using System;
using UnityEngine;

[Serializable]
public class ContainerReference
{
	public Container m_Container;

	public string m_Guid;

	public GameObject GetObject(GameObject parent)
	{
		if (m_Container != null)
		{
			m_Guid = m_Container.m_Guid;
			return m_Container.gameObject;
		}
		if (m_Guid != string.Empty)
		{
			UnityEngine.Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(Container));
			for (int i = 0; i < array.Length; i++)
			{
				Container container = (Container)array[i];
				if (container.m_Guid == m_Guid)
				{
					m_Container = container;
					return container.gameObject;
				}
			}
		}
		return null;
	}

	public SetPieceLogic GetInternalSetPieceLogic(GameObject parent)
	{
		GameObject @object = GetObject(parent);
		if (@object != null)
		{
			return @object.GetComponentInChildren(typeof(SetPieceLogic)) as SetPieceLogic;
		}
		return null;
	}
}
