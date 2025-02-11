using System;
using UnityEngine;

public class SocialBroadcastDialogHelper : MonoBehaviour
{
	private bool m_AllowedToPost = true;

	private string m_Message;

	private string m_OnFacebookPost;

	private string m_OnTwitterPost;

	private MonoBehaviour m_Caller;

	public bool AllowedToPost
	{
		set
		{
			m_AllowedToPost = value;
		}
	}

	private void MissionEnded(object sender, Events.EndMission args)
	{
		m_AllowedToPost = true;
	}

	private void OnEnable()
	{
		Bedrock.FacebookRequestSuccess += OnFacebookSuccess;
		EtceteraManager.TwitterPostSuccess += TwitterSuccess;
	}

	private void OnDisable()
	{
		Bedrock.FacebookRequestSuccess -= OnFacebookSuccess;
		EtceteraManager.TwitterPostSuccess -= TwitterSuccess;
	}

	public bool PostMessage(string message, MonoBehaviour caller, string onFacebookPost, string onTwitterPost)
	{
		if (m_AllowedToPost)
		{
			m_Caller = caller;
			m_OnFacebookPost = onFacebookPost;
			m_OnTwitterPost = onTwitterPost;
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				MessageBoxController.Instance.DoNoConnectionShareDialog();
				return false;
			}
			MessageBoxController instance = MessageBoxController.Instance;
			if (instance != null)
			{
				m_Message = message;
				instance.DoSocialBroadcastDialogue(message, this, "PostFacebook", "PostTwitter");
				return true;
			}
		}
		return false;
	}

	public void PostFacebook()
	{
		Debug.Log("Posting to facebook... facebook enabled = " + Bedrock.FacebookEnabled);
		Bedrock.FacebookEnabled = true;
		Debug.Log("Posting to facebook... doing post");
		int num = StatsHelper.PlayerXP();
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		XPManager.Instance.ConvertXPToLevel(num, out level, out prestigeLevel, out xpToNextLevel, out percent);
		string text = Language.Get("S_XPLEVEL_" + level);
		string aLinkCaption = ((prestigeLevel != 0) ? Language.GetFormatString("S_FACEBOOK_LINK_CAPTION_PRESTIGE", text, prestigeLevel, num) : Language.GetFormatString("S_FACEBOOK_LINK_CAPTION", text, num));
		Bedrock.FacebookRequestReadPermissions();
		Bedrock.brFacebookPostParameters post = new Bedrock.brFacebookPostParameters(string.Empty, SwrveServerVariables.Instance.FacebookPictureURL, SwrveServerVariables.Instance.FacebookLinkURL, Language.Get("S_FACEBOOK_LINK_NAME"), aLinkCaption, m_Message, string.Empty);
		if (Bedrock.FacebookPostToWall(post))
		{
			if (m_Caller != null)
			{
				m_Caller.Invoke(m_OnFacebookPost, 0f);
			}
			Debug.Log("Post to facebook success...");
		}
	}

	public void PostTwitter()
	{
		TwitterPlugin.ComposeTweet(m_Message + " " + Language.Get("S_TWITTER_HASHTAG"), SwrveServerVariables.Instance.AppStoreUrl);
		if (m_Caller != null)
		{
			m_Caller.Invoke(m_OnTwitterPost, 0f);
		}
	}

	private void OnFacebookSuccess(object sender, EventArgs args)
	{
		EventHub.Instance.Report(new Events.Share());
	}

	public void TwitterSuccess(object caller, EventArgs args)
	{
		EventHub.Instance.Report(new Events.Share());
	}
}
