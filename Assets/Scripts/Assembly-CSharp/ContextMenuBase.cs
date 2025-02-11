using System.Collections;
using UnityEngine;

public class ContextMenuBase : MonoBehaviour
{
	public GameObject MenuButton;

	public GameObject MenuCancelButton;

	public float TransitionOnTime = 0.5f;

	public float TransitionOffTime = 0.1f;

	protected MenuButton[] mButtons;

	private UIButton mCancelButton;

	private Vector2 mOriginatePosition;

	private Vector2 mFingerPosition;

	public static int NumContextMenusActive;

	public InterfaceableObject mInterfaceObj { get; private set; }

	public Vector2 OriginatePosition
	{
		get
		{
			return mOriginatePosition;
		}
		protected set
		{
			mOriginatePosition = value;
		}
	}

	public Vector2 FingerPosition
	{
		get
		{
			return mFingerPosition;
		}
	}

	public MenuButton GetButton(ContextMenuIcons iconType)
	{
		for (int i = 0; i < mButtons.Length; i++)
		{
			if (mButtons[i].IconType == iconType)
			{
				return mButtons[i];
			}
		}
		return null;
	}

	public void ConstructForInteractableObject(InterfaceableObject iobj, Vector2 selectedScreenPos)
	{
		ConstructForInteractableObject(iobj, selectedScreenPos, false);
	}

	public void ConstructForInteractableObject(InterfaceableObject iobj, Vector2 selectedScreenPos, bool asGhost)
	{
		mInterfaceObj = iobj;
		mFingerPosition = selectedScreenPos;
		OriginatePosition = selectedScreenPos;
		ContextMenuItem[] liveMenuItems = iobj.GetLiveMenuItems();
		int num = liveMenuItems.Length;
		mButtons = new MenuButton[num];
		for (int i = 0; i < num; i++)
		{
			GameObject gameObject = (GameObject)SceneNanny.Instantiate(MenuButton);
			mButtons[i] = gameObject.GetComponent<MenuButton>();
			mButtons[i].IconType = liveMenuItems[i].mIconId;
			UIButton button = mButtons[i].Button;
			button.name = liveMenuItems[i].mIconId.ToString();
			button.scriptWithMethodToInvoke = iobj;
			if (liveMenuItems[i].mEnabled)
			{
				button.methodToInvoke = liveMenuItems[i].mMethodName;
			}
			else
			{
				button.methodToInvoke = "CancelMenu";
			}
			mButtons[i].gameObject.AddComponent(typeof(BouncyButton));
			PlaySoundFx playSoundFx = mButtons[i].gameObject.AddComponent<PlaySoundFx>();
			playSoundFx.PlayOnStart = false;
			playSoundFx.PlayAs3D = false;
			playSoundFx.SoundFunction = "ContextMenuSelect";
			playSoundFx.SoundBank = InterfaceSFX.Instance;
			if (liveMenuItems[i].mLabel == null || liveMenuItems[i].mLabel == string.Empty)
			{
				button.Text = AutoLocalize.Get(liveMenuItems[i].mMethodName.ToUpper());
			}
			else
			{
				button.Text = AutoLocalize.Get(liveMenuItems[i].mLabel.ToUpper());
			}
			if (liveMenuItems[i].mMethodName == "S_CMHeal")
			{
				button.Text += string.Format("({0})", PlayerSquadManager.Instance.MedkitCount);
			}
			else
			{
				InterfaceSFX.Instance.ContextMenuSelect.Play2D();
			}
			if (asGhost)
			{
				button.SetFrame(0, 1);
			}
			else
			{
				button.SetFrame(0, 0);
			}
			PackedSprite componentInChildren = mButtons[i].gameObject.GetComponentInChildren<PackedSprite>();
			if (componentInChildren != null)
			{
				componentInChildren.SetFrame(0, (int)liveMenuItems[i].mIconId);
			}
			if (liveMenuItems[i].mEnabled)
			{
				button.SetColor(ColourChart.ContextMenuItemNormal);
			}
			else
			{
				button.SetColor(ColourChart.ContextMenuItemInactive);
			}
		}
		PositionButtons();
		mCancelButton = ((GameObject)SceneNanny.Instantiate(MenuCancelButton)).GetComponent<UIButton>();
		mCancelButton.transform.position = new Vector3(0f, 0f, 0f);
		UIButton component = mCancelButton.GetComponent<UIButton>();
		component.scriptWithMethodToInvoke = iobj;
		component.methodToInvoke = "CancelMenu";
		NumContextMenusActive++;
		InterfaceSFX.Instance.ContextMenuOn.Play2D();
		StartCoroutine(TransitionOnLoop());
	}

	protected virtual void PositionButtons()
	{
		TBFAssert.DoAssert(false);
	}

	public virtual void TransitionOff()
	{
		TBFAssert.DoAssert(false);
	}

	private IEnumerator TransitionOnLoop()
	{
		float timeCheck = Time.realtimeSinceStartup + TransitionOnTime;
		while (timeCheck > Time.realtimeSinceStartup)
		{
			yield return new WaitForEndOfFrame();
		}
		if (mButtons != null)
		{
			for (int i = 0; i < mButtons.Length; i++)
			{
				UIButton button = mButtons[i].Button;
				button.collider.enabled = !button.collider.enabled;
				button.collider.enabled = !button.collider.enabled;
			}
		}
	}

	protected void DestroyButtons()
	{
		for (int i = 0; i < mButtons.Length; i++)
		{
			if (mButtons[i] != null)
			{
				Object.Destroy(mButtons[i].gameObject);
				mButtons[i] = null;
			}
		}
		if (mCancelButton != null)
		{
			Object.Destroy(mCancelButton.gameObject);
			NumContextMenusActive--;
			TBFAssert.DoAssert(NumContextMenusActive >= 0);
		}
		Object.Destroy(base.gameObject);
	}

	private void Awake()
	{
	}

	private void OnDestroy()
	{
		DestroyButtons();
		if (mInterfaceObj != null && mInterfaceObj.DestroyOnFinish)
		{
			Object.Destroy(mInterfaceObj.gameObject);
		}
	}
}
