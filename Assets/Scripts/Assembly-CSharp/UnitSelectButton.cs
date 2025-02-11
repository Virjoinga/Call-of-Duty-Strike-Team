using UnityEngine;

public class UnitSelectButton : MonoBehaviour
{
	public SpriteText UnitName;

	public PackedSprite Border;

	public PackedSprite PointManIcon;

	public PackedSprite OrderIcon;

	public PackedSprite WeaponIcon;

	public PackedSprite CameoImage;

	private int mCameoFrame;

	private UIButton mButton;

	private Actor mActor;

	private bool mBorderColourSelected;

	private static UnitSelectButton mLastButtonTapped;

	private float mLastTappedTime;

	public Actor MyActor
	{
		get
		{
			return mActor;
		}
	}

	private void Start()
	{
		mButton = base.gameObject.GetComponent<UIButton>();
		mButton.AddValueChangedDelegate(ButtonSelectedDelegate);
		mButton.SetInputDelegate(ButtonInputDelegate);
		CameoImage.SetFrame(0, mCameoFrame);
	}

	private void Update()
	{
		if (MyActor == null)
		{
			return;
		}
		if (MyActor.realCharacter.IsMortallyWounded())
		{
			Color hudDarkerRed = ColourChart.HudDarkerRed;
			if (mButton.Color != hudDarkerRed)
			{
				mButton.SetColor(hudDarkerRed);
				CameoImage.SetColor(hudDarkerRed);
			}
		}
		else if (Time.time - MyActor.health.TimeHeathLastReduced < 2f)
		{
			mButton.SetColor((!(Time.time % 1f > 0.5f)) ? ColourChart.SoldierCameoSelectedBg : ColourChart.HudDarkRed);
		}
		else if (mBorderColourSelected)
		{
			if (mButton.Color != ColourChart.SoldierCameoSelectedBg)
			{
				mButton.SetColor(ColourChart.SoldierCameoSelectedBg);
				CameoImage.SetColor(Color.white);
			}
		}
		else if (mButton.Color != ColourChart.SoldierCameoUnSelectedBg)
		{
			mButton.SetColor(ColourChart.SoldierCameoUnSelectedBg);
			CameoImage.SetColor(Color.gray);
		}
	}

	private void ButtonSelectedDelegate(IUIObject obj)
	{
		if (mActor != null)
		{
			GameplayController gameplayController = GameplayController.Instance();
			if (gameplayController != null)
			{
				if (!TutorialToggles.PlayerSelectionLocked)
				{
					InterfaceSFX.Instance.SelectUnit.Play2D();
					if (gameplayController.IsInMultiSelectMode())
					{
						if (gameplayController.IsSelected(mActor))
						{
							if (mLastButtonTapped == this && gameplayController.MultiSelectModeTime < 0.25f)
							{
								FocusOnUnit();
							}
						}
						else
						{
							gameplayController.AddToSelected(mActor);
						}
					}
					else
					{
						gameplayController.SelectOnlyThis(mActor);
					}
				}
				else if (mLastButtonTapped == this && Time.realtimeSinceStartup - mLastTappedTime < 0.25f)
				{
					InterfaceSFX.Instance.SelectUnit.Play2D();
					FocusOnUnit();
				}
			}
		}
		mLastButtonTapped = this;
		mLastTappedTime = Time.realtimeSinceStartup;
	}

	private void FocusOnUnit()
	{
		CameraManager instance = CameraManager.Instance;
		if (instance != null)
		{
			NavMeshCamera navMeshCamera = instance.PlayCameraController.CurrentCameraBase as NavMeshCamera;
			if (navMeshCamera != null)
			{
				navMeshCamera.FocusOnPoint(MyActor.transform.position + new Vector3(0f, 1f, 0f));
			}
		}
	}

	public void ButtonInputDelegate(ref POINTER_INFO ptr)
	{
		switch (ptr.evt)
		{
		case POINTER_INFO.INPUT_EVENT.PRESS:
			if (MyActor.realCharacter.IsMortallyWounded())
			{
				FocusOnUnit();
			}
			break;
		}
		if (ptr.evt != POINTER_INFO.INPUT_EVENT.RELEASE)
		{
		}
	}

	public void SetupForUnit(Actor actor, int index)
	{
		mBorderColourSelected = false;
		mActor = actor;
		if (UnitName != null)
		{
			UnitName.Text = actor.baseCharacter.UnitName;
		}
		SetBorderColour();
		OrderIcon.Hide(true);
		PointManIcon.Hide(true);
		mCameoFrame = index;
		CameoImage.SetFrame(0, mCameoFrame);
	}

	public void UpdateForUnit()
	{
		GameplayController gameplayController = GameplayController.Instance();
		if (gameplayController == null)
		{
			return;
		}
		if (gameplayController.IsSelectedLeader(mActor))
		{
			if (PointManIcon.IsHidden())
			{
				PointManIcon.Hide(false);
			}
		}
		else if (!PointManIcon.IsHidden())
		{
			PointManIcon.Hide(true);
		}
		mBorderColourSelected = gameplayController.IsSelected(mActor);
		SetBorderColour();
		SoldierMarker.UpdateOrderIcon(mActor, mActor.health, ref OrderIcon);
		int num = MyActor.weapon.ActiveWeapon.GetWeaponType();
		if (num == 0 && MyActor.weapon.ActiveWeapon.GetClass() == WeaponDescriptor.WeaponClass.Special)
		{
			if (!WeaponIcon.IsHidden())
			{
				WeaponIcon.Hide(true);
			}
		}
		else if (WeaponIcon.IsHidden())
		{
			WeaponIcon.Hide(false);
		}
		if (WeaponIcon.GetCurAnim() == null || WeaponIcon.GetCurAnim().GetCurPosition() != num)
		{
			int num2 = 0;
			if (WeaponIcon.GetCurAnim() != null)
			{
				num2 = WeaponIcon.GetCurAnim().GetFrameCount();
			}
			if (num >= num2)
			{
				num = num2 - 1;
			}
			WeaponIcon.SetFrame(0, num);
		}
	}

	private void SetBorderColour()
	{
		Color color = ColourChart.UnSelectedPlayerSoldier;
		if (mBorderColourSelected)
		{
			color = ColourChart.GetHealthBarColour(mActor.health.Health01);
			Border.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
		}
		else
		{
			Border.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (Border.color != color)
		{
			Border.SetColor(color);
		}
	}
}
