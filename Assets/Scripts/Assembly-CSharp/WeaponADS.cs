using UnityEngine;

internal class WeaponADS
{
	private float mBlendTime;

	private float mTransitionDuration;

	private float mTransitionModifier;

	public bool WantsSights { get; set; }

	public ADSState State { get; private set; }

	public float BlendAmount
	{
		get
		{
			return WeaponUtils.CalculateHipsToSightsBlend(State, mBlendTime, mTransitionDuration * mTransitionModifier);
		}
	}

	public WeaponADS(float transitionDuration)
	{
		mTransitionModifier = 1f;
		mTransitionDuration = transitionDuration;
		Reset();
	}

	public WeaponADS(float transitionDuration, float modifier)
		: this(transitionDuration)
	{
		mTransitionModifier = modifier;
	}

	public void Reset()
	{
		State = ADSState.Hips;
		WantsSights = false;
	}

	public void Update(float deltaTime, bool suppressADS)
	{
		bool wantsSights = WantsSights && !suppressADS;
		UpdateState(wantsSights, mTransitionModifier);
		mBlendTime += deltaTime;
	}

	private void UpdateState(bool wantsSights, float transitionModifier)
	{
		if (wantsSights && State != 0)
		{
			mBlendTime = Mathf.Lerp(0f, mTransitionDuration * transitionModifier, BlendAmount);
			State = ADSState.SwitchingToADS;
		}
		else if (!wantsSights && State != ADSState.Hips)
		{
			mBlendTime = Mathf.Lerp(mTransitionDuration * transitionModifier, 0f, BlendAmount);
			State = ADSState.SwitchingToHips;
		}
		if (IsInTransition() && mBlendTime >= mTransitionDuration * transitionModifier)
		{
			State = ((State != ADSState.SwitchingToADS) ? ADSState.Hips : ADSState.ADS);
		}
	}

	private bool IsInTransition()
	{
		return State == ADSState.SwitchingToADS || State == ADSState.SwitchingToHips;
	}

	public void SetModifier(float amount)
	{
		mTransitionModifier = amount;
	}
}
