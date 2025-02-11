using UnityEngine;

public class VirtualStickFeedback : MonoBehaviour
{
	public PackedSprite Stick;

	public PackedSprite StickLimits;

	private Vector3 mCachedOrigin;

	private float mLastInputTime;

	private void Start()
	{
		mCachedOrigin = Vector3.zero;
		mLastInputTime = -1f;
		Stick.Hide(true);
		StickLimits.Hide(true);
	}

	private void Update()
	{
		if (Time.time - mLastInputTime > 0.2f)
		{
			if (!Stick.IsHidden())
			{
				Stick.Hide(true);
				StickLimits.Hide(true);
			}
		}
		else if (Stick.IsHidden())
		{
			Stick.Hide(false);
			StickLimits.Hide(false);
		}
	}

	public void UpdateFromInput(Vector3 origPos, Vector2 offset)
	{
		mLastInputTime = Time.time;
		if (origPos != mCachedOrigin)
		{
			mCachedOrigin = origPos;
			Vector3 position = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(origPos);
			base.transform.position = position;
		}
		Stick.transform.localPosition = new Vector3(offset.x, offset.y);
	}
}
