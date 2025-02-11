using System.Collections.Generic;
using UnityEngine;

public class Carousel : MonoBehaviour
{
	private enum TransitionType
	{
		Next = 0,
		Previous = 1
	}

	private const float SCALE_OFFSET = 0.1f;

	private const float ALPHA_OFFSET = 0.1f;

	private const float TRANSITION_TIME = 0.16f;

	private const float SKEW_Y = 0.2f;

	private const float DEPTH_MULTIPLIER = 0.3f;

	public List<CarouselItem> VisibleItems;

	public CarouselItem SpareItem;

	public Vector2 Size;

	private List<TransitionType> mTransitionQueue;

	private CarouselDataSource mDataSource;

	private Vector3[] mItemPositions;

	private Vector3[] mItemScales;

	private float[] mItemAlphas;

	private Rect[] mRects;

	private Vector2 mDragStart;

	private float mStartDepth;

	private float mLastTransition;

	private int mNumVisibleItems;

	private int mTotalItems;

	private int mDraggingIndex;

	private int mSpareIndex;

	private bool mDraggingForward;

	private bool mPositioned;

	private void Awake()
	{
		mTransitionQueue = new List<TransitionType>();
		Reset();
		mNumVisibleItems = VisibleItems.Count;
		mTotalItems = mNumVisibleItems + 2;
		mStartDepth = base.transform.position.z;
		mItemPositions = new Vector3[mTotalItems];
		mItemScales = new Vector3[mTotalItems];
		mItemAlphas = new float[mTotalItems];
		mRects = new Rect[mNumVisibleItems];
		mDraggingIndex = -1;
		mPositioned = false;
	}

	public void SetDataSource(CarouselDataSource data)
	{
		mDataSource = data;
	}

	private void CalculatePositions()
	{
		mPositioned = true;
		mNumVisibleItems = VisibleItems.Count;
		mTotalItems = mNumVisibleItems + 2;
		float offset = 0f;
		int num = mNumVisibleItems / 2;
		bool flag = false;
		float num2 = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
		for (int i = 0; i < mNumVisibleItems; i++)
		{
			CarouselItem carouselItem = VisibleItems[i];
			if (carouselItem != null)
			{
				if (!flag)
				{
					float x = carouselItem.Size.x;
					offset = (Size.x - x) * 0.5f;
					flag = true;
				}
				int positionFromMiddleItem = num - i;
				SetupPositionScaleAndColour(positionFromMiddleItem, i, offset, false);
				carouselItem.transform.position = mItemPositions[i];
				carouselItem.transform.localScale = mItemScales[i];
				carouselItem.transform.parent = base.transform;
				float num3 = 0f;
				float num4 = 0f;
				Vector3 vector = Camera.main.WorldToScreenPoint(mItemPositions[i]);
				Transform transform = carouselItem.transform.FindChild("background");
				PackedSprite component = transform.GetComponent<PackedSprite>();
				if (component != null)
				{
					num3 = component.width / num2;
					num4 = component.height / num2;
				}
				mRects[i] = new Rect(vector.x - num3 * 0.5f, vector.y - num4 * 0.5f, num3, num4);
			}
			else
			{
				Debug.LogWarning("Unable to create Items for Carousel as prefab does not contain CarouselItem component");
			}
			VisibleItems[i].MoveAndScaleTo(mItemPositions[i], mItemScales[i], 0.01f);
			VisibleItems[i].UpdateAlpha(mItemAlphas[i], 0.01f);
		}
		SetupPositionScaleAndColour(-(num + 1), mNumVisibleItems, offset, true);
		SetupPositionScaleAndColour(num + 1, mNumVisibleItems + 1, offset, true);
		if (SpareItem != null)
		{
			SpareItem.transform.parent = base.transform;
			SpareItem.SetTo(new Vector3(1000f, 0f, 0f), Vector3.one, 1f);
		}
	}

	private void SetupPositionScaleAndColour(int positionFromMiddleItem, int count, float offset, bool clear)
	{
		int num = Mathf.Abs(positionFromMiddleItem);
		float num2 = 1f - 0.1f * (float)num;
		float x = base.transform.position.x - offset * (float)positionFromMiddleItem;
		float y = base.transform.position.y + (float)num * 0.2f;
		float z = mStartDepth + (float)num * 0.3f;
		float num3 = (clear ? 0f : (1f - 0.1f * (float)num));
		mItemPositions[count] = new Vector3(x, y, z);
		mItemScales[count] = new Vector3(num2, num2, 1f);
		mItemAlphas[count] = num3;
	}

	private void Reset()
	{
		mLastTransition = 0f;
	}

	private void Update()
	{
		if (mPositioned)
		{
			mLastTransition += TimeManager.DeltaTime;
			if (!(mLastTransition > 0.176f))
			{
				return;
			}
			if (mTransitionQueue.Count > 0)
			{
				if (mTransitionQueue[0] == TransitionType.Next)
				{
					DoTransitionToNext();
				}
				else
				{
					DoTransitionToPrevious();
				}
				mTransitionQueue.RemoveAt(0);
			}
			else
			{
				mSpareIndex = -1;
			}
		}
		else
		{
			bool flag = true;
			EZScreenPlacement component = GetComponent<EZScreenPlacement>();
			if (component != null)
			{
				flag = component.Started;
			}
			if (flag)
			{
				CalculatePositions();
			}
		}
	}

	public void OnEnable()
	{
		for (int i = 0; i < mNumVisibleItems; i++)
		{
			VisibleItems[i].MoveAndScaleTo(mItemPositions[i], mItemScales[i], 0.01f);
			VisibleItems[i].UpdateAlpha(mItemAlphas[i], 0.01f);
		}
		if (SpareItem != null)
		{
			SpareItem.SetTo(new Vector3(1000f, 0f, 0f), Vector3.one, 1f);
		}
		FingerGestures.OnFingerTap += OnFingerTap;
		FingerGestures.OnFingerDragBegin += OnFingerDragBegin;
		FingerGestures.OnFingerDragMove += OnFingerDragMove;
		FingerGestures.OnFingerDragEnd += OnFingerDragEnd;
	}

	public void OnDisable()
	{
		FingerGestures.OnFingerTap -= OnFingerTap;
		FingerGestures.OnFingerDragBegin -= OnFingerDragBegin;
		FingerGestures.OnFingerDragMove -= OnFingerDragMove;
		FingerGestures.OnFingerDragEnd -= OnFingerDragEnd;
	}

	public void OnFingerDragBegin(int fingerIndex, Vector2 fingerPos, Vector2 startPos)
	{
		if (mDraggingIndex != -1)
		{
			return;
		}
		for (int i = 0; i < mRects.Length; i++)
		{
			if (mRects[i].Contains(fingerPos))
			{
				mDraggingIndex = i;
				mDragStart = mRects[mDraggingIndex].center;
				break;
			}
		}
	}

	public void OnFingerDragMove(int fingerIndex, Vector2 fingerPos, Vector2 delta)
	{
		if (mDraggingIndex == -1 || mDataSource == null)
		{
			return;
		}
		float num = Mathf.Abs((mDragStart - fingerPos).x / mRects[mDraggingIndex].width);
		mDraggingForward = fingerPos.x > mDragStart.x;
		bool flag = true;
		if (num >= 0.5f)
		{
			MenuSFX.Instance.SelectToggle.Play2D();
			int num2 = ((!mDraggingForward) ? GetPreviousIndex(mDraggingIndex) : GetNextIndex(mDraggingIndex));
			if (num2 < mNumVisibleItems)
			{
				if (!mDraggingForward)
				{
					mDataSource.Previous();
				}
				else
				{
					mDataSource.Next();
				}
				Refresh();
				mDraggingIndex = num2;
				mDragStart = mRects[mDraggingIndex].center;
				mDraggingForward = !mDraggingForward;
				num = Mathf.Abs((mDragStart - fingerPos).x / mRects[mDraggingIndex].width);
			}
			else
			{
				flag = false;
			}
		}
		if (flag)
		{
			for (int i = 0; i < mNumVisibleItems; i++)
			{
				int num3 = ((!mDraggingForward) ? GetPreviousIndex(i) : GetNextIndex(i));
				Vector3 newPosition = Vector3.Lerp(mItemPositions[i], mItemPositions[num3], num);
				Vector3 newScale = Vector3.Lerp(mItemScales[i], mItemScales[num3], num);
				float alpha = Mathf.Lerp(mItemAlphas[i], mItemAlphas[num3], num);
				VisibleItems[i].SetTo(newPosition, newScale, alpha);
			}
			if (SpareItem != null)
			{
				int num4 = ((!mDraggingForward) ? (mNumVisibleItems - 1) : 0);
				int num5 = ((!mDraggingForward) ? mNumVisibleItems : (mNumVisibleItems + 1));
				Vector3 newPosition2 = Vector3.Lerp(mItemPositions[num5], mItemPositions[num4], num);
				Vector3 newScale2 = Vector3.Lerp(mItemScales[num5], mItemScales[num4], num);
				float alpha2 = Mathf.Lerp(mItemAlphas[num5], mItemAlphas[num4], num);
				SpareItem.SetTo(newPosition2, newScale2, alpha2);
				mSpareIndex = (mDraggingForward ? mNumVisibleItems : 0);
			}
		}
	}

	public void OnFingerDragEnd(int fingerIndex, Vector2 fingerPos)
	{
		if (mDraggingIndex != -1)
		{
			for (int i = 0; i < mNumVisibleItems; i++)
			{
				VisibleItems[i].MoveAndScaleTo(mItemPositions[i], mItemScales[i], 0.16f);
				VisibleItems[i].UpdateAlpha(mItemAlphas[i], 0.16f);
			}
			if (SpareItem != null)
			{
				mSpareIndex = (mDraggingForward ? mNumVisibleItems : 0);
				int num = ((!mDraggingForward) ? mNumVisibleItems : (mNumVisibleItems + 1));
				SpareItem.MoveAndScaleTo(mItemPositions[num], mItemScales[num], 0.16f);
				SpareItem.UpdateAlpha(0f, 0.16f);
			}
			mDraggingIndex = -1;
		}
	}

	public void OnFingerTap(int fingerIndex, Vector2 fingerPos)
	{
		if (mTransitionQueue.Count != 0)
		{
			return;
		}
		for (int i = 0; i < mRects.Length; i++)
		{
			if (mRects[i].Contains(fingerPos))
			{
				MoveToIndex(i);
				break;
			}
		}
	}

	private void TransitionToNext()
	{
		TransitionToNext(1);
	}

	private void TransitionToPrevious()
	{
		TransitionToPrevious(1);
	}

	private void TransitionToNext(int count)
	{
		for (int i = 0; i < count; i++)
		{
			mTransitionQueue.Add(TransitionType.Next);
		}
	}

	private void TransitionToPrevious(int count)
	{
		for (int i = 0; i < count; i++)
		{
			mTransitionQueue.Add(TransitionType.Previous);
		}
	}

	private void DoTransitionToNext()
	{
		MenuSFX.Instance.SelectToggle.Play2D();
		for (int i = 0; i < mNumVisibleItems; i++)
		{
			int nextIndex = GetNextIndex(i);
			VisibleItems[i].SetTo(mItemPositions[i], mItemScales[i], mItemAlphas[nextIndex]);
			VisibleItems[i].MoveAndScaleFrom(mItemPositions[nextIndex], mItemScales[nextIndex], 0.16f);
			VisibleItems[i].UpdateAlpha(mItemAlphas[i], 0.16f);
		}
		if (SpareItem != null)
		{
			int num = 0;
			int num2 = mNumVisibleItems + 1;
			SpareItem.SetTo(mItemPositions[num2], mItemScales[num2], mItemAlphas[num]);
			SpareItem.MoveAndScaleFrom(mItemPositions[num], mItemScales[num], 0.16f);
			SpareItem.UpdateAlpha(mItemAlphas[num2], 0.16f);
			mSpareIndex = num;
		}
		if (mDataSource != null)
		{
			mDataSource.Previous();
			Refresh();
		}
		Reset();
	}

	private void DoTransitionToPrevious()
	{
		MenuSFX.Instance.SelectToggle.Play2D();
		for (int i = 0; i < mNumVisibleItems; i++)
		{
			int previousIndex = GetPreviousIndex(i);
			VisibleItems[i].SetTo(mItemPositions[i], mItemScales[i], mItemAlphas[previousIndex]);
			VisibleItems[i].MoveAndScaleFrom(mItemPositions[previousIndex], mItemScales[previousIndex], 0.16f);
			VisibleItems[i].UpdateAlpha(mItemAlphas[i], 0.16f);
		}
		if (SpareItem != null)
		{
			int num = mNumVisibleItems - 1;
			int num2 = mNumVisibleItems;
			SpareItem.SetTo(mItemPositions[num2], mItemScales[num2], mItemAlphas[num]);
			SpareItem.MoveAndScaleFrom(mItemPositions[num], mItemScales[num], 0.16f);
			SpareItem.UpdateAlpha(mItemAlphas[num2], 0.16f);
			mSpareIndex = num;
		}
		if (mDataSource != null)
		{
			mDataSource.Next();
			Refresh();
		}
		Reset();
	}

	private int GetNextIndex(int count)
	{
		int num = count + 1;
		if (num == mNumVisibleItems)
		{
			num = mNumVisibleItems;
		}
		return num;
	}

	private int GetPreviousIndex(int count)
	{
		int num = count - 1;
		if (num == -1)
		{
			num = mNumVisibleItems + 1;
		}
		return num;
	}

	private void MoveToIndex(int index)
	{
		int num = index - mNumVisibleItems / 2;
		if (num != 0)
		{
			if (num < 0)
			{
				TransitionToPrevious(-num);
			}
			else
			{
				TransitionToNext(num);
			}
		}
	}

	public void Refresh()
	{
		if (mDataSource != null)
		{
			int num = VisibleItems.Count / 2;
			List<CarouselItem> list = new List<CarouselItem>();
			if (mSpareIndex == 0)
			{
				list.Add(SpareItem);
				num++;
			}
			list.AddRange(VisibleItems);
			if (mSpareIndex > 0)
			{
				list.Add(SpareItem);
			}
			if (SpareItem != null && mSpareIndex == -1)
			{
				SpareItem.SetTo(new Vector3(1000f, 0f, 0f), Vector3.one, 1f);
			}
			mDataSource.Populate(list, num);
		}
	}

	public void CancelTransition()
	{
		for (int i = 0; i < mNumVisibleItems; i++)
		{
			VisibleItems[i].CancelUpdate();
		}
		if (SpareItem != null)
		{
			SpareItem.CancelUpdate();
		}
	}
}
