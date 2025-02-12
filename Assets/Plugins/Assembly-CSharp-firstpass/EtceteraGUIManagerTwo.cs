using System.Collections;
using UnityEngine;

public class EtceteraGUIManagerTwo : MonoBehaviour
{
	public GameObject testPlane;

	private string imagePath;

	private void Start()
	{
		EtceteraManager.imagePickerChoseImage += imagePickerChoseImage;
	}

	private void OnDisable()
	{
		EtceteraManager.imagePickerChoseImage -= imagePickerChoseImage;
	}

	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float num2 = 200f;
		float num3 = 40f;
		float num4 = num3 + 10f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Show Activity View"))
		{
			EtceteraBinding.showActivityView();
			StartCoroutine(hideActivityView());
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Bezel Activity View"))
		{
			EtceteraBinding.showBezelActivityViewWithLabel("Loading Stuff...");
			StartCoroutine(hideActivityView());
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Rate This App"))
		{
			EtceteraBinding.askForReview("Do you like this game?", "Please review the game if you do!", "https://userpub.itunes.apple.com/WebObjects/MZUserPublishing.woa/wa/addUserReview?id=366238041&type=Prime31+Studios");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Prompt for Photo"))
		{
			EtceteraBinding.promptForPhoto(0.25f, PhotoPromptType.CameraAndAlbum);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Register for Push"))
		{
			EtceteraBinding.registerForRemoteNotifcations((P31RemoteNotificationType)7);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get Registered Push Types"))
		{
			P31RemoteNotificationType enabledRemoteNotificationTypes = EtceteraBinding.getEnabledRemoteNotificationTypes();
			if ((enabledRemoteNotificationTypes & P31RemoteNotificationType.Alert) != 0)
			{
				Debug.Log("registered for alerts");
			}
			if ((enabledRemoteNotificationTypes & P31RemoteNotificationType.Sound) != 0)
			{
				Debug.Log("registered for sounds");
			}
			if ((enabledRemoteNotificationTypes & P31RemoteNotificationType.Badge) != 0)
			{
				Debug.Log("registered for badges");
			}
		}
		num = 5f;
		left = (float)Screen.width - num2 - 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Set Urban Airship Credentials"))
		{
			EtceteraBinding.setUrbanAirshipCredentials("S8Tf2CiUQSuh2A4NVdD2CA", "J6O97Dm2QK2-GGXZsPMlEA", "optional alias");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Load Photo Texture"))
		{
			if (imagePath == null)
			{
				string[] buttons = new string[1] { "OK" };
				EtceteraBinding.showAlertWithTitleMessageAndButtons("Load Photo Texture Error", "You have to choose a photo before loading", buttons);
				return;
			}
			StartCoroutine(EtceteraManager.textureFromFileAtPath("file://" + imagePath, textureLoaded, textureLoadFailed));
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Save Photo to Album"))
		{
			if (imagePath == null)
			{
				string[] buttons2 = new string[1] { "OK" };
				EtceteraBinding.showAlertWithTitleMessageAndButtons("Load Photo Texture Error", "You have to choose a photo before loading", buttons2);
				return;
			}
			EtceteraBinding.saveImageToPhotoAlbum(imagePath);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get Image Size"))
		{
			if (imagePath == null)
			{
				string[] buttons3 = new string[1] { "OK" };
				EtceteraBinding.showAlertWithTitleMessageAndButtons("Error Getting Image Size", "You have to choose a photo before checking it's size", buttons3);
				return;
			}
			Vector2 imageSize = EtceteraBinding.getImageSize(imagePath);
			Debug.Log("image size: " + imageSize);
		}
		if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 50, 95f, 45f), "Next"))
		{
			Application.LoadLevel("EtceteraTestSceneThree");
		}
	}

	private void imagePickerChoseImage(string imagePath)
	{
		this.imagePath = imagePath;
	}

	public IEnumerator hideActivityView()
	{
		yield return new WaitForSeconds(2f);
		EtceteraBinding.hideActivityView();
	}

	public void textureLoaded(Texture2D texture)
	{
		testPlane.GetComponent<Renderer>().material.mainTexture = texture;
	}

	public void textureLoadFailed(string error)
	{
		string[] buttons = new string[1] { "OK" };
		EtceteraBinding.showAlertWithTitleMessageAndButtons("Error Loading Texture.  Did you choose a photo first?", error, buttons);
		Debug.Log("textureLoadFailed: " + error);
	}
}
