using System.Collections;
using UnityEngine;

public class RespotOnFail : MonoBehaviour
{
	public RespotData m_Interface;

	private void Start()
	{
		int num = 0;
		foreach (RespotActorDescriptor item in m_Interface.RespotInfo)
		{
			TBFAssert.DoAssert(item.ActorToRespot != null, "You either have an invalid spawner or a null list entry in RespotOnFail on " + base.name);
			m_Interface.RespotInfo[num].actorWrapper = item.ActorToRespot.GetComponentInChildren<ActorWrapper>();
			m_Interface.RespotInfo[num].spawner = item.ActorToRespot.GetComponentInChildren<Spawner>();
			num++;
		}
	}

	private void Update()
	{
	}

	public void Activate()
	{
		CommonHudController.Instance.StealthKillPressed = false;
		CommonHudController.Instance.ActorToStealthKill = null;
		TutorialToggles.IsRespotting = true;
		TutorialToggles.ShouldClearHighlights = true;
		TutorialToggles.RespotCount++;
		SetAnyPlayerControl(false);
		PlaySoundFx.PlaySfxHelper(SoundManager.Instance.gameObject, InterfaceSFX.Instance, "ObjectiveFail", false, 1f);
		HUDMessenger.Instance.PushPriorityMessage(Language.Get(m_Interface.FailMessage), Language.Get(m_Interface.RespotMessage), string.Empty, false);
		StartCoroutine(DelayedTrigger());
	}

	private IEnumerator DelayedTrigger()
	{
		yield return new WaitForSeconds(2f);
		GameController.Instance.TransitionEffectAmount = 0f;
		yield return new WaitForEndOfFrame();
		foreach (RespotActorDescriptor ra in m_Interface.RespotInfo)
		{
			Actor a2 = ra.actorWrapper.GetActor();
			if ((bool)a2 && !WorldHelper.IsPlayerControlledActor(a2))
			{
				ra.spawner.RespottedAfterDeath = a2.realCharacter != null && a2.realCharacter.IsDead();
				CharacterPropertyModifier.SilentRemove(a2, false);
			}
		}
		int i = 0;
		foreach (RespotActorDescriptor ra2 in m_Interface.RespotInfo)
		{
			Vector3 pos;
			Vector3 rot;
			if ((bool)m_Interface.RespotInfo[i].OptionalRespotLocation)
			{
				pos = m_Interface.RespotInfo[i].OptionalRespotLocation.transform.position;
				rot = m_Interface.RespotInfo[i].OptionalRespotLocation.transform.eulerAngles;
			}
			else
			{
				pos = m_Interface.RespotInfo[i].ActorToRespot.transform.position;
				rot = m_Interface.RespotInfo[i].ActorToRespot.transform.eulerAngles;
			}
			Condition_NotSpawned cn = ra2.spawner.gameObject.GetComponent<Condition_NotSpawned>();
			if ((bool)cn)
			{
				ra2.spawner.gameObject.transform.position = pos;
				ra2.spawner.gameObject.transform.eulerAngles = rot;
				if ((bool)ra2.OptionalRespawnRoutine)
				{
					RoutineDescriptor rd = ra2.spawner.GetComponentInChildren(typeof(RoutineDescriptor)) as RoutineDescriptor;
					if (rd != null)
					{
						rd.m_Interface.TaskListObject = ra2.OptionalRespawnRoutine;
						rd.m_Interface.CopyContainerData(rd);
					}
				}
				cn.Reset();
			}
			else
			{
				Actor a = ra2.actorWrapper.GetActor();
				DebugWarp.WarpActor(a, pos, rot);
				if (a == GameController.Instance.mFirstPersonActor)
				{
					a.realCharacter.IsAimingDownSights = false;
				}
			}
			i++;
		}
		DoObjectCallOuts(false);
		StartCoroutine(DeferedPortionOfRespot());
	}

	private IEnumerator DeferedPortionOfRespot()
	{
		yield return new WaitForSeconds(2f);
		foreach (MissionObjective mo in GlobalObjectiveManager.Instance.CurrentObjectiveManager.Objectives)
		{
			if (mo.State == MissionObjective.ObjectiveState.InProgress)
			{
				mo.OutputObjectiveDescription();
			}
		}
		SetAnyPlayerControl(true);
		DoObjectCallOuts(true);
		TutorialToggles.IsRespotting = false;
	}

	public void DoObjectCallOuts(bool deferred)
	{
		int num = 0;
		foreach (GameObject item in m_Interface.ObjectsToCall)
		{
			if (!deferred == m_Interface.DontDeferCall[num])
			{
				string message = string.Empty;
				if (m_Interface.FunctionsToCall != null && num < m_Interface.FunctionsToCall.Count)
				{
					message = m_Interface.FunctionsToCall[num];
				}
				Container.SendMessage(item, message);
			}
			num++;
		}
	}

	public void SetAnyPlayerControl(bool on)
	{
		foreach (RespotActorDescriptor item in m_Interface.RespotInfo)
		{
			Condition_NotSpawned component = item.spawner.gameObject.GetComponent<Condition_NotSpawned>();
			if (!component)
			{
				Actor actor = item.actorWrapper.GetActor();
				CharacterPropertyModifier.SetControl(actor, on, base.gameObject, true);
			}
		}
	}
}
