using UnityEngine;

public class BlendFinder : MonoBehaviour
{
	private enum CategoryEnum
	{
		kMovement = 0,
		kCount = 1
	}

	private enum ActionEnum
	{
		kForward = 0,
		kRight = 1,
		kBackward = 2,
		kLeft = 3,
		kCount = 4
	}

	private AnimDirector mAnimDirector;

	private int[] mCategoryHandle;

	private AnimDirector.ActionHandle[] mActionHandle;

	public int mAction1;

	public int mAction2;

	public float mTime1;

	public float mTime2;

	public int mScrub;

	public float mFrac;

	private float fTargTime;

	private float fCurrentTime;

	private void Start()
	{
		mAnimDirector = GetComponent<AnimDirector>();
		Start_GetActionHandles();
		mTime1 = 0f;
		mTime2 = 0f;
		mAction1 = 0;
		mAction2 = 0;
		mScrub = 0;
		fTargTime = 0f;
		fCurrentTime = 0f;
		mFrac = 0.1f;
	}

	private void Start_GetActionHandles()
	{
		mCategoryHandle = new int[1];
		mActionHandle = new AnimDirector.ActionHandle[4];
		GetCategoryHandle(CategoryEnum.kMovement, "Movement");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kForward, "Forward");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kRight, "Right");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kBackward, "Backward");
		GetActionHandle(CategoryEnum.kMovement, ActionEnum.kLeft, "Left");
	}

	private void GetCategoryHandle(CategoryEnum cat, string name)
	{
		mCategoryHandle[(int)cat] = mAnimDirector.GetCategoryHandle(name);
	}

	private void GetActionHandle(CategoryEnum cat, ActionEnum act, string name)
	{
		mActionHandle[(int)act] = mAnimDirector.GetActionHandle(mCategoryHandle[(int)cat], name);
	}

	private void Update()
	{
		mScrub = Mathf.Clamp(mScrub, 0, 100);
		if (mScrub < 50)
		{
			mAnimDirector.PlayAction(mActionHandle[mAction1]);
			fTargTime = mTime1;
		}
		else
		{
			mAnimDirector.PlayAction(mActionHandle[mAction2]);
			fTargTime = mTime2;
		}
		mAnimDirector.SetCategoryTime(mCategoryHandle[0], fCurrentTime);
		mAnimDirector.SetCategorySpeed(mCategoryHandle[0], 0f);
		fCurrentTime = BlendLooped(fCurrentTime, fTargTime, mAnimDirector.GetCategoryLength(mCategoryHandle[0]), mFrac);
	}

	private float BlendLooped(float fr, float to, float looplen, float frac)
	{
		if (fr < to)
		{
			if (fr - (to - looplen) < to - fr)
			{
				to -= looplen;
			}
		}
		else if (to + looplen - fr < fr - to)
		{
			to += looplen;
		}
		ExpBlend(ref fr, to, frac);
		if (fr < 0f)
		{
			fr += looplen;
		}
		if (fr > looplen)
		{
			fr -= looplen;
		}
		return fr;
	}

	private void ExpBlend(ref float fr, float to, float frac)
	{
		fr = to * frac + fr * (1f - frac);
	}
}
