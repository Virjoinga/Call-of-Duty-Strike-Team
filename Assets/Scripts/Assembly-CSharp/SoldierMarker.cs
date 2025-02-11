using System.Collections;
using UnityEngine;

public class SoldierMarker : HudBlipIcon
{
	public enum OrderIconEnum
	{
		Invalid = -1,
		Busy = 0,
		InCover = 1,
		UnderFire = 2,
		Moving = 3
	}

	public bool BlipAtScreenEdge = true;

	public bool DestroyOnEmpty = true;

	public SpriteText UnitName;

	public SpriteText UnitNameShadow;

	public PackedSprite StatusBg;

	public PackedSprite OrderIcon;

	public UIProgressBar StatusIcon;

	public PackedSprite PointManIcon;

	public PackedSprite NotInControlIcon;

	private HealthComponent mHealComp;

	private Actor mActor;

	private UIButton uiButton;

	private int currentFrame;

	private static int roundRobinMaster = 0;

	private int roundRobinIndex;

	private bool mHighlightForSelect;

	private bool mShowOrders;

	private bool mSelected;

	private static Vector3 OnScreenScale = new Vector3(1f, 1f, 1f);

	private static Vector3 OffScreenScale = new Vector3(1f, 1f, 1f);

	private static float AnimateOnOffTime = 0.2f;

	public void Awake()
	{
		uiButton = GetComponentInChildren<UIButton>();
	}

	public override void Start()
	{
		ScreenEdgeOffsetMin = new Vector2(20f, 0f);
		ScreenEdgeOffsetMax = new Vector2(20f, 40f);
		roundRobinIndex = roundRobinMaster++;
		base.Start();
		mShowOrders = false;
		OrderIcon.Hide(true);
		NotInControlIcon.Hide(true);
		NotInControlIcon.SetColor(ColourChart.FriendlyBlip);
		base.ClampToEdgeOfScreen = true;
		if (Target != null)
		{
			mHealComp = Target.GetComponent<HealthComponent>();
			mActor = Target.gameObject.GetComponent<Actor>();
		}
		mSelected = true;
		MarkSelected(false);
		mHighlightForSelect = false;
		MarkAsLeader(false);
		currentFrame = -1;
		base.gameObject.AddComponent<Rigidbody>().isKinematic = true;
		base.gameObject.transform.localScale = new Vector3(0f, 1f, 1f);
		IsAllowedInFirstPerson = true;
		if (mActor == null)
		{
			ToggleScriptOverrideMarker(false);
		}
	}

	public static void SwitchOffAllSoldierMakers(bool switchOff)
	{
		SoldierMarker[] array = Object.FindObjectsOfType(typeof(SoldierMarker)) as SoldierMarker[];
		SoldierMarker[] array2 = array;
		foreach (SoldierMarker soldierMarker in array2)
		{
			if (soldierMarker.mActor != null)
			{
				soldierMarker.mActor.realCharacter.ChangeHUDStatus(!switchOff);
			}
		}
	}

	public void ToggleScriptOverrideMarker(bool on)
	{
		if (on)
		{
			StatusBg.Hide(false);
			if (mActor == GameplayController.instance.SelectedLeader)
			{
				PointManIcon.Hide(false);
			}
			else
			{
				PointManIcon.Hide(true);
			}
			StatusIcon.Hide(false);
			NotInControlIcon.Hide(true);
			if (!base.IsSwitchedOn)
			{
				SwitchOn();
			}
		}
		else
		{
			StatusBg.Hide(true);
			PointManIcon.Hide(true);
			StatusIcon.Hide(true);
			NotInControlIcon.Hide(false);
		}
	}

	public override void LateUpdate()
	{
		if (GUISystem.Instance == null)
		{
			return;
		}
		if (mHealComp != null)
		{
			int animFrameForHealth = ColourChart.GetAnimFrameForHealth(mHealComp.Health01);
			if (animFrameForHealth != currentFrame)
			{
				currentFrame = animFrameForHealth;
				StatusIcon.SetFrame(0, animFrameForHealth);
				StatusBg.SetFrame(0, animFrameForHealth);
			}
		}
		if ((Time.frameCount & 3) == (roundRobinIndex & 3))
		{
			if (mSelected)
			{
				float num = Time.time * 2f;
				StatusIcon.Value = num - (float)Mathf.FloorToInt(num);
			}
			else
			{
				StatusIcon.Value = 1f;
			}
		}
		UpdateOrderIcons();
		if (mActor != null && (mActor.realCharacter.IsDead() || mActor.realCharacter.IsMortallyWounded()))
		{
			WorldOffset = base.OriginalWorldOffset - Vector3.up;
		}
		else
		{
			WorldOffset = base.OriginalWorldOffset;
		}
		Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
		if (mFirstPersonActor != null && mFirstPersonActor == mActor)
		{
			base.transform.localScale = Vector3.zero;
		}
		else if (base.IsSwitchedOn && base.transform.localScale == Vector3.zero && !HudBlipIcon.AreAllSetForCutscene)
		{
			base.transform.localScale = Vector3.one;
		}
		base.LateUpdate();
		if (currentFrame == 4)
		{
			Color color = ((!(Time.time % 1f > 0.5f)) ? ColourChart.SelectedPlayerSoldier : ColourChart.UnSelectedPlayerSoldierDim);
			if (StatusBg.color != color)
			{
				StatusBg.SetColor(color);
			}
		}
	}

	private void StartCanSelectAnim()
	{
		StatusBg.gameObject.ScaleTo(new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 0f);
	}

	private void StopCanSelectAnim()
	{
		StatusBg.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), 0.2f, 0f);
	}

	public void FlashWhileHealing(float timeToFlashFor)
	{
		StatusBg.gameObject.ScaleBy(new Vector3(0.5f, 0.5f, 0.5f), 0.2f, 0f, EaseType.linear, LoopType.pingPong);
		StatusBg.gameObject.ScaleBy(new Vector3(0.5f, 0.5f, 0.5f), 0.2f, timeToFlashFor + 0.05f, EaseType.linear);
		StatusBg.gameObject.ScaleTo(new Vector3(1f, 1f, 1f), 0.2f, timeToFlashFor + 0.25f, EaseType.linear);
		StartCoroutine(EnsureCorrectHighlightColourAfterHeal());
	}

	private IEnumerator EnsureCorrectHighlightColourAfterHeal()
	{
		while (mActor.health.IsReviving)
		{
			yield return null;
		}
		Color col = ((!GameplayController.instance.IsSelected(mActor)) ? ColourChart.UnSelectedPlayerSoldierDim : ColourChart.SelectedPlayerSoldier);
		if (StatusBg.color != col)
		{
			StatusBg.SetColor(col);
		}
	}

	public void MarkCanSelect(bool canSelect)
	{
		if (mHighlightForSelect != canSelect)
		{
			mHighlightForSelect = canSelect;
			if (mHighlightForSelect && base.IsOnScreen)
			{
				StartCanSelectAnim();
			}
			else
			{
				StopCanSelectAnim();
			}
		}
	}

	public void MarkSelected(bool selected)
	{
		if (selected != mSelected)
		{
			if (selected)
			{
				StatusBg.SetColor(ColourChart.SelectedPlayerSoldier);
				StatusIcon.SetColor(ColourChart.SelectedPlayerSoldier);
				UnitName.SetColor(ColourChart.HudWhite);
			}
			else
			{
				StatusBg.SetColor(ColourChart.UnSelectedPlayerSoldierDim);
				StatusIcon.SetColor(ColourChart.UnSelectedPlayerSoldier);
				UnitName.SetColor(ColourChart.UnSelectedPlayerSoldierNameDim);
			}
			mSelected = selected;
		}
	}

	public override void UpdateOnScreen()
	{
		Vector3 vector = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(base.ScreenPos);
		base.transform.position = new Vector3(vector.x, vector.y, FinalPositionZOffset);
	}

	public override void UpdateOffScreen()
	{
		Vector3 vector = GUISystem.Instance.m_guiCamera.ScreenToWorldPoint(base.ScreenPos);
		base.transform.position = new Vector3(vector.x, vector.y, FinalPositionZOffset);
	}

	public override void JustGoneOffScreen()
	{
		base.gameObject.ScaleTo(OffScreenScale, AnimateOnOffTime, 0f);
		if (mHighlightForSelect)
		{
			StopCanSelectAnim();
		}
		ShowOrders(true);
		uiButton.controlIsEnabled = true;
		uiButton.transform.localPosition = Vector3.zero;
	}

	public override void JustComeOnScreen()
	{
		base.gameObject.ScaleTo(OnScreenScale, AnimateOnOffTime, 0f);
		if (mHighlightForSelect)
		{
			StartCanSelectAnim();
		}
		ShowOrders(false);
		uiButton.controlIsEnabled = false;
		uiButton.transform.localPosition = new Vector3(0f, 1000f, 0f);
	}

	public override void JustSwitchedOff()
	{
		base.gameObject.ScaleTo(new Vector3(0f, 0f, 0f), AnimateOnOffTime, 0f);
	}

	public override void JustSwitchedOn()
	{
		base.gameObject.ScaleTo(OnScreenScale, AnimateOnOffTime, 0f);
		if (base.IsOnScreen)
		{
			JustComeOnScreen();
		}
		else
		{
			JustGoneOffScreen();
		}
	}

	public void BlipPressed()
	{
		if (!TutorialToggles.enableSoldierMarkerButton || mActor == null || mActor.realCharacter == null || !mActor.realCharacter.Selectable)
		{
			return;
		}
		if (mActor != null && mActor.realCharacter != null && mActor.realCharacter.IsMortallyWounded())
		{
			CameraManager instance = CameraManager.Instance;
			if (instance != null)
			{
				NavMeshCamera navMeshCamera = instance.PlayCameraController.CurrentCameraBase as NavMeshCamera;
				if (navMeshCamera != null)
				{
					navMeshCamera.FocusOnTarget(mActor.transform, true);
				}
			}
		}
		else
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null && Target != null)
			{
				gameplayController.ProcessUnitSelectLogic(Target.gameObject);
			}
		}
	}

	public void MarkAsLeader(bool leader)
	{
		if (PointManIcon != null)
		{
			PointManIcon.Hide(!leader);
		}
	}

	public void SetDisplayName(string name)
	{
		if (UnitName != null)
		{
			UnitName.Text = name.ToUpper();
			UnitNameShadow.Text = name.ToUpper();
		}
	}

	public void ShowOrders(bool show)
	{
		if (show != mShowOrders)
		{
			mShowOrders = show;
			if (mShowOrders)
			{
				UpdateOrderIcons();
			}
			else
			{
				OrderIcon.Hide(true);
			}
		}
	}

	public static OrderIconEnum UpdateOrderIcon(Actor actor, HealthComponent health, ref PackedSprite orderIcon)
	{
		if (actor == null)
		{
			return OrderIconEnum.Invalid;
		}
		if (health != null && (Time.time - health.TimeHeathLastReduced < 3f || actor.weapon.IsFiring() || actor.realCharacter.IsMortallyWounded()))
		{
			if (orderIcon.IsHidden())
			{
				orderIcon.Hide(false);
			}
			orderIcon.SetFrame(0, 2);
			return OrderIconEnum.UnderFire;
		}
		if (actor.awareness.IsInCover())
		{
			if (orderIcon.IsHidden())
			{
				orderIcon.Hide(false);
			}
			orderIcon.SetFrame(0, 1);
			return OrderIconEnum.InCover;
		}
		if (actor.realCharacter.IsMoving())
		{
			if (orderIcon.IsHidden())
			{
				orderIcon.Hide(false);
			}
			orderIcon.SetFrame(0, 3);
			return OrderIconEnum.Moving;
		}
		if (actor.tasks.IsRunningTask<TaskSetPiece>() || actor.tasks.IsRunningTask<TaskMultiCharacterSetPiece>())
		{
			if (orderIcon.IsHidden())
			{
				orderIcon.Hide(false);
			}
			orderIcon.SetFrame(0, 0);
			return OrderIconEnum.Busy;
		}
		orderIcon.Hide(true);
		return OrderIconEnum.Invalid;
	}

	private void UpdateOrderIcons()
	{
		if (mShowOrders)
		{
			UpdateOrderIcon(mActor, mHealComp, ref OrderIcon);
		}
	}

	public static void SetStatesForVTOLGunner()
	{
		SoldierMarker[] array = Object.FindObjectsOfType(typeof(SoldierMarker)) as SoldierMarker[];
		SoldierMarker[] array2 = array;
		foreach (SoldierMarker soldierMarker in array2)
		{
			soldierMarker.IsAllowedInFirstPerson = true;
			soldierMarker.ClampToEdgeOfScreen = false;
		}
	}

	public static void ResetStatesFromVTOLGunner()
	{
		SoldierMarker[] array = Object.FindObjectsOfType(typeof(SoldierMarker)) as SoldierMarker[];
		SoldierMarker[] array2 = array;
		foreach (SoldierMarker soldierMarker in array2)
		{
			soldierMarker.IsAllowedInFirstPerson = false;
			soldierMarker.ClampToEdgeOfScreen = true;
		}
	}
}
