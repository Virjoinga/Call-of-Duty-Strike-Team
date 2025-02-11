using UnityEngine;

public class PathSmoothing
{
	private static float SMOOTHING_RADIUS = 3f;

	public static Vector2 GetPointOnCurve(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
	{
		Vector2 m = (0.5f * (p1 - p0 + (p2 - p1))).normalized * SMOOTHING_RADIUS;
		Vector2 m2 = (0.5f * (p2 - p1 + (p3 - p2))).normalized * SMOOTHING_RADIUS;
		return GetPointOnCurveFromControl(p1, p2, m, m2, t);
	}

	public static Vector2 GetPointOnCurve(Vector2 p0, Vector2 p1, Vector2 p2, float t, bool isStartProvided)
	{
		if (isStartProvided)
		{
			Vector2 m = (0.5f * (p1 - p0 + (p2 - p1))).normalized * SMOOTHING_RADIUS;
			Vector2 m2 = (1f * (p2 - p1)).normalized * SMOOTHING_RADIUS;
			return GetPointOnCurveFromControl(p1, p2, m, m2, t);
		}
		Vector2 m3 = (1f * (p1 - p0)).normalized * SMOOTHING_RADIUS;
		Vector2 m4 = (0.5f * (p1 - p0 + (p2 - p1))).normalized * SMOOTHING_RADIUS;
		return GetPointOnCurveFromControl(p0, p1, m3, m4, t);
	}

	public static Vector2 GetPointOnCurve(Vector2 p1, Vector2 p2, float t)
	{
		Vector2 m = (1f * (p2 - p1)).normalized * SMOOTHING_RADIUS;
		Vector2 m2 = (1f * (p2 - p1)).normalized * SMOOTHING_RADIUS;
		return GetPointOnCurveFromControl(p1, p2, m, m2, t);
	}

	private static Vector2 GetPointOnCurveFromControl(Vector2 p1, Vector2 p2, Vector2 m0, Vector2 m1, float t)
	{
		float num = 2f * t * t * t - 3f * t * t + 1f;
		float num2 = t * t * t - 2f * t * t + t;
		float num3 = -2f * t * t * t + 3f * t * t;
		float num4 = t * t * t - t * t;
		return num * p1 + num2 * m0 + num3 * p2 + num4 * m1;
	}
}
