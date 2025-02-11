using System;
using UnityEngine;

public class GuidRefHook : MonoBehaviour
{
	public string m_Guid = Guid.NewGuid().ToString();

	[NonSerialized]
	public int duplicateTestIndex = -1;

	public void GenerateNewGuid()
	{
		m_Guid = Guid.NewGuid().ToString();
	}
}
