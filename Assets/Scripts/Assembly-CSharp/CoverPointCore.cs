using System;
using UnityEngine;

[Serializable]
public class CoverPointCore : ScriptableObject
{
	public enum Type
	{
		OpenGround = 0,
		ShootOver = 1,
		HighWall = 2,
		HighCornerLeft = 3,
		HighCornerRight = 4,
		InAir = 5
	}

	public const float kDontStandOnBodyTime = 10f;

	public Type type;

	public int index = -1;

	public Vector3 snappedPos;

	public Vector3 snappedNormal;

	public Vector3 snappedTangent;

	public Vector3 gamePos;

	public Vector3 coverCheckPos;

	private Actor occupant;

	private uint occupantIdent;

	private float checkInTime;

	public bool isLowLowCover;

	public int debugTag;

	public int[] neighbours;

	public int[] doorMasks;

	public int[] routeOnward;

	public int[] significantCover;

	public uint stupidCoverAgainst;

	public uint fullCoverAgainst;

	public uint highCoverAgainst;

	public uint lowCoverAgainst;

	public uint noCoverAgainst;

	public uint crouchCoverAgainst;

	public uint hiddenTo;

	public uint interestingTo;

	public uint goodForFlanking;

	public uint blocked;

	public bool stepOutLeft;

	public bool stepOutRight;

	public float minExtent = -0.5f;

	public float maxExtent = 0.5f;

	public float minProprietaryExtent = -0.5f;

	public float maxProprietaryExtent = 0.5f;

	public int adjacentLeft = -1;

	public int adjacentRight = -1;

	public float coverAngleSinL;

	public float coverAngleSinR;

	public int subsectionMask;

	public Vector2 cappedLookL;

	public Vector2 cappedLookR;

	private int earMarked;

	private static int earMarkVal = 1;

	private float[] DeathPenalty = new float[8];

	public Vector3 StandFacing
	{
		get
		{
			switch (type)
			{
			case Type.HighCornerLeft:
			case Type.HighCornerRight:
				return snappedTangent;
			case Type.HighWall:
				return snappedNormal;
			default:
				return snappedNormal * -1f;
			}
		}
	}

	public Vector3 AnimationFacing
	{
		get
		{
			if (type >= Type.HighWall)
			{
				return snappedNormal;
			}
			return snappedNormal * -1f;
		}
	}

	public Actor Occupant
	{
		get
		{
			return occupant;
		}
	}

	public float CheckInTime
	{
		get
		{
			return checkInTime;
		}
	}

	public void Expunge(uint invident)
	{
		fullCoverAgainst &= invident;
		highCoverAgainst &= invident;
		lowCoverAgainst &= invident;
		noCoverAgainst &= invident;
		hiddenTo &= invident;
		interestingTo &= invident;
		goodForFlanking &= invident;
		blocked &= invident;
		occupantIdent &= invident;
		if (occupantIdent == 0)
		{
			occupant = null;
		}
	}

	public static void ClearEarMarks()
	{
		earMarkVal++;
	}

	public void Init()
	{
		occupant = null;
		occupantIdent = 0u;
		checkInTime = 0f;
		earMarked = 0;
		for (int i = 0; i < DeathPenalty.Length; i++)
		{
			DeathPenalty[i] = -100f;
		}
	}

	public bool IsHighCornerCover()
	{
		return type == Type.HighCornerLeft || type == Type.HighCornerRight;
	}

	public bool Available(Actor a)
	{
		if (type == Type.OpenGround)
		{
			return false;
		}
		if (earMarked == earMarkVal)
		{
			return false;
		}
		if ((blocked & ~a.ident) != 0)
		{
			return false;
		}
		return occupantIdent == 0 || occupantIdent == a.ident;
	}

	public bool AvailableAndDesirable(Actor a)
	{
		if (type == Type.OpenGround)
		{
			return false;
		}
		if (earMarked == earMarkVal)
		{
			return false;
		}
		if (Time.time - DeathPenalty[(int)a.awareness.faction] < 10f)
		{
			return false;
		}
		if ((blocked & ~a.ident) != 0)
		{
			return false;
		}
		return occupantIdent == 0 || occupantIdent == a.ident;
	}

	public void EarMark()
	{
		earMarked = earMarkVal;
	}

	public bool Book(Actor ac, float eta)
	{
		if (occupantIdent == ac.ident)
		{
			return true;
		}
		if (occupantIdent != 0)
		{
			return false;
		}
		checkInTime = eta;
		occupant = ac;
		occupantIdent = ac.ident;
		return true;
	}

	public void Cancel(Actor ac)
	{
		if (occupantIdent == ac.ident)
		{
			occupant = null;
			occupantIdent = 0u;
		}
	}

	public void HiddenTo(uint ident)
	{
		hiddenTo |= ident;
		interestingTo &= ~ident;
	}

	public void VisibleTo(uint ident)
	{
		if ((hiddenTo & ident) != 0)
		{
			interestingTo |= ident;
			hiddenTo &= ~ident;
		}
	}

	public void FullCoverAgainst(uint ident)
	{
		uint num = ~ident;
		fullCoverAgainst |= ident;
		highCoverAgainst &= num;
		lowCoverAgainst &= num;
		noCoverAgainst &= num;
		stupidCoverAgainst &= num;
		crouchCoverAgainst &= num;
	}

	public void StupidCoverAgainst(uint ident)
	{
		uint num = ~ident;
		fullCoverAgainst |= ident;
		highCoverAgainst &= num;
		lowCoverAgainst &= num;
		noCoverAgainst &= num;
		stupidCoverAgainst |= ident;
		crouchCoverAgainst &= num;
	}

	public void HighCoverAgainst(uint ident)
	{
		uint num = ~ident;
		highCoverAgainst |= ident;
		fullCoverAgainst &= num;
		lowCoverAgainst &= num;
		noCoverAgainst &= num;
		stupidCoverAgainst &= num;
		crouchCoverAgainst &= num;
	}

	public void LowCoverAgainst(uint ident)
	{
		uint num = ~ident;
		lowCoverAgainst |= ident;
		highCoverAgainst &= num;
		fullCoverAgainst &= num;
		noCoverAgainst &= num;
		stupidCoverAgainst &= num;
		crouchCoverAgainst &= num;
	}

	public void CrouchCoverAgainst(uint ident)
	{
		uint num = ~ident;
		noCoverAgainst &= num;
		crouchCoverAgainst |= ident;
		highCoverAgainst &= num;
		lowCoverAgainst &= num;
		fullCoverAgainst &= num;
		stupidCoverAgainst &= num;
	}

	public void NoCoverAgainst(uint ident)
	{
		uint num = ~ident;
		noCoverAgainst |= ident;
		highCoverAgainst &= num;
		lowCoverAgainst &= num;
		fullCoverAgainst &= num;
		stupidCoverAgainst &= num;
		crouchCoverAgainst &= num;
	}

	public void GoodForFlanking(uint ident, bool val)
	{
		if (val)
		{
			goodForFlanking |= ident;
		}
		else
		{
			goodForFlanking &= ~ident;
		}
	}

	public CoverTable.CoverProvided CoverProvidedAgainstSteppingOut(Actor a, int step)
	{
		if ((noCoverAgainst & a.ident) != 0)
		{
			return CoverTable.CoverProvided.None;
		}
		Vector2 lhs = a.GetPosition().xz() - snappedPos.xz();
		float num = Vector2.Dot(lhs, snappedNormal.xz());
		float num2 = Vector2.Dot(lhs, snappedTangent.xz());
		if (num >= 0f)
		{
			return CoverTable.CoverProvided.None;
		}
		if (num2 <= 0f)
		{
			return CoverTable.CoverProvided.Full;
		}
		float num3 = (0f - num) / num2;
		switch (step)
		{
		case 1:
			if (num3 > 0.456f)
			{
				return CoverTable.CoverProvided.Full;
			}
			return CoverTable.CoverProvided.High;
		case 2:
			if (num3 < 0.456f)
			{
				return CoverTable.CoverProvided.None;
			}
			if (num3 < 1.211f)
			{
				return CoverTable.CoverProvided.High;
			}
			return CoverTable.CoverProvided.Full;
		case 3:
			if (num3 > 1.211f)
			{
				return CoverTable.CoverProvided.High;
			}
			return CoverTable.CoverProvided.None;
		default:
			return CoverTable.CoverProvided.Full;
		}
	}

	public void DeathNearby(Actor a)
	{
		uint hostile = GlobalKnowledgeManager.Instance().FactionHostileTo[(int)a.awareness.faction];
		for (int i = 0; i < neighbours.Length; i++)
		{
			float num = CoverNeighbour.navMeshDistance(neighbours[i]);
			if (num > 10f)
			{
				break;
			}
			CoverNeighbour.CoverPoint(neighbours[i]).ApplyDeathPenalty(hostile, num);
		}
	}

	private void ApplyDeathPenalty(uint hostile, float distance)
	{
		for (int i = 0; i < 8; i++)
		{
			if ((hostile & (uint)(1 << i)) == 0)
			{
				DeathPenalty[i] = Time.time - distance;
			}
		}
	}

	public void DetermineStanceNecessaryToShoot(Vector2 aimDir, out bool canShootFromCoverStance, out bool canShootFromCrouch)
	{
		canShootFromCrouch = false;
		canShootFromCoverStance = true;
		if (Vector2.Dot(aimDir, snappedNormal.xz()) >= 0f)
		{
			canShootFromCrouch = true;
			canShootFromCoverStance = false;
		}
		else
		{
			if (type != Type.ShootOver)
			{
				return;
			}
			float num = coverAngleSinL;
			if (Vector2.Dot(aimDir, snappedTangent.xz()) >= 0f)
			{
				num = coverAngleSinR;
			}
			float num2 = num * (0f - num);
			float num3 = Vector2.Dot(aimDir, snappedNormal.xz());
			float num4 = num3 * Mathf.Abs(num3) / aimDir.sqrMagnitude;
			if (num4 > -0.5f)
			{
				canShootFromCoverStance = false;
				if (num4 > num2)
				{
					canShootFromCrouch = true;
				}
			}
		}
	}
}
