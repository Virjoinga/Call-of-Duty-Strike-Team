using System;
using System.Collections.Generic;
using UnityEngine;

public class SwrveTalkHandler : MonoBehaviour
{
	private static SwrveTalkHandler m_Instance;

	public static SwrveTalkHandler Instance
	{
		get
		{
			return m_Instance;
		}
	}

	private void Awake()
	{
		TBFAssert.DoAssert(m_Instance == null);
		m_Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
	}

	private void OnEnable()
	{
		Bedrock.SwrveTalkMessageClosed += HandleBedrockSwrveTalkMessageClosed;
	}

	private void OnDisable()
	{
		Bedrock.SwrveTalkMessageClosed -= HandleBedrockSwrveTalkMessageClosed;
	}

	private void HandleBedrockSwrveTalkMessageClosed(object sender, EventArgs e)
	{
		Bedrock.brSwrveTalkActionType actionType = Bedrock.brSwrveTalkActionType.BR_SWRVE_ACTION_NONE;
		int customActionBufferSize = 0;
		int game = 0;
		string customActionBuffer;
		if (Bedrock.GetSwrveTalkMessageResult(out actionType, out customActionBuffer, out customActionBufferSize, out game))
		{
			HandleAction(actionType, customActionBuffer);
			Bedrock.ClearSwrveTalkMessageResult();
		}
	}

	public void HandleAction(Bedrock.brSwrveTalkActionType actionType, string customAction)
	{
		Debug.Log("Swrve Talk Action: " + customAction);
		if (actionType == Bedrock.brSwrveTalkActionType.BR_SWRVE_ACTION_CUSTOM)
		{
			HandleCustomAction(customAction);
		}
		else
		{
			Debug.Log("Unhandled action type : " + actionType);
		}
	}

	private bool CheckNumParams(List<string> actionData, int expectedParams)
	{
		if (actionData.Count == expectedParams)
		{
			return true;
		}
		Debug.Log("Malformed custom action");
		return false;
	}

	private void HandleCustomAction(string customAction)
	{
		if (customAction == null)
		{
			return;
		}
		List<string> list = new List<string>(customAction.Split('#'));
		if (list.Count > 0)
		{
			string text = list[0];
			list.RemoveAt(0);
			switch (text)
			{
			case "open_url":
				Action_OpenUrl(list);
				break;
			case "open_store":
				Action_OpenStore();
				break;
			case "give":
				Action_GiveResource(list);
				break;
			case "redirect_buy":
				Action_RedirectBuy(list);
				break;
			case "discount_buy":
				Action_DiscountBuy(list);
				break;
			case "rate_app":
				Action_RateApp();
				break;
			case "redirect_ui":
				Action_RedirectUI(list);
				break;
			case "trigger_event":
				Action_TriggerEvent(list);
				break;
			default:
				Debug.Log("Ignoring unhandled command: " + text);
				break;
			}
		}
	}

	private void Action_OpenUrl(List<string> actionData)
	{
		if (CheckNumParams(actionData, 1))
		{
			TBFUtils.LaunchURL(actionData[0]);
		}
	}

	private void Action_OpenStore()
	{
		FrontEndController.Instance.TransitionTo(ScreenID.MTXSelect);
	}

	private void Action_GiveResource(List<string> actionData)
	{
	}

	private void Action_RedirectBuy(List<string> actionData)
	{
	}

	private void Action_DiscountBuy(List<string> actionData)
	{
	}

	private void Action_RateApp()
	{
		SwrveEventsUI.RatedApp();
		TBFUtils.LaunchURL(SwrveServerVariables.Instance.RateAppUrl);
	}

	private void Action_RedirectUI(List<string> actionData)
	{
		if (!CheckNumParams(actionData, 1))
		{
			return;
		}
		switch (actionData[0])
		{
		case "CHALLENGE":
			SwrveEventsUI.ViewedChallenges();
			FrontEndController.Instance.TransitionTo(ScreenID.ChallengeSelect);
			break;
		case "SURVIVAL_ARCTIC":
			if (GlobeSelectNavigator.Instance != null)
			{
				GlobeSelectNavigator.Instance.GotoMission(MissionListings.eMissionID.MI_MISSION_ARCTIC_GMG);
			}
			break;
		case "SURVIVAL_AFGHANISTAN":
			if (GlobeSelectNavigator.Instance != null)
			{
				GlobeSelectNavigator.Instance.GotoMission(MissionListings.eMissionID.MI_MISSION_AFGHANISTAN_GMG);
			}
			break;
		case "SURVIVAL_KOWLOON":
			if (GlobeSelectNavigator.Instance != null)
			{
				GlobeSelectNavigator.Instance.GotoMission(MissionListings.eMissionID.MI_MISSION_KOWLOON_GMG);
			}
			break;
		case "SURVIVAL_MOROCCO":
			if (GlobeSelectNavigator.Instance != null)
			{
				GlobeSelectNavigator.Instance.GotoMission(MissionListings.eMissionID.MI_MISSION_MOROCCO_GMG);
			}
			break;
		case "ACTIVATE":
			ActivateWatcher.Instance.LaunchActivate(Bedrock.brUserInterfaceScreen.BR_LOG_ON_UI);
			break;
		case "STORE_BUNDLES":
			if (LoadoutMenuNavigator.LoadOutActive)
			{
				FrontEndController.Instance.TransitionTo(ScreenID.BundleSelect);
			}
			break;
		case "STORE_ARMOR":
			if (LoadoutMenuNavigator.LoadOutActive)
			{
				FrontEndController.Instance.TransitionTo(ScreenID.ArmourSelect);
			}
			break;
		case "STORE_PERKS":
			if (LoadoutMenuNavigator.LoadOutActive)
			{
				FrontEndController.Instance.TransitionTo(ScreenID.PerkSelect);
			}
			break;
		case "STORE_EQUIPMENT":
			if (LoadoutMenuNavigator.LoadOutActive)
			{
				FrontEndController.Instance.TransitionTo(ScreenID.EquipmentSelect);
			}
			break;
		}
	}

	private void Action_TriggerEvent(List<string> actionData)
	{
		if (CheckNumParams(actionData, 1))
		{
			Bedrock.AnalyticsLogEvent(actionData[0]);
		}
	}
}
