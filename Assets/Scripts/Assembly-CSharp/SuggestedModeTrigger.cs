using System.Collections.Generic;
using UnityEngine;

public class SuggestedModeTrigger : MonoBehaviour
{
	private const int NUM_FLASHES_BEFORE_DELAY = 3;

	private const float ANIMATION_TIME = 0.2f;

	private const float GAP_TIME = 0.15f;

	private const float DELAY_TIME = 3f;

	public SuggestedStateData m_Interface;

	public Collider TriggerVolume;

	public HudStateController.HudState SuggestedState;

	public bool RequireSelectedSoldiers = true;

	private List<Actor> mEntered;

	private float mTime;

	private int mNumFlashes;

	private bool mDelay;

	private bool mActive;

	private void Start()
	{
		if (TriggerVolume == null)
		{
			TriggerVolume = base.gameObject.GetComponent<Collider>();
		}
		SuggestedState = m_Interface.SuggestedState;
		RequireSelectedSoldiers = m_Interface.RequireSelectedSoldiers;
		mEntered = new List<Actor>();
		mDelay = false;
		mActive = false;
	}

	private void Update()
	{
		if (!mActive)
		{
			return;
		}
		HudStateController instance = HudStateController.Instance;
		if (instance.State == SuggestedState)
		{
			return;
		}
		bool flag = !RequireSelectedSoldiers;
		if (!flag)
		{
			GameplayController gameplayController = GameplayController.Instance();
			for (int i = 0; i < mEntered.Count; i++)
			{
				if (gameplayController != null && gameplayController.IsSelected(mEntered[i]))
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		mTime += Time.deltaTime;
		if ((mDelay || !(mTime >= 0.35000002f)) && (!mDelay || !(mTime >= 3f)))
		{
			return;
		}
		mTime = 0f;
		mDelay = false;
		if (SuggestedState == HudStateController.HudState.FPP && instance.State == HudStateController.HudState.TPP)
		{
			CommonHudController.Instance.FlashZoomButton(ColourChart.HudGreen, 0.2f);
		}
		else if (SuggestedState == HudStateController.HudState.TPP && instance.State == HudStateController.HudState.FPP)
		{
			CommonHudController.Instance.FlashZoomButton(ColourChart.HudGreen, 0.2f);
		}
		else if (instance.State == HudStateController.HudState.Strategy)
		{
			CommonHudController.Instance.FlashZoomButton(ColourChart.HudGreen, 0.2f);
		}
		else if (instance.State == HudStateController.HudState.FPPLockedMountedWeapon)
		{
			CommonHudController.Instance.FlashZoomButton(ColourChart.HudGreen, 0.2f);
		}
		mNumFlashes++;
		if (mNumFlashes >= 3)
		{
			mNumFlashes = 0;
			if (ActStructure.Instance.CurrentMissionSectionIsTutorial())
			{
				mDelay = true;
			}
			else
			{
				mActive = false;
			}
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		Actor component = other.GetComponent<Actor>();
		if (RequireSelectedSoldiers && component != null && component.behaviour.PlayerControlled && !mEntered.Contains(component))
		{
			mEntered.Add(component);
			Activate();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		Actor component = other.GetComponent<Actor>();
		if (RequireSelectedSoldiers && component != null && component.behaviour.PlayerControlled && mEntered.Contains(component))
		{
			mEntered.Remove(component);
			Deactivate();
		}
	}

	public void Activate()
	{
		mActive = true;
	}

	public void Deactivate()
	{
		if (mEntered.Count == 0)
		{
			mActive = false;
		}
	}

	public void OnDrawGizmos()
	{
		Vector3 center = base.transform.position + new Vector3(0f, 0.5f, 0f);
		Gizmos.DrawIcon(center, "Tutorial");
	}

	public void OnDrawGizmosSelected()
	{
		BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.magenta.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
