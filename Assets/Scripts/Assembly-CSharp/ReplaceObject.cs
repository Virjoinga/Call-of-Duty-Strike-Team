using System;
using UnityEngine;

[Serializable]
public class ReplaceObject
{
	public GameObject m_Source;

	public GameObject m_TargetBones;

	public GameObject m_Replace;

	[HideInInspector]
	public GameObject m_PreviousReplacement;

	[HideInInspector]
	public GameObject m_ReplacedWith;
}
