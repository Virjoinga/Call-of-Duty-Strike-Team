using System;
using System.Collections.Generic;
using UnityEngine;

public class InterfaceableObject : SelectableObject
{
	public enum InvocationTypeEnum
	{
		PressHold = 0,
		Immediate = 1
	}

	protected InvocationTypeEnum invocationType;

	public bool UseBlip = true;

	public Vector3 BlipWorldOffset = Vector3.zero;

	public bool DestroyOnFinish;

	public BuildingWithInterior mInterior;

	private float mTimeHeld;

	private bool mHoldingButton;

	private float mTimeToHold;

	public float FacingAngle;

	public float CaptureAngle = 360f;

	private float mCaptureAngleCosTheta;

	private Vector2 mNormalForwardVector;

	public float FirstPersonTriggerRadius = 3f;

	public float FirstPersonVisibleRadius = 10f;

	private float mFirstPersonVisibleRadiusSqr;

	public CoverCluster MultiManCoverCluster;

	protected HudBlipIcon mContextBlip;

	private string mDefaultMethod = string.Empty;

	private ContextMenuIcons mDefaultIconId;

	public bool StartViewable;

	private ContextMenuDistanceManager cmdm;

	private bool mIgnoreTutorialLock;

	protected bool CanBeTurnedOn = true;

	private List<ContextMenuItem> mLiveMenuItems;

	public InvocationTypeEnum InvocationType
	{
		get
		{
			return invocationType;
		}
		set
		{
			invocationType = value;
		}
	}

	public BuildingWithInterior Interior
	{
		get
		{
			return mInterior;
		}
		set
		{
			mInterior = value;
		}
	}

	public float FirstPersonTriggerRadiusSqr { get; private set; }

	public float FirstPersonVisibleRadiusSqr
	{
		get
		{
			return mFirstPersonVisibleRadiusSqr;
		}
		private set
		{
			mFirstPersonVisibleRadiusSqr = value;
		}
	}

	public HudBlipIcon ContextBlip
	{
		get
		{
			return mContextBlip;
		}
	}

	public ContextMenuIcons DefaultContextIcon
	{
		get
		{
			return mDefaultIconId;
		}
	}

	public ContextMenuDistanceManager DistanceManager
	{
		get
		{
			return cmdm;
		}
		set
		{
			cmdm = value;
		}
	}

	public bool IgnoreTutorialLock
	{
		get
		{
			return mIgnoreTutorialLock;
		}
		set
		{
			mIgnoreTutorialLock = value;
		}
	}

	public bool SuppressSound { get; set; }

	public override Vector3 GetInterfacePosition()
	{
		return base.transform.position + BlipWorldOffset;
	}

	public void SetDefaultMethodForFirstPerson(string label, string method, ContextMenuIcons iconId)
	{
		SetDefaultMethodForFirstPerson(label, method, iconId, 0);
	}

	public void SetDefaultMethodForFirstPerson(string label, string method, ContextMenuIcons iconId, int cost)
	{
		mDefaultMethod = method;
		mDefaultIconId = iconId;
		ContextObjectBlip contextObjectBlip = mContextBlip as ContextObjectBlip;
		if (contextObjectBlip != null)
		{
			contextObjectBlip.SetCost(cost);
			contextObjectBlip.SetActionIcon(iconId);
			contextObjectBlip.SetInfoText(label);
			if (method == null)
			{
				contextObjectBlip.IsAllowedInFirstPerson = false;
			}
			else
			{
				contextObjectBlip.IsAllowedInFirstPerson = true;
			}
		}
	}

	public ContextObjectBlip GetContextObjectBlip()
	{
		if (mContextBlip == null)
		{
			return null;
		}
		return mContextBlip as ContextObjectBlip;
	}

	public void ContextBlipPressed()
	{
		ContextObjectBlip contextObjectBlip = mContextBlip as ContextObjectBlip;
		Vector3 position = contextObjectBlip.m_ContextButton.mTapHitPoint + contextObjectBlip.transform.position;
		Vector2 vector = GUISystem.Instance.m_guiCamera.WorldToScreenPoint(position).xy();
		SelectableObject selectableObject = SelectableObject.PickSelectableObject(vector);
		if (selectableObject != null)
		{
			if (selectableObject.quickType == QuickType.PlayerSoldier)
			{
				Actor component = selectableObject.AssociatedObject.GetComponent<Actor>();
				if (!GameplayController.Instance().IsSelected(component))
				{
					selectableObject.OnSelected(vector, true);
					return;
				}
			}
			else if (selectableObject.quickType == QuickType.EnemySoldier)
			{
				selectableObject.OnSelected(vector, true);
				return;
			}
		}
		mHoldingButton = true;
	}

	protected override void Awake()
	{
		mLiveMenuItems = new List<ContextMenuItem>();
		invocationType = InvocationTypeEnum.Immediate;
		float num = Mathf.Cos(CaptureAngle * 0.5f * ((float)Math.PI / 180f));
		mCaptureAngleCosTheta = num * Mathf.Abs(num);
		mNormalForwardVector = base.transform.forward.xz() * -1f;
		mNormalForwardVector = Maths.RotateAround(mNormalForwardVector, Vector2.zero, FacingAngle);
		base.Awake();
	}

	protected override void Start()
	{
		base.Start();
		FirstPersonTriggerRadius = Mathf.Abs(FirstPersonTriggerRadius);
		FirstPersonVisibleRadius = Mathf.Abs(FirstPersonVisibleRadius);
		FirstPersonTriggerRadiusSqr = FirstPersonTriggerRadius * FirstPersonTriggerRadius;
		FirstPersonVisibleRadiusSqr = FirstPersonVisibleRadius * FirstPersonVisibleRadius;
		if (GameplayController.instance == null)
		{
			UseBlip = false;
		}
		if (UseBlip)
		{
			CreateBlip();
		}
	}

	protected void CreateBlip()
	{
		if (!UseBlip || !(mContextBlip == null))
		{
			return;
		}
		ContextObjectBlip contextObjectBlip = UnityEngine.Object.Instantiate(GameplayController.instance.ContextObjBlip) as ContextObjectBlip;
		contextObjectBlip.StartViewable = StartViewable;
		mContextBlip = contextObjectBlip;
		mContextBlip.Target = base.transform;
		Vector3 zero = Vector3.zero;
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		Vector3 position = Vector3.zero;
		if (component != null && !base.renderer.isPartOfStaticBatch)
		{
			Mesh mesh = component.mesh;
			if (mesh != null)
			{
				position = mesh.bounds.center;
			}
		}
		zero = base.transform.TransformPoint(position) - base.transform.position;
		mContextBlip.WorldOffset = zero + BlipWorldOffset;
		if (StartViewable)
		{
			ShowBlip(true);
		}
		if (contextObjectBlip.m_ContextButton != null)
		{
			contextObjectBlip.m_ContextButton.scriptWithMethodToInvoke = this;
			contextObjectBlip.m_ContextButton.methodToInvoke = "ContextBlipPressed";
		}
	}

	protected void DeleteBlip()
	{
		if (UseBlip)
		{
			ShowBlip(false);
			UnityEngine.Object.Destroy(mContextBlip);
			mContextBlip = null;
		}
	}

	public virtual void Update()
	{
		if (!CanBeTurnedOn)
		{
			return;
		}
		ContextObjectBlip contextObjectBlip = mContextBlip as ContextObjectBlip;
		if (contextObjectBlip != null && contextObjectBlip.IsOnScreen && contextObjectBlip.m_ContextButton != null && contextObjectBlip.m_ContextButtonCollider != null)
		{
			if (GameController.Instance.IsFirstPerson)
			{
				BoxCollider contextButtonCollider = contextObjectBlip.m_ContextButtonCollider;
				bool flag = contextObjectBlip.IsInTriggerRange();
				contextObjectBlip.m_ContextButton.enabled = flag;
				contextButtonCollider.enabled = flag;
			}
			else
			{
				BoxCollider contextButtonCollider2 = contextObjectBlip.m_ContextButtonCollider;
				bool flag = contextObjectBlip.IsSwitchedOn;
				contextObjectBlip.m_ContextButton.enabled = flag;
				contextButtonCollider2.enabled = flag;
			}
		}
		if (mHoldingButton)
		{
			mTimeHeld += Time.deltaTime;
			if (mTimeHeld > mTimeToHold)
			{
				mHoldingButton = false;
				mTimeHeld = 0f;
				if (TimeManager.instance.GlobalTimeState != TimeManager.State.IngamePaused)
				{
					if (GameController.Instance.IsFirstPerson)
					{
						CallDefaultMethod();
					}
					else
					{
						OnSelected(contextObjectBlip.ScreenPos, true);
					}
				}
			}
		}
		LockedbyTutorial();
	}

	private void OnDestroy()
	{
		if ((bool)mContextBlip)
		{
			UnityEngine.Object.Destroy(mContextBlip.gameObject);
		}
	}

	private void OnEnable()
	{
		if (CanBeTurnedOn && !LockedbyTutorial())
		{
			ShowBlip(true);
		}
	}

	private void OnDisable()
	{
		ShowBlip(false);
	}

	public override int OnSelected(Vector2 selectedScreenPos, bool fromTap)
	{
		int result = 0;
		if (CommonHudController.Instance.gameObject.activeInHierarchy)
		{
			if (GameController.Instance.IsFirstPerson && GameController.Instance.AllowContextActionsInFirstPerson)
			{
				if (!CommonHudController.Instance.AnyTriggerInput)
				{
					CallDefaultMethod();
				}
			}
			else
			{
				PopulateMenuItems(fromTap);
				result = mLiveMenuItems.Count;
				if (mLiveMenuItems.Count > 0)
				{
					CommonHudController.Instance.AddContextMenu(this, selectedScreenPos);
				}
				ClearMenuItems();
			}
		}
		return result;
	}

	public void CallDefaultMethod()
	{
		if (mDefaultMethod != null)
		{
			InterfaceSFX.Instance.GeneralButtonPress.Play2D();
			Invoke(mDefaultMethod, 0f);
		}
		else
		{
			Debug.LogWarning("No default option");
		}
	}

	protected virtual void SetIconForDefaultOption()
	{
		if (mContextBlip != null)
		{
			mContextBlip.IsAllowedInFirstPerson = false;
		}
	}

	public bool IsAllowedInStrateryView()
	{
		if (mContextBlip != null)
		{
			return mContextBlip.IsAllowedInStrateryView;
		}
		return false;
	}

	public virtual void ShowBlip(bool OnOff)
	{
		if (UseBlip && CanBeTurnedOn && mContextBlip != null)
		{
			if (OnOff)
			{
				mContextBlip.SwitchOn();
				mContextBlip.gameObject.CheckedScaleTo(Vector3.one, 0.2f, 0f);
			}
			else
			{
				mContextBlip.gameObject.CheckedScaleTo(Vector3.zero, 0.2f, 0f);
			}
		}
	}

	public void TurnOff()
	{
		if (mContextBlip != null)
		{
			mContextBlip.SwitchOff();
		}
		base.enabled = false;
	}

	public void TurnOn()
	{
		if (CanBeTurnedOn && UseBlip && !LockedbyTutorial())
		{
			StartViewable = true;
			if (GameController.Instance.IsFirstPerson && GameController.Instance.AllowContextActionsInFirstPerson && mContextBlip != null)
			{
				SetIconForDefaultOption();
			}
			ShowBlip(true);
			base.enabled = true;
		}
	}

	public ContextMenuItem[] GetLiveMenuItems()
	{
		ContextMenuItem[] array = new ContextMenuItem[mLiveMenuItems.Count];
		mLiveMenuItems.CopyTo(array);
		return array;
	}

	public void AddCallableMethod(string label, string method, ContextMenuIcons iconId)
	{
		AddCallableMethod(label, method, iconId, true);
	}

	public void AddCallableMethod(string method, ContextMenuIcons iconId)
	{
		AddCallableMethod(string.Empty, method, iconId, true);
	}

	public void AddCallableMethod(string method, ContextMenuIcons iconId, bool enabled)
	{
		AddCallableMethod(string.Empty, method, iconId, enabled);
	}

	public void AddCallableMethod(string label, string method, ContextMenuIcons iconId, bool enabled)
	{
		ContextMenuItem item = new ContextMenuItem(label, method, iconId, enabled);
		mLiveMenuItems.Add(item);
	}

	protected virtual void PopulateMenuItems()
	{
	}

	protected virtual void PopulateMenuItems(bool fromTap)
	{
		PopulateMenuItems();
	}

	protected virtual void ClearMenuItems()
	{
		mLiveMenuItems.Clear();
	}

	public virtual void CancelMenu()
	{
		if (InterfaceSFX.HasInstance && !SuppressSound)
		{
			InterfaceSFX.Instance.ContextMenuCancel.Play2D();
		}
		SuppressSound = false;
		CommonHudController.Instance.ClearContextMenu();
	}

	public bool IsInTPPCaptureRange(Vector3 actorRelPos, float distSqrMag)
	{
		if (cmdm == null)
		{
			return false;
		}
		if (distSqrMag > cmdm.RadiusSqr)
		{
			return false;
		}
		return IsInCaptureRange(actorRelPos, distSqrMag);
	}

	public bool IsInCaptureRange(Vector3 actorRelPos, float distSqrMag)
	{
		if (CaptureAngle == 360f)
		{
			return true;
		}
		float num = Vector2.Dot(mNormalForwardVector, actorRelPos.xz());
		return mCaptureAngleCosTheta < Mathf.Abs(num) * num / distSqrMag;
	}

	private void OnDrawGizmosSelected()
	{
	}

	public void Deactivate()
	{
		TurnOff();
		CanBeTurnedOn = false;
	}

	public void Activate()
	{
		if (!LockedbyTutorial())
		{
			CanBeTurnedOn = true;
			StartViewable = false;
			if (mContextBlip == null)
			{
				CreateBlip();
			}
			if (cmdm != null)
			{
				cmdm.SetActive(false);
			}
			else
			{
				TurnOn();
			}
		}
	}

	public bool CanTurnOn()
	{
		return CanBeTurnedOn;
	}

	protected void AdjustCollider(Actor target)
	{
	}

	public bool IsActionIconVisible()
	{
		if (mContextBlip as ContextObjectBlip == null)
		{
			return false;
		}
		return (mContextBlip as ContextObjectBlip).IsInTriggerRange();
	}

	public Vector3 GetBlipPos()
	{
		if (mContextBlip == null)
		{
			return Vector3.zero;
		}
		return mContextBlip.transform.position;
	}

	public bool LockedbyTutorial()
	{
		if (TutorialToggles.DisableAllInteractions && !mIgnoreTutorialLock)
		{
			if (mContextBlip != null && mContextBlip.IsSwitchedOn)
			{
				TurnOff();
			}
			return true;
		}
		return false;
	}
}
