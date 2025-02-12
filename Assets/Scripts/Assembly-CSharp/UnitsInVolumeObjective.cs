using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class UnitsInVolumeObjective : MissionObjective
{
	public enum TriggerCheck
	{
		Any = 0,
		All_Alive = 1,
		Specific_Number = 2
	}

	private class UnitData
	{
		public GameObject mUnit;

		public HealthComponent mHealth;

		public bool mIsInArea;

		public UnitData(GameObject go)
		{
			mUnit = go;
			mHealth = mUnit.GetComponent<HealthComponent>();
			mIsInArea = false;
		}

		public bool IsAlive()
		{
			if (mHealth == null)
			{
				return true;
			}
			if (!mHealth.HealthEmpty && !mHealth.IsMortallyWounded())
			{
				return true;
			}
			return false;
		}
	}

	public UnitsInVolData m_UnitsVolInterface;

	[HideInInspector]
	public List<ActorWrapper> Actors = new List<ActorWrapper>();

	private DialogueManager diagManager;

	private UnitData[] mUnitsToCheck;

	private int mUnitsInArea;

	private int mUnitsAlive;

	private bool mUnitInAreaCheck;

	private bool PerformedCheckOnActivate;

	private bool mAlreadyFlashing;

	private GameObject mIcon;

	public override void Start()
	{
		base.Start();
		if (m_UnitsVolInterface.TriggerVolume == null)
		{
			m_UnitsVolInterface.TriggerVolume = base.gameObject.GetComponent<Collider>();
		}
		mUnitsToCheck = new UnitData[Actors.Count];
		foreach (ActorWrapper actor in Actors)
		{
			actor.TargetActorChanged += Setup;
		}
		if (m_UnitsVolInterface.PromptIfNotRequiredNumber != string.Empty)
		{
			Transform parent = base.transform;
			while (parent.parent != null)
			{
				parent = parent.parent;
			}
			diagManager = parent.gameObject.GetComponentInChildren<DialogueManager>();
			if (diagManager == null)
			{
				Debug.LogError("Unable to find dialogue manager!");
			}
		}
		UpdateBlipTarget();
	}

	private void Setup(object sender)
	{
		GameObject gameObject = sender as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		for (int i = 0; i < mUnitsToCheck.Length; i++)
		{
			if (mUnitsToCheck[i] == null)
			{
				mUnitsToCheck[i] = new UnitData(gameObject);
				break;
			}
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (ActorWrapper actor in Actors)
		{
			actor.TargetActorChanged -= Setup;
		}
	}

	private void UpdateUnitCounts()
	{
		mUnitsInArea = 0;
		mUnitsAlive = 0;
		mUnitInAreaCheck = false;
		UnitData[] array = mUnitsToCheck;
		foreach (UnitData unitData in array)
		{
			if (unitData == null)
			{
				continue;
			}
			if (unitData.IsAlive())
			{
				mUnitsAlive++;
				if (unitData.mIsInArea)
				{
					mUnitsInArea++;
				}
			}
			if (unitData.mIsInArea)
			{
				mUnitInAreaCheck = true;
			}
		}
	}

	private void PassCheck()
	{
		bool flag = false;
		switch (m_UnitsVolInterface.Check)
		{
		case TriggerCheck.Any:
			if (mUnitsInArea >= 1)
			{
				flag = true;
			}
			break;
		case TriggerCheck.All_Alive:
			if (mUnitsInArea == mUnitsAlive && mUnitsInArea == m_UnitsVolInterface.SpecificNumber)
			{
				flag = true;
			}
			break;
		case TriggerCheck.Specific_Number:
			if (mUnitsInArea >= m_UnitsVolInterface.SpecificNumber)
			{
				flag = true;
			}
			else if (m_UnitsVolInterface.SpecificNumber > mUnitsAlive)
			{
				StopDialogue(false);
				Fail();
			}
			break;
		default:
			TBFAssert.DoAssert(false, "unknown check type");
			break;
		}
		if (flag)
		{
			StopDialogue(m_UnitsVolInterface.ClearDialogueQueueOnSuccess);
			Pass();
			StopCoroutine("VocalPromptPlayer");
			GetComponent<Collider>().enabled = false;
			base.enabled = false;
		}
	}

	private IEnumerator VocalPromptPlayer(int trigger_number)
	{
		float delay = 3f;
		while (delay > 0f)
		{
			delay -= Time.deltaTime;
			yield return null;
		}
		if (trigger_number < mUnitsInArea || !(m_UnitsVolInterface.PromptIfNotRequiredNumber != string.Empty) || !(diagManager != null))
		{
			yield break;
		}
		switch (m_UnitsVolInterface.Check)
		{
		case TriggerCheck.All_Alive:
			if (mUnitsInArea != mUnitsAlive || mUnitsInArea != m_UnitsVolInterface.SpecificNumber)
			{
				diagManager.StartRepeatingDialogue(m_UnitsVolInterface.PromptIfNotRequiredNumber);
				HighlightFollowButton(true);
			}
			break;
		case TriggerCheck.Specific_Number:
			if (mUnitsInArea < m_UnitsVolInterface.SpecificNumber && m_UnitsVolInterface.SpecificNumber <= mUnitsAlive)
			{
				diagManager.StartRepeatingDialogue(m_UnitsVolInterface.PromptIfNotRequiredNumber);
				HighlightFollowButton(true);
			}
			break;
		default:
			TBFAssert.DoAssert(false, "unknown check type");
			break;
		}
	}

	private void StopDialogue(bool clearQueue)
	{
		if (m_UnitsVolInterface.PromptIfNotRequiredNumber != string.Empty && diagManager != null)
		{
			diagManager.FinishRepeatingDialogue(m_UnitsVolInterface.PromptIfNotRequiredNumber);
			if (clearQueue)
			{
				diagManager.ClearDialogueQueue(true);
			}
			StopCoroutine("VocalPromptPlayer");
			HighlightFollowButton(false);
		}
	}

	private void HighlightFollowButton(bool on)
	{
		if (on)
		{
			if (!mAlreadyFlashing)
			{
				mAlreadyFlashing = true;
				StartCoroutine(FlashButton());
			}
		}
		else if (mAlreadyFlashing)
		{
			mAlreadyFlashing = false;
		}
	}

	private IEnumerator FlashButton()
	{
		SetupIcon(CommonHudController.Instance.FollowMeButton.transform, Vector3.zero);
		float delayTime = 0f;
		bool quitLoop = false;
		float TimeToHighlight = 5f;
		while ((delayTime < TimeToHighlight || TimeToHighlight == -1f) && !quitLoop && mIcon != null)
		{
			delayTime += Time.deltaTime;
			if (!GameController.Instance.IsFirstPerson || CommonHudController.Instance.FollowMeButton.controlState == UIButton.CONTROL_STATE.ACTIVE || !mAlreadyFlashing)
			{
				quitLoop = true;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
		mAlreadyFlashing = false;
	}

	private void SetupIcon(Transform target, Vector3 altPos)
	{
		mIcon = (GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighter);
		Vector3 zero = Vector3.zero;
		zero = ((!(altPos != Vector3.zero)) ? target.transform.position : altPos);
		zero.z = 0f;
		HudBlipIcon component = mIcon.GetComponent<HudBlipIcon>();
		component.Target = target;
		mIcon.transform.position = zero;
		TutorialToggles.ActiveHighlights.Add(mIcon);
		StartCoroutine("Blink", mIcon);
	}

	private void DestroyIcon(GameObject icon)
	{
		if (TutorialToggles.ActiveHighlights.Contains(icon))
		{
			TutorialToggles.ActiveHighlights.Remove(icon);
		}
		Object.Destroy(icon);
	}

	private IEnumerator Blink(GameObject icon)
	{
		float blinkTime = 0.2f;
		float currentTime = 0f;
		while (icon != null)
		{
			if (!mAlreadyFlashing)
			{
				currentTime = 0f;
				yield return null;
			}
			if (icon == null)
			{
				break;
			}
			currentTime += Time.deltaTime;
			if (currentTime >= blinkTime)
			{
				icon.SetActive(!icon.activeInHierarchy);
				currentTime = 0f;
			}
			yield return null;
		}
	}

	private UnitData IsUnitOfInterest(GameObject unitToCheck)
	{
		UnitData[] array = mUnitsToCheck;
		foreach (UnitData unitData in array)
		{
			if (unitData != null && unitData.mUnit.Equals(unitToCheck))
			{
				return unitData;
			}
		}
		return null;
	}

	public void OnTriggerEnter(Collider other)
	{
		UnitData unitData = IsUnitOfInterest(other.gameObject);
		if (unitData != null)
		{
			if (mUnitsInArea == 0)
			{
				GameplayController.Instance().FirstScriptTriggerActor = other.gameObject;
			}
			unitData.mIsInArea = true;
			UpdateUnitCounts();
			if (base.State != ObjectiveState.Dormant)
			{
				StartCoroutine(VocalPromptPlayer(mUnitsInArea));
				PassCheck();
			}
		}
	}

	private void Update()
	{
		if (base.State == ObjectiveState.Dormant)
		{
			return;
		}
		if (!PerformedCheckOnActivate)
		{
			if (mUnitsInArea != 0)
			{
				StartCoroutine(VocalPromptPlayer(mUnitsInArea));
				PassCheck();
			}
			PerformedCheckOnActivate = true;
		}
		if (mUnitInAreaCheck)
		{
			UpdateUnitCounts();
			PassCheck();
		}
	}

	public void OnTriggerExit(Collider other)
	{
		UnitData unitData = IsUnitOfInterest(other.gameObject);
		if (unitData == null)
		{
			return;
		}
		unitData.mIsInArea = false;
		UpdateUnitCounts();
		if (base.State != ObjectiveState.Dormant)
		{
			if (mUnitsInArea == 0)
			{
				StopDialogue(false);
			}
			PassCheck();
		}
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.blue.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}

	public void ForceFail()
	{
		Fail();
	}
}
