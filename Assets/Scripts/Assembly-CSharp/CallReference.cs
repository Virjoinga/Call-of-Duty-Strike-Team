using System;
using UnityEngine;

[Serializable]
public class CallReference
{
	public GameObject m_CallObject;

	public string m_ExposedLink;

	public string m_MethodToCall;

	public GameObject GetCallingObject()
	{
		if (m_CallObject != null)
		{
			Container container = m_CallObject.GetComponent(typeof(Container)) as Container;
			if (container != null && container.ReferenceId != string.Empty)
			{
				return ContainerLinks.GetLink(container.gameObject, m_ExposedLink) as GameObject;
			}
		}
		return m_CallObject;
	}
}
