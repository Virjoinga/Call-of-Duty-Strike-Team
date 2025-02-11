using System;
using UnityEngine;

[Serializable]
public class SegueData
{
	public Vector3 segueStartOffsetPos;

	public Quaternion segueStartOffsetRot;

	public Vector3 segueEndOffsetPos;

	public Quaternion segueEndOffsetRot;

	public Vector3 segueEndVelocity;

	[SerializeField]
	private bool valid;

	public void MakeValid()
	{
		valid = true;
	}

	public bool IsValid()
	{
		return valid;
	}
}
