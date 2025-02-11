using System.Collections.Generic;
using UnityEngine;

public class MysteryCacheController : MonoBehaviour
{
	public MysteryCacheControllerData m_Interface;

	public List<MysteryCache> mMysteryCaches = new List<MysteryCache>();

	private bool mActive;

	private float mTimeTillNextActivation;

	private int mActiveCache = -1;

	private List<int> iRandNumbers = new List<int>();

	private int iCurRandIdx;

	private void Start()
	{
		if (mMysteryCaches.Count == 0)
		{
			Debug.LogWarning("mMysteryCaches count = ZERO!");
			mActive = false;
		}
		else
		{
			Reset();
			GenerateRandomNumberRange(mMysteryCaches.Count);
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
		if (!(mTimeTillNextActivation <= 0f))
		{
			return;
		}
		if (mActiveCache == -1)
		{
			WeaponAmmo weaponAmmo = GameController.Instance.mFirstPersonActor.weapon.PrimaryWeapon.GetWeaponAmmo();
			if (weaponAmmo.Stashed < weaponAmmo.MaxAmmo)
			{
				GMGSFX.Instance.MysteryCacheAvailable.Play2D();
				mActiveCache = PickMeACache();
				Container.SendMessage(mMysteryCaches[mActiveCache].gameObject, "Activate");
			}
		}
		else if (!mMysteryCaches[mActiveCache].mActive)
		{
			Container.SendMessage(mMysteryCaches[mActiveCache].gameObject, "Deactivate");
			Reset();
		}
	}

	public int PickMeACache()
	{
		iCurRandIdx++;
		if (iCurRandIdx >= mMysteryCaches.Count)
		{
			GenerateRandomNumberRange(mMysteryCaches.Count);
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
