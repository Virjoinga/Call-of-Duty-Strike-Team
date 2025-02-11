using UnityEngine;

public class FingerHoldDragMarker : MonoBehaviour
{
	public enum FingerType
	{
		Point = 0,
		Rotate = 1
	}

	public PackedSprite Finger;

	public PackedSprite Ring;

	private void Start()
	{
		Ring.gameObject.ScaleTo(Vector3.zero, 0.5f, 0f, EaseType.linear, LoopType.loop);
		Ring.gameObject.FadeFrom(0f, 0.5f, 0f, LoopType.loop);
	}

	public void SetFingerType(FingerType type)
	{
		if (Finger != null)
		{
			Finger.SetFrame(0, (int)type);
		}
	}
}
