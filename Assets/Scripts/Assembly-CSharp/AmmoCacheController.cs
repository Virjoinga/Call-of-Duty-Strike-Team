using System.Collections.Generic;
using UnityEngine;

public class AmmoCacheController : MonoBehaviour
{
	public AmmoCacheControllerData m_Interface;

	public List<AmmoCache> mAmmoCaches = new List<AmmoCache>();

	private bool mActive = true;

	private float mTimeTillNextActivation;

	private int mActiveCache = -1;

	private List<int> iRandNumbers = new List<int>();

	private int iCurRandIdx;

	private void Start()
	{
		if (mAmmoCaches.Count == 0)
		{
			Debug.LogWarning("mAmmoCaches count = ZERO!");
			mActive = false;
		}
		else
		{
			Reset();
			GenerateRandomNumberRange(mAmmoCaches.Count);
			iCurRandIdx = -1;
		}
	}

	private void Update()
	{
		if (!mActive || !(GameController.Instance != null) || !(GameController.Instance.mFirstPersonActor != null))
		{
			return;
		}
		mTimeTillNextActivation -= Time.deltaTime;
		if (mTimeTillNextActivation <= 0f)
		{
			if (mActiveCache == -1)
			{
				WeaponAmmo weaponAmmo = GameController.Instance.mFirstPersonActor.weapon.PrimaryWeapon.GetWeaponAmmo();
				if (weaponAmmo.Stashed < weaponAmmo.MaxAmmo)
				{
					GMGSFX.Instance.AmmoCacheAvailable.Play2D();
					mActiveCache = PickMeACache();
					Container.SendMessage(mAmmoCaches[mActiveCache].gameObject, "Activate");
				}
			}
			else if (!mAmmoCaches[mActiveCache].mActive)
			{
				Container.SendMessage(mAmmoCaches[mActiveCache].gameObject, "Deactivate");
				Reset();
			}
		}
		if ((bool)GameController.Instance && (bool)GameController.Instance.mFirstPersonActor && mActiveCache != -1)
		{
			WeaponAmmo weaponAmmo2 = GameController.Instance.mFirstPersonActor.weapon.PrimaryWeapon.GetWeaponAmmo();
			if (weaponAmmo2.Stashed >= weaponAmmo2.MaxAmmo)
			{
				Container.SendMessage(mAmmoCaches[mActiveCache].gameObject, "Deactivate");
				Reset();
			}
		}
	}

	public int PickMeACache()
	{
		iCurRandIdx++;
		if (iCurRandIdx >= mAmmoCaches.Count)
		{
			GenerateRandomNumberRange(mAmmoCaches.Count);
		}
		return iRandNumbers[iCurRandIdx];
	}

	public void Reset()
	{
		mTimeTillNextActivation = m_Interface.TimeBetweenActivation;
		mActiveCache = -1;
	}

	public void Activate()
	{
		mTimeTillNextActivation = m_Interface.TimeBetweenActivation;
	}

	public void Deactivate()
	{
	}

	private void GenerateRandomNumberRange(int RangeSize)
	{
		iRandNumbers.Clear();
		int i;
		for (i = 0; i < RangeSize; i++)
		{
			iRandNumbers.Add(i);
		}
		i = RangeSize;
		while (i > 1)
		{
			int index = Random.Range(0, RangeSize);
			i--;
			int value = iRandNumbers[index];
			iRandNumbers[index] = iRandNumbers[i];
			iRandNumbers[i] = value;
		}
		iCurRandIdx = 0;
	}
}
