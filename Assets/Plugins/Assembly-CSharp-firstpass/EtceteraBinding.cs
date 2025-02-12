using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using Prime31;
using UnityEngine;

public class EtceteraBinding
{
	public static IEnumerator takeScreenShot(string filename)
	{
		yield return new WaitForEndOfFrame();
		ScreenCapture.CaptureScreenshot(filename);
		string path = Application.persistentDataPath + "/" + filename;
		int frame = 0;
		while (!File.Exists(path) && frame < 300)
		{
			yield return null;
			frame++;
		}
	}

	[DllImport("__Internal")]
	private static extern string _etceteraGetCurrentLanguage();

	public static string getCurrentLanguage()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _etceteraGetCurrentLanguage();
		}
		return "en";
	}

	[DllImport("__Internal")]
	private static extern string _etceteraGetLocalizedString(string key, string defaultValue);

	public static string getLocalizedString(string key, string defaultValue)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _etceteraGetLocalizedString(key, defaultValue);
		}
		return string.Empty;
	}

	[Obsolete("Use the _etceteraShowAlertWithTitleMessageAndButtons. This method will be removed.")]
	public static void showAlertWithTitleMessageAndButton(string title, string message, string buttonTitle)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			showAlertWithTitleMessageAndButtons(title, message, new string[1] { buttonTitle });
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowAlertWithTitleMessageAndButtons(string title, string message, string buttons);

	public static void showAlertWithTitleMessageAndButtons(string title, string message, string[] buttons)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			ArrayList obj = new ArrayList(buttons);
			_etceteraShowAlertWithTitleMessageAndButtons(title, message, Json.jsonEncode(obj));
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraSetPromptColors(uint borderColor, uint gradientStopOne, uint gradientStopTwo);

	public static void setPromptColors(uint borderColor, uint gradientStopOne, uint gradientStopTwo)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraSetPromptColors(borderColor, gradientStopOne, gradientStopTwo);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowPromptWithOneField(string title, string message, string placeHolder, bool autocomplete);

	public static void showPromptWithOneField(string title, string message, string placeHolder, bool autocomplete)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowPromptWithOneField(title, message, placeHolder, autocomplete);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowPromptWithTwoFields(string title, string message, string placeHolder1, string placeHolder2, bool autocomplete);

	public static void showPromptWithTwoFields(string title, string message, string placeHolder1, string placeHolder2, bool autocomplete)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowPromptWithTwoFields(title, message, placeHolder1, placeHolder2, autocomplete);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowWebPage(string url, bool showControls);

	public static void showWebPage(string url, bool showControls)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowWebPage(url, showControls);
		}
	}

	[DllImport("__Internal")]
	private static extern bool _etceteraIsEmailAvailable();

	public static bool isEmailAvailable()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _etceteraIsEmailAvailable();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern bool _etceteraIsSMSAvailable();

	public static bool isSMSAvailable()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _etceteraIsSMSAvailable();
		}
		return false;
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowMailComposer(string toAddress, string subject, string body, bool isHTML);

	public static void showMailComposer(string toAddress, string subject, string body, bool isHTML)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowMailComposer(toAddress, subject, body, isHTML);
		}
	}

	public static IEnumerator showMailComposerWithScreenshot(MonoBehaviour mono, string toAddress, string subject, string body, bool isHTML)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string tempName = string.Concat(Guid.NewGuid(), ".png");
			yield return mono.StartCoroutine(takeScreenShot(tempName));
			string path = Application.persistentDataPath + "/" + tempName;
			showMailComposerWithAttachment(path, "image/png", "screenshot.png", toAddress, subject, body, isHTML);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowMailComposerWithAttachment(string filePathToAttachment, string attachementMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML);

	public static void showMailComposerWithAttachment(string filePathToAttachment, string attachmentMimeType, string attachmentFilename, string toAddress, string subject, string body, bool isHTML)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowMailComposerWithAttachment(filePathToAttachment, attachmentMimeType, attachmentFilename, toAddress, subject, body, isHTML);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowSMSComposer(string body);

	public static void showSMSComposer(string body)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowSMSComposer(body);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowActivityView();

	public static void showActivityView()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowActivityView();
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraHideActivityView();

	public static void hideActivityView()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraHideActivityView();
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowBezelActivityViewWithLabel(string label);

	public static void showBezelActivityViewWithLabel(string label)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowBezelActivityViewWithLabel(label);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraShowBezelActivityViewWithImage(string label, string imagePath);

	public static void showBezelActivityViewWithImage(string label, string imagePath)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraShowBezelActivityViewWithImage(label, imagePath);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraAskForReview(int launchCount, float hoursBetweenPrompts, string title, string message, string iTunesUrl);

	public static void askForReview(int launchCount, float hoursBetweenPrompts, string title, string message, string iTunesUrl)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraAskForReview(launchCount, hoursBetweenPrompts, title, message, iTunesUrl);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraAskForReviewImmediately(string title, string message, string iTunesUrl);

	public static void askForReview(string title, string message, string iTunesUrl)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraAskForReviewImmediately(title, message, iTunesUrl);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraSetPopoverPoint(float xPos, float yPos);

	public static void setPopoverPoint(float xPos, float yPos)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraSetPopoverPoint(xPos, yPos);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraPromptForPhoto(float scaledToSize, int promptType);

	public static void promptForPhoto(float scaledToSize)
	{
		promptForPhoto(scaledToSize, PhotoPromptType.CameraAndAlbum);
	}

	public static void promptForPhoto(float scaledToSize, PhotoPromptType promptType)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraPromptForPhoto(scaledToSize, (int)promptType);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraResizeImageAtPath(string filePath, float width, float height);

	public static void resizeImageAtPath(string filePath, float width, float height)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraResizeImageAtPath(filePath, width, height);
		}
	}

	[DllImport("__Internal")]
	private static extern string _etceteraGetImageSize(string filePath);

	public static Vector2 getImageSize(string filePath)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			string text = _etceteraGetImageSize(filePath);
			string[] array = text.Split(',');
			if (array.Length == 2)
			{
				return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
			}
		}
		return Vector2.zero;
	}

	[DllImport("__Internal")]
	private static extern void _etceteraSaveImageToPhotoAlbum(string filePath);

	public static void saveImageToPhotoAlbum(string filePath)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraSaveImageToPhotoAlbum(filePath);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraSetUrbanAirshipCredentials(string appKey, string appSecret, string alias);

	public static void setUrbanAirshipCredentials(string appKey, string appSecret)
	{
		setUrbanAirshipCredentials(appKey, appSecret, null);
	}

	public static void setUrbanAirshipCredentials(string appKey, string appSecret, string alias)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraSetUrbanAirshipCredentials(appKey, appSecret, alias);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraRegisterForRemoteNotifications(int types);

	public static void registerForRemoteNotifcations(P31RemoteNotificationType types)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraRegisterForRemoteNotifications((int)types);
		}
	}

	[DllImport("__Internal")]
	private static extern int _etceteraGetEnabledRemoteNotificationTypes();

	public static P31RemoteNotificationType getEnabledRemoteNotificationTypes()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return (P31RemoteNotificationType)_etceteraGetEnabledRemoteNotificationTypes();
		}
		return P31RemoteNotificationType.None;
	}

	[DllImport("__Internal")]
	private static extern int _etceteraGetBadgeCount();

	public static int getBadgeCount()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _etceteraGetBadgeCount();
		}
		return 0;
	}

	[DllImport("__Internal")]
	private static extern void _etceteraSetBadgeCount(int badgeCount);

	public static void setBadgeCount(int badgeCount)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraSetBadgeCount(badgeCount);
		}
	}

	[DllImport("__Internal")]
	private static extern int _etceteraGetStatusBarOrientation();

	public static UIInterfaceOrientation getStatusBarOrientation()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return (UIInterfaceOrientation)_etceteraGetStatusBarOrientation();
		}
		return UIInterfaceOrientation.Portrait;
	}

	[DllImport("__Internal")]
	private static extern string _etceteraUniqueDeviceIdentifier();

	public static string uniqueDeviceIdentifier()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _etceteraUniqueDeviceIdentifier();
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern string _etceteraUniqueGlobalDeviceIdentifier();

	public static string uniqueGlobalDeviceIdentifier()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			return _etceteraUniqueGlobalDeviceIdentifier();
		}
		return string.Empty;
	}

	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewShow(int x, int y, int width, int height);

	public static void inlineWebViewShow(int x, int y, int width, int height)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraInlineWebViewShow(x, y, width, height);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewClose();

	public static void inlineWebViewClose()
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraInlineWebViewClose();
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewSetUrl(string url);

	public static void inlineWebViewSetUrl(string url)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraInlineWebViewSetUrl(url);
		}
	}

	[DllImport("__Internal")]
	private static extern void _etceteraInlineWebViewSetFrame(int x, int y, int width, int height);

	public static void inlineWebViewSetFrame(int x, int y, int width, int height)
	{
		if (Application.platform == RuntimePlatform.IPhonePlayer)
		{
			_etceteraInlineWebViewSetFrame(x, y, width, height);
		}
	}
}
