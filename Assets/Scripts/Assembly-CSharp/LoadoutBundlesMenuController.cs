using System.Collections.Generic;

public class LoadoutBundlesMenuController : FrontEndScreen
{
	private enum Bundle
	{
		Offers = 0,
		Assault = 1,
		Stealth = 2,
		Balanced = 3,
		Heavy = 4,
		SMG = 5,
		None = -1
	}

	public BundleDescriptor[] Bundles;

	public LoadoutBundlePanel BundlePanel;

	public MenuScreenBlade OfferPanel;

	public LoadoutSideMenu BundleSidePanel;

	private FrontEndButton mCompletePanelButton;

	private MenuScreenBlade mCurrentBlade;

	private SocialBroadcastDialogHelper mDialogHelper;

	private PurchaseFlowHelper.PurchaseData mData;

	private Bundle mSelectedBundle;

	protected override void Awake()
	{
		if (TBFUtils.UseAlternativeLayout())
		{
			EZScreenPlacement componentInChildren = GetComponentInChildren<EZScreenPlacement>();
			componentInChildren.smallScreenDevicePos.y = 0f;
			componentInChildren.screenPos = componentInChildren.smallScreenDevicePos * 2f;
		}
		ID = ScreenID.BundleSelect;
		base.Awake();
		mData = new PurchaseFlowHelper.PurchaseData();
		mData.Type = PurchaseFlowHelper.PurchaseData.PurchaseType.Bundles;
		mData.ScriptToCallWithResult = this;
		mData.MethodToCallWithResult = "RefreshScreen";
		mData.ConfirmPurchase = true;
		if (mDialogHelper == null)
		{
			mDialogHelper = base.gameObject.AddComponent<SocialBroadcastDialogHelper>();
			mDialogHelper.AllowedToPost = true;
		}
		if (OfferPanel != null)
		{
			mCompletePanelButton = OfferPanel.GetComponentInChildren<FrontEndButton>();
			SetEnabledStateOfCompleteBundle();
		}
		mSelectedBundle = Bundle.None;
	}

	public override void EnterScreen()
	{
		base.EnterScreen();
		if (mSelectedBundle == Bundle.None)
		{
			SetSelectedBundle(0);
			mCurrentBlade = OfferPanel;
		}
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		if (mCurrentBlade.IsActive || mCurrentBlade.IsTransitioningOn)
		{
			mCurrentBlade.Deactivate();
		}
	}

	public override void OnScreen()
	{
		base.OnScreen();
		RefreshBundleDataWhenOffScreen(mCurrentBlade, MenuScreenBlade.BladeTransition.None);
	}

	public void OnSelectedBundle()
	{
		int num = BundleSidePanel.FindPressed();
		if (num != -1)
		{
			SetSelectedBundle(num);
		}
	}

	private void SetSelectedBundle(int bundleIndex)
	{
		if (mSelectedBundle != (Bundle)bundleIndex)
		{
			mSelectedBundle = (Bundle)bundleIndex;
			BundleSidePanel.SetSelected(bundleIndex);
			if (mCurrentBlade != null && (mCurrentBlade.IsActive || mCurrentBlade.IsTransitioningOn))
			{
				mCurrentBlade.Deactivate(RefreshBundleDataWhenOffScreen);
			}
			ToolTipController.Instance.ClearToolTip();
		}
	}

	private void RefreshBundleDataWhenOffScreen(MenuScreenBlade blade, MenuScreenBlade.BladeTransition type)
	{
		if (base.IsActive && !base.IsTransitioning)
		{
			if (mSelectedBundle == Bundle.Offers)
			{
				SetEnabledStateOfCompleteBundle();
				OfferPanel.Activate();
				mCurrentBlade = OfferPanel;
			}
			else
			{
				BundlePanel.Refresh(Bundles[(int)(mSelectedBundle - 1)]);
				BundlePanel.Activate();
				mCurrentBlade = BundlePanel;
			}
		}
	}

	private void CompleteBundlePressed()
	{
		GameSettings instance = GameSettings.Instance;
		if (mData == null || !(instance != null))
		{
			return;
		}
		List<BundleDescriptor> list = new List<BundleDescriptor>();
		for (int i = 0; i < Bundles.Length; i++)
		{
			if (!instance.WasUnlockedEarly(Bundles[i].Weapon.Name) && Bundles[i].CompletePackage)
			{
				list.Add(Bundles[i]);
			}
		}
		mData.Bundles = list.ToArray();
		mData.NumItems = list.Count;
		PurchaseFlowHelper.Instance.Purchase(mData);
	}

	private void BundlePressed()
	{
		int num = (int)(mSelectedBundle - 1);
		if (num != -1 && num < Bundles.Length && Bundles[num] != null && mData != null)
		{
			mData.Bundles = new BundleDescriptor[1];
			mData.Bundles[0] = Bundles[num];
			mData.NumItems = 1;
			PurchaseFlowHelper.Instance.Purchase(mData);
		}
	}

	private void RefreshScreen()
	{
		if (mSelectedBundle == Bundle.Offers)
		{
			SetEnabledStateOfCompleteBundle();
		}
		RefreshBundleDataWhenOffScreen(mCurrentBlade, MenuScreenBlade.BladeTransition.None);
		MessageBoxController instance = MessageBoxController.Instance;
		if (instance != null)
		{
			instance.DoBundleDialogue(this, "MessageBoxResultShareBundlePurchase", mData.Bundles);
		}
	}

	private void MessageBoxResultShareBundlePurchase()
	{
		string formatString = Language.GetFormatString("S_BUNDLEBOUGHT_SOCIAL_MESSAGE", Language.Get(mData.Bundles[0].Name));
		if (mData.Bundles.Length > 1)
		{
			formatString = Language.GetFormatString("S_BUNDLEBOUGHT_SOCIAL_MESSAGE", Language.Get("S_COMPLETE_BUNDLE"));
		}
		mDialogHelper.PostMessage(formatString, this, "OnFacebookPost", "OnTwitterPost");
	}

	public void OnFacebookPost()
	{
		SwrveEventsMetaGame.FacebookBroadcast("BundleScreen", ActStructure.Instance.CurrentMissionID);
	}

	public void OnTwitterPost()
	{
		SwrveEventsMetaGame.TwitterBroadcast("BundleScreen", ActStructure.Instance.CurrentMissionID);
	}

	public bool SetEnabledStateOfCompleteBundle()
	{
		bool flag = false;
		GameSettings instance = GameSettings.Instance;
		if (mCompletePanelButton != null)
		{
			flag = GetCostOfBundlesNotOwned() > instance.AllBundleCost();
			mCompletePanelButton.CurrentState = ((!flag) ? FrontEndButton.State.Disabled : FrontEndButton.State.Normal);
			char c = CommonHelper.HardCurrencySymbol();
			mCompletePanelButton.Text = string.Format("{0}{1}", c, instance.AllBundleCost());
		}
		return flag;
	}

	private int GetCostOfBundlesNotOwned()
	{
		int num = 0;
		GameSettings instance = GameSettings.Instance;
		for (int i = 0; i < Bundles.Length; i++)
		{
			BundleDescriptor bundleDescriptor = Bundles[i];
			if (bundleDescriptor != null && !instance.WasUnlockedEarly(bundleDescriptor.Weapon.Name))
			{
				num += bundleDescriptor.HardCost;
			}
		}
		return num;
	}
}
