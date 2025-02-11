using System;
using UnityEngine;

[Serializable]
public class AttachObject
{
	public GameObject m_Source;

	public GameObject m_AttachTo;

	[HideInInspector]
	public GameObject m_PreviousAttachment;
}
