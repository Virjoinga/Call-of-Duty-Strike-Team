using UnityEngine;

public static class TBFiTweenExtensions
{
	public static void PunchScale(this GameObject go, Vector3 amount, float time, float delay, string onCompleteMethod, GameObject onCompleteTarget)
	{
		iTween.PunchScale(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "oncomplete", onCompleteMethod, "oncompletetarget", onCompleteTarget));
	}

	public static void ScaleTo(this GameObject go, Vector3 scale, float time, float delay, EaseType easeType, string onCompleteMethod, GameObject onCompleteTarget)
	{
		iTween.ScaleTo(go, iTween.Hash("scale", scale, "time", time, "delay", delay, "easeType", easeType.ToString(), "oncomplete", onCompleteMethod, "oncompletetarget", onCompleteTarget));
	}

	public static void RotateTo(this GameObject go, Vector3 rotation, float time, float delay, EaseType easeType, string onCompleteMethod, GameObject onCompleteTarget)
	{
		iTween.RotateTo(go, iTween.Hash("rotation", rotation, "time", time, "delay", delay, "easeType", easeType.ToString(), "oncomplete", onCompleteMethod, "oncompletetarget", onCompleteTarget));
	}

	public static void ColorTo(this GameObject go, Color color, float time, float delay, EaseType easeType, string onCompleteMethod, GameObject onCompleteTarget)
	{
		iTween.ColorTo(go, iTween.Hash("color", color, "time", time, "delay", delay, "easeType", easeType.ToString(), "oncomplete", onCompleteMethod, "oncompletetarget", onCompleteTarget));
	}

	public static void ColorFrom_FrontEndButton(this GameObject go, Color color, float time)
	{
		iTween.ColorFrom(go, iTween.Hash("color", color, "time", time, "includechildren", false));
	}

	public static void CheckedScaleTo(this GameObject go, Vector3 scale, float time, float delay)
	{
		if (go.transform.localScale != scale)
		{
			iTween.ScaleTo(go, iTween.Hash("scale", scale, "time", time, "delay", delay));
		}
	}
}
