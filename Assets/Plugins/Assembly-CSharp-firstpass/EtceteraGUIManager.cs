using UnityEngine;

public class EtceteraGUIManager : MonoBehaviour
{
	private void Start()
	{
		EtceteraBinding.setPopoverPoint(500f, 200f);
	}

	private void Update()
	{
		Screen.orientation = (ScreenOrientation)Input.deviceOrientation;
	}

	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float num2 = ((Screen.width < 960 && Screen.height < 960) ? 160 : 320);
		float num3 = ((Screen.width < 960 && Screen.height < 960) ? 40 : 80);
		float num4 = num3 + 10f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Get Current Language"))
		{
			Debug.Log("current launguage: " + EtceteraBinding.getCurrentLanguage());
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get Localized String"))
		{
			string localizedString = EtceteraBinding.getLocalizedString("hello", "hello in English");
			Debug.Log("'hello' localized: " + localizedString);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Alert with one Button"))
		{
			string[] buttons = new string[1] { "OK" };
			EtceteraBinding.showAlertWithTitleMessageAndButtons("This is the title", "You should really read this before pressing OK", buttons);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Alert with three Buttons"))
		{
			string[] buttons2 = new string[3] { "OK", "In The Middle", "Cancel" };
			EtceteraBinding.showAlertWithTitleMessageAndButtons("This is another title", "You should really read this before pressing a button", buttons2);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Set Prompt Colors"))
		{
			EtceteraBinding.setPromptColors(uint.MaxValue, 4288217343u, 4288217156u);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Prompt with 1 Field"))
		{
			EtceteraBinding.showPromptWithOneField("Enter your name", "This is the name of the main character", "name", false);
		}
		num = 5f;
		left = (float)Screen.width - num2 - 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Show Prompt with 2 Fields"))
		{
			EtceteraBinding.showPromptWithTwoFields("Enter your credentials", string.Empty, "username", "password", false);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Open Web Page"))
		{
			EtceteraBinding.showWebPage("http://www.prime31.com", true);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show Mail Composer"))
		{
			EtceteraBinding.showMailComposer("support@somecompany.com", "Tell us what you think", "I <b>really</b> like this game!", true);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Show SMS Composer") && EtceteraBinding.isSMSAvailable())
		{
			EtceteraBinding.showSMSComposer("some text to prefill the message with");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Mail Composer with Screenshot"))
		{
			StartCoroutine(EtceteraBinding.showMailComposerWithScreenshot(this, string.Empty, "Game Screenshot", "I like this game!", false));
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Take Screen Shot"))
		{
			StartCoroutine(EtceteraBinding.takeScreenShot("someScreenshot.png"));
		}
		if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 40, 95f, 35f), "Next Scene"))
		{
			Application.LoadLevel("EtceteraTestSceneTwo");
		}
	}
}
