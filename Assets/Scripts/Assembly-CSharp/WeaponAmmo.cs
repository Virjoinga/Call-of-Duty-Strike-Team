using UnityEngine;

public class WeaponAmmo
{
	public bool UnlimitedAmmo;

	public static bool InfiniteAmmo;

	private WeaponDescriptor mDescriptor;

	private int mQuantityToRefundStash;

	private int mStashModifier;

	public int Loaded { get; private set; }

	public int Stashed { get; private set; }

	public int NumClips
	{
		get
		{
			int num = (int)mDescriptor.Capacity;
			return (Loaded + Stashed + (num - 1)) / num;
		}
	}

	public bool Available
	{
		get
		{
			return Loaded > 0 || Stashed > 0;
		}
	}

	public bool LowAmmo
	{
		get
		{
			return !UnlimitedAmmo && Stashed + Loaded < Mathf.CeilToInt(mDescriptor.Capacity / 100f * (float)mDescriptor.LowAmmoPercentage);
		}
	}

	public bool CanReload
	{
		get
		{
			return Loaded < (int)mDescriptor.Capacity && Stashed > 0;
		}
	}

	public bool NeedsReload
	{
		get
		{
			return Loaded <= 0 && (Stashed > 0 || UnlimitedAmmo);
		}
	}

	public float PercentageLoaded
	{
		get
		{
			return (float)Loaded / mDescriptor.Capacity;
		}
	}

	public int MaxAmmo
	{
		get
		{
			return mDescriptor.BulletStartQuantity + mStashModifier;
		}
	}

	public WeaponAmmo(WeaponDescriptor descriptor, float modifier)
	{
		mDescriptor = descriptor;
		int num = Mathf.CeilToInt((float)mDescriptor.BulletStartQuantity / mDescriptor.Capacity);
		int num2 = Mathf.CeilToInt((float)num * modifier) - num;
		mStashModifier = num2 * (int)mDescriptor.Capacity;
		Reset();
	}

	public void Reset()
	{
		Loaded = (int)mDescriptor.Capacity;
		Stashed = mDescriptor.BulletStartQuantity + mStashModifier;
		UnlimitedAmmo = mDescriptor.UnlimitedAmmo;
		mQuantityToRefundStash = 0;
	}

	public int Take(int quantity, bool affectStash)
	{
		if (InfiniteAmmo)
		{
			return quantity;
		}
		int num = Mathf.Min(quantity, Loaded);
		Loaded -= num;
		mQuantityToRefundStash += ((!affectStash) ? quantity : 0);
		return num;
	}

	public void Reload()
	{
		if (UnlimitedAmmo)
		{
			Loaded = (int)mDescriptor.Capacity;
		}
		else
		{
			Stashed += mQuantityToRefundStash;
			int num = Mathf.Min(Stashed, (int)mDescriptor.Capacity - Loaded);
			Loaded += num;
			Stashed -= num;
		}
		mQuantityToRefundStash = 0;
	}

	public void Reload(int quantity)
	{
		if (UnlimitedAmmo)
		{
			Loaded = Mathf.Min(Loaded + quantity, (int)mDescriptor.Capacity);
			mQuantityToRefundStash = 0;
			return;
		}
		int num = Mathf.Min(mQuantityToRefundStash, quantity);
		mQuantityToRefundStash -= num;
		Stashed += num;
		int num2 = Mathf.Min(Stashed, quantity);
		Loaded += num2;
		Stashed -= num2;
	}

	public bool Pickup(float quantity)
	{
		EventHub.Instance.Report(new Events.AmmoCollected((int)quantity));
		return AddAmmo((int)quantity);
	}

	public bool AddAmmo(int amount)
	{
		bool flag = false;
		if (Stashed != MaxAmmo)
		{
			Stashed += amount;
			if (Stashed > MaxAmmo)
			{
				Stashed = MaxAmmo;
			}
			flag = true;
		}
		else if (Loaded != (int)mDescriptor.Capacity)
		{
			Loaded += amount;
			if (Loaded > (int)mDescriptor.Capacity)
			{
				Loaded = (int)mDescriptor.Capacity;
			}
			flag = true;
		}
		if (flag && GameController.Instance.IsFirstPerson)
		{
			RealCharacter.SetUpdateFPPHudText();
			CommonHudController.Instance.AnimateAddedAmmo();
		}
		return flag;
	}

	public void AddAmmoByPercent(float amt)
	{
		AddAmmo((int)((float)MaxAmmo * amt));
		if (GameController.Instance.IsFirstPerson)
		{
			RealCharacter.SetUpdateFPPHudText();
			CommonHudController.Instance.AnimateAddedAmmo();
		}
	}

	public void FillAmmoByPercent(float amt)
	{
		Stashed = (int)((float)MaxAmmo * amt);
		if (GameController.Instance.IsFirstPerson)
		{
			RealCharacter.SetUpdateFPPHudText();
			CommonHudController.Instance.AnimateAddedAmmo();
		}
	}

	public float CalcReloadSfx(float oldTimeInAnim, float newTimeInAnim, bool isTactical, float[,] eventTimes, ref int whichEvent, ref int eventToPlay)
	{
		TBFAssert.DoAssert(eventTimes != null, "eventData null");
		int num = (isTactical ? 1 : 0);
		if (eventTimes[0, num] != -1f && GameController.Instance.mFirstPersonActor != null)
		{
			int length = eventTimes.GetLength(0);
			whichEvent = Mathf.Clamp(whichEvent, 0, length - 2);
			float num2 = eventTimes[whichEvent, num];
			if (oldTimeInAnim < num2 && newTimeInAnim >= num2)
			{
				eventToPlay = whichEvent;
				whichEvent++;
				if (eventTimes[whichEvent, num] == -1f)
				{
					whichEvent = 0;
				}
			}
		}
		return newTimeInAnim;
	}
}
