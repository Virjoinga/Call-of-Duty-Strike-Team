using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
	public MusicTriggerData m_Interface;

	private List<Actor> mEnemiesWithin = new List<Actor>();

	private bool mHasTriggered;

	private bool mWasForced;

	private bool mPreviouslyHadEnemies;

	private bool mRequestedPlay;

	private void Awake()
	{
		if (m_Interface != null)
		{
			SetTriggerActiveness(m_Interface.StartEnabled);
		}
	}

	private void SetTriggerActiveness(bool isActive)
	{
		if (base.GetComponent<Collider>() != null)
		{
			base.GetComponent<Collider>().enabled = isActive;
		}
	}

	public void EnableTrigger()
	{
		if (m_Interface == null || !m_Interface.OnlyTriggerOnce || !mHasTriggered)
		{
			mEnemiesWithin = new List<Actor>();
			mWasForced = false;
			mPreviouslyHadEnemies = false;
			mRequestedPlay = false;
			SetTriggerActiveness(true);
		}
	}

	public void DisableTrigger()
	{
		Deactivate();
		SetTriggerActiveness(false);
	}

	public void Activate()
	{
		PlayTrack();
		mWasForced = true;
	}

	public void Deactivate()
	{
		if (mRequestedPlay && m_Interface != null && MusicManager.Instance != null)
		{
			MusicManager.Instance.ScriptStopMusic(m_Interface.GetTrackNameAsString(), 129f + (float)m_Interface.Priority, m_Interface.FadeOutTime);
			mRequestedPlay = false;
		}
		mWasForced = false;
		mPreviouslyHadEnemies = false;
		SpreadMessageToGroup(m_Interface.GroupObjectToCallOnAllDead, m_Interface.GroupFunctionToCallOnAllDead);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!(base.GetComponent<Collider>() == null) && base.GetComponent<Collider>().enabled)
		{
			Actor componentInChildren = other.gameObject.GetComponentInChildren<Actor>();
			if (!(componentInChildren == null) && IsAnEnemy(componentInChildren) && !componentInChildren.realCharacter.IsDead())
			{
				mEnemiesWithin.Add(componentInChildren);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!(base.GetComponent<Collider>() == null) && base.GetComponent<Collider>().enabled)
		{
			Actor componentInChildren = other.gameObject.GetComponentInChildren<Actor>();
			if (!(componentInChildren == null) && IsAnEnemy(componentInChildren))
			{
				RemoveEnemy(componentInChildren);
			}
		}
	}

	private bool IsAnEnemy(Actor actor)
	{
		return actor.awareness.faction == FactionHelper.Category.Enemy || actor.awareness.faction == FactionHelper.Category.SoloEnemy;
	}

	private void RemoveEnemy(Actor actor)
	{
		if (mEnemiesWithin.Contains(actor))
		{
			mEnemiesWithin.Remove(actor);
			mPreviouslyHadEnemies = true;
		}
	}

	private void Update()
	{
		if (m_Interface != null && MusicManager.Instance != null)
		{
			m_Interface.CurrentlyPlaying = MusicManager.Instance.CurrentlyPlaying;
		}
		if (!mWasForced && m_Interface != null && MusicManager.Instance != null)
		{
			bool flag = AreConditionsMet();
			if (!MusicManager.Instance.IsPlayingTrack(m_Interface.GetTrackNameAsString()) && flag)
			{
				PlayTrack();
			}
			if ((!flag && mRequestedPlay) || (mPreviouslyHadEnemies && mEnemiesWithin.Count == 0))
			{
				Deactivate();
			}
		}
	}

	private bool AreConditionsMet()
	{
		if (mEnemiesWithin == null || mEnemiesWithin.Count == 0)
		{
			return false;
		}
		for (int num = mEnemiesWithin.Count - 1; num >= 0; num--)
		{
			if (mEnemiesWithin[num].realCharacter == null || (mEnemiesWithin[num].realCharacter != null && mEnemiesWithin[num].realCharacter.IsDead()))
			{
				RemoveEnemy(mEnemiesWithin[num]);
			}
			else
			{
				BehaviourController.AlertState alertState = mEnemiesWithin[num].behaviour.alertState;
				if (alertState == m_Interface.WhenEnemiesAre || alertState == m_Interface.Or || alertState == m_Interface._Or)
				{
					return true;
				}
			}
		}
		return false;
	}

	private void PlayTrack()
	{
		if (!mRequestedPlay && m_Interface != null && MusicManager.Instance != null)
		{
			mRequestedPlay = true;
			if (m_Interface.OnlyTriggerOnce && mHasTriggered)
			{
				Debug.LogWarning(string.Format("Music Trigger \"{0}\" has already been triggered", base.name));
				return;
			}
			MusicManager.Instance.ScriptPlay(m_Interface.GetTrackNameAsString(), m_Interface.Volume, m_Interface.FadeInTime, m_Interface.FadeOutTime, 129f + (float)m_Interface.Priority);
			mHasTriggered = true;
		}
	}

	private void SpreadMessageToGroup(List<GameObject> groupObjects, List<string> groupFuncs)
	{
		if (groupObjects == null || groupObjects.Count <= 0 || groupFuncs == null)
		{
			return;
		}
		string message = string.Empty;
		int count = groupObjects.Count;
		for (int i = 0; i < count; i++)
		{
			if (i < groupFuncs.Count)
			{
				message = groupFuncs[i];
			}
			Container.SendMessage(groupObjects[i], message, base.gameObject);
		}
	}

	public void OnDrawGizmoSelected()
	{
		BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
		if (boxCollider != null && boxCollider.enabled)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.red.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
