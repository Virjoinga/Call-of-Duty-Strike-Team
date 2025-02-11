using System.Collections.Generic;
using UnityEngine;

public class AnimDirector : MonoBehaviour
{
	public class ActionHandle
	{
		public int CategoryID;

		public int ActionID;
	}

	public enum BlendEasing
	{
		Soft = 0,
		Linear = 1,
		Sharp = 2
	}

	public class Blender
	{
		public float weight;

		public float startWeight;

		public float deltaWeight;

		public float startTime;

		public float duration;

		public BlendEasing easeIn;

		public BlendEasing easeOut;

		public static float Evaluate(float sWeight, float eWeight, float l, BlendEasing eIn, BlendEasing eOut)
		{
			float num = 0f;
			float num2 = 1f - l;
			float num3;
			switch (eIn)
			{
			case BlendEasing.Linear:
				num3 = l;
				break;
			case BlendEasing.Sharp:
				num3 = 1f - num2 * num2;
				break;
			default:
				num3 = l * l;
				break;
			}
			float num4;
			switch (eOut)
			{
			case BlendEasing.Linear:
				num4 = l;
				break;
			case BlendEasing.Sharp:
				num4 = l * l;
				break;
			default:
				num4 = 1f - num2 * num2;
				break;
			}
			num = num3 * num2 + num4 * l;
			return sWeight + (eWeight - sWeight) * num;
		}

		public void Update()
		{
			float num = (WorldHelper.ThisFrameTime - startTime) * (1f / duration);
			float num2 = 0f;
			if (num >= 1f)
			{
				startWeight += deltaWeight;
				deltaWeight = 0f;
			}
			else
			{
				float num3 = 1f - num;
				float num4;
				switch (easeIn)
				{
				case BlendEasing.Linear:
					num4 = num;
					break;
				case BlendEasing.Sharp:
					num4 = 1f - num3 * num3;
					break;
				default:
					num4 = num * num;
					break;
				}
				float num5;
				switch (easeOut)
				{
				case BlendEasing.Linear:
					num5 = num;
					break;
				case BlendEasing.Sharp:
					num5 = num * num;
					break;
				default:
					num5 = 1f - num3 * num3;
					break;
				}
				num2 = num4 * num3 + num5 * num;
			}
			weight = startWeight + deltaWeight * num2;
		}

		public void ManualWeight(float w)
		{
			weight = w;
			startWeight = w;
			deltaWeight = 0f;
			duration = 1f;
		}

		public void Start(float fromWeight, float toWeight, float dur, BlendEasing i, BlendEasing o)
		{
			startTime = WorldHelper.ThisFrameTime;
			if (dur <= 0f)
			{
				weight = toWeight;
				startWeight = toWeight;
				deltaWeight = 0f;
				duration = 1f;
			}
			else
			{
				startWeight = fromWeight;
				weight = fromWeight;
				deltaWeight = toWeight - startWeight;
				duration = dur;
				easeIn = i;
				easeOut = o;
			}
		}
	}

	public class AnimationStateArray
	{
		public AnimationState[] animState = new AnimationState[10];

		private int count;

		private float blendScale;

		private float speed;

		private float referenceTime;

		private bool mEnabled;

		public float Speed
		{
			get
			{
				return speed;
			}
			set
			{
				float animTime = AnimTime;
				speed = value;
				AnimTime = animTime;
			}
		}

		public float AnimTime
		{
			get
			{
				if (speed == 0f)
				{
					return referenceTime;
				}
				return (WorldHelper.ThisFrameTime - referenceTime) * speed;
			}
			set
			{
				if (speed == 0f)
				{
					referenceTime = value;
				}
				else
				{
					referenceTime = WorldHelper.ThisFrameTime - value / speed;
				}
			}
		}

		public int layer
		{
			get
			{
				if (count == 0)
				{
					return 0;
				}
				return animState[0].layer;
			}
		}

		public bool enabled
		{
			get
			{
				return mEnabled;
			}
			set
			{
				mEnabled = value;
				if (!value)
				{
					for (int i = 0; i < count; i++)
					{
						animState[i].enabled = false;
					}
				}
			}
		}

		public AnimationBlendMode blendMode
		{
			get
			{
				if (count > 0)
				{
					return animState[0].blendMode;
				}
				return AnimationBlendMode.Blend;
			}
			set
			{
				for (int i = 0; i < count; i++)
				{
					animState[i].blendMode = value;
				}
			}
		}

		public float length
		{
			get
			{
				if (count > 0)
				{
					return animState[0].length;
				}
				return 0f;
			}
		}

		public WrapMode wrapMode
		{
			get
			{
				if (count > 0)
				{
					return animState[0].wrapMode;
				}
				return WrapMode.Default;
			}
			set
			{
				for (int i = 0; i < count; i++)
				{
					animState[i].wrapMode = value;
				}
			}
		}

		public void InitFromRawAnim(Animation player, RawAnimation ra)
		{
			count = ra.branchCount;
			for (int i = 0; i < count; i++)
			{
				animState[i] = player[ra.GetBranch(i).AnimClip.name];
				animState[i].speed = 0f;
			}
			blendScale = count - 1;
		}

		public bool Matches(AnimationState aState)
		{
			if (count == 0)
			{
				return false;
			}
			return animState[0] == aState;
		}

		public void SetBlendAndWeight(float weight, float blend, bool onScreen)
		{
			mEnabled = true;
			if (count == 0)
			{
				return;
			}
			if (count == 1)
			{
				animState[0].weight = weight;
				animState[0].enabled = weight > 0.01f && onScreen;
				animState[0].time = AnimTime;
				return;
			}
			blend *= blendScale;
			int num = (int)blend;
			blend -= (float)num;
			for (int i = 0; i < num; i++)
			{
				animState[i].enabled = false;
			}
			float num2 = weight * (1f - blend);
			animState[num].enabled = num2 > 0.01f && onScreen;
			animState[num].weight = num2;
			animState[num].time = AnimTime;
			int num3 = num + 1;
			if (num3 < count)
			{
				float num4 = weight * blend;
				animState[num3].enabled = num4 > 0.01f && onScreen;
				animState[num3].weight = num4;
				animState[num3].time = AnimTime;
			}
			for (int i = num3 + 1; i < count; i++)
			{
				animState[i].enabled = false;
			}
		}
	}

	public class AnimBlender : Blender
	{
		public AnimationStateArray animState = new AnimationStateArray();

		public int myAction;

		public RawAnimation rawAnim;

		public bool animStateEnabled;

		public float blendOutTime;

		public bool available = true;

		public bool UpdateAnimCheckDone(AnimDirector ad, int thisIndex)
		{
			Update();
			CategoryInfo categoryInfo = ad.ActiveCategoryInfo[animState.layer];
			animStateEnabled = animState.enabled;
			if (!animStateEnabled)
			{
				if (categoryInfo.currentAnimBlender == thisIndex)
				{
					categoryInfo.currentAnimBlender = -1;
				}
				available = true;
				return true;
			}
			if (weight == 0f && deltaWeight == 0f)
			{
				animState.enabled = false;
				if (categoryInfo.currentAnimBlender == thisIndex)
				{
					categoryInfo.currentAnimBlender = -1;
				}
				available = true;
				return true;
			}
			float num = weight * categoryInfo.blender.weight;
			animState.SetBlendAndWeight(num, categoryInfo.blendTreeBlend, ad.ShouldAnimate() && (animState.layer >= ad.MonopolyLayer || animState.blendMode == AnimationBlendMode.Additive));
			if (categoryInfo.currentAnimBlender == -1)
			{
				categoryInfo.currentAnimBlender = thisIndex;
			}
			if (animState.layer > ad.MonopolyLayer && rawAnim.Monopoly && num > 0.99f)
			{
				ad.MonopolyLayer = animState.layer;
			}
			ad.AnimsPreventReloading = rawAnim.PreventReloading;
			return false;
		}

		public void Start(AnimationState astate, AnimDirector ad, int act, RawAnimation ra)
		{
			animState.InitFromRawAnim(ad.AnimationPlayer, ra);
			myAction = act;
			animState.SetBlendAndWeight(weight * ad.ActiveCategoryInfo[astate.layer].blender.weight, 0f, ad.ShouldAnimate());
			rawAnim = ra;
			blendOutTime = 0.25f;
			available = false;
		}
	}

	public class CategoryInfo
	{
		public bool enabled;

		public int currentAnimBlender;

		public int chainedCategoryHandle;

		public int chainedActionHandle;

		public float chainedActionStartTime;

		public Blender blender;

		public float blendTreeBlend = 0.5f;
	}

	private const float kDefaultCrossFade = 0.25f;

	public const int kMaxSimultaneousBlends = 12;

	public const int kMaxLayers = 16;

	public AnimLibrary DefaultAnimLibrary;

	private bool enableAfterFirstUpdate = true;

	public Animation AnimationPlayer;

	public GameObject GameModel;

	public bool AnimsPreventAiming;

	public bool AnimsPreventReloading;

	public bool OnScreen = true;

	public AnimationCullingType DefaultCulling = AnimationCullingType.BasedOnRenderers;

	private float ForceUpdateUntil;

	private bool forcedRawAnimValid;

	private RawAnimation forcedRawAnim;

	private int[] CurrentOverrideStack;

	public List<AnimOverride> ActiveOverrides;

	public List<CategoryInfo> ActiveCategoryInfo;

	public AnimBlender[] animBlends;

	public bool[] OverrideState;

	public bool[] clipAdded;

	public int MonopolyLayer;

	private bool[] categoriesToPlay;

	private AnimDirector()
	{
		ActiveOverrides = new List<AnimOverride>();
		forcedRawAnim = null;
		forcedRawAnimValid = false;
	}

	public void ForceUpdateFor(float t)
	{
		ForceUpdateUntil = Time.time + t;
		AnimationPlayer.cullingType = AnimationCullingType.AlwaysAnimate;
	}

	public bool ShouldAnimate()
	{
		return OnScreen || AnimationPlayer.cullingType == AnimationCullingType.AlwaysAnimate;
	}

	public int GetCategoryHandle(string CategoryName)
	{
		int num = -1;
		if (DefaultAnimLibrary.AnimDefaults.Categories != null)
		{
			for (int i = 0; i < DefaultAnimLibrary.AnimDefaults.Categories.Count; i++)
			{
				AnimCategory animCategory = DefaultAnimLibrary.AnimDefaults.Categories[i];
				if (animCategory.CategoryName == CategoryName)
				{
					num = i;
					break;
				}
			}
		}
		if (num == -1)
		{
		}
		return num;
	}

	public ActionHandle GetActionHandle(string CategoryName, string ActionName)
	{
		return GetActionHandle(GetCategoryHandle(CategoryName), ActionName);
	}

	public ActionHandle GetActionHandle(int CategoryHandle, string ActionName)
	{
		ActionHandle actionHandle = new ActionHandle();
		actionHandle.CategoryID = CategoryHandle;
		actionHandle.ActionID = GetActionIndex(CategoryHandle, ActionName);
		return actionHandle;
	}

	public int GetActionIndex(int CategoryHandle, string ActionName)
	{
		int num = -1;
		if (CategoryHandle == -1)
		{
			return -1;
		}
		if (DefaultAnimLibrary.AnimDefaults.Categories != null)
		{
			if (CategoryHandle >= DefaultAnimLibrary.AnimDefaults.Categories.Count)
			{
				return -1;
			}
			AnimCategory animCategory = DefaultAnimLibrary.AnimDefaults.Categories[CategoryHandle];
			for (int i = 0; i < animCategory.Actions.Count; i++)
			{
				AnimAction animAction = animCategory.Actions[i];
				if (animAction.ActionName == ActionName)
				{
					num = i;
					break;
				}
			}
		}
		if (num == -1)
		{
		}
		return num;
	}

	public int GetOverrideHandle(string OverrideName)
	{
		int result = -1;
		if (DefaultAnimLibrary.AnimOverrides != null)
		{
			for (int i = 0; i < DefaultAnimLibrary.AnimOverrides.Count; i++)
			{
				AnimOverride animOverride = DefaultAnimLibrary.AnimOverrides[i];
				if (animOverride.OverrideName == OverrideName)
				{
					result = i;
					break;
				}
			}
		}
		return result;
	}

	public void EnableOverride(int OverrideHandle, bool OnOff)
	{
		EnableOverride(OverrideHandle, OnOff, 0.25f);
	}

	public void EnableOverride(int OverrideHandle, bool OnOff, float xfade)
	{
		if (OverrideHandle == -1 || OverrideHandle >= DefaultAnimLibrary.AnimOverrides.Count || OnOff == OverrideState[OverrideHandle])
		{
			return;
		}
		EnableOverrideWithStacking(OverrideHandle, OnOff, xfade);
		for (int i = 0; i < DefaultAnimLibrary.AnimDefaults.Categories.Count; i++)
		{
			if (categoriesToPlay[i])
			{
				categoriesToPlay[i] = false;
				bool flag = !ActiveCategoryInfo[i].enabled;
				PlayAction(i, GetActiveCategoryAction(i), xfade);
				if (flag)
				{
					ActiveCategoryInfo[i].enabled = false;
					ActiveCategoryInfo[i].blender.ManualWeight(0f);
				}
			}
		}
	}

	private void EnableOverrideWithStacking(int OverrideHandle, bool OnOff, float xfade)
	{
		if (OverrideHandle == -1 || OverrideHandle >= DefaultAnimLibrary.AnimOverrides.Count || OnOff == OverrideState[OverrideHandle])
		{
			return;
		}
		OverrideState[OverrideHandle] = OnOff;
		AnimOverride animOverride = DefaultAnimLibrary.AnimOverrides[OverrideHandle];
		if (OnOff)
		{
			for (int i = 0; i < animOverride.OverrideStacks.Count; i++)
			{
				int stackThisIndex = animOverride.OverrideStacks[i].stackThisIndex;
				int onThatIndex = animOverride.OverrideStacks[i].onThatIndex;
				if (CurrentOverrideStack[onThatIndex] != -1)
				{
					Debug.LogError(string.Concat("Override ", animOverride.OverrideName, " is trying to stack ", DefaultAnimLibrary.AnimOverrides[stackThisIndex], " on ", DefaultAnimLibrary.AnimOverrides[onThatIndex], ", conflicting with existing stack ", DefaultAnimLibrary.AnimOverrides[CurrentOverrideStack[onThatIndex]].OverrideName));
				}
				else
				{
					if (ActiveOverridesIndexOfQuick(DefaultAnimLibrary.AnimOverrides[onThatIndex].OverrideName.GetHashCode()) >= 0)
					{
						EnableOverride(stackThisIndex, true, xfade);
					}
					CurrentOverrideStack[onThatIndex] = stackThisIndex;
				}
			}
			ActiveOverrides.Add(animOverride);
			foreach (AnimCategoryOverride categoryOverride in animOverride.CategoryOverrides)
			{
				categoriesToPlay[categoryOverride.CategoryIndex] = true;
			}
			if (CurrentOverrideStack[OverrideHandle] != -1)
			{
				EnableOverride(CurrentOverrideStack[OverrideHandle], true, xfade);
			}
			return;
		}
		for (int j = 0; j < animOverride.OverrideStacks.Count; j++)
		{
			int stackThisIndex2 = animOverride.OverrideStacks[j].stackThisIndex;
			int onThatIndex2 = animOverride.OverrideStacks[j].onThatIndex;
			if (CurrentOverrideStack[onThatIndex2] == -1)
			{
				Debug.LogError(string.Concat("Override ", animOverride.OverrideName, " is trying to unstack ", DefaultAnimLibrary.AnimOverrides[stackThisIndex2], " on ", DefaultAnimLibrary.AnimOverrides[onThatIndex2], " when it was never stacked in the first place."));
			}
			else
			{
				EnableOverride(stackThisIndex2, false, xfade);
				CurrentOverrideStack[onThatIndex2] = -1;
			}
		}
		ActiveOverrides.Remove(animOverride);
		foreach (AnimCategoryOverride categoryOverride2 in animOverride.CategoryOverrides)
		{
			categoriesToPlay[categoryOverride2.CategoryIndex] = true;
		}
		if (CurrentOverrideStack[OverrideHandle] != -1)
		{
			EnableOverride(CurrentOverrideStack[OverrideHandle], false, xfade);
		}
	}

	private int ActiveOverridesIndexOfQuick(int overRideHash)
	{
		for (int i = 0; i < ActiveOverrides.Count; i++)
		{
			if (ActiveOverrides[i].OverrideName.GetHashCode() == overRideHash)
			{
				return i;
			}
		}
		return -1;
	}

	public SegueData GetSegueData(ActionHandle actHandle)
	{
		return FindAnimRawAnim(actHandle.CategoryID, actHandle.ActionID, false).segueData;
	}

	public void ChainAction(int categoryHandle, ActionHandle actHandle, float startTime)
	{
		ActiveCategoryInfo[categoryHandle].chainedActionHandle = actHandle.ActionID;
		ActiveCategoryInfo[categoryHandle].chainedCategoryHandle = actHandle.CategoryID;
		ActiveCategoryInfo[categoryHandle].chainedActionStartTime = startTime;
	}

	public void CancelChainedActions()
	{
		for (int i = 0; i < ActiveCategoryInfo.Count; i++)
		{
			ActiveCategoryInfo[i].chainedActionHandle = -1;
		}
	}

	public void StopAnim(RawAnimation rawAnim)
	{
		StopAnim(rawAnim, 0f);
	}

	public void StopAnim(RawAnimation rawAnim, float time)
	{
		if (rawAnim == null)
		{
			return;
		}
		int i = 0;
		AnimationState animationState = AnimationPlayer[rawAnim.AnimClip.name];
		for (; i < 12; i++)
		{
			if (!animBlends[i].available && animBlends[i].animState.Matches(animationState))
			{
				CategoryInfo categoryInfo = ActiveCategoryInfo[animBlends[i].animState.layer];
				if (categoryInfo.currentAnimBlender == i)
				{
					categoryInfo.currentAnimBlender = -1;
				}
				animBlends[i].Start(animationState.weight, 0f, time, BlendEasing.Linear, BlendEasing.Linear);
			}
		}
	}

	public void StopAction(ActionHandle actHandle)
	{
		StopAnim(FindAnimRawAnim(actHandle.CategoryID, actHandle.ActionID, true));
	}

	public bool IsPlayingAction(ActionHandle actHandle)
	{
		RawAnimation rawAnimation = FindAnimRawAnim(actHandle.CategoryID, actHandle.ActionID, true);
		if (rawAnimation == null)
		{
			return false;
		}
		if (rawAnimation.AnimClip == null)
		{
			return false;
		}
		return AnimationPlayer.IsPlaying(rawAnimation.AnimClip.name);
	}

	public RawAnimation PlayAction(ActionHandle actHandle)
	{
		return PlayAction(actHandle.CategoryID, actHandle.ActionID, 0.25f, false, false);
	}

	public RawAnimation PlayAction(ActionHandle actHandle, bool ForceRestart)
	{
		return PlayAction(actHandle.CategoryID, actHandle.ActionID, 0.25f, false, ForceRestart);
	}

	public RawAnimation PlayAction(ActionHandle actHandle, float xfade)
	{
		return PlayAction(actHandle.CategoryID, actHandle.ActionID, xfade, false, false);
	}

	public RawAnimation PlayAction(ActionHandle actHandle, float xfade, bool ForceRestart)
	{
		return PlayAction(actHandle.CategoryID, actHandle.ActionID, xfade, false, ForceRestart);
	}

	public RawAnimation PlayAction(int categoryHandle, int actionHandle)
	{
		return PlayAction(categoryHandle, actionHandle, 0.25f, false, false);
	}

	public RawAnimation PlayAction(int categoryHandle, int actionHandle, float xfade)
	{
		return PlayAction(categoryHandle, actionHandle, xfade, false, false);
	}

	public void SetBlendOutTime(RawAnimation ra, float duration)
	{
		int num = 0;
		for (num = 0; num < 12; num++)
		{
			if (!animBlends[num].available && animBlends[num].rawAnim.AnimClip == ra.AnimClip)
			{
				animBlends[num].blendOutTime = duration;
			}
		}
	}

	public RawAnimation PlayAction(int categoryHandle, int actionHandle, float xfade, bool IgnoreOverrides, bool ForceRestart)
	{
		if (AnimationPlayer == null)
		{
		}
		if (!base.enabled)
		{
			return null;
		}
		RawAnimation rawAnimation = FindAnimRawAnim(categoryHandle, actionHandle, IgnoreOverrides);
		if (rawAnimation != null && rawAnimation.AnimClip != null)
		{
			CategoryInfo categoryInfo = ActiveCategoryInfo[categoryHandle];
			AnimCategory animCategory = DefaultAnimLibrary.AnimDefaults.Categories[categoryHandle];
			AnimationState animationState = AnimationPlayer[rawAnimation.AnimClip.name];
			if (xfade < 0f)
			{
				xfade = rawAnimation.BlendTime;
			}
			int num = -1;
			if (animationState != null)
			{
				RawAnimation.AnimBlendType blendType = rawAnimation.BlendType;
				if (blendType == RawAnimation.AnimBlendType.kSnapTo)
				{
					if (!categoryInfo.enabled)
					{
						categoryInfo.blender.Start(0f, 1f, 0f, BlendEasing.Linear, BlendEasing.Linear);
					}
					num = CrossFade(actionHandle, animationState, 0f, BlendEasing.Linear, BlendEasing.Linear, rawAnimation);
					xfade = 0f;
				}
				else if (categoryInfo.enabled)
				{
					num = CrossFade(actionHandle, animationState, xfade, rawAnimation.EaseBegin(), rawAnimation.EaseEnd(), rawAnimation);
				}
				else
				{
					categoryInfo.blender.Start(0f, 1f, xfade, rawAnimation.EaseBegin(), rawAnimation.EaseEnd());
					num = CrossFade(actionHandle, animationState, 0f, BlendEasing.Linear, BlendEasing.Linear, rawAnimation);
				}
			}
			else
			{
				Debug.LogWarning("Woah, failed to find anim clip!");
			}
			if (num == -1)
			{
				return rawAnimation;
			}
			AnimBlender animBlender = animBlends[num];
			animBlender.animState.Speed = rawAnimation.Speed;
			if (ForceRestart)
			{
				animBlender.animState.AnimTime = 0f;
			}
			AnimCategory.BlendingType blendingType = animCategory.BlendType;
			if (rawAnimation.ForceAdditive)
			{
				blendingType = AnimCategory.BlendingType.Add;
			}
			switch (blendingType)
			{
			case AnimCategory.BlendingType.Blended:
				animBlender.animState.blendMode = AnimationBlendMode.Blend;
				break;
			case AnimCategory.BlendingType.Add:
				animBlender.animState.blendMode = AnimationBlendMode.Additive;
				break;
			}
			if (categoryHandle >= 0 && categoryHandle < ActiveCategoryInfo.Count)
			{
				categoryInfo.currentAnimBlender = num;
				categoryInfo.chainedCategoryHandle = -1;
				categoryInfo.chainedActionHandle = -1;
				categoryInfo.enabled = true;
			}
		}
		return rawAnimation;
	}

	private int CrossFade(int act, AnimationState anim, float duration, BlendEasing easeBegin, BlendEasing easeEnd, RawAnimation rawAnim)
	{
		int i = 0;
		int num = -1;
		int num2 = -1;
		float animTime = 0f;
		if (anim.layer < ActiveCategoryInfo.Count && ActiveCategoryInfo[anim.layer].currentAnimBlender >= 0)
		{
			animTime = animBlends[ActiveCategoryInfo[anim.layer].currentAnimBlender].animState.AnimTime;
		}
		for (; i < 12; i++)
		{
			AnimBlender animBlender = animBlends[i];
			if (animBlender.available)
			{
				num2 = i;
			}
			else if (animBlender.animState.Matches(anim))
			{
				animBlender.Start(anim.weight, 1f, duration, easeBegin, easeEnd);
				animBlender.Start(anim, this, act, rawAnim);
				num = i;
			}
			else if (animBlender.deltaWeight < 0f && animBlender.startTime + animBlender.duration < Time.time)
			{
				animBlender.UpdateAnimCheckDone(this, i);
				num2 = i;
			}
			else if (animBlender.animState.wrapMode == WrapMode.Default && animBlender.deltaWeight == 0f)
			{
				float num3 = animBlender.animState.length - animBlender.animState.AnimTime;
				if (num3 <= 0f)
				{
					animBlender.Start(animBlender.weight, 0f, 0f, BlendEasing.Soft, BlendEasing.Soft);
					animBlender.UpdateAnimCheckDone(this, i);
					num2 = i;
				}
			}
			else if (animBlender.animState.layer == anim.layer)
			{
				animBlender.Start(animBlender.weight, 0f, duration, easeBegin, easeEnd);
				if (animBlender.deltaWeight == 0f)
				{
					animBlender.UpdateAnimCheckDone(this, i);
					num2 = i;
				}
			}
		}
		if (num != -1)
		{
			return num;
		}
		if (num2 >= 0)
		{
			animBlends[num2].animState.AnimTime = animTime;
			animBlends[num2].Start(0f, 1f, duration, easeBegin, easeEnd);
			animBlends[num2].Start(anim, this, act, rawAnim);
			return num2;
		}
		Debug.LogWarning("Too many simultaneous animations! Latest (" + anim.clip.name + ") will not play.");
		return -1;
	}

	public void EnableCategory(int categoryHandle, bool OnOff, float BlendTime)
	{
		EnableCategory(categoryHandle, OnOff, BlendTime, RawAnimation.AnimBlendType.kSoftSoft);
	}

	public void EnableCategoryRetainWeight(int categoryHandle)
	{
		CategoryInfo categoryInfo = ActiveCategoryInfo[categoryHandle];
		categoryInfo.enabled = true;
	}

	public void EnableCategory(int categoryHandle, bool OnOff, float BlendTime, RawAnimation.AnimBlendType easing)
	{
		CategoryInfo categoryInfo = ActiveCategoryInfo[categoryHandle];
		if (OnOff)
		{
			if (!categoryInfo.enabled)
			{
				categoryInfo.blender.Start(0f, 1f, BlendTime, RawAnimation.EaseBegin(easing), RawAnimation.EaseEnd(easing));
				categoryInfo.enabled = true;
			}
		}
		else if (categoryInfo.enabled)
		{
			categoryInfo.enabled = false;
			categoryInfo.blender.Start(categoryInfo.blender.weight, 0f, BlendTime, RawAnimation.EaseBegin(easing), RawAnimation.EaseEnd(easing));
		}
	}

	public void SetCategoryTimeSpeedWeightBlendTreeBlend(int categoryHandle, float AnimTime, float speed, float weight, float blendTreeBlend)
	{
		if (ActiveCategoryInfo[categoryHandle].enabled)
		{
			AnimationStateArray activeCategoryState = GetActiveCategoryState(categoryHandle);
			if (activeCategoryState != null)
			{
				activeCategoryState.AnimTime = AnimTime;
				activeCategoryState.Speed = speed;
				ActiveCategoryInfo[categoryHandle].blender.ManualWeight(weight);
				ActiveCategoryInfo[categoryHandle].blendTreeBlend = blendTreeBlend;
			}
		}
	}

	public void SetCategoryTime(int categoryHandle, float AnimTime)
	{
		CategoryInfo categoryInfo = ActiveCategoryInfo[categoryHandle];
		if (categoryInfo.enabled && categoryInfo.currentAnimBlender >= 0)
		{
			AnimationStateArray animState = animBlends[categoryInfo.currentAnimBlender].animState;
			animState.AnimTime = AnimTime;
		}
	}

	public void SetCategoryBlendTreeBlend(int categoryHandle, float btb)
	{
		if (ActiveCategoryInfo[categoryHandle].enabled)
		{
			ActiveCategoryInfo[categoryHandle].blendTreeBlend = btb;
		}
	}

	public void SetCategoryWeight(int categoryHandle, float Weight)
	{
		if (ActiveCategoryInfo[categoryHandle].enabled)
		{
			ActiveCategoryInfo[categoryHandle].blender.ManualWeight(Weight);
		}
	}

	public void SetCategorySpeed(int categoryHandle, float Speed)
	{
		CategoryInfo categoryInfo = ActiveCategoryInfo[categoryHandle];
		if (categoryInfo.currentAnimBlender != -1)
		{
			AnimationStateArray animState = animBlends[categoryInfo.currentAnimBlender].animState;
			animState.Speed = Speed;
		}
	}

	public float GetCategoryTime(int categoryHandle)
	{
		CategoryInfo categoryInfo = ActiveCategoryInfo[categoryHandle];
		if (categoryInfo.currentAnimBlender == -1)
		{
			return 0f;
		}
		AnimationStateArray animState = animBlends[categoryInfo.currentAnimBlender].animState;
		return animState.AnimTime;
	}

	public float GetCategoryLength(int categoryHandle)
	{
		CategoryInfo categoryInfo = ActiveCategoryInfo[categoryHandle];
		if (categoryInfo.currentAnimBlender == -1)
		{
			return 0f;
		}
		AnimationStateArray animState = animBlends[categoryInfo.currentAnimBlender].animState;
		return animState.length;
	}

	public void ForcePlayAnimation(RawAnimation rawAnim, int CategoryHandle, int ActionHandle)
	{
		forcedRawAnim = rawAnim;
		forcedRawAnimValid = true;
		PlayAction(CategoryHandle, ActionHandle, -1f, true, true);
	}

	public bool AddCategoryClip(RawAnimation rawAnim, int CategoryHandle)
	{
		return AddClip(rawAnim, CategoryHandle);
	}

	private void SetRawDefaultAnimation(RawAnimation rawAnim, int CategoryHandle, int ActionHandle)
	{
		DefaultAnimLibrary.AnimDefaults.Categories[CategoryHandle].Actions[ActionHandle].Anim.Anims[0] = rawAnim;
	}

	public void Finalise()
	{
		BuildAnimationList();
		ActiveCategoryInfo = new List<CategoryInfo>();
		for (int i = 0; i < DefaultAnimLibrary.AnimDefaults.Categories.Count; i++)
		{
			CategoryInfo categoryInfo = new CategoryInfo();
			categoryInfo.currentAnimBlender = -1;
			categoryInfo.chainedActionHandle = -1;
			categoryInfo.blender = new Blender();
			ActiveCategoryInfo.Add(categoryInfo);
		}
		CurrentOverrideStack = new int[DefaultAnimLibrary.AnimOverrides.Count];
		OverrideState = new bool[DefaultAnimLibrary.AnimOverrides.Count];
		for (int i = 0; i < CurrentOverrideStack.Length; i++)
		{
			CurrentOverrideStack[i] = -1;
			OverrideState[i] = false;
		}
		animBlends = new AnimBlender[12];
		for (int i = 0; i < 12; i++)
		{
			animBlends[i] = new AnimBlender();
		}
		categoriesToPlay = new bool[DefaultAnimLibrary.AnimDefaults.Categories.Count];
	}

	private void UpdateAnimBlend(int i)
	{
		if (!animBlends[i].UpdateAnimCheckDone(this, i) && animBlends[i].animState.wrapMode == WrapMode.Default && animBlends[i].deltaWeight == 0f)
		{
			float num = animBlends[i].animState.length - animBlends[i].animState.AnimTime;
			if (num < animBlends[i].blendOutTime)
			{
				animBlends[i].Start(animBlends[i].weight, 0f, num, BlendEasing.Soft, BlendEasing.Soft);
			}
		}
	}

	private void OnDisable()
	{
		if (enableAfterFirstUpdate)
		{
			if (GameModel != null)
			{
				GameModel.SetActive(true);
			}
			enableAfterFirstUpdate = false;
		}
	}

	private void Update()
	{
		if (enableAfterFirstUpdate)
		{
			if (GameModel != null)
			{
				GameModel.SetActive(true);
			}
			enableAfterFirstUpdate = false;
		}
		if (!ShouldAnimate())
		{
			return;
		}
		for (int i = 0; i < ActiveCategoryInfo.Count; i++)
		{
			ActiveCategoryInfo[i].blender.Update();
		}
		AnimsPreventReloading = false;
		MonopolyLayer = 0;
		int num = ActiveCategoryInfo.Count;
		int num2 = 0;
		while (num > 0)
		{
			num--;
			int currentAnimBlender = ActiveCategoryInfo[num].currentAnimBlender;
			if (currentAnimBlender >= 0)
			{
				UpdateAnimBlend(currentAnimBlender);
				num2 |= 1 << (currentAnimBlender & 0x1F);
			}
		}
		for (num = 0; num < 12; num++)
		{
			if ((num2 & (1 << num)) == 0)
			{
				UpdateAnimBlend(num);
			}
		}
		for (num = 0; num < ActiveCategoryInfo.Count; num++)
		{
			if (ActiveCategoryInfo[num].chainedActionHandle >= 0 && ActiveCategoryInfo[num].currentAnimBlender == -1 && AnimationPlayer != null)
			{
				int chainedCategoryHandle = ActiveCategoryInfo[num].chainedCategoryHandle;
				int chainedActionHandle = ActiveCategoryInfo[num].chainedActionHandle;
				float chainedActionStartTime = ActiveCategoryInfo[num].chainedActionStartTime;
				PlayAction(chainedCategoryHandle, chainedActionHandle);
				SetCategoryTime(chainedCategoryHandle, chainedActionStartTime);
				ActiveCategoryInfo[num].chainedActionHandle = -1;
				ActiveCategoryInfo[num].chainedCategoryHandle = -1;
			}
		}
		if (ForceUpdateUntil > 0f && ForceUpdateUntil < Time.time)
		{
			ForceUpdateUntil = 0f;
			AnimationPlayer.cullingType = DefaultCulling;
		}
		if (AnimationPlayer.animatePhysics)
		{
			AnimationPlayer.Sample();
		}
	}

	private void ValidatePlayingAnimations()
	{
	}

	public bool HasCurrentActionCompleted(int categoryHandle)
	{
		if (categoryHandle != -1)
		{
			AnimationStateArray activeCategoryState = GetActiveCategoryState(categoryHandle);
			if (activeCategoryState != null)
			{
				return !activeCategoryState.enabled;
			}
			return true;
		}
		return false;
	}

	private void PlayTestAction()
	{
		int categoryHandle = GetCategoryHandle("Movement");
		int actionIndex = GetActionIndex(categoryHandle, "Left");
		PlayAction(categoryHandle, actionIndex);
	}

	private AnimationClip GetActiveCategoryClip(int categoryHandle)
	{
		int currentAnimBlender = ActiveCategoryInfo[categoryHandle].currentAnimBlender;
		if (currentAnimBlender >= 0)
		{
			return animBlends[currentAnimBlender].animState.animState[0].clip;
		}
		return null;
	}

	private AnimationStateArray GetActiveCategoryState(int categoryHandle)
	{
		int currentAnimBlender = ActiveCategoryInfo[categoryHandle].currentAnimBlender;
		if (currentAnimBlender >= 0)
		{
			return animBlends[currentAnimBlender].animState;
		}
		return null;
	}

	private int GetActiveCategoryAction(int categoryHandle)
	{
		int currentAnimBlender = ActiveCategoryInfo[categoryHandle].currentAnimBlender;
		if (currentAnimBlender >= 0)
		{
			return animBlends[currentAnimBlender].myAction;
		}
		return -1;
	}

	private RawAnimation FindAnimRawAnim(int categoryHandle, int actionHandle, bool IgnoreOverrides)
	{
		RawAnimation rawAnimation = null;
		if (forcedRawAnimValid)
		{
			rawAnimation = forcedRawAnim;
			forcedRawAnim = null;
			forcedRawAnimValid = false;
			return rawAnimation;
		}
		if (categoryHandle < 0 || categoryHandle >= DefaultAnimLibrary.AnimDefaults.Categories.Count)
		{
			return null;
		}
		AnimCategory animCategory = DefaultAnimLibrary.AnimDefaults.Categories[categoryHandle];
		if (actionHandle < 0 || actionHandle >= animCategory.Actions.Count)
		{
			return null;
		}
		AnimAction animAction = animCategory.Actions[actionHandle];
		if (!IgnoreOverrides)
		{
			int num = 0;
			int count = ActiveOverrides.Count;
			for (num = 0; num < count; num++)
			{
				AnimOverride animOverride = ActiveOverrides[num];
				for (int i = 0; i < animOverride.CategoryOverrides.Count; i++)
				{
					AnimCategoryOverride animCategoryOverride = animOverride.CategoryOverrides[i];
					if (animCategoryOverride.CategoryIndex != categoryHandle || animCategoryOverride.ActionOverrides == null || actionHandle <= -1 || actionHandle >= animCategoryOverride.ActionOverrides.Count)
					{
						continue;
					}
					AnimActionOverride animActionOverride = animCategoryOverride.ActionOverrides[actionHandle];
					AnimAction actionOverride = animActionOverride.ActionOverride;
					DirectedAnim anim = actionOverride.Anim;
					if (anim.Anims != null && anim.Anims.Count > 0)
					{
						int index = 0;
						if (anim.PickRandom)
						{
							index = Random.Range(0, anim.Anims.Count);
						}
						if (anim.Anims[index].AnimClip != null)
						{
							rawAnimation = anim.Anims[index];
						}
					}
				}
			}
		}
		if (rawAnimation == null)
		{
			DirectedAnim anim2 = animAction.Anim;
			rawAnimation = ((!anim2.PickRandom) ? anim2.Anims[0] : anim2.Anims[Random.Range(0, anim2.Anims.Count)]);
		}
		if (rawAnimation != null && !clipAdded[rawAnimation.RefIndex])
		{
			clipAdded[rawAnimation.RefIndex] = true;
			AddClip(rawAnimation, categoryHandle);
		}
		return rawAnimation;
	}

	private void BuildAnimationList()
	{
		TBFAssert.DoAssert(DefaultAnimLibrary != null, "No animation library attached to the AnimDirector");
		while (AnimationPlayer.GetClipCount() > 0)
		{
			foreach (AnimationState item in AnimationPlayer)
			{
				if ((bool)item && (bool)item.clip)
				{
					AnimationPlayer.RemoveClip(item.clip);
					break;
				}
			}
		}
		int num = 0;
		bool flag = DefaultAnimLibrary.totalAnimCount == 0;
		if (flag)
		{
			for (int i = 0; i < DefaultAnimLibrary.AnimDefaults.Categories.Count; i++)
			{
				for (int j = 0; j < DefaultAnimLibrary.AnimDefaults.Categories[i].Actions.Count; j++)
				{
					for (int k = 0; k < DefaultAnimLibrary.AnimDefaults.Categories[i].Actions[j].Anim.Anims.Count; k++)
					{
						DefaultAnimLibrary.AnimDefaults.Categories[i].Actions[j].Anim.Anims[k].RefIndex = DefaultAnimLibrary.totalAnimCount++;
					}
				}
				num++;
			}
		}
		num = 0;
		for (int l = 0; l < DefaultAnimLibrary.AnimDefaults.Categories.Count; l++)
		{
			for (int m = 0; m < DefaultAnimLibrary.AnimOverrides.Count; m++)
			{
				for (int n = 0; n < DefaultAnimLibrary.AnimOverrides[m].CategoryOverrides.Count; n++)
				{
					if (DefaultAnimLibrary.AnimOverrides[m].CategoryOverrides[n].DefaultCategory == DefaultAnimLibrary.AnimDefaults.Categories[l])
					{
						DefaultAnimLibrary.AnimOverrides[m].CategoryOverrides[n].CategoryIndex = num;
					}
				}
				for (int num2 = 0; num2 < DefaultAnimLibrary.AnimOverrides[m].OverrideStacks.Count; num2++)
				{
					if (DefaultAnimLibrary.AnimOverrides[m].OverrideStacks[num2].stackThisIndex == -1)
					{
						DefaultAnimLibrary.AnimOverrides[m].OverrideStacks[num2].stackThisIndex = GetOverrideHandle(DefaultAnimLibrary.AnimOverrides[m].OverrideStacks[num2].stackThis);
						DefaultAnimLibrary.AnimOverrides[m].OverrideStacks[num2].onThatIndex = GetOverrideHandle(DefaultAnimLibrary.AnimOverrides[m].OverrideStacks[num2].onThat);
					}
				}
			}
			num++;
		}
		if (flag)
		{
			for (int num3 = 0; num3 < DefaultAnimLibrary.AnimOverrides.Count; num3++)
			{
				num = 0;
				for (int num4 = 0; num4 < DefaultAnimLibrary.AnimOverrides[num3].CategoryOverrides.Count; num4++)
				{
					num = DefaultAnimLibrary.AnimOverrides[num3].CategoryOverrides[num4].CategoryIndex;
					for (int num5 = 0; num5 < DefaultAnimLibrary.AnimOverrides[num3].CategoryOverrides[num4].ActionOverrides.Count; num5++)
					{
						AnimAction actionOverride = DefaultAnimLibrary.AnimOverrides[num3].CategoryOverrides[num4].ActionOverrides[num5].ActionOverride;
						for (int num6 = 0; num6 < actionOverride.Anim.Anims.Count; num6++)
						{
							actionOverride.Anim.Anims[num6].RefIndex = DefaultAnimLibrary.totalAnimCount++;
						}
					}
				}
			}
		}
		clipAdded = new bool[DefaultAnimLibrary.totalAnimCount];
	}

	private bool AddClip(RawAnimation rawAnimRoot, int categoryIndex)
	{
		if (categoryIndex < DefaultAnimLibrary.AnimDefaults.Categories.Count)
		{
			AnimCategory animCategory = DefaultAnimLibrary.AnimDefaults.Categories[categoryIndex];
			string text = null;
			for (int i = 0; i < rawAnimRoot.branchCount; i++)
			{
				RawAnimation branch = rawAnimRoot.GetBranch(i);
				AnimationClip animClip = branch.AnimClip;
				if (!(animClip != null))
				{
					continue;
				}
				text = animClip.name;
				bool multibone = branch.Multibone;
				string text2 = branch.OverrideBone;
				if (text2 == null || text2.Length == 0)
				{
					text2 = animCategory.FromBone;
					multibone = animCategory.Multibone;
				}
				if (branch.Looping)
				{
					animClip.wrapMode = WrapMode.Loop;
				}
				else if (branch.Clamp)
				{
					animClip.wrapMode = WrapMode.ClampForever;
				}
				else
				{
					animClip.wrapMode = WrapMode.Default;
				}
				if (AnimationPlayer.GetClip(text) == null)
				{
					AnimationPlayer.AddClip(animClip, text);
					AnimationPlayer[text].layer = categoryIndex;
				}
				else if (AnimationPlayer[text].layer != categoryIndex)
				{
					return false;
				}
				if (text2 != null && text2.Length > 0 && text2 != "All")
				{
					if (multibone)
					{
						string[] array = text2.Split(';');
						string[] array2 = array;
						foreach (string text3 in array2)
						{
							Transform transform = GameModel.transform.Find(text3);
							if (!(transform == null))
							{
								AnimationPlayer[text].AddMixingTransform(transform);
							}
						}
					}
					else
					{
						Transform transform2 = GameModel.transform.Find(text2);
						if (!(transform2 == null))
						{
							AnimationPlayer[text].AddMixingTransform(transform2);
						}
					}
				}
				else if (!branch.ForceAdditive && animCategory.BlendType != AnimCategory.BlendingType.Add)
				{
					branch.Monopoly = true;
				}
			}
			return true;
		}
		return false;
	}
}
