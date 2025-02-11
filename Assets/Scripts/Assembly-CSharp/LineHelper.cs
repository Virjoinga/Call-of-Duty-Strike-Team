using System.Collections.Generic;
using UnityEngine;

public static class LineHelper
{
	private static float SphereRadius = 0.25f;

	private static float drawOffsetOnY = 1f;

	public static void DrawLine(LineDetail startPoint, LineDetail endPoint, string label)
	{
		if (endPoint.trans == null || startPoint.trans == null)
		{
			return;
		}
		Vector3 vector = endPoint.trans.position - startPoint.trans.position;
		Vector3 normalized = vector.normalized;
		float magnitude = vector.magnitude;
		Vector3 position = endPoint.trans.position;
		position.y += drawOffsetOnY;
		Vector3 position2 = startPoint.trans.position;
		position2.y += drawOffsetOnY;
		Gizmos.DrawLine(position2, position);
		Vector3 position3 = position2;
		for (int i = 0; i < 3; i++)
		{
			position3 += normalized * (magnitude * 0.25f);
			if (endPoint.flag == LineFlag.In)
			{
				DrawArrow(position3, -normalized);
			}
			else
			{
				DrawArrow(position3, normalized);
			}
		}
		Gizmos.DrawSphere(position, SphereRadius);
		if (label != null)
		{
			Gizmos.DrawIcon(position, label, true);
		}
	}

	public static void DrawAsConnectedLines(List<LineDetail> points, bool loop)
	{
		for (int i = 0; i < points.Count - 1; i++)
		{
			Gizmos.color = points[i + 1].color;
			DrawLine(points[i], points[i + 1], null);
		}
		if (loop)
		{
			DrawLine(points[points.Count - 1], points[0], null);
		}
	}

	public static void DrawAsSplayedLines(List<LineDetail> points)
	{
		for (int i = 0; i < points.Count - 1; i++)
		{
			Gizmos.color = points[i + 1].color;
			DrawLine(points[0], points[i + 1], (i + 1).ToString());
		}
	}

	private static void DrawArrow(Vector3 position, Vector3 forward)
	{
		Quaternion quaternion = Quaternion.AngleAxis(45f, Vector3.up);
		Vector3 to = position - quaternion * forward * 0.3f;
		Gizmos.DrawLine(position, to);
		Quaternion quaternion2 = Quaternion.AngleAxis(-45f, Vector3.up);
		Vector3 to2 = position - quaternion2 * forward * 0.3f;
		Gizmos.DrawLine(position, to2);
	}
}
