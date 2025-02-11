using System;
using UnityEngine;

[Serializable]
public class TimedFxDestroy : MonoBehaviour
{
	public GameObject pfx;

	public float time;

	public virtual void Update()
	{
		UnityEngine.Object.Destroy(pfx, time);
	}

	public virtual void Main()
	{
	}
}
