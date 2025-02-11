using UnityEngine;

public static class iTweenExtensions
{
	public static void AudioFrom(this GameObject go, float volume, float pitch, float time, float delay)
	{
		iTween.AudioFrom(go, iTween.Hash("volume", volume, "pitch", pitch, "time", time, "delay", delay));
	}

	public static void AudioFrom(this GameObject go, float volume, float pitch, float time, float delay, LoopType loopType)
	{
		iTween.AudioFrom(go, iTween.Hash("volume", volume, "pitch", pitch, "time", time, "delay", delay, "looptype", loopType.ToString()));
	}

	public static void AudioTo(this GameObject go, float volume, float pitch, float time, float delay)
	{
		iTween.AudioTo(go, iTween.Hash("volume", volume, "pitch", pitch, "time", time, "delay", delay));
	}

	public static void AudioTo(this GameObject go, float volume, float pitch, float time, float delay, LoopType loopType)
	{
		iTween.AudioTo(go, iTween.Hash("volume", volume, "pitch", pitch, "time", time, "delay", delay, "looptype", loopType.ToString()));
	}

	public static void AudioUpdate(this GameObject go, float volume, float pitch, float time)
	{
		iTween.AudioUpdate(go, volume, pitch, time);
	}

	public static void ColorFrom(this GameObject go, Color color, float time, float delay)
	{
		iTween.ColorFrom(go, iTween.Hash("color", color, "time", time, "delay", delay));
	}

	public static void ColorFrom(this GameObject go, Color color, float time, float delay, LoopType loopType)
	{
		iTween.ColorFrom(go, iTween.Hash("color", color, "time", time, "delay", delay, "looptype", loopType.ToString()));
	}

	public static void ColorTo(this GameObject go, Color color, float time, float delay)
	{
		iTween.ColorTo(go, iTween.Hash("color", color, "time", time, "delay", delay));
	}

	public static void ColorTo(this GameObject go, Color color, float time, float delay, LoopType loopType)
	{
		iTween.ColorTo(go, iTween.Hash("color", color, "time", time, "delay", delay, "looptype", loopType.ToString()));
	}

	public static void ColorUpdate(this GameObject go, Color color, float time)
	{
		iTween.ColorUpdate(go, color, time);
	}

	public static void FadeFrom(this GameObject go, float alpha, float time, float delay)
	{
		iTween.FadeFrom(go, iTween.Hash("alpha", alpha, "time", time, "delay", delay));
	}

	public static void FadeFrom(this GameObject go, float alpha, float time, float delay, LoopType loopType)
	{
		iTween.FadeFrom(go, iTween.Hash("alpha", alpha, "time", time, "delay", delay, "looptype", loopType.ToString()));
	}

	public static void FadeTo(this GameObject go, float alpha, float time, float delay)
	{
		iTween.FadeTo(go, iTween.Hash("alpha", alpha, "time", time, "delay", delay));
	}

	public static void FadeTo(this GameObject go, float alpha, float time, float delay, LoopType loopType)
	{
		iTween.FadeTo(go, iTween.Hash("alpha", alpha, "time", time, "delay", delay, "looptype", loopType.ToString()));
	}

	public static void FadeUpdate(this GameObject go, float alpha, float time)
	{
		iTween.FadeUpdate(go, alpha, time);
	}

	public static void Init(this GameObject go)
	{
		iTween.Init(go);
	}

	public static void LookFrom(this GameObject go, Vector3 lookTarget, float time, float delay)
	{
		iTween.LookFrom(go, iTween.Hash("lookTarget", lookTarget, "time", time, "delay", delay));
	}

	public static void LookFrom(this GameObject go, Vector3 lookTarget, float time, float delay, EaseType easeType)
	{
		iTween.LookFrom(go, iTween.Hash("lookTarget", lookTarget, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void LookFrom(this GameObject go, Vector3 lookTarget, float time, float delay, EaseType easeType, LoopType looptype)
	{
		iTween.LookFrom(go, iTween.Hash("lookTarget", lookTarget, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", looptype.ToString()));
	}

	public static void LookTo(this GameObject go, Vector3 lookTarget, float time, float delay)
	{
		iTween.LookTo(go, iTween.Hash("lookTarget", lookTarget, "time", time, "delay", delay));
	}

	public static void LookTo(this GameObject go, Vector3 lookTarget, float time, float delay, EaseType easeType)
	{
		iTween.LookTo(go, iTween.Hash("lookTarget", lookTarget, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void LookTo(this GameObject go, Vector3 lookTarget, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.LookTo(go, iTween.Hash("lookTarget", lookTarget, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void LookUpdate(this GameObject go, Vector3 lookTarget, float time)
	{
		iTween.LookUpdate(go, lookTarget, time);
	}

	public static void MoveAdd(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.MoveAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void MoveAdd(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType)
	{
		iTween.MoveAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void MoveAdd(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.MoveAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void MoveBy(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.MoveBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void MoveBy(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType)
	{
		iTween.MoveBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void MoveBy(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.MoveBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void MoveFrom(this GameObject go, Vector3 position, float time, float delay)
	{
		iTween.MoveFrom(go, iTween.Hash("position", position, "time", time, "delay", delay));
	}

	public static void MoveFrom(this GameObject go, Vector3 position, float time, float delay, EaseType easeType)
	{
		iTween.MoveFrom(go, iTween.Hash("position", position, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void MoveFrom(this GameObject go, Vector3 position, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.MoveFrom(go, iTween.Hash("position", position, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void MoveTo(this GameObject go, Vector3 position, float time, float delay)
	{
		iTween.MoveTo(go, iTween.Hash("position", position, "time", time, "delay", delay));
	}

	public static void MoveTo(this GameObject go, Vector3[] path, float time, float delay)
	{
		iTween.MoveTo(go, iTween.Hash("path", path, "time", time, "delay", delay));
	}

	public static void MoveTo(this GameObject go, Transform[] path, float time, float delay)
	{
		iTween.MoveTo(go, iTween.Hash("path", path, "time", time, "delay", delay));
	}

	public static void MoveTo(this GameObject go, Vector3 position, float time, float delay, EaseType easeType)
	{
		iTween.MoveTo(go, iTween.Hash("position", position, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void MoveTo(this GameObject go, Vector3[] path, float time, float delay, EaseType easeType)
	{
		iTween.MoveTo(go, iTween.Hash("path", path, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void MoveTo(this GameObject go, Transform[] path, float time, float delay, EaseType easeType)
	{
		iTween.MoveTo(go, iTween.Hash("path", path, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void MoveTo(this GameObject go, Vector3 position, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.MoveTo(go, iTween.Hash("position", position, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void MoveTo(this GameObject go, Vector3[] path, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.MoveTo(go, iTween.Hash("path", path, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void MoveTo(this GameObject go, Transform[] path, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.MoveTo(go, iTween.Hash("path", path, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void MoveUpdate(this GameObject go, Vector3 position, float time)
	{
		iTween.MoveUpdate(go, position, time);
	}

	public static void PunchPosition(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.PunchPosition(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void PunchRotation(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.PunchRotation(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void PunchScale(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.PunchScale(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void PutOnPath(this GameObject go, Transform[] path, float percent)
	{
		iTween.PutOnPath(go, path, percent);
	}

	public static void PutOnPath(this GameObject go, Vector3[] path, float percent)
	{
		iTween.PutOnPath(go, path, percent);
	}

	public static void RotateAdd(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.RotateAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void RotateAdd(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType)
	{
		iTween.RotateAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void RotateAdd(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.RotateAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void RotateBy(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.RotateBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void RotateBy(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType)
	{
		iTween.RotateBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void RotateBy(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.RotateBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void RotateFrom(this GameObject go, Vector3 rotation, float time, float delay)
	{
		iTween.RotateFrom(go, iTween.Hash("rotation", rotation, "time", time, "delay", delay));
	}

	public static void RotateFrom(this GameObject go, Vector3 rotation, float time, float delay, EaseType easeType)
	{
		iTween.RotateFrom(go, iTween.Hash("rotation", rotation, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void RotateFrom(this GameObject go, Vector3 rotation, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.RotateFrom(go, iTween.Hash("rotation", rotation, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void RotateTo(this GameObject go, Vector3 rotation, float time, float delay)
	{
		iTween.RotateTo(go, iTween.Hash("rotation", rotation, "time", time, "delay", delay));
	}

	public static void RotateTo(this GameObject go, Vector3 rotation, float time, float delay, EaseType easeType)
	{
		iTween.RotateTo(go, iTween.Hash("rotation", rotation, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void RotateTo(this GameObject go, Vector3 rotation, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.RotateTo(go, iTween.Hash("rotation", rotation, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void RotateUpdate(this GameObject go, Vector3 rotation, float time)
	{
		iTween.RotateUpdate(go, rotation, time);
	}

	public static void ScaleAdd(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.ScaleAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void ScaleAdd(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType)
	{
		iTween.ScaleAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void ScaleAdd(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.ScaleAdd(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void ScaleBy(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.ScaleBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void ScaleBy(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType)
	{
		iTween.ScaleBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void ScaleBy(this GameObject go, Vector3 amount, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.ScaleBy(go, iTween.Hash("amount", amount, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void ScaleFrom(this GameObject go, Vector3 scale, float time, float delay)
	{
		iTween.ScaleFrom(go, iTween.Hash("scale", scale, "time", time, "delay", delay));
	}

	public static void ScaleFrom(this GameObject go, Vector3 scale, float time, float delay, EaseType easeType)
	{
		iTween.ScaleFrom(go, iTween.Hash("scale", scale, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void ScaleFrom(this GameObject go, Vector3 scale, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.ScaleFrom(go, iTween.Hash("scale", scale, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void ScaleTo(this GameObject go, Vector3 scale, float time, float delay)
	{
		iTween.ScaleTo(go, iTween.Hash("scale", scale, "time", time, "delay", delay));
	}

	public static void ScaleTo(this GameObject go, Vector3 scale, float time, float delay, EaseType easeType)
	{
		iTween.ScaleTo(go, iTween.Hash("scale", scale, "time", time, "delay", delay, "easeType", easeType.ToString()));
	}

	public static void ScaleTo(this GameObject go, Vector3 scale, float time, float delay, EaseType easeType, LoopType loopType)
	{
		iTween.ScaleTo(go, iTween.Hash("scale", scale, "time", time, "delay", delay, "easeType", easeType.ToString(), "looptype", loopType.ToString()));
	}

	public static void ScaleUpdate(this GameObject go, Vector3 scale, float time)
	{
		iTween.ScaleUpdate(go, scale, time);
	}

	public static void ShakePosition(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.ShakePosition(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void ShakeRotation(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.ShakeRotation(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void ShakeScale(this GameObject go, Vector3 amount, float time, float delay)
	{
		iTween.ShakeScale(go, iTween.Hash("amount", amount, "time", time, "delay", delay));
	}

	public static void Stab(this GameObject go, AudioClip audioClip, float volume, float pitch, float delay)
	{
		iTween.Stab(go, iTween.Hash("audioClip", audioClip, "volume", volume, "pitch", pitch, "delay", delay));
	}
}
