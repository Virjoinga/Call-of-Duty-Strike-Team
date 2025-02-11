using System.Diagnostics;
using UnityEngine;

public class DrawTransformHierarchy : MonoBehaviour
{
	private void OnDrawGizmos()
	{
	}

	[Conditional("UNITY_EDITOR")]
	private void Draw(Transform t)
	{
	}
}
