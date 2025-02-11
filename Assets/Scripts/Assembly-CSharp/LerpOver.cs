using System;
using UnityEngine;

[Serializable]
public class LerpOver
{
	public float inMin;

	public float inMax;

	public float outMin;

	public float outMax;

	public float Get(float input)
	{
		input = Mathf.Clamp(input, inMin, inMax);
		return outMin + (outMax - outMin) * (input - inMin) / (inMax - inMin);
	}
}
