using System;
using UnityEngine;

[Serializable]
public class LightingGroup
{
	public Transform Anchor;

	public Transform[] Roots;

	public UnityEngine.Object Override;
}
