using System;
using UnityEngine;

[Serializable]
public class InheritableMovementParams
{
	public enum StanceOrder
	{
		Stand = 0,
		StandInCover = 1,
		CrouchFromStealth = 2,
		CrouchInCover = 3
	}

	public Vector3 mDestination;

	public BaseCharacter.MovementStyle mMovementStyle;

	public float mLookTowardsDistSqr;

	public float mDestinationThresholdSqr;

	public Vector3 mFinalLookAt;

	public bool mFinalLookAtValid;

	public StanceOrder stanceAtEnd;

	public bool mGunAwayAtEnd;

	public bool navigateDoors;

	public bool stopDead;

	public bool abortOnExitTether;

	public bool holdCoverWhenBored;

	public bool holdCoverWhenFlanked;

	public float minimumCoverTime;

	public GuidRef GR_coverCluster = new GuidRef();

	public bool forceCrouch;

	public bool forceFaceForwards;

	public GameObject mFinalLookAtObject;

	public float DestinationThreshold
	{
		set
		{
			mDestinationThresholdSqr = value * value;
		}
	}

	public Vector3 FinalLookAt
	{
		set
		{
			mFinalLookAt = value;
			mFinalLookAtValid = true;
		}
	}

	public GameObject FinalLookAtObject
	{
		set
		{
			mFinalLookAtObject = value;
		}
	}

	public Vector3 FinalLookDirection
	{
		set
		{
			mFinalLookAt = mDestination + value;
			mFinalLookAtValid = true;
		}
	}

	public float LookTowardsDist
	{
		set
		{
			mLookTowardsDistSqr = value * value;
		}
	}

	public CoverCluster coverCluster
	{
		get
		{
			if (GR_coverCluster.theObject == null)
			{
				return null;
			}
			return GR_coverCluster.theObject.GetComponent<CoverCluster>();
		}
		set
		{
			if (value == null)
			{
				GR_coverCluster.theObject = null;
			}
			else
			{
				GR_coverCluster.theObject = value.gameObject;
			}
		}
	}

	public InheritableMovementParams()
	{
		stopDead = false;
		mMovementStyle = BaseCharacter.MovementStyle.Walk;
		mLookTowardsDistSqr = 6.25f;
		mDestinationThresholdSqr = 0f;
		mFinalLookAtValid = false;
		stanceAtEnd = StanceOrder.Stand;
		navigateDoors = true;
		abortOnExitTether = false;
		holdCoverWhenBored = false;
		holdCoverWhenFlanked = false;
		minimumCoverTime = 0f;
		forceCrouch = false;
		forceFaceForwards = false;
	}

	public InheritableMovementParams(BaseCharacter.MovementStyle ms)
	{
		stopDead = false;
		mMovementStyle = ms;
		mLookTowardsDistSqr = 6.25f;
		mDestinationThresholdSqr = 0f;
		mFinalLookAtValid = false;
		stanceAtEnd = StanceOrder.Stand;
		navigateDoors = true;
		holdCoverWhenBored = false;
		holdCoverWhenFlanked = false;
		minimumCoverTime = 0f;
		forceCrouch = false;
		forceFaceForwards = false;
	}

	public InheritableMovementParams(Vector3 destination)
	{
		stopDead = false;
		mDestination = destination;
		mMovementStyle = BaseCharacter.MovementStyle.Walk;
		mLookTowardsDistSqr = 6.25f;
		mDestinationThresholdSqr = 0f;
		mFinalLookAtValid = false;
		stanceAtEnd = StanceOrder.Stand;
		navigateDoors = true;
		holdCoverWhenBored = false;
		holdCoverWhenFlanked = false;
		minimumCoverTime = 0f;
		forceCrouch = false;
		forceFaceForwards = false;
	}

	public InheritableMovementParams(BaseCharacter.MovementStyle ms, Vector3 destination)
	{
		stopDead = false;
		mDestination = destination;
		mMovementStyle = ms;
		mLookTowardsDistSqr = 6.25f;
		mDestinationThresholdSqr = 0f;
		mFinalLookAtValid = false;
		stanceAtEnd = StanceOrder.Stand;
		navigateDoors = true;
		holdCoverWhenBored = false;
		holdCoverWhenFlanked = false;
		minimumCoverTime = 0f;
		forceCrouch = false;
		forceFaceForwards = false;
	}

	public InheritableMovementParams Clone()
	{
		InheritableMovementParams inheritableMovementParams = new InheritableMovementParams();
		inheritableMovementParams.mDestination = mDestination;
		inheritableMovementParams.mMovementStyle = mMovementStyle;
		inheritableMovementParams.mLookTowardsDistSqr = mLookTowardsDistSqr;
		inheritableMovementParams.mDestinationThresholdSqr = mDestinationThresholdSqr;
		inheritableMovementParams.mFinalLookAt = mFinalLookAt;
		inheritableMovementParams.mFinalLookAtValid = mFinalLookAtValid;
		inheritableMovementParams.stanceAtEnd = stanceAtEnd;
		inheritableMovementParams.mGunAwayAtEnd = mGunAwayAtEnd;
		inheritableMovementParams.navigateDoors = navigateDoors;
		inheritableMovementParams.stopDead = stopDead;
		inheritableMovementParams.abortOnExitTether = abortOnExitTether;
		inheritableMovementParams.holdCoverWhenBored = holdCoverWhenBored;
		inheritableMovementParams.holdCoverWhenFlanked = holdCoverWhenFlanked;
		inheritableMovementParams.minimumCoverTime = minimumCoverTime;
		inheritableMovementParams.coverCluster = coverCluster;
		inheritableMovementParams.forceCrouch = forceCrouch;
		inheritableMovementParams.forceFaceForwards = forceFaceForwards;
		inheritableMovementParams.mFinalLookAtObject = mFinalLookAtObject;
		return inheritableMovementParams;
	}

	public bool CrouchAtEnd()
	{
		return stanceAtEnd > StanceOrder.StandInCover;
	}

	public void ResolveGuidLinks()
	{
		GR_coverCluster.ResolveLink();
	}
}
