using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TweenTimeList
{
	public List<Vector2> span;

	public float Get(float time, float length)
	{
		if (span.Count == 0)
		{
			return -1f;
		}
		if (span.Count == 1)
		{
			return span[0].y;
		}
		int index = -1;
		int i;
		for (i = 0; i < span.Count && !(span[i].x > time); i++)
		{
			index = i;
		}
		if (i == 0)
		{
			return span[0].y;
		}
		if (i == span.Count)
		{
			return span[index].y;
		}
		return Mathf.Repeat(span[index].y + (span[i].y - span[index].y) * (time - span[index].x) / (span[i].x - span[index].x), length);
	}
}
