using UnityEngine;

public class CarouselItem : MonoBehaviour
{
	public Vector2 Size;

	private PackedSprite[] mSprites;

	private IconControllerBase mIcon;

	private float[] mAlpha;

	private Vector3 mCachedPosition;

	private Vector3 mCachedScale;

	private float mIconAlpha;

	private float mToAlpha;

	private float mTime;

	private float mTimeToTake;

	private void Awake()
	{
		mCachedPosition = base.transform.position;
		mCachedScale = base.transform.localScale;
		mSprites = GetComponentsInChildren<PackedSprite>();
		mIcon = GetComponentInChildren<IconControllerBase>();
		mAlpha = new float[mSprites.Length];
		mIconAlpha = 0f;
	}

	private void Update()
	{
		mTime += TimeManager.DeltaTime;
		if (mTime < mTimeToTake && mSprites != null)
		{
			float t = mTime / mTimeToTake;
			for (int i = 0; i < mSprites.Length; i++)
			{
				Color color = mSprites[i].Color;
				color.a = Mathf.Lerp(mAlpha[i], mToAlpha, t);
				mSprites[i].SetColor(color);
			}
			if (mIcon != null)
			{
				float alpha = Mathf.Lerp(mIconAlpha, mToAlpha, t);
				mIcon.SetAlpha(alpha);
			}
		}
	}

	public void SetTo(Vector3 newPosition, Vector3 newScale, float alpha)
	{
		mCachedPosition = newPosition;
		mCachedScale = newScale;
		base.gameObject.transform.position = newPosition;
		base.gameObject.transform.localScale = newScale;
		if (mSprites != null)
		{
			for (int i = 0; i < mSprites.Length; i++)
			{
				Color color = mSprites[i].Color;
				color.a = alpha;
				mSprites[i].SetColor(color);
			}
		}
		if (mIcon != null)
		{
			mIcon.SetAlpha(alpha);
		}
	}

	public void MoveAndScaleTo(Vector3 newPosition, Vector3 newScale, float time)
	{
		mCachedPosition = newPosition;
		mCachedScale = newScale;
		base.gameObject.MoveTo(newPosition, time, 0f, EaseType.easeInOutCubic);
		base.gameObject.ScaleTo(newScale, time, 0f, EaseType.easeInOutCubic);
	}

	public void MoveAndScaleFrom(Vector3 fromPosition, Vector3 fromScale, float time)
	{
		mCachedPosition = base.transform.position;
		mCachedScale = base.transform.localScale;
		base.gameObject.MoveFrom(fromPosition, time, 0f, EaseType.easeInOutCubic);
		base.gameObject.ScaleFrom(fromScale, time, 0f, EaseType.easeInOutCubic);
	}

	public void UpdateAlpha(float toAlpha, float time)
	{
		if (mSprites != null)
		{
			for (int i = 0; i < mSprites.Length; i++)
			{
				Color color = mSprites[i].Color;
				mAlpha[i] = color.a;
			}
		}
		if (mIcon != null && mIcon.Sprite != null)
		{
			mIconAlpha = mIcon.Sprite.Color.a;
		}
		mToAlpha = toAlpha;
		mTimeToTake = time;
		mTime = 0f;
	}

	public void CancelUpdate()
	{
		iTween[] componentsInChildren = GetComponentsInChildren<iTween>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i]);
		}
		SetTo(mCachedPosition, mCachedScale, mToAlpha);
	}
}
