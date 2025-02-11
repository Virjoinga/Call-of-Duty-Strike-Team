using System.Collections;
using UnityEngine;

public class HighlightCMCommand : Command
{
	private enum States
	{
		NoHighlight = 0,
		HighlightCM = 1,
		HighlightCMButton = 2,
		Done = 3
	}

	public InteriorItem ObjectWithCM;

	public ContextMenuIcons CMButtonToHighlight;

	public bool ClearOnOutOfRange;

	public int NumPlayersRequired = 1;

	public string BeforePressMessage;

	public string AfterPressMessage;

	private States mState;

	private GameObject mIcon;

	private InterfaceableObject cm;

	private bool mIsActor;

	public override bool Blocking()
	{
		return true;
	}

	public override IEnumerator Execute()
	{
		TutorialToggles.HighlightingCM = true;
		if (ObjectWithCM.Interior != null && ObjectWithCM != null && ObjectWithCM.Interior != null)
		{
			InteriorOverride io = ObjectWithCM.Interior.theObject.GetComponentInChildren<InteriorOverride>();
			if ((bool)io)
			{
				if (ObjectWithCM.ContextType == HighlightHudCommand.ContextType.Door)
				{
					cm = io.m_ActiveDoors[ObjectWithCM.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>();
				}
				else
				{
					cm = io.m_ActiveWindows[ObjectWithCM.IndexToReference].m_Object.GetComponentInChildren<InterfaceableObject>();
				}
			}
			else
			{
				ActorWrapper aw = ObjectWithCM.Interior.theObject.GetComponentInChildren<ActorWrapper>();
				if (aw != null)
				{
					Actor a = aw.GetActor();
					if (a != null)
					{
						cm = a.GetComponentInChildren<InterfaceableObject>();
						mIsActor = true;
					}
				}
				if (cm == null)
				{
					cm = ObjectWithCM.Interior.theObject.GetComponentInChildren<InterfaceableObject>();
				}
			}
		}
		if (cm == null)
		{
			TutorialToggles.HighlightingCM = false;
			Debug.LogError("Unable to find a context menu to highlight on " + ObjectWithCM.Interior.theObject);
			yield break;
		}
		mState = States.NoHighlight;
		ChangeState();
		while (mState != States.Done && cm != null && (mIsActor || cm.ContextBlip != null))
		{
			if (TutorialToggles.ShouldClearHighlights)
			{
				StopAllCoroutines();
				foreach (GameObject go in TutorialToggles.ActiveHighlights)
				{
					Object.Destroy(go);
				}
				TutorialToggles.ActiveHighlights.RemoveAll((GameObject item) => item == null);
				TutorialToggles.ShouldClearHighlights = false;
				break;
			}
			yield return null;
		}
		DestroyIcon(mIcon);
		StopCoroutine("StateNoHighlight");
		StopCoroutine("StateHighlightCM");
		StopCoroutine("StateHighlightCMButton");
		if (HUDMessenger.Instance != null)
		{
			HUDMessenger.Instance.ClearHeldMessage();
		}
		yield return new WaitForSeconds(0.25f);
		TutorialToggles.HighlightingCM = false;
	}

	private IEnumerator StateNoHighlight()
	{
		DestroyIcon(mIcon);
		if (HUDMessenger.Instance != null)
		{
			HUDMessenger.Instance.ClearHeldMessage();
		}
		while (mState == States.NoHighlight)
		{
			if ((mIsActor || (cm.ContextBlip.IsOnScreen && cm.ContextBlip.IsSwitchedOn)) && GameplayController.instance.Selected.Count >= NumPlayersRequired)
			{
				mState = States.HighlightCM;
			}
			yield return null;
		}
		ChangeState();
	}

	private IEnumerator StateHighlightCM()
	{
		if (mIsActor)
		{
			SetupIcon(cm.gameObject.transform, Vector3.zero, true);
		}
		else
		{
			SetupIcon(cm.ContextBlip.Target, cm.GetBlipPos(), true);
			HudBlipIcon hbi = mIcon.GetComponent<HudBlipIcon>();
			hbi.WorldOffset = cm.ContextBlip.WorldOffset;
		}
		if (BeforePressMessage != null && BeforePressMessage != string.Empty)
		{
			string strToDisplay = Language.Get(BeforePressMessage);
			HUDMessenger.Instance.PushPriorityMessage(strToDisplay, true);
		}
		while (mState == States.HighlightCM)
		{
			if (mIcon == null || GameplayController.instance.Selected.Count < NumPlayersRequired)
			{
				mState = States.NoHighlight;
				break;
			}
			if (CommonHudController.Instance.ContextMenu != null && CommonHudController.Instance.ContextMenu.mInterfaceObj == cm)
			{
				mState = States.HighlightCMButton;
			}
			else if (!mIsActor && (!cm.ContextBlip.IsOnScreen || !cm.ContextBlip.IsSwitchedOn))
			{
				DestroyIcon(mIcon);
				if (ClearOnOutOfRange)
				{
					mState = States.Done;
				}
				else
				{
					mState = States.NoHighlight;
				}
			}
			yield return null;
		}
		ChangeState();
	}

	private IEnumerator StateHighlightCMButton()
	{
		DestroyIcon(mIcon);
		mIcon = null;
		MenuButton button2 = null;
		if (CommonHudController.Instance.ContextMenu != null && CommonHudController.Instance.ContextMenu.mInterfaceObj == cm)
		{
			button2 = CommonHudController.Instance.ContextMenu.GetButton(CMButtonToHighlight);
			while (button2 == null)
			{
				if (CommonHudController.Instance.ContextMenu == null)
				{
					mState = States.NoHighlight;
					ChangeState();
					yield break;
				}
				button2 = CommonHudController.Instance.ContextMenu.GetButton(CMButtonToHighlight);
				yield return null;
			}
			SetupIcon(button2.Button.transform, Vector3.zero, false);
			if (AfterPressMessage != null && AfterPressMessage != string.Empty)
			{
				string strToDisplay = Language.Get(AfterPressMessage);
				HUDMessenger.Instance.PushPriorityMessage(strToDisplay, true);
			}
			bool moveStateBack2 = false;
			float delay = 0f;
			while (mState == States.HighlightCMButton)
			{
				if (mIcon == null || GameplayController.instance.Selected.Count < NumPlayersRequired)
				{
					mState = States.NoHighlight;
					break;
				}
				if (button2 == null && CommonHudController.Instance.ContextMenu != null && CommonHudController.Instance.ContextMenu.mInterfaceObj == cm)
				{
					button2 = CommonHudController.Instance.ContextMenu.GetButton(CMButtonToHighlight);
				}
				if (button2 == null || CommonHudController.Instance.ContextMenu == null || CommonHudController.Instance.ContextMenu.mInterfaceObj != cm)
				{
					moveStateBack2 = true;
				}
				else if (button2.Button.controlState == UIButton.CONTROL_STATE.ACTIVE)
				{
					DestroyIcon(mIcon);
					mState = States.Done;
					moveStateBack2 = false;
				}
				else
				{
					moveStateBack2 = false;
				}
				if (moveStateBack2)
				{
					delay += Time.deltaTime;
					if (delay >= 0.2f)
					{
						DestroyIcon(mIcon);
						mState = States.HighlightCM;
						delay = 0f;
					}
				}
				else
				{
					delay = 0f;
				}
				if (mIcon != null && button2 != null && button2.Button != null)
				{
					mIcon.transform.position = button2.Button.transform.position;
					mIcon.transform.localScale = Vector3.one;
				}
				else if (mIcon == null && button2 != null)
				{
					SetupIcon(button2.Button.transform, Vector3.zero, false);
				}
				yield return null;
			}
			ChangeState();
		}
		else
		{
			mState = States.NoHighlight;
			ChangeState();
		}
	}

	private void ChangeState()
	{
		switch (mState)
		{
		case States.NoHighlight:
			StartCoroutine(StateNoHighlight());
			break;
		case States.HighlightCM:
			StartCoroutine(StateHighlightCM());
			break;
		case States.HighlightCMButton:
			StartCoroutine(StateHighlightCMButton());
			break;
		}
	}

	private void SetupIcon(Transform target, Vector3 altPos, bool smallHighlight)
	{
		if (mIcon == null)
		{
			mIcon = ((!smallHighlight) ? ((GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighter)) : ((GameObject)Object.Instantiate(CommonHudController.Instance.TutorialHighlighterSmall)));
			TutorialToggles.ActiveHighlights.Add(mIcon);
			StartCoroutine(Blink(mIcon));
		}
		Vector3 zero = Vector3.zero;
		zero = ((!(altPos != Vector3.zero)) ? target.transform.position : altPos);
		zero.z = 0f;
		HudBlipIcon component = mIcon.GetComponent<HudBlipIcon>();
		component.Target = target;
		mIcon.transform.position = zero;
	}

	private void DestroyIcon(GameObject icon)
	{
		if (icon != null)
		{
			if (TutorialToggles.ActiveHighlights.Contains(icon))
			{
				TutorialToggles.ActiveHighlights.Remove(icon);
			}
			Object.Destroy(icon);
		}
	}

	private IEnumerator Blink(GameObject icon)
	{
		float blinkTime = 0.2f;
		float currentTime = 0f;
		while (icon != null && !(icon == null))
		{
			currentTime += Time.deltaTime;
			if (currentTime >= blinkTime)
			{
				icon.SetActive(!icon.activeInHierarchy);
				currentTime = 0f;
			}
			yield return null;
		}
	}

	public override void ResolveGuidLinks()
	{
		if (ObjectWithCM != null)
		{
			ObjectWithCM.ResolveGuidLinks();
		}
	}
}
