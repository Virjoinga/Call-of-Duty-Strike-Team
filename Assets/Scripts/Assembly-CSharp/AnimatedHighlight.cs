using UnityEngine;

public class AnimatedHighlight : MonoBehaviour
{
	private const float ANIMATION_LENGTH = 0.3f;

	private Scale9Grid mHighlight;

	private Vector3 mOldPosition;

	private Vector3 mNewPosition;

	private Vector3 mCurrentPosition;

	private Vector2 mOldDimensions;

	private Vector2 mNewDimensions;

	private Vector2 mCurrentDimensions;

	private float mAnimateTime;

	public void SetHighlightNow(Vector3 to, Vector2 size)
	{
		if (mHighlight == null)
		{
			FindHighlight();
		}
		mNewPosition = (mOldPosition = to);
		mNewDimensions = (mOldDimensions = size);
		mAnimateTime = 0.3f;
		base.transform.position = mCurrentPosition;
		mHighlight.size = mCurrentDimensions;
		mHighlight.Resize();
	}

	public void HighlightSprite(SpriteRoot sprite)
	{
		if (mHighlight == null)
		{
			FindHighlight();
		}
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		mOldPosition = base.transform.position;
		mNewPosition = FindPositionFromSprite(sprite);
		mOldDimensions = mHighlight.size * 1.1f;
		mNewDimensions = new Vector2(sprite.width / num, sprite.height / num);
		mAnimateTime = 0f;
	}

	public void HighlightCollider(BoxCollider collider)
	{
		if (mHighlight == null)
		{
			FindHighlight();
		}
		float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		mOldPosition = base.transform.position;
		mNewPosition = collider.transform.position;
		mOldDimensions = mHighlight.size * 1.1f;
		mNewDimensions = new Vector2(collider.size.x / num, collider.size.y / num);
		mAnimateTime = 0f;
	}

	public void HighlightRect(Rect rect)
	{
		if (mHighlight == null)
		{
			FindHighlight();
		}
		mOldPosition = base.transform.position;
		mNewPosition = Camera.main.ScreenToWorldPoint(new Vector3(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f));
		mOldDimensions = mHighlight.size * 1.1f;
		mNewDimensions = new Vector2(rect.width, rect.height);
		mAnimateTime = 0f;
	}

	public void DismissHighlight()
	{
		if (mHighlight == null)
		{
			FindHighlight();
		}
		mOldPosition = base.transform.position;
		mNewPosition = base.transform.position;
		mOldDimensions = mHighlight.size;
		mNewDimensions = Vector2.zero;
		mAnimateTime = 0f;
	}

	private void Awake()
	{
		mAnimateTime = 0.3f;
	}

	private void Update()
	{
		if (mAnimateTime < 0.3f && mHighlight != null)
		{
			mAnimateTime += TimeManager.DeltaTime;
			float t = Mathf.Clamp(mAnimateTime / 0.3f, 0f, 1f);
			mCurrentPosition = Vector3.Lerp(mOldPosition, mNewPosition, t);
			mCurrentDimensions = Vector2.Lerp(mOldDimensions, mNewDimensions, t);
			base.transform.position = mCurrentPosition;
			mHighlight.size = mCurrentDimensions;
			mHighlight.Resize();
		}
	}

	private void FindHighlight()
	{
		if (mHighlight == null)
		{
			mHighlight = GetComponentInChildren<Scale9Grid>();
		}
	}

	private Vector3 FindPositionFromSprite(SpriteRoot sprite)
	{
		Vector3 result = Vector3.zero;
		if (sprite != null)
		{
			result = sprite.transform.position;
			switch (sprite.anchor)
			{
			case SpriteRoot.ANCHOR_METHOD.UPPER_LEFT:
			case SpriteRoot.ANCHOR_METHOD.MIDDLE_LEFT:
			case SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT:
				result.x += sprite.width * 0.5f;
				break;
			case SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT:
			case SpriteRoot.ANCHOR_METHOD.MIDDLE_RIGHT:
			case SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT:
				result.x -= sprite.width * 0.5f;
				break;
			}
			switch (sprite.anchor)
			{
			case SpriteRoot.ANCHOR_METHOD.UPPER_LEFT:
			case SpriteRoot.ANCHOR_METHOD.UPPER_CENTER:
			case SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT:
				result.y -= sprite.height * 0.5f;
				break;
			case SpriteRoot.ANCHOR_METHOD.BOTTOM_LEFT:
			case SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER:
			case SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT:
				result.y += sprite.height * 0.5f;
				break;
			}
		}
		return result;
	}
}
