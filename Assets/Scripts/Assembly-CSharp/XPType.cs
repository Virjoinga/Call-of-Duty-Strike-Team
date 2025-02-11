using System;
using UnityEngine;

[Serializable]
public class XPType
{
	public string Identifier;

	public string Message;

	[SerializeField]
	private int XP;

	public int GetXP()
	{
		return XP;
	}
}
