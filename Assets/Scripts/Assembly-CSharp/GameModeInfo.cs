using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameModeInfo
{
	public List<WaveInfo> WaveInfo = new List<WaveInfo>();

	public int StartWave;

	public bool ForceFirstPerson = true;

	public float WaitAfterMissionComplete = 5f;

	public float TimeLimit;

	public bool LoopWave;

	public int LoopPoint;

	public List<GameObject> MissionComplete = new List<GameObject>();

	public void Clear()
	{
		WaveInfo.Clear();
		StartWave = 0;
		ForceFirstPerson = true;
		WaitAfterMissionComplete = 5f;
		TimeLimit = 0f;
		LoopWave = false;
		LoopPoint = 0;
		MissionComplete.Clear();
	}
}
