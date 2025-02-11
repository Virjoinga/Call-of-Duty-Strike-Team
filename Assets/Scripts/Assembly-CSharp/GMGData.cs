using System.Collections.Generic;
using UnityEngine;

public class GMGData : SingletonMonoBehaviour, iSwrveUpdatable
{
	public enum GameType
	{
		Specops = 0,
		TimeAttack = 1,
		Domination = 2,
		Escort = 3,
		Sniper = 4,
		Flashpoint = 5,
		Total = 6
	}

	private static readonly string[] m_GeneratedTag = new string[6] { "Generated_Survival", "Generated_TimeAttack", "Generated_Domination", "Generated_Escort", "Generated_Sniper", "Generated_Flashpoint" };

	private GameType m_CurrentGameType = GameType.Total;

	private int m_BaseReward = 5;

	private int m_IncReward = 2;

	private int m_WaveCap = 18;

	private int m_WaveStep = 3;

	private int m_WaitBetweenWaves = 15;

	private static int m_FPFlavourMaxWeighting = 10;

	private int[] m_FPFlavourWeightings = new int[4] { m_FPFlavourMaxWeighting, m_FPFlavourMaxWeighting, m_FPFlavourMaxWeighting, m_FPFlavourMaxWeighting };

	private int m_FPBaseRewardScore = 100;

	private int m_FPTimeLimitChance = 2;

	private int[] m_FPTimeLimitRange = new int[2] { 180, 180 };

	private int[] m_FPTimeLimitRange_Survive = new int[2] { 180, 180 };

	private int[] m_FPTimeLimitRange_Collect = new int[2] { 180, 180 };

	private int[] m_FPTimeLimitRange_Destroy = new int[2] { 35, 35 };

	private int[] m_FPTimeLimitRange_Clear = new int[2] { 180, 180 };

	private int m_FPTimeReward = 100;

	private int[] m_FPRequiredKills = new int[2] { 20, 20 };

	private int[] m_FPRequiredIntel = new int[2] { 8, 8 };

	private int[] m_FPRequiredTargets = new int[2] { 6, 6 };

	private int m_FPTimeBonusDestroy = 20;

	private int m_FPDifficultyLevel1_Time = 60;

	private int m_FPDifficultyLevel2_Time = 120;

	private int m_FPDifficultyInterval = 60;

	private int[,] m_FPMaxSimGroupEnemies_Survive = new int[3, 7]
	{
		{ 3, 2, 2, 0, 0, 0, 0 },
		{ 0, 3, 2, 3, 1, 0, 0 },
		{ 0, 4, 1, 4, 3, 1, 0 }
	};

	private int[,] m_FPMaxSimGroupEnemies_Destroy = new int[3, 7]
	{
		{ 3, 2, 1, 0, 0, 0, 0 },
		{ 1, 3, 2, 1, 1, 0, 0 },
		{ 1, 3, 2, 1, 1, 0, 0 }
	};

	private int[,] m_FPMaxSimGroupEnemies_Collect = new int[3, 7]
	{
		{ 3, 2, 2, 0, 0, 0, 0 },
		{ 1, 3, 3, 1, 1, 0, 0 },
		{ 1, 3, 3, 1, 1, 0, 0 }
	};

	private int m_TTBonusKnifeKill = 5;

	private int m_TTBonusExplosion = 7;

	private int m_TTBonusHeadshot = 6;

	private int m_TTBonusKill = 4;

	private int m_TTDifficultyRampTimeGap = 30;

	private int[] m_TTMaxWaveGroupsActive = new int[5] { 0, 1, 2, 3, 4 };

	private int[,] m_TTMaxSimGroupEnemies = new int[6, 7]
	{
		{ 5, 0, 0, 0, 0, 0, 0 },
		{ 2, 3, 0, 0, 0, 0, 0 },
		{ 0, 2, 3, 0, 0, 0, 0 },
		{ 0, 1, 1, 3, 0, 0, 0 },
		{ 0, 2, 0, 2, 1, 0, 0 },
		{ 0, 2, 0, 1, 2, 1, 1 }
	};

	private int m_TTNextReward = 1;

	private int m_TTNextRewardMultiplier = 2;

	private int m_TTRewardInterval = 60;

	private int m_TTMaximumReward = 8;

	private int m_DominationTargetScore = 5000;

	private int m_DominationTokenReward = 2;

	private int m_DominationTokenRewardMultiplier = 2;

	private int m_DominationScoreInterval = 5000;

	private int m_DominationScoreIntervalMultiplier = 1;

	private static int[] m_RewardChance = new int[12]
	{
		1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
		1, 1
	};

	private int m_InvincibleTimer = 30;

	private int m_SpeedBoostTimer = 30;

	private int m_XPBonusTimer = 30;

	private float m_SpeedBoostMultiplier = 2.5f;

	private float m_XPBonusMultiplier = 2f;

	private int m_TimeFreezeTimer = 30;

	private int m_MysteryCost = 5;

	public bool XPBonusRewardActive;

	public List<int> FriendIds = new List<int>();

	private int TotalPurchases;

	private int CurWave = 1;

	public int BaseReward
	{
		get
		{
			return m_BaseReward;
		}
	}

	public int IncReward
	{
		get
		{
			return m_IncReward;
		}
	}

	public int WaveCap
	{
		get
		{
			return m_WaveCap;
		}
	}

	public int WaveStep
	{
		get
		{
			return m_WaveStep;
		}
	}

	public int WaitBetweenWaves
	{
		get
		{
			return m_WaitBetweenWaves;
		}
	}

	public int FPFlavourMaxWeighting
	{
		get
		{
			return m_FPFlavourMaxWeighting;
		}
	}

	public int[] FPFlavourWeightings
	{
		get
		{
			return m_FPFlavourWeightings;
		}
	}

	public int FPBaseRewardScore
	{
		get
		{
			return m_FPBaseRewardScore;
		}
	}

	public int FPTimeLimitChance
	{
		get
		{
			return m_FPTimeLimitChance;
		}
	}

	public int[] FPTimeLimitRange
	{
		get
		{
			return m_FPTimeLimitRange;
		}
	}

	public int[] FPTimeLimitRange_Survive
	{
		get
		{
			return m_FPTimeLimitRange_Survive;
		}
	}

	public int[] FPTimeLimitRange_Collect
	{
		get
		{
			return m_FPTimeLimitRange_Collect;
		}
	}

	public int[] FPTimeLimitRange_Destroy
	{
		get
		{
			return m_FPTimeLimitRange_Destroy;
		}
	}

	public int[] FPTimeLimitRange_Clear
	{
		get
		{
			return m_FPTimeLimitRange_Clear;
		}
	}

	public int FPTimeReward
	{
		get
		{
			return m_FPTimeReward;
		}
	}

	public int[] FPRequiredKills
	{
		get
		{
			return m_FPRequiredKills;
		}
	}

	public int[] FPRequiredIntel
	{
		get
		{
			return m_FPRequiredIntel;
		}
	}

	public int[] FPRequiredTargets
	{
		get
		{
			return m_FPRequiredTargets;
		}
	}

	public int FPTimeBonusDestroy
	{
		get
		{
			return m_FPTimeBonusDestroy;
		}
	}

	public int FPDifficultyLevel1_Time
	{
		get
		{
			return m_FPDifficultyLevel1_Time;
		}
	}

	public int FPDifficultyLevel2_Time
	{
		get
		{
			return m_FPDifficultyLevel2_Time;
		}
	}

	public int FPDifficultyInterval
	{
		get
		{
			return m_FPDifficultyInterval;
		}
	}

	public int[,] FPMaxSimGroupEnemies_Survive
	{
		get
		{
			return m_FPMaxSimGroupEnemies_Survive;
		}
	}

	public int[,] FPMaxSimGroupEnemies_Destroy
	{
		get
		{
			return m_FPMaxSimGroupEnemies_Destroy;
		}
	}

	public int[,] FPMaxSimGroupEnemies_Collect
	{
		get
		{
			return m_FPMaxSimGroupEnemies_Collect;
		}
	}

	public int TTBonusKnifeKill
	{
		get
		{
			return m_TTBonusKnifeKill;
		}
	}

	public int TTBonusExplosion
	{
		get
		{
			return m_TTBonusExplosion;
		}
	}

	public int TTBonusHeadshot
	{
		get
		{
			return m_TTBonusHeadshot;
		}
	}

	public int TTBonusKill
	{
		get
		{
			return m_TTBonusKill;
		}
	}

	public int TTDifficultyRampTimeGap
	{
		get
		{
			return m_TTDifficultyRampTimeGap;
		}
	}

	public int TTMaxDifficultyLevel
	{
		get
		{
			return m_TTMaxWaveGroupsActive.Length + 1;
		}
	}

	public int[] TTMaxWaveGroupsActive
	{
		get
		{
			return m_TTMaxWaveGroupsActive;
		}
	}

	public int[,] TTMaxSimGroupEnemies
	{
		get
		{
			return m_TTMaxSimGroupEnemies;
		}
	}

	public int TTNextReward
	{
		get
		{
			return m_TTNextReward;
		}
	}

	public int TTNextRewardMultiplier
	{
		get
		{
			return m_TTNextRewardMultiplier;
		}
	}

	public int TTRewardInterval
	{
		get
		{
			return m_TTRewardInterval;
		}
	}

	public int TTMaximumReward
	{
		get
		{
			return m_TTMaximumReward;
		}
	}

	public int DominationTargetScore
	{
		get
		{
			return m_DominationTargetScore;
		}
	}

	public int DominationTokenReward
	{
		get
		{
			return m_DominationTokenReward;
		}
	}

	public int DominationTokenRewardMultiplier
	{
		get
		{
			return m_DominationTokenRewardMultiplier;
		}
	}

	public int DominationScoreInterval
	{
		get
		{
			return m_DominationScoreInterval;
		}
	}

	public int DominationScoreIntervalMultiplier
	{
		get
		{
			return m_DominationScoreIntervalMultiplier;
		}
	}

	public int[] RewardChance
	{
		get
		{
			return m_RewardChance;
		}
	}

	public int InvincibleTimer
	{
		get
		{
			return m_InvincibleTimer;
		}
	}

	public int SpeedBoostTimer
	{
		get
		{
			return m_SpeedBoostTimer;
		}
	}

	public float SpeedBoostMultiplier
	{
		get
		{
			return m_SpeedBoostMultiplier;
		}
	}

	public int XPBonusTimer
	{
		get
		{
			return m_XPBonusTimer;
		}
	}

	public float XPBonusMultiplier
	{
		get
		{
			return m_XPBonusMultiplier;
		}
		set
		{
			m_XPBonusMultiplier = value;
		}
	}

	public int TimeFreezeTimer
	{
		get
		{
			return m_TimeFreezeTimer;
		}
	}

	public int MysteryCost
	{
		get
		{
			return m_MysteryCost;
		}
	}

	public GameType CurrentGameType
	{
		get
		{
			return m_CurrentGameType;
		}
		set
		{
			m_CurrentGameType = value;
		}
	}

	public static GMGData Instance
	{
		get
		{
			return SingletonMonoBehaviour.GetAutoGeneratedSingletonInstance<GMGData>();
		}
	}

	public static string GetGeneratedTag(GameType gt)
	{
		return m_GeneratedTag[(int)gt];
	}

	protected override void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		base.Awake();
	}

	public void Reset()
	{
		TotalPurchases = 0;
		CurWave = 1;
		FriendIds.Clear();
	}

	public int GetRewardForWave(int wave)
	{
		if (wave % WaveStep == 0)
		{
			int baseReward = BaseReward;
			wave = Mathf.Min(wave, WaveCap);
			int num = wave / WaveStep - 1;
			num = (int)Mathf.Pow(IncReward, num);
			return BaseReward * num;
		}
		return 0;
	}

	public int GetAmmoCost()
	{
		return AmmoDropManager.Instance.BaseAmmoCost + TotalPurchases * AmmoDropManager.Instance.IncAmmoCost;
	}

	public void RegisterAmmoPurchase()
	{
		TotalPurchases++;
	}

	public void RegisterMysteryPurchase()
	{
		TotalPurchases++;
	}

	public int CurrentWave()
	{
		return CurWave;
	}

	public void IncrementCurrentWave()
	{
		CurWave++;
	}

	public void SetStartWave(int val)
	{
		CurWave = val;
	}

	public void UpdateFromSwrve()
	{
		Dictionary<string, string> resourceDictionary;
		if (Bedrock.GetRemoteUserResources("GMGRewards", out resourceDictionary))
		{
			m_BaseReward = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "RewardBase", m_BaseReward);
			m_IncReward = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "RewardInc", m_IncReward);
			m_WaveCap = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "WaveCap", m_WaveCap);
			m_WaveStep = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "WaveStep", m_WaveStep);
			m_WaitBetweenWaves = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "WaitBetweenWaves", m_WaitBetweenWaves);
		}
		if (Bedrock.GetRemoteUserResources("FPMissions", out resourceDictionary))
		{
			m_FPFlavourMaxWeighting = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPFlavourMaxWeighting", m_FPFlavourMaxWeighting);
			m_FPFlavourWeightings[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPFlavourWeightings1", m_FPFlavourWeightings[0]);
			m_FPFlavourWeightings[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPFlavourWeightings2", m_FPFlavourWeightings[1]);
			m_FPFlavourWeightings[2] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPFlavourWeightings3", m_FPFlavourWeightings[2]);
			m_FPFlavourWeightings[3] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPFlavourWeightings4", m_FPFlavourWeightings[3]);
			m_FPBaseRewardScore = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPBaseRewardScore", m_FPBaseRewardScore);
			m_FPTimeLimitChance = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitChance", m_FPTimeLimitChance);
			m_FPTimeLimitRange[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRangeMin", m_FPTimeLimitRange[0]);
			m_FPTimeLimitRange[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRangeMax", m_FPTimeLimitRange[1]);
			m_FPRequiredKills[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPRequiredKillsMin", m_FPRequiredKills[0]);
			m_FPRequiredKills[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPRequiredKillsMax", m_FPRequiredKills[1]);
			m_FPRequiredIntel[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPRequiredIntelMin", m_FPRequiredIntel[0]);
			m_FPRequiredIntel[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPRequiredIntelMax", m_FPRequiredIntel[1]);
			m_FPRequiredTargets[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPRequiredTargetsMin", m_FPRequiredTargets[0]);
			m_FPRequiredTargets[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPRequiredTargetsMax", m_FPRequiredTargets[1]);
			m_FPTimeLimitRange_Survive[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Survive_Min", m_FPTimeLimitRange_Survive[0]);
			m_FPTimeLimitRange_Survive[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Survive_Max", m_FPTimeLimitRange_Survive[1]);
			m_FPTimeLimitRange_Collect[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Collect_Min", m_FPTimeLimitRange_Collect[0]);
			m_FPTimeLimitRange_Collect[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Collect_Max", m_FPTimeLimitRange_Collect[1]);
			m_FPTimeLimitRange_Destroy[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Destroy_Min", m_FPTimeLimitRange_Destroy[0]);
			m_FPTimeLimitRange_Destroy[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Destroy_Max", m_FPTimeLimitRange_Destroy[1]);
			m_FPTimeLimitRange_Clear[0] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Clear_Min", m_FPTimeLimitRange_Clear[0]);
			m_FPTimeLimitRange_Clear[1] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeLimitRange_Clear_Max", m_FPTimeLimitRange_Clear[1]);
			m_FPTimeBonusDestroy = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "FPTimeBonusDestroy", m_FPTimeBonusDestroy);
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 7; j++)
				{
					string key = "FPMaxSimGroupEnemies_Collect" + (i + 1) + "_" + (j + 1);
					m_FPMaxSimGroupEnemies_Collect[i, j] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, key, m_FPMaxSimGroupEnemies_Collect[i, j]);
				}
			}
			for (int k = 0; k < 3; k++)
			{
				for (int l = 0; l < 7; l++)
				{
					string key = "FPMaxSimGroupEnemies_Collect" + (k + 1) + "_" + (l + 1);
					m_FPMaxSimGroupEnemies_Destroy[k, l] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, key, m_FPMaxSimGroupEnemies_Destroy[k, l]);
				}
			}
			for (int m = 0; m < 3; m++)
			{
				for (int n = 0; n < 7; n++)
				{
					string key = "FPMaxSimGroupEnemies_Survive" + (m + 1) + "_" + (n + 1);
					m_FPMaxSimGroupEnemies_Survive[m, n] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, key, m_FPMaxSimGroupEnemies_Survive[m, n]);
				}
			}
		}
		if (Bedrock.GetRemoteUserResources("TTMission", out resourceDictionary))
		{
			m_TTBonusKnifeKill = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTBonusKnifeKill", m_TTBonusKnifeKill);
			m_TTBonusExplosion = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTBonusExplosion", m_TTBonusExplosion);
			m_TTBonusHeadshot = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTBonusHeadshot", m_TTBonusHeadshot);
			m_TTBonusKill = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTBonusKill", m_TTBonusKill);
			m_TTDifficultyRampTimeGap = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTDifficultyRampTimeGap", m_TTDifficultyRampTimeGap);
			m_TTNextReward = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTNextReward", m_TTNextReward);
			m_TTNextRewardMultiplier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTNextRewardMultiplier", m_TTNextRewardMultiplier);
			m_TTRewardInterval = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTMaximumReward", m_TTRewardInterval);
			m_TTMaximumReward = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TTMaximumReward", m_TTMaximumReward);
			for (int num = 0; num < 6; num++)
			{
				if (num < 5)
				{
					string key2 = "TTMaxWaveGroupsActive" + (num + 1);
					m_TTMaxWaveGroupsActive[num] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, key2, m_TTMaxWaveGroupsActive[num]);
				}
				for (int num2 = 0; num2 < 7; num2++)
				{
					string key2 = "TTMaxSimGroupEnemies" + (num + 1) + "_" + (num2 + 1);
					m_TTMaxSimGroupEnemies[num, num2] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, key2, m_TTMaxSimGroupEnemies[num, num2]);
				}
			}
		}
		if (Bedrock.GetRemoteUserResources("DomMission", out resourceDictionary))
		{
			m_DominationTargetScore = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "DominationTargetScore", m_DominationTargetScore);
			m_DominationTokenReward = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "DominationTokenReward", m_DominationTokenReward);
			m_DominationTokenRewardMultiplier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "DominationTokenRewardMultiplier", m_DominationTokenRewardMultiplier);
			m_DominationScoreInterval = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "DominationScoreInterval", m_DominationScoreInterval);
			m_DominationScoreIntervalMultiplier = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "DominationScoreIntervalMultiplier", m_DominationScoreIntervalMultiplier);
		}
		if (Bedrock.GetRemoteUserResources("MysterCache", out resourceDictionary))
		{
			m_InvincibleTimer = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "InvincibleTimer", m_InvincibleTimer);
			m_SpeedBoostTimer = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "SpeedBoostTimer", m_SpeedBoostTimer);
			m_XPBonusTimer = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "XPBonusTimer", m_XPBonusTimer);
			m_XPBonusMultiplier = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "XPBonusMultiplier", m_XPBonusMultiplier);
			m_SpeedBoostMultiplier = Bedrock.GetFromResourceDictionaryAsFloat(resourceDictionary, "SpeedBoostMultiplier", m_SpeedBoostMultiplier);
			m_TimeFreezeTimer = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "TimeFreezeTimer", m_TimeFreezeTimer);
			m_MysteryCost = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, "MysteryCost", m_MysteryCost);
			for (int num3 = 0; num3 < m_RewardChance.Length; num3++)
			{
				string key3 = "MysteryCost" + (num3 + 1);
				m_RewardChance[num3] = Bedrock.GetFromResourceDictionaryAsInt(resourceDictionary, key3, m_RewardChance[num3]);
			}
		}
	}
}
