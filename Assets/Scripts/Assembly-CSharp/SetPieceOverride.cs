using System;
using UnityEngine;

public class SetPieceOverride : ContainerOverride
{
	public SetPieceOverrideData m_OverrideData = new SetPieceOverrideData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		SetPieceLogic componentInChildren = IncludeDisabled.GetComponentInChildren<SetPieceLogic>(cont.gameObject);
		if (componentInChildren == null)
		{
			Debug.Log("No set piece logic to override");
			return;
		}
		GameObject gameObject = null;
		if (m_OverrideData.m_PlayerSquad == null)
		{
			if (m_OverrideData.m_PlayerSquadGUID != string.Empty)
			{
				gameObject = ContainerLinks.GetObjectFromGuid(m_OverrideData.m_PlayerSquadGUID);
			}
		}
		else
		{
			gameObject = m_OverrideData.m_PlayerSquad.gameObject;
			m_OverrideData.m_PlayerSquadGUID = m_OverrideData.m_PlayerSquad.m_Guid;
		}
		if (gameObject != null)
		{
			m_OverrideData.m_PlayerSquad = gameObject.GetComponent<Container>();
			Spawner[] componentsInChildren = gameObject.GetComponentsInChildren<Spawner>();
			if (componentsInChildren != null)
			{
				if (componentsInChildren.Length > componentInChildren.ObjectActors.Length)
				{
					Array.Resize(ref componentInChildren.ObjectActors, componentsInChildren.Length);
				}
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != null)
					{
						componentInChildren.ObjectActors[i] = componentsInChildren[i].gameObject;
					}
				}
			}
		}
		if (componentInChildren != null)
		{
			if (m_OverrideData.m_Actors != null)
			{
				if (m_OverrideData.m_Actors.Length > componentInChildren.ObjectActors.Length)
				{
					Array.Resize(ref componentInChildren.ObjectActors, m_OverrideData.m_Actors.Length);
				}
				for (int j = 0; j < m_OverrideData.m_Actors.Length; j++)
				{
					if (m_OverrideData.m_Actors[j] != null)
					{
						componentInChildren.ObjectActors[j] = m_OverrideData.m_Actors[j];
					}
				}
			}
			if (m_OverrideData.m_ReplaceCharacters != null)
			{
				for (int k = 0; k < m_OverrideData.m_ReplaceCharacters.Length; k++)
				{
					GameObject gameObject2 = m_OverrideData.m_ReplaceCharacters[k];
					if (!(gameObject2 != null))
					{
						continue;
					}
					Spawner componentInChildren2 = gameObject2.GetComponentInChildren<Spawner>();
					if (componentInChildren2 != null)
					{
						if (k >= componentInChildren.m_ReplaceCharacters.Count)
						{
							componentInChildren.m_ReplaceCharacters.Add(componentInChildren2);
						}
						else
						{
							componentInChildren.m_ReplaceCharacters[k] = componentInChildren2;
						}
					}
				}
				SetPieceModule setPieceModule = null;
				if (componentInChildren.SetPiece != null)
				{
					setPieceModule = componentInChildren.SetPiece.GetComponent<SetPieceModule>();
				}
				if (componentInChildren.m_ReplaceCharacters.Count == 0 && setPieceModule != null)
				{
					UnityEngine.Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(Spawner));
					for (int l = 0; l < array.Length; l++)
					{
						Spawner spawner = (Spawner)array[l];
						if (spawner.m_Interface.EntityType.StartsWith("Player"))
						{
							componentInChildren.m_ReplaceCharacters.Add(spawner);
							Debug.Log("Added spawner " + spawner.name + " to replace actors list for setpiece " + cont.name);
						}
					}
				}
				if (setPieceModule != null)
				{
					if (m_OverrideData.TransitionToFirstPersonSpawner != null)
					{
						GameObject theObject = m_OverrideData.TransitionToFirstPersonSpawner.theObject;
						if (theObject != null)
						{
							setPieceModule.FirstPersonSpawner = IncludeDisabled.GetComponentInChildren<Spawner>(theObject);
						}
					}
					if (m_OverrideData.SpawnController != null)
					{
						GameObject theObject2 = m_OverrideData.SpawnController.theObject;
						if (theObject2 != null)
						{
							setPieceModule.SpawnerController = IncludeDisabled.GetComponentInChildren<SpawnController>(theObject2);
						}
					}
					if (m_OverrideData.m_WarpTo != null && m_OverrideData.m_WarpTo.Length > 0)
					{
						setPieceModule.m_WarpTo.Clear();
						for (int m = 0; m < m_OverrideData.m_WarpTo.Length; m++)
						{
							GameObject gameObject3 = m_OverrideData.m_WarpTo[m];
							if (gameObject3 != null)
							{
								setPieceModule.m_WarpTo.Add(gameObject3.transform);
							}
						}
					}
				}
			}
			if (m_OverrideData.m_ActivateOnCompletion != null)
			{
				componentInChildren.m_CallOnCompletion = m_OverrideData.m_ActivateOnCompletion;
			}
		}
		CMSetPiece componentInChildren3 = base.gameObject.GetComponentInChildren<CMSetPiece>();
		if (componentInChildren3 != null)
		{
			componentInChildren3.enabled = m_OverrideData.ContextInteractionTriggered;
		}
		ScriptedSequence componentInChildren4 = base.gameObject.GetComponentInChildren<ScriptedSequence>();
		if (componentInChildren4 != null)
		{
			componentInChildren4.m_Interface.autoRun = m_OverrideData.AutoRun;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		ScriptedSequence componentInChildren = gameObj.GetComponentInChildren<ScriptedSequence>();
		if (componentInChildren != null && methodName != "Pause")
		{
			componentInChildren.SendMessage(methodName);
			return;
		}
		SetPieceLogic componentInChildren2 = gameObj.GetComponentInChildren<SetPieceLogic>();
		if (componentInChildren2 != null)
		{
			componentInChildren2.SendMessage(methodName);
		}
	}
}
