using System;
using UnityEngine;

[Serializable]
public class SetPieceOverrideData
{
	public bool ContextInteractionTriggered = true;

	public bool AutoRun;

	public GameObject[] m_Actors;

	public Container m_PlayerSquad;

	public string m_PlayerSquadGUID = string.Empty;

	public GuidRef TransitionToFirstPersonSpawner;

	public GuidRef SpawnController;

	public GuidRef[] m_ReplaceCharacters;

	public GameObject[] m_WarpTo;

	public GuidRef m_ActivateOnCompletion;
}
