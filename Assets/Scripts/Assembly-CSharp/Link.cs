using System;
using UnityEngine;

[Serializable]
public class Link
{
	public enum LinkType
	{
		kContainerLink = 0,
		kObjectLink = 1
	}

	public Container m_SourceContainer;

	public GameObject m_Source;

	public string m_SourceGuid = string.Empty;

	public string m_SourceName = string.Empty;

	public LinkReference m_Destination;

	public LinkType m_Type;

	public Link()
	{
	}

	public Link(GameObject gameObj)
	{
		if (gameObj.GetComponent<Container>() != null)
		{
			m_Type = LinkType.kContainerLink;
		}
	}
}
