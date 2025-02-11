using System;
using System.Collections;
using UnityEngine;

public class EtceteraManager : MonoBehaviour
{
	public delegate void EceteraTextureDelegate(Texture2D texture);

	public delegate void EceteraTextureFailedDelegate(string error);

	public static event Action dismissingViewControllerEvent;

	public static event Action imagePickerCancelled;

	public static event Action<string> imagePickerChoseImage;

	public static event Action<string> alertButtonClicked;

	public static event Action promptCancelled;

	public static event Action<string> singleFieldPromptTextEntered;

	public static event Action<string, string> twoFieldPromptTextEntered;

	public static event Action<string> remoteRegistrationSucceeded;

	public static event Action<string> remoteRegistrationFailed;

	public static event Action urbanAirshipRegistrationSucceeded;

	public static event Action<string> urbanAirshipRegistrationFailed;

	public static event Action<Hashtable> remoteNotificationReceived;

	public static event Action<string> mailComposerFinished;

	public static event Action<string> smsComposerFinished;

	public static event EventHandler<EventArgs> TwitterPostSuccess;

	private void Awake()
	{
		base.gameObject.name = GetType().ToString();
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	public void dismissingViewController()
	{
		if (EtceteraManager.dismissingViewControllerEvent != null)
		{
			EtceteraManager.dismissingViewControllerEvent();
		}
	}

	public void imagePickerDidCancel(string empty)
	{
		if (EtceteraManager.imagePickerCancelled != null)
		{
			EtceteraManager.imagePickerCancelled();
		}
	}

	public void imageSavedToDocuments(string filePath)
	{
		if (EtceteraManager.imagePickerChoseImage != null)
		{
			EtceteraManager.imagePickerChoseImage(filePath);
		}
	}

	public static IEnumerator textureFromFileAtPath(string filePath, EceteraTextureDelegate del, EceteraTextureFailedDelegate errorDel)
	{
		using (WWW www = new WWW(filePath))
		{
			yield return www;
			if (www.error != null && errorDel != null)
			{
				errorDel(www.error);
			}
			Texture2D tex = www.texture;
			if (tex != null)
			{
				del(tex);
			}
		}
	}

	public void alertViewClickedButton(string buttonTitle)
	{
		if (EtceteraManager.alertButtonClicked != null)
		{
			EtceteraManager.alertButtonClicked(buttonTitle);
		}
	}

	public void alertPromptCancelled(string empty)
	{
		if (EtceteraManager.promptCancelled != null)
		{
			EtceteraManager.promptCancelled();
		}
	}

	public void alertPromptEnteredText(string text)
	{
		string[] array = text.Split(new string[1] { "|||" }, StringSplitOptions.None);
		if (array.Length == 1 && EtceteraManager.singleFieldPromptTextEntered != null)
		{
			EtceteraManager.singleFieldPromptTextEntered(array[0]);
		}
		if (array.Length == 2 && EtceteraManager.twoFieldPromptTextEntered != null)
		{
			EtceteraManager.twoFieldPromptTextEntered(array[0], array[1]);
		}
	}

	public void remoteRegistrationDidSucceed(string deviceToken)
	{
		if (EtceteraManager.remoteRegistrationSucceeded != null)
		{
			EtceteraManager.remoteRegistrationSucceeded(deviceToken);
		}
	}

	public void remoteRegistrationDidFail(string error)
	{
		if (EtceteraManager.remoteRegistrationFailed != null)
		{
			EtceteraManager.remoteRegistrationFailed(error);
		}
	}

	public void urbanAirshipRegistrationDidSucceed(string empty)
	{
		if (EtceteraManager.urbanAirshipRegistrationSucceeded != null)
		{
			EtceteraManager.urbanAirshipRegistrationSucceeded();
		}
	}

	public void urbanAirshipRegistrationDidFail(string error)
	{
		if (EtceteraManager.urbanAirshipRegistrationFailed != null)
		{
			EtceteraManager.urbanAirshipRegistrationFailed(error);
		}
	}

	public void remoteNotificationWasReceived(string json)
	{
		if (EtceteraManager.remoteNotificationReceived != null)
		{
			EtceteraManager.remoteNotificationReceived(json.hashtableFromJson());
		}
	}

	public void mailComposerFinishedWithResult(string result)
	{
		if (EtceteraManager.mailComposerFinished != null)
		{
			EtceteraManager.mailComposerFinished(result);
		}
	}

	public void smsComposerFinishedWithResult(string result)
	{
		if (EtceteraManager.smsComposerFinished != null)
		{
			EtceteraManager.smsComposerFinished(result);
		}
	}

	public void TwitterSuccess()
	{
		if (EtceteraManager.TwitterPostSuccess != null)
		{
			EtceteraManager.TwitterPostSuccess(this, new EventArgs());
		}
	}
}
