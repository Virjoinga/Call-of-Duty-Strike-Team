using System;
using UnityEngine;

public class MathUtils
{
	public static float Towards(float _value, float _target, float _maxDelta)
	{
		float num = _value;
		float num2 = _target - num;
		float num3 = Mathf.Abs(num2);
		if (num3 <= _maxDelta)
		{
			return _target;
		}
		if (num2 > 0f)
		{
			return num + _maxDelta;
		}
		return num - _maxDelta;
	}

	public static float CosineLerp(float _a, float _b, float _lerp)
	{
		float f = _lerp * (float)Math.PI;
		float num = (1f - Mathf.Cos(f)) * 0.5f;
		return _a * (1f - num) + _b * num;
	}
}
