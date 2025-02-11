using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseHandler : SingletonMonoBehaviour, iSwrveUpdatable
{
	private enum PurchaseState
	{
		Starting = 0,
		Disconnected = 1,
		WaitingForProducts = 2,
		Purchasing = 3,
		Idle = 4
	}

	public enum PurchaseFailedReason
	{
		NormalStoreError = 0,
		UserCancelledPurchase = 1,
		BedrockError = 2
	}

	public delegate void PurchaseSuccessDelegate(string id);

	public delegate void PurchaseFailedDelegate(PurchaseFailedReason reason);

	private const int MAX_VALIDATION_START_FAILURES = 5;

	private const int NUM_PRODUCTS = 6;

	[HideInInspector]
	public bool PurchaseInProgress;

	[HideInInspector]
	public bool gotProducts;

	private TokenOfferData m_TokenOffer = new TokenOfferData(6);

	private Bedrock.IAPCatalogEntry[] products = new Bedrock.IAPCatalogEntry[6];

	private short purchaseTaskHandle = -1;

	private short receiptValidationTaskHandle = -1;

	private int receiptValidationFailures;

	private string[] DefaultPrices = new string[6] { "$0.99", "$1.99", "$9.99", "$19.99", "$49.99", "$69.99" };

	private string[] productIdStrings = new string[6] { "pack01", "pack02", "pack03", "pack04", "pack05", "pack06" };

	private int[] HardCurrencyAmount = new int[6] { 1000, 2600, 8000, 17000, 30000, 70000 };

	private int[] DisplayOrder = new int[6] { 5, 4, 3, 2, 1, 0 };

	private string[] ValueKeys = new string[6]
	{
		string.Empty,
		string.Empty,
		string.Empty,
		string.Empty,
		"S_GOOD_VALUE",
		"S_BEST_VALUE"
	};

	private Bedrock.brIAPProductCategory[] productCategories = new Bedrock.brIAPProductCategory[6];

	public static PurchaseHandler Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetSingletonInstance<PurchaseHandler>();
		}
	}

	public bool WaitingForProducts { get; private set; }

	public bool TokenOfferInProgress
	{
		get
		{
			return m_TokenOffer.InProgress;
		}
	}

	public static event PurchaseSuccessDelegate PurchaseSuccess;

	public static event PurchaseFailedDelegate PurchaseFailed;

	public string ProductId(int index)
	{
		if (index >= 0 && index < productIdStrings.Length)
		{
			return productIdStrings[index];
		}
		return "Unknown";
	}

	public int GetDisplayOrder(int index)
	{
		return DisplayOrder[index];
	}

	public int NumProducts()
	{
		return productIdStrings.Length;
	}

	public string GetProductValueKey(int id)
	{
		return ValueKeys[id];
	}

	public string GetProductPrice(int id)
	{
		if (Application.isEditor)
		{
			return DefaultPrices[id];
		}
		if (gotProducts)
		{
			return products[id].IAPLocalizedProductPrice;
		}
		return string.Empty;
	}

	public string GetProductTitle(int id)
	{
		if (Application.isEditor)
		{
			string key = "S_MTX_OPTION_0" + (id + 1);
			return Language.Get(key);
		}
		if (gotProducts)
		{
			string key2 = "S_MTX_OPTION_0" + (id + 1);
			return Language.Get(key2);
		}
		return string.Empty;
	}

	public int GetProductValueAmount(int id)
	{
		return HardCurrencyAmount[id];
	}

	public int GetProductValueAmountIncludingPromotion(int id)
	{
		if (ItemHasTokenOffer(id))
		{
			return GetProductTokenOfferHardCurrencyAmount(id);
		}
		return HardCurrencyAmount[id];
	}

	public bool ItemHasTokenOffer(int id)
	{
		return m_TokenOffer.InProgress && m_TokenOffer.ItemHasTokenOffer(id);
	}

	public int GetProductTokenOfferHardCurrencyAmount(int id)
	{
		return m_TokenOffer.TokenOfferHardCurrencyAmount(id);
	}

	public uint GetTokenOfferRemainingSeconds()
	{
		return m_TokenOffer.TimeRemainingSeconds();
	}

	public float TokenOfferBestPercentageExtra()
	{
		float num = 0f;
		for (int i = 0; i < 6; i++)
		{
			int productTokenOfferHardCurrencyAmount = GetProductTokenOfferHardCurrencyAmount(i);
			if (productTokenOfferHardCurrencyAmount != 0)
			{
				int productValueAmount = GetProductValueAmount(i);
				float num2 = (float)(productTokenOfferHardCurrencyAmount - productValueAmount) / (float)productValueAmount;
				Debug.Log("nonSaleAmount " + productValueAmount + " saleAmount " + productTokenOfferHardCurrencyAmount + " salePercent " + num2);
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		return num;
	}

	protected override void AwakeOnce()
	{
		base.AwakeOnce();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void Start()
	{
	}

	public void GetProducts()
	{
		gotProducts = false;
		WaitingForProducts = true;
		purchaseTaskHandle = -1;
		Bedrock.InitializeIAPCatalog(productIdStrings, productCategories, (uint)NumProducts());
	}

	private void OnEnable()
	{
		Bedrock.IAPCatalogRetrieved += HandleIAPCatalogRetrieved;
		Bedrock.IAPRequestCompleted += HandleIAPRequestCompleted;
	}

	private void OnDisable()
	{
		Bedrock.IAPCatalogRetrieved -= HandleIAPCatalogRetrieved;
		Bedrock.IAPRequestCompleted -= HandleIAPRequestCompleted;
	}

	public IEnumerator Buy(int id)
	{
		if (purchaseTaskHandle == -1)
		{
			PurchaseInProgress = true;
			string productId = productIdStrings[id];
			ulong hardCurrencyAmount = (ulong)HardCurrencyAmount[id];
			Bedrock.SetInAppPurchasingCatalogEntryVirtualCurrencyInfo(productId, "Tokens", hardCurrencyAmount);
			purchaseTaskHandle = Bedrock.RequestInAppPurchase(productId);
			if (purchaseTaskHandle == -1)
			{
				Debug.Log("RequestInAppPurchase failure");
				HandleUserPurchaseFailed(productId);
				yield break;
			}
			using (BedrockTask task = new BedrockTask(purchaseTaskHandle))
			{
				Debug.Log("Waiting for task...");
				yield return StartCoroutine(task.WaitForTaskToCompleteCoroutine());
				Debug.Log("Task completed with status " + task.Status);
			}
			purchaseTaskHandle = -1;
			if (!ReCheckForPendingCompletedPurchases())
			{
				OnPurchaseFailed(productId, PurchaseFailedReason.BedrockError);
			}
		}
		else
		{
			Debug.Log("Attempting to start purchase while one is already pending.");
		}
	}

	private void HandleProductListFailed(string error)
	{
		Debug.Log(error);
		WaitingForProducts = false;
		gotProducts = false;
	}

	private void HandlePurchaseSuccessful(Bedrock.IAPCatalogEntry entry)
	{
		Debug.Log("Purchase success: " + entry.IAPProductID);
		for (int i = 0; i < productIdStrings.Length; i++)
		{
			if (entry.IAPProductID == productIdStrings[i])
			{
				int productValueAmountIncludingPromotion = GetProductValueAmountIncludingPromotion(i);
				GameSettings.Instance.PlayerCash().AdjustHardCashFromStore(productValueAmountIncludingPromotion);
				ulong cost = (ulong)(products[i].IAPProductRawPrice * 100f);
				SwrveEventsPurchase.HardCurrencyPack(entry.IAPProductID, i, cost, (ulong)productValueAmountIncludingPromotion);
				break;
			}
		}
		PurchaseInProgress = false;
		if (PurchaseHandler.PurchaseSuccess != null)
		{
			PurchaseHandler.PurchaseSuccess(entry.IAPProductID);
		}
	}

	private void HandleUserPurchaseFailed(string productId)
	{
		Debug.Log("HandleUserPurchaseFailed: " + productId);
		SwrveEventsPurchase.HardCurrencyPackFailed(productId);
		OnPurchaseFailed(productId, PurchaseFailedReason.NormalStoreError);
		PurchaseInProgress = false;
	}

	private void HandlePurchaseCancel(Bedrock.IAPCatalogEntry entry)
	{
		Debug.Log("HandlePurchaseCancel: " + entry.IAPProductID);
		SwrveEventsPurchase.HardCurrencyPackCancelled(entry.IAPProductID);
		OnPurchaseFailed(entry.IAPProductID, PurchaseFailedReason.UserCancelledPurchase);
		PurchaseInProgress = false;
	}

	private void OnPurchaseFailed(string productId, PurchaseFailedReason reason)
	{
		Debug.Log("OnPurchaseFailed: " + productId + " reason: " + reason);
		if (PurchaseHandler.PurchaseFailed != null)
		{
			PurchaseHandler.PurchaseFailed(reason);
		}
		PurchaseInProgress = false;
	}

	private void HandleIAPCatalogRetrieved(object sender, EventArgs e)
	{
		Debug.Log("Products retrieved. Idle");
		WaitingForProducts = false;
		int num = 0;
		bool flag = true;
		string[] array = productIdStrings;
		foreach (string productId in array)
		{
			Bedrock.IAPCatalogEntry iAPCatalogEntry = Bedrock.GetIAPCatalogEntry(productId);
			if (num < products.Length)
			{
				products[num++] = iAPCatalogEntry;
			}
			if (iAPCatalogEntry == null)
			{
				flag = false;
				continue;
			}
			Debug.Log("Product " + iAPCatalogEntry.IAPProductID + " retrieved with status " + iAPCatalogEntry.IAPProductStatus);
			switch (iAPCatalogEntry.IAPProductStatus)
			{
			case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_UNKNOWN:
			case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_NOT_VALID:
			case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PENDING_CATALOG_UPDATE:
				flag = false;
				break;
			}
		}
		gotProducts = flag;
	}

	private bool ReCheckForPendingCompletedPurchases()
	{
		if (Application.isEditor)
		{
			return true;
		}
		uint numberOfItems = 0u;
		if (!Bedrock.GetInAppPurchasingStoredCompletedPurchaseCount(out numberOfItems))
		{
			return false;
		}
		if (numberOfItems == 0)
		{
			return true;
		}
		Bedrock.IAPCatalogEntry catalogEntry;
		if (!Bedrock.GetInAppPurchasingFirstCompletedStoredPurchase(out catalogEntry))
		{
			return false;
		}
		string iAPProductID = catalogEntry.IAPProductID;
		Bedrock.brIAPProductStatus iAPProductStatus = catalogEntry.IAPProductStatus;
		bool flag = true;
		switch (iAPProductStatus)
		{
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATED:
			HandlePurchaseSuccessful(catalogEntry);
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_CANCELED:
			Debug.Log("Purchase of " + iAPProductID + " cancelled");
			HandlePurchaseCancel(catalogEntry);
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_FAILED:
			Debug.Log("Purchase of " + iAPProductID + " failed with status " + iAPProductStatus);
			HandleUserPurchaseFailed(iAPProductID);
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_VALIDATION_FAILED:
			Debug.Log("Purchase of " + iAPProductID + " failed with status " + iAPProductStatus);
			HandleUserPurchaseFailed(iAPProductID);
			SecureStorage.Instance.ReceiptValidationFailed = true;
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATING:
			flag = false;
			if (receiptValidationTaskHandle == -1)
			{
				Debug.Log("No validation task underway, starting one");
				if (receiptValidationFailures > 5)
				{
					Debug.Log("Failed to start validation " + receiptValidationFailures + " times.  Giving up and forcing removal from queue");
					receiptValidationFailures = 0;
					HandlePurchaseSuccessful(catalogEntry);
					flag = true;
				}
				else
				{
					Debug.Log("Starting new validation task");
					StartCoroutine(StartValidatingLastItemInQueue());
				}
			}
			else
			{
				Debug.Log("Validation task underway. Waiting for completion");
			}
			break;
		default:
			flag = false;
			break;
		}
		if (flag)
		{
			Debug.Log(string.Concat("Status ", iAPProductStatus, " for top item was a final status, removing from queue"));
			if (!Bedrock.ClearInAppPurchasingFirstCompletedStoredPurchase())
			{
				Debug.Log("Removed item from queue, but tokens were already given");
				return false;
			}
			Debug.Log("Removed item successfully");
		}
		else
		{
			Debug.Log(string.Concat("Status ", iAPProductStatus, " for top item not a final statys. Not removing from queue"));
		}
		return true;
	}

	private IEnumerator StartValidatingLastItemInQueue()
	{
		Debug.Log("StartValidatingLastItemInQueue");
		if (receiptValidationTaskHandle != -1)
		{
			Debug.Log("Called StartInvalidatinLastItemInQueue() while a validation task was already underway");
			yield break;
		}
		receiptValidationTaskHandle = Bedrock.ValidateLastInAppPurchaseReceipt();
		if (receiptValidationTaskHandle == -1)
		{
			receiptValidationFailures++;
			Debug.Log("Failed to start validation task (failurecount " + receiptValidationFailures + " ) This is bad, waiting 1s");
			float finishTime = Time.realtimeSinceStartup + 1f;
			while (Time.realtimeSinceStartup < finishTime)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		else
		{
			receiptValidationFailures = 0;
			using (BedrockTask task = new BedrockTask(receiptValidationTaskHandle))
			{
				Debug.Log("Waiting for validation task");
				yield return StartCoroutine(task.WaitForTaskToCompleteCoroutine());
			}
			Debug.Log("Validation task complete. Re-checking for completed purchase");
			receiptValidationTaskHandle = -1;
		}
		ReCheckForPendingCompletedPurchases();
	}

	private void HandleIAPRequestCompleted(object sender, EventArgs e)
	{
		Debug.Log("HandleIAPRequestCompleted Event raised from bedrock");
		ReCheckForPendingCompletedPurchases();
	}

	private void DumpIAPItemStatus(Bedrock.brIAPProductStatus productStatus)
	{
		switch (productStatus)
		{
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_UNKNOWN:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_UNKNOWN");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_NOT_VALID:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_NOT_VALID");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PENDING_CATALOG_UPDATE:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_PENDING_CATALOG_UPDATE");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_AVAILABLE:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_AVAILABLE");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_PENDING:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_PURCHASE_PENDING");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_CANCELED:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_PURCHASE_CANCELED");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_FAILED:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_PURCHASE_FAILED");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_VALIDATION_FAILED:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_PURCHASE_VALIDATION_FAILED");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATING:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATING");
			break;
		case Bedrock.brIAPProductStatus.BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATED:
			Debug.Log("productStatus: BR_IAP_PRODUCT_STATUS_PURCHASE_SUCCEEDED_VALIDATED");
			break;
		}
	}

	public void UpdateFromSwrve()
	{
		string itemId = "Exchange";
		Dictionary<string, string> resourceDictionary = null;
		if (Bedrock.GetRemoteUserResources(itemId, out resourceDictionary) && resourceDictionary != null)
		{
			for (int i = 0; i < HardCurrencyAmount.Length; i++)
			{
				HardCurrencyAmount[i] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "Pack" + (i + 1), HardCurrencyAmount[i]);
				ValueKeys[i] = Bedrock.GetFromResourceDictionaryAsString(resourceDictionary, "PackString" + (i + 1), ValueKeys[i]);
				DisplayOrder[i] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "Order" + (i + 1), DisplayOrder[i]);
			}
		}
		m_TokenOffer.UpdateFromSwrve(6);
	}
}
