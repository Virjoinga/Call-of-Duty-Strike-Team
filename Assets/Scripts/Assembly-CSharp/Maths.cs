using System;
using UnityEngine;

public class Maths
{
	public static int Modulus(int a, int b)
	{
		return (Mathf.Abs(a * b) + a) % b;
	}

	public static Vector3 GetPointAlongSegment(Vector3 a, Vector3 b, float distance)
	{
		Vector3 vector = b - a;
		vector.Normalize();
		return a + vector * distance;
	}

	public static float DistanceAlongLineSegment(Vector3 point, Vector3 segmentA, Vector3 segmentB)
	{
		Vector3 rhs = segmentB - segmentA;
		Vector3 lhs = point - segmentA;
		return Vector3.Dot(lhs, rhs) / rhs.magnitude;
	}

	public static float DistanceToLineSegment(Vector3 point, Vector3 segmentA, Vector3 segmentB)
	{
		Vector3 vector = segmentB - segmentA;
		Vector3 lhs = point - segmentA;
		float sqrMagnitude = vector.sqrMagnitude;
		if ((double)sqrMagnitude == 0.0)
		{
			return lhs.magnitude;
		}
		float num = Vector3.Dot(lhs, vector) / sqrMagnitude;
		if ((double)num < 0.0)
		{
			return lhs.magnitude;
		}
		if ((double)num > 1.0)
		{
			return (point - segmentB).magnitude;
		}
		Vector3 vector2 = segmentA + num * vector;
		return (point - vector2).magnitude;
	}

	public static Vector3 ClosestPointOnLineSegment(Vector3 point, Vector3 segmentA, Vector3 segmentB)
	{
		Vector3 vector = segmentB - segmentA;
		Vector3 lhs = point - segmentA;
		float sqrMagnitude = vector.sqrMagnitude;
		if ((double)sqrMagnitude == 0.0)
		{
			return segmentA;
		}
		float num = Vector3.Dot(lhs, vector) / sqrMagnitude;
		if ((double)num < 0.0)
		{
			return segmentA;
		}
		if ((double)num > 1.0)
		{
			return segmentB;
		}
		return segmentA + num * vector;
	}

	public static Vector2 RotateAround(Vector2 point, Vector2 origin, float angleInDegs)
	{
		float num = Mathf.Cos(angleInDegs * ((float)Math.PI / 180f));
		float num2 = Mathf.Sin(angleInDegs * ((float)Math.PI / 180f));
		return new Vector2(num * (point.x - origin.x) - num2 * (point.y - origin.y) + origin.x, num2 * (point.x - origin.x) + num * (point.y - origin.y) + origin.y);
	}
}
