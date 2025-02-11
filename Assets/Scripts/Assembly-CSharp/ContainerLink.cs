using System;
using UnityEngine;

[Serializable]
public class ContainerLink
{
	public GameObject Source;

	public string ReferenceId = string.Empty;

	public Container.ContainerType Type;
}
