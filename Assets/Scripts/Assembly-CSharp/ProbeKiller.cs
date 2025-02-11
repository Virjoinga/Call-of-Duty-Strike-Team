using System;
using UnityEngine;

public class ProbeKiller : MonoBehaviour
{
	public bool visible;

	public void OnDrawGizmos()
	{
		if (visible)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.green.Alpha(0.25f);
			Gizmos.DrawCube(Vector3.zero, Vector3.one);
		}
	}

	public void Execute()
	{
		LightProbeGroup[] array = (LightProbeGroup[])UnityEngine.Object.FindObjectsOfType(typeof(LightProbeGroup));
		LightProbeGroup[] array2 = array;
		foreach (LightProbeGroup lightProbeGroup in array2)
		{
			Vector3[] array3 = new Vector3[lightProbeGroup.probePositions.Length];
			int newSize = 0;
			for (int j = 0; j < lightProbeGroup.probePositions.Length; j++)
			{
				Vector3 vector = base.transform.InverseTransformPoint(lightProbeGroup.transform.TransformPoint(lightProbeGroup.probePositions[j]));
				if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
				{
					array3[newSize++] = lightProbeGroup.probePositions[j];
				}
			}
			Array.Resize(ref array3, newSize);
			lightProbeGroup.probePositions = array3;
		}
	}
}
