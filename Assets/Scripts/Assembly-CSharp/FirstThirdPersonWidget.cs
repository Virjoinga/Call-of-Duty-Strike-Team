using System.Collections;
using UnityEngine;

public class FirstThirdPersonWidget : MonoBehaviour
{
	private Vector3 mRefNodePos;

	private Quaternion mRefNodeRot;

	private bool mIsPolling;

	private bool mHoldFinishPose;

	private AnimationClip mAnimClip;

	public bool Reset(Actor actor, AnimationClip firstPersonClip, float animSpeed, Vector3 refNodePos, Quaternion refNodeRot, string weaponID, bool dontClearDepth)
	{
		if (firstPersonClip == null)
		{
			CommonHudController.Instance.IsContextActionInProgress = false;
			return false;
		}
		if (mIsPolling)
		{
			TearDown();
		}
		mRefNodePos = refNodePos;
		mRefNodeRot = refNodeRot;
		StartCoroutine(Poll(actor, firstPersonClip, animSpeed, weaponID, dontClearDepth));
		return true;
	}

	private IEnumerator Poll(Actor actor, AnimationClip firstPersonClip, float animSpeed, string weaponID, bool dontClearDepth)
	{
		float startTime = Time.time;
		float endTime = startTime + firstPersonClip.length * (1f / animSpeed);
		mAnimClip = firstPersonClip;
		if (endTime - startTime > 1f / 30f)
		{
			mIsPolling = true;
			CommonHudController.Instance.IsContextActionInProgress = true;
			if (weaponID == null || weaponID.Length == 0)
			{
				weaponID = "Empty";
			}
			float animTime3 = 0f;
			do
			{
				if (GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.ident == actor.ident && ViewModelRig.Instance() != null)
				{
					animTime3 = (Time.time - startTime) * animSpeed;
					animTime3 = Mathf.Clamp(animTime3, 0f, firstPersonClip.length);
					ViewModelRig.Instance().SetOverride(weaponID, firstPersonClip, animTime3, mRefNodePos, mRefNodeRot, dontClearDepth);
				}
				yield return 0;
			}
			while (Time.time <= endTime);
			while (mHoldFinishPose)
			{
				yield return 0;
			}
		}
		TearDown();
		yield return 0;
	}

	public void FinishEarly()
	{
		TearDown();
	}

	private void TearDown()
	{
		mIsPolling = false;
		mHoldFinishPose = false;
		ViewModelRig.Instance().ClearOverride();
		CommonHudController.Instance.IsContextActionInProgress = false;
	}

	public float GetCurrentAnimLength()
	{
		return (!mIsPolling) ? 0f : mAnimClip.length;
	}

	public void HoldAnimFinishPose()
	{
		mHoldFinishPose = true;
	}

	public void ClearAnimFinishPoseHold()
	{
		mHoldFinishPose = false;
	}
}
