using UnityEngine;

public class StandardWeaponEquip : IWeaponEquip
{
	private IWeaponADS mWeaponADS;

	private GameObject mWeaponModel;

	private WeaponDescriptor mDescriptor;

	private float mTakeOutTime;

	private float mPutAwayTime;

	private float mTimeToTakeOut;

	private float mTimeToPutAway;

	private bool mNoWeapon;

	public StandardWeaponEquip(IWeaponADS weaponADS, GameObject weaponModel, WeaponDescriptor descriptor)
	{
		mWeaponADS = weaponADS;
		mWeaponModel = weaponModel;
		mDescriptor = descriptor;
	}

	public void Reset()
	{
		mPutAwayTime = float.MinValue;
		mTakeOutTime = float.MinValue;
	}

	public float GetEquipedAmount()
	{
		if (IsPuttingAway())
		{
			return 1f - Mathf.Clamp01((Time.time - mPutAwayTime) / mTimeToPutAway);
		}
		if (IsTakingOut())
		{
			return Mathf.Clamp01((Time.time - mTakeOutTime) / mTimeToTakeOut);
		}
		if (mNoWeapon)
		{
			return 0f;
		}
		return 1f;
	}

	public float GetEquipedBlendAmount()
	{
		if (IsPuttingAway())
		{
			return Mathf.Clamp01((Time.time - mPutAwayTime) / mTimeToPutAway);
		}
		if (IsTakingOut())
		{
			return Mathf.Clamp01((Time.time - mTakeOutTime) / mTimeToTakeOut);
		}
		if (mNoWeapon)
		{
			return 1f;
		}
		return 0f;
	}

	public bool IsPuttingAway()
	{
		return Time.time < mPutAwayTime + mTimeToPutAway;
	}

	public bool IsTakingOut()
	{
		return Time.time < mTakeOutTime + mTimeToTakeOut;
	}

	public void PutAway(float speedModifier)
	{
		mWeaponADS.SwitchToHips();
		if (!IsPuttingAway())
		{
			mPutAwayTime = Time.time;
			mTimeToPutAway = mDescriptor.PutAwayTime * (1f / speedModifier);
			if (IsTakingOut())
			{
				mPutAwayTime -= Mathf.Lerp(mTimeToPutAway, 0f, GetEquipedBlendAmount());
			}
			mTakeOutTime = 0f;
			mWeaponModel.SetActive(false);
		}
	}

	public void TakeOut(float speedModifier)
	{
		if (!IsTakingOut())
		{
			mTakeOutTime = Time.time;
			mTimeToTakeOut = mDescriptor.TakeOutTime * (1f / speedModifier);
			if (IsPuttingAway())
			{
				mTakeOutTime -= Mathf.Lerp(mTimeToTakeOut, 0f, GetEquipedBlendAmount());
			}
			mPutAwayTime = 0f;
			mWeaponModel.SetActive(true);
		}
	}

	public bool HasNoWeapon()
	{
		return mNoWeapon;
	}

	public void HasNoWeapon(bool noWeapon)
	{
		mNoWeapon = noWeapon;
	}
}
