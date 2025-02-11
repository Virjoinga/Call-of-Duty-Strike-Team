using System.Collections;
using UnityEngine;

public class MTXMenuController : FrontEndScreen
{
	private const float WaitForProductsTimeout = 20f;

	public MTXMenuOption[] Options;

	private HudStateController.HudState mHudStateOnStart;

	protected override void Awake()
	{
		ID = ScreenID.MTXSelect;
		base.Awake();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public override void EnterScreen()
	{
		base.EnterScreen();
		SwrveEventsUI.ViewedTokenPurchase();
		if (HudStateController.Instance != null)
		{
			mHudStateOnStart = HudStateController.Instance.State;
			HudStateController.Instance.SetState(HudStateController.HudState.Hidden);
		}
		if (Options.Length != PurchaseHandler.Instance.NumProducts())
		{
			Debug.Log("Unexpected number of options in MTX screen");
		}
		UpdateOptions();
		PurchaseHandler.PurchaseSuccess += HandlePurchaseSuccessful;
		PurchaseHandler.PurchaseFailed += HandlePurchaseFailed;
		if (!Application.isEditor)
		{
			StartWaitingForProducts();
		}
	}

	public override void ExitScreen()
	{
		base.ExitScreen();
		PurchaseHandler.PurchaseSuccess -= HandlePurchaseSuccessful;
		PurchaseHandler.PurchaseFailed -= HandlePurchaseFailed;
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		if (instance != null)
		{
			instance.PurchaseInProgress = null;
		}
	}

	public override void OnScreen()
	{
		for (int i = 0; i < Options.Length; i++)
		{
			if (Options[i] != null)
			{
				Options[i].OnScreen();
			}
		}
	}

	public override void OffScreen()
	{
		base.OffScreen();
		if (HudStateController.Instance != null)
		{
			HudStateController.Instance.SetState(mHudStateOnStart);
		}
		if (FrontEndController.Instance.ActiveScreen == ScreenID.None)
		{
			AnimatedScreenBackground instance = AnimatedScreenBackground.Instance;
			if (instance != null)
			{
				instance.Deactivate();
			}
		}
	}

	private void UpdateOptions()
	{
		Debug.Log("Update options " + PurchaseHandler.Instance.NumProducts());
		bool tokenOfferInProgress = PurchaseHandler.Instance.TokenOfferInProgress;
		for (int i = 0; i < PurchaseHandler.Instance.NumProducts(); i++)
		{
			int displayOrder = PurchaseHandler.Instance.GetDisplayOrder(i);
			if (Options.Length > displayOrder && Options[displayOrder] != null)
			{
				string callback = "PurchasePack0" + (i + 1);
				Options[displayOrder].SetupOption(PurchaseHandler.Instance.GetProductTitle(i).ToUpper(), PurchaseHandler.Instance.GetProductValueKey(i), PurchaseHandler.Instance.GetProductPrice(i), PurchaseHandler.Instance.GetProductValueAmount(i), this, callback);
				if (tokenOfferInProgress && PurchaseHandler.Instance.ItemHasTokenOffer(i))
				{
					Options[displayOrder].SetupTokenSale(PurchaseHandler.Instance.GetProductTokenOfferHardCurrencyAmount(i), PurchaseHandler.Instance.GetTokenOfferRemainingSeconds());
				}
				else
				{
					Options[displayOrder].ClearSale();
				}
			}
		}
	}

	private void ClearOptions()
	{
		for (int i = 0; i < Options.Length; i++)
		{
			if (Options[i] != null)
			{
				Options[i].ClearOption(i);
			}
		}
	}

	private void PurchasePack01()
	{
		PurchasePack(0);
	}

	private void PurchasePack02()
	{
		PurchasePack(1);
	}

	private void PurchasePack03()
	{
		PurchasePack(2);
	}

	private void PurchasePack04()
	{
		PurchasePack(3);
	}

	private void PurchasePack05()
	{
		PurchasePack(4);
	}

	private void PurchasePack06()
	{
		PurchasePack(5);
	}

	private void PurchasePack(int item)
	{
		if (FrontEndController.Instance.IsBusy)
		{
			return;
		}
		if (Application.isEditor)
		{
			Debug.Log("Purchased pack " + item + " for " + PurchaseHandler.Instance.GetProductPrice(item) + " and returned " + PurchaseHandler.Instance.GetProductValueAmount(item) + " hard currency");
			GameSettings instance = GameSettings.Instance;
			instance.PlayerCash().AdjustHardCashFromStore(PurchaseHandler.Instance.GetProductValueAmountIncludingPromotion(item));
			HandlePurchaseSuccessful(string.Empty);
		}
		else if (!PurchaseHandler.Instance.gotProducts)
		{
			Debug.LogError("Not got products yet...");
		}
		else if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			string[] array = new string[1] { Language.Get("S_OKAY") };
			TBFUtils.AndroidShowMessageBox(Language.Get("S_PURCHASE_ERROR_TITLE"), Language.Get("S_PURCHASE_INTERNET_REQUIRED"), array[0], string.Empty, string.Empty);
		}
		else
		{
			Bedrock.brIAPAvailabilityStatus iAPAvailabilityStatus = Bedrock.GetIAPAvailabilityStatus();
			if (iAPAvailabilityStatus == Bedrock.brIAPAvailabilityStatus.BR_IAP_AVAILABILITY_PURCHASES_ENABLED || iAPAvailabilityStatus == Bedrock.brIAPAvailabilityStatus.BR_IAP_AVAILABILITY_PURCHASES_ENABLED_NO_VERIFICATION)
			{
				StartCoroutine(PurchaseHandler.Instance.Buy(item));
				StartActivityViewWithLabel(Language.Get("S_PURCHASE_PURCHASING"));
			}
			else
			{
				string[] array2 = new string[1] { Language.Get("S_OKAY") };
				TBFUtils.AndroidShowMessageBox(Language.Get("S_PURCHASE_ERROR_TITLE"), Language.Get("S_PURCHASE_INTERNET_REQUIRED"), array2[0], string.Empty, string.Empty);
			}
		}
	}

	private void StartWaitingForProducts()
	{
		ClearOptions();
		if (PurchaseHandler.Instance != null && PurchaseHandler.Instance.gotProducts && Application.internetReachability != 0)
		{
			Debug.Log("Got products....");
			HandlePurchaseHandlerProductListRecieved();
			return;
		}
		Bedrock.brIAPAvailabilityStatus iAPAvailabilityStatus = Bedrock.GetIAPAvailabilityStatus();
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			ShowWifiErrorMessage();
			return;
		}
		switch (iAPAvailabilityStatus)
		{
		case Bedrock.brIAPAvailabilityStatus.BR_IAP_AVAILABILITY_PURCHASES_DISABLED:
			ShowRestrictionMessage();
			return;
		case Bedrock.brIAPAvailabilityStatus.BR_IAP_AVAILABILITY_UNAVAILABLE:
			OnRetrieveProductsFailed(Language.Get("S_PURCHASE_RESTRICTION_TITLE"), Language.Get("S_PURCHASE_NEED_GP_ACCOUNT"));
			return;
		}
		StartActivityViewWithLabel(Language.Get("S_PURCHASE_LOADING_PRODUCTS"));
		if (!PurchaseHandler.Instance.WaitingForProducts)
		{
			PurchaseHandler.Instance.GetProducts();
		}
		StartCoroutine(WaitForProducts());
	}

	private IEnumerator WaitForProducts()
	{
		float timeoutTime = Time.time + 20f;
		while (PurchaseHandler.Instance.WaitingForProducts && Time.time < timeoutTime)
		{
			yield return new WaitForEndOfFrame();
		}
		FinishActivityView();
		if (PurchaseHandler.Instance.gotProducts)
		{
			HandlePurchaseHandlerProductListRecieved();
			yield break;
		}
		Debug.LogError("PurchaseHandler said we didn't get any products");
		OnRetrieveProductsFailed(Language.Get("S_PURCHASE_UNABLE_TO_CONNECT"), Language.Get("S_PURCHASE_CANNOT_CONNECT_GP"));
	}

	private void HandlePurchaseHandlerProductListRecieved()
	{
		SwrveEventsPurchase.ProductListRecieved(PurchaseHandler.Instance.NumProducts());
		UpdateOptions();
		SwrveEventsUI.SwrveTalkTrigger_Store();
	}

	private void ShowRestrictionMessage()
	{
		OnRetrieveProductsFailed(Language.Get("S_PURCHASE_RESTRICTION_TITLE"), Language.Get("S_PURCHASE_RESTRICTION_BODY_GP"));
	}

	private void ShowWifiErrorMessage()
	{
		OnRetrieveProductsFailed(Language.Get("S_PURCHASE_LOADING_PRODUCTS"), Language.Get("S_PURCHASE_INTERNET_REQUIRED"));
	}

	private void OnRetrieveProductsFailed(string title, string text)
	{
		string[] array = new string[1] { Language.Get("S_PURCHASE_GOBACK") };
		TBFUtils.AndroidShowMessageBox(title, text, array[0], "MTXMenuController", "HandleUnableToConnectButtonPressed");
		Debug.LogError("OnRetrieveProductsFailed " + title + text);
	}

	private void HandleUnableToConnectButtonPressed(string buttonText)
	{
		EtceteraManager.alertButtonClicked -= HandleUnableToConnectButtonPressed;
		FrontEndController.Instance.ForceReturnToPrevious();
	}

	private void HandlePurchaseFailed(PurchaseHandler.PurchaseFailedReason reason)
	{
		Debug.Log("Purchase FAILED");
		FinishActivityView();
		if (reason != PurchaseHandler.PurchaseFailedReason.UserCancelledPurchase)
		{
			string[] array = new string[1] { Language.Get("S_OKAY") };
			TBFUtils.AndroidShowMessageBox(Language.Get("S_PURCHASE_FAILED_TITLE"), Language.Get("S_PURCHASE_FAILED_BODY"), array[0], string.Empty, string.Empty);
		}
	}

	private void HandlePurchaseSuccessful(string id)
	{
		Debug.Log("Purchase SUCCEEEDED " + id);
		SwrveUserData.Instance.LogMTXPurchase();
		FinishActivityView();
		string[] array = new string[1] { Language.Get("S_OKAY") };
		TBFUtils.AndroidShowMessageBox(Language.Get("S_PURCHASE_SUCCEEDED_TITLE"), Language.Get("S_PURCHASE_SUCCEEDED_BODY"), array[0], string.Empty, string.Empty);
		PurchaseFlowHelper instance = PurchaseFlowHelper.Instance;
		if (instance != null && instance.PurchaseInProgress != null)
		{
			PurchaseFlowHelper.PurchaseData purchaseInProgress = instance.PurchaseInProgress;
			GameSettings instance2 = GameSettings.Instance;
			if (instance2 != null && instance2.PlayerCash().HardCash() >= instance.RequiredFunds)
			{
				FrontEndController.Instance.ReturnToPrevious();
				PurchaseFlowHelper.Instance.Purchase(purchaseInProgress);
			}
		}
	}

	private void StartActivityViewWithLabel(string content)
	{
		UIManager.instance.blockInput = true;
		EtceteraBinding.showBezelActivityViewWithLabel(content);
	}

	private void FinishActivityView()
	{
		UIManager.instance.blockInput = false;
		EtceteraBinding.hideActivityView();
	}
}
