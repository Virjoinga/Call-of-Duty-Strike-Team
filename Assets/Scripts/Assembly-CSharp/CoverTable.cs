using UnityEngine;

public class CoverTable : ScriptableObject
{
	public enum CoverProvided : byte
	{
		None = 0,
		Crouch = 1,
		Low = 2,
		High = 3,
		Full = 4,
		Stupid = 5,
		LowMask = 7,
		VisibilityUncertainAwayFromThere = 8,
		VisibilityUncertainAwayFromEither = 16,
		CoverHereWorseAwayFromThere = 32,
		ShootingFromCrouchToCrouchBlocked = 64,
		ShootingFromCrouchToStandingBlocked = 128
	}

	public enum VisibilityEstimate
	{
		Impossible = 0,
		Possible = 1,
		Certain = 2
	}

	public int[] crouchCoverUncertain;

	public byte[] table;

	public int mySize;

	public bool uncertaintyMapInvalid = true;

	public void CreateTable(int size)
	{
		table = new byte[size * size];
		mySize = size;
		CreateCrouchCoverUncertaintyTable();
	}

	private void CreateCrouchCoverUncertaintyTable()
	{
		crouchCoverUncertain = new int[mySize * mySize / 32 + 1];
		for (int i = 0; i < crouchCoverUncertain.Length; i++)
		{
			crouchCoverUncertain[i] = -1;
		}
	}

	public CoverProvided GetRawData(int ncp1, int ncp2)
	{
		return (CoverProvided)table[ncp1 + ncp2 * mySize];
	}

	public void SetRawData(int ncp1, int ncp2, CoverProvided cp)
	{
		table[ncp1 + ncp2 * mySize] = (byte)cp;
	}

	public void SetCoverProvided(int ncp1, int ncp2, CoverProvided c)
	{
		table[ncp1 + ncp2 * mySize] = (byte)c;
	}

	public CoverProvided GetCoverProvided(int ncp1, int ncp2)
	{
		CoverProvided coverProvided = (CoverProvided)table[ncp1 + ncp2 * mySize];
		return coverProvided & CoverProvided.LowMask;
	}

	public CoverProvided GetCoverProvided(int ncp1, int ncp2, out bool visibilityUncertain, out bool coverUncertain)
	{
		CoverProvided coverProvided = (CoverProvided)table[ncp1 + ncp2 * mySize];
		visibilityUncertain = (coverProvided & CoverProvided.VisibilityUncertainAwayFromThere) != 0;
		coverUncertain = (coverProvided & CoverProvided.CoverHereWorseAwayFromThere) != 0;
		return coverProvided & CoverProvided.LowMask;
	}

	public CoverProvided GetCoverProvidedAwayFromBoth(int ncp1, int ncp2, out bool visibilityUncertain, out bool coverUncertain)
	{
		CoverProvided coverProvided = (CoverProvided)table[ncp1 + ncp2 * mySize];
		visibilityUncertain = (coverProvided & CoverProvided.VisibilityUncertainAwayFromEither) != 0;
		coverUncertain = (coverProvided & CoverProvided.CoverHereWorseAwayFromThere) != 0;
		return coverProvided & CoverProvided.LowMask;
	}

	public void SetVisibilityUncertainAwayFromThere(int ncp1, int ncp2, bool val)
	{
		if (val)
		{
			table[ncp1 + ncp2 * mySize] |= 8;
		}
		else
		{
			table[ncp1 + ncp2 * mySize] &= 247;
		}
	}

	public void SetVisibilityUncertainAwayFromEither(int ncp1, int ncp2, bool val)
	{
		if (val)
		{
			table[ncp1 + ncp2 * mySize] |= 16;
		}
		else
		{
			table[ncp1 + ncp2 * mySize] &= 239;
		}
	}

	public bool GetVisibilityUncertainAwayFromThere(int ncp1, int ncp2)
	{
		return (table[ncp1 + ncp2 * mySize] & 8) != 0;
	}

	public bool GetVisibilityUncertainAwayFromEither(int ncp1, int ncp2)
	{
		return (table[ncp1 + ncp2 * mySize] & 0x10) != 0;
	}

	public void SetCoverHereWorseAwayFromThere(int ncp1, int ncp2, bool val)
	{
		if (val)
		{
			table[ncp1 + ncp2 * mySize] |= 32;
		}
		else
		{
			table[ncp1 + ncp2 * mySize] &= 223;
		}
	}

	public void SetShootingFromCrouchToCrouchBlocked(int ncp1, int ncp2, bool val)
	{
		if (val)
		{
			table[ncp1 + ncp2 * mySize] |= 64;
			table[ncp2 + ncp1 * mySize] |= 64;
		}
		else
		{
			table[ncp1 + ncp2 * mySize] &= 191;
			table[ncp2 + ncp1 * mySize] &= 191;
		}
	}

	public bool GetShootingFromCrouchToCrouchBlocked(int ncp1, int ncp2)
	{
		return (table[ncp1 + ncp2 * mySize] & 0x40) != 0;
	}

	public void SetShootingFromCrouchToStandingBlocked(int ncp1, int ncp2, bool val)
	{
		if (val)
		{
			table[ncp1 + ncp2 * mySize] |= 128;
		}
		else
		{
			table[ncp1 + ncp2 * mySize] &= 127;
		}
	}

	public bool GetShootingFromCrouchToStandingBlocked(int ncp1, int ncp2)
	{
		return (table[ncp1 + ncp2 * mySize] & 0x80) != 0;
	}

	public void SetCrouchCoverUncertain(int ncp1, int ncp2, bool val)
	{
		int num = ncp1 + ncp2 * mySize;
		int num2 = 1 << (num & 0x1F);
		if (val)
		{
			crouchCoverUncertain[num >> 5] |= num2;
		}
		else
		{
			crouchCoverUncertain[num >> 5] &= ~num2;
		}
	}

	public bool GetCrouchCoverUncertain(int ncp1, int ncp2)
	{
		int num = ncp1 + ncp2 * mySize;
		int num2 = 1 << (num & 0x1F);
		return (crouchCoverUncertain[num >> 5] & num2) != 0;
	}

	public VisibilityEstimate EstimateVisibility(int ncp1, bool docked1, int ncp2, bool docked2)
	{
		bool visibilityUncertain;
		bool coverUncertain;
		CoverProvided coverProvided;
		if (docked1)
		{
			if (docked2)
			{
				coverProvided = GetCoverProvided(ncp1, ncp2);
				if ((int)coverProvided >= 4)
				{
					return VisibilityEstimate.Impossible;
				}
				CoverProvided coverProvided2 = GetCoverProvided(ncp2, ncp1);
				if (coverProvided == CoverProvided.High || coverProvided2 == CoverProvided.High)
				{
					return VisibilityEstimate.Possible;
				}
				return VisibilityEstimate.Certain;
			}
			coverProvided = GetCoverProvided(ncp1, ncp2, out visibilityUncertain, out coverUncertain);
			if (coverProvided == CoverProvided.High || visibilityUncertain)
			{
				return VisibilityEstimate.Possible;
			}
			if ((int)coverProvided >= 4)
			{
				return VisibilityEstimate.Impossible;
			}
			return VisibilityEstimate.Certain;
		}
		coverProvided = GetCoverProvidedAwayFromBoth(ncp2, ncp1, out visibilityUncertain, out coverUncertain);
		if (visibilityUncertain || uncertaintyMapInvalid)
		{
			return VisibilityEstimate.Possible;
		}
		if ((int)coverProvided >= 4)
		{
			return VisibilityEstimate.Impossible;
		}
		return VisibilityEstimate.Certain;
	}

	public void FixUpAnythingMissing()
	{
		if (crouchCoverUncertain == null || crouchCoverUncertain.Length == 0)
		{
			CreateCrouchCoverUncertaintyTable();
		}
	}
}
