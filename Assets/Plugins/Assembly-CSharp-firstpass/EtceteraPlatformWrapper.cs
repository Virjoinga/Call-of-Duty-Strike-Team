public class EtceteraPlatformWrapper
{
	public static void ShowWaitingWithLabel(string strLabel)
	{
		EtceteraAndroid.showProgressDialog(string.Empty, strLabel);
	}

	public static void ShowBlankWaiting()
	{
		EtceteraAndroid.showProgressDialog(string.Empty, string.Empty);
	}

	public static void HideWaitingDialog()
	{
		EtceteraAndroid.hideProgressDialog();
	}

	public static void ShowAlert(string strTitle, string strBody, string strOK)
	{
		EtceteraAndroid.showAlert(strTitle, strBody, strOK);
	}

	public static void ShowWebPage(string strURI)
	{
		EtceteraAndroid.showWebView(strURI);
	}

	public static void SetBadgeCount(int iBadge)
	{
	}

	public static void ShowAlert(string title, string message, string positiveButton, string negativeButton)
	{
		EtceteraAndroid.showAlert(title, message, positiveButton, negativeButton);
	}
}
