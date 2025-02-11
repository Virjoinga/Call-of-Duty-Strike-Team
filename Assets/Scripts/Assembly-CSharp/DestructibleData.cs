using System;
using UnityEngine;

[Serializable]
public class DestructibleData
{
	public bool Invincible;

	public GameObject ObjectToCallOnDestruction;

	public string FunctionToCallOnDestruction;

	public bool DontDoAOEDamage;

	public FactionHelper.Category DamagedBy;
}
