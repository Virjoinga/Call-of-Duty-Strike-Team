using UnityEngine;

public class EtceteraGUIManagerThree : MonoBehaviour
{
	private void OnGUI()
	{
		float num = 5f;
		float left = 5f;
		float num2 = ((Screen.width < 960 && Screen.height < 960) ? 160 : 320);
		float num3 = ((Screen.width < 960 && Screen.height < 960) ? 40 : 80);
		float num4 = num3 + 10f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Show Inline WebView"))
		{
			EtceteraBinding.inlineWebViewShow(50, 10, 260, 300);
			EtceteraBinding.inlineWebViewSetUrl("http://google.com");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Close Inline WebView"))
		{
			EtceteraBinding.inlineWebViewClose();
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Set Url of Inline WebView"))
		{
			EtceteraBinding.inlineWebViewSetUrl("http://prime31.com");
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Set Frame of Inline WebView"))
		{
			EtceteraBinding.inlineWebViewSetFrame(10, 200, 250, 250);
		}
		if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 50, 95f, 45f), "Back"))
		{
			Application.LoadLevel("EtceteraTestScene");
		}
		num = 5f;
		left = (float)Screen.width - num2 - 5f;
		if (GUI.Button(new Rect(left, num, num2, num3), "Get Badge Count"))
		{
			Debug.Log("badge count is: " + EtceteraBinding.getBadgeCount());
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Set Badge Count"))
		{
			EtceteraBinding.setBadgeCount(46);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get Orientation"))
		{
			UIInterfaceOrientation statusBarOrientation = EtceteraBinding.getStatusBarOrientation();
			Debug.Log("status bar orientation: " + statusBarOrientation);
		}
		if (GUI.Button(new Rect(left, num += num4, num2, num3), "Get UDIDs"))
		{
			string arg = EtceteraBinding.uniqueDeviceIdentifier();
			string arg2 = EtceteraBinding.uniqueGlobalDeviceIdentifier();
			Debug.Log(string.Format("unique: {0}\nglobal: {1}", arg, arg2));
		}
	}
}
