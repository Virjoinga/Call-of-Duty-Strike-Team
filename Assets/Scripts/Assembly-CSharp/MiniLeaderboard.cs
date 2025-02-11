using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class MiniLeaderboard : MonoBehaviour
{
	public MissionOverviewHighscoreEntry[] HighScorePanels;

	public SpriteText StatusMessage;

	public SpriteText SubTitle;

	public FrontEndButton InviteButton;

	private int m_CurrentLeaderboardID = -1;

	private bool m_IsSpecOps;

	private NumberFormatInfo mNfi;

	private void Awake()
	{
		mNfi = GlobalizationUtils.GetNumberFormat(0);
	}

	private void OnEnable()
	{
		ClearScores();
		Bedrock.FriendCacheUpdated += OnUpdatedFriendsList;
	}

	private void OnDisable()
	{
		Bedrock.FriendCacheUpdated -= OnUpdatedFriendsList;
		BedrockWorker.Instance.InvalidateGotleaderboardCallback();
	}

	private void OnUpdatedFriendsList(object sender, EventArgs e)
	{
		RefreshHighScores(m_CurrentLeaderboardID, m_IsSpecOps);
	}

	public void HideAllElements()
	{
		ClearScores();
		if (InviteButton != null)
		{
			InviteButton.gameObject.SetActive(false);
		}
		if (SubTitle != null)
		{
			SubTitle.gameObject.SetActive(false);
		}
		if (StatusMessage != null)
		{
			StatusMessage.Text = string.Empty;
		}
	}

	public void RefreshFriendsAndHighScores(int leaderboardId, bool isSpecOps)
	{
		m_CurrentLeaderboardID = leaderboardId;
		m_IsSpecOps = isSpecOps;
		ClearScores();
		if (SubTitle != null)
		{
			SubTitle.gameObject.SetActive(true);
		}
		if (Bedrock.isUserConnected() && !Bedrock.isDeviceAnonymouslyLoggedOn())
		{
			Bedrock.UpdateFriendsList();
			if (StatusMessage != null)
			{
				StatusMessage.Text = string.Empty;
			}
			if (InviteButton != null)
			{
				InviteButton.gameObject.SetActive(true);
			}
		}
		else
		{
			if (StatusMessage != null)
			{
				StatusMessage.Text = AutoLocalize.Get("S_ACTIVATE_LOGIN");
			}
			if (InviteButton != null)
			{
				InviteButton.gameObject.SetActive(false);
			}
		}
	}

	public void RefreshHighScores(int leaderboardId, bool isSpecOps)
	{
		m_CurrentLeaderboardID = leaderboardId;
		m_IsSpecOps = isSpecOps;
		ClearScores();
		if (SubTitle != null)
		{
			SubTitle.gameObject.SetActive(true);
		}
		if (Bedrock.isUserConnected() && !Bedrock.isDeviceAnonymouslyLoggedOn())
		{
			BedrockWorker.Instance.GetFriendLeaderboardValues((uint)leaderboardId, HighScoresRetrieved);
			if (StatusMessage != null)
			{
				StatusMessage.Text = string.Empty;
			}
			if (InviteButton != null)
			{
				InviteButton.gameObject.SetActive(true);
			}
		}
		else
		{
			if (StatusMessage != null)
			{
				StatusMessage.Text = AutoLocalize.Get("S_ACTIVATE_LOGIN");
			}
			if (InviteButton != null)
			{
				InviteButton.gameObject.SetActive(false);
			}
		}
	}

	private void ClearScores()
	{
		int num = HighScorePanels.Length;
		for (int i = 0; i < num; i++)
		{
			HighScorePanels[i].Name.Text = string.Empty;
			HighScorePanels[i].Value.Text = string.Empty;
			HighScorePanels[i].Name.SetColor(ColourChart.HudWhite);
			HighScorePanels[i].Value.SetColor(ColourChart.HudWhite);
			HighScorePanels[i].Rank.Hide(true);
			HighScorePanels[i].Veteran.Hide(true);
			HighScorePanels[i].Elite.Hide(true);
		}
	}

	public void HighScoresRetrieved(uint leaderboardIndex, List<LeaderboardResult> leaderBoard)
	{
		if (!base.enabled)
		{
			return;
		}
		if (leaderboardIndex != m_CurrentLeaderboardID)
		{
			Debug.Log("Got scores for leaderboard " + leaderboardIndex + " Expecting " + m_CurrentLeaderboardID);
			RefreshHighScores(m_CurrentLeaderboardID, m_IsSpecOps);
		}
		else
		{
			if (HighScorePanels == null)
			{
				return;
			}
			int num = HighScorePanels.Length;
			int num2 = BedrockWorker.FindPlayerInLeaderboard(leaderBoard);
			if (num2 == -1)
			{
				Debug.Log("No leaderboard entry for current player, retrieving friend score by rank instead");
				BedrockWorker.Instance.GetLeaderboardValuesByRank(1uL, (uint)m_CurrentLeaderboardID, true, HighScoresRetrievedByRank);
				return;
			}
			ClearScores();
			Debug.Log("Leaderboard len " + leaderBoard.Count);
			int num3 = num / 2;
			int num4 = num2 - num3;
			if (num4 < 0)
			{
				num4 = 0;
			}
			while (num4 + num > leaderBoard.Count - 1 && num4 > 0)
			{
				num4--;
			}
			for (int i = 0; i < num; i++)
			{
				if (num4 >= 0 && num4 < leaderBoard.Count)
				{
					SetScore(i, leaderBoard[num4], !m_IsSpecOps);
				}
				num4++;
			}
		}
	}

	public void HighScoresRetrievedByRank(uint leaderboardIndex, List<LeaderboardResult> leaderBoard)
	{
		if (!base.enabled)
		{
			return;
		}
		if (leaderboardIndex != m_CurrentLeaderboardID)
		{
			RefreshHighScores(m_CurrentLeaderboardID, m_IsSpecOps);
		}
		else
		{
			if (HighScorePanels == null)
			{
				return;
			}
			ClearScores();
			int num = HighScorePanels.Length;
			for (int i = 0; i < num; i++)
			{
				if (i < leaderBoard.Count && leaderBoard[i].Name.Length > 0)
				{
					SetScore(i, leaderBoard[i], !m_IsSpecOps);
				}
			}
		}
	}

	private void SetScore(int position, LeaderboardResult data, bool showVeteran)
	{
		if (data.Name.Length > 0)
		{
			HighScorePanels[position].Value.Text = data.Rating.ToString("N", mNfi);
			HighScorePanels[position].Name.Text = data.Name;
			Color color = ColourChart.HudWhite;
			if (data.IsPlayer)
			{
				color = ColourChart.HudYellow;
				data.Rank = XPManager.Instance.GetXPLevelAbsolute();
			}
			HighScorePanels[position].Value.SetColor(color);
			HighScorePanels[position].Name.SetColor(color);
			HighScorePanels[position].Rank.Hide(false);
			HighScorePanels[position].Veteran.Hide(!showVeteran);
			HighScorePanels[position].Veteran.SetColor((!data.Veteran) ? ColourChart.GreyedOut : Color.white);
			HighScorePanels[position].Elite.Hide(false);
			HighScorePanels[position].Elite.SetColor((!data.Elite) ? ColourChart.GreyedOut : Color.white);
			HighScorePanels[position].Rank.SetRank(data.Rank);
		}
	}
}
