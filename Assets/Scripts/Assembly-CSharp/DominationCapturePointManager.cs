using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DominationCapturePointManager : MonoBehaviour
{
	public GameObject FailLevelObjective;

	[HideInInspector]
	public int TotalUnderPlayerControl;

	public List<GameObject> DominationObjects = new List<GameObject>();

	private List<DominationCapturePoint> mCapturePoints = new List<DominationCapturePoint>();

	private bool mWarnPlayer = true;

	private bool mCongratulatePlayer = true;

	private string dial = string.Empty;

	private static DominationCapturePointManager m_Instance;

	public static DominationCapturePointManager Instance
	{
		get
		{
			return m_Instance;
		}
	}

	private void Awake()
	{
		m_Instance = this;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Start()
	{
		DominationCapturePoint[] array = Object.FindObjectsOfType(typeof(DominationCapturePoint)) as DominationCapturePoint[];
		DominationCapturePoint[] array2 = array;
		foreach (DominationCapturePoint item in array2)
		{
			mCapturePoints.Add(item);
		}
	}

	private void Update()
	{
		int num = 0;
		TotalUnderPlayerControl = 0;
		foreach (DominationCapturePoint mCapturePoint in mCapturePoints)
		{
			if (mCapturePoint.mCurrentStatus == DominationCapturePoint.CapturePointStatusEnum.EnemyControlled)
			{
				num++;
			}
			else if (mCapturePoint.mCurrentStatus == DominationCapturePoint.CapturePointStatusEnum.PlayerControlled)
			{
				TotalUnderPlayerControl++;
			}
			if (num == mCapturePoints.Count)
			{
				if (mWarnPlayer)
				{
					dial = "S_DOMINATION_DIALOGUE_ALLPOINTSLOST_VAR3";
					CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
					PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
					StartCoroutine("ResetWarning");
				}
			}
			else if (AllPointsHeldByPlayer() && mCongratulatePlayer)
			{
				dial = "S_DOMINATION_DIALOGUE_ALLPOINTSWON";
				CommonHudController.Instance.MissionDialogueQueue.AddDialogueToQueue(dial);
				PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, DominationVOSFX.Instance, dial, false, 1f);
				StartCoroutine("ResetCongrats");
			}
		}
	}

	private IEnumerator ResetWarning()
	{
		mWarnPlayer = false;
		yield return new WaitForSeconds(30f);
		mWarnPlayer = true;
	}

	private IEnumerator ResetCongrats()
	{
		mCongratulatePlayer = false;
		yield return new WaitForSeconds(30f);
		mCongratulatePlayer = true;
	}

	public bool AllPointsHeldByPlayer()
	{
		return TotalUnderPlayerControl == mCapturePoints.Count;
	}
}
