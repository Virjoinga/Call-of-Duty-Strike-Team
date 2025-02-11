using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPieceLogic : MonoBehaviour
{
	public GameObject SetPiece;

	public bool WasSuccessful;

	public bool m_IsCutscene;

	public bool m_DealWithEnemies = true;

	public bool IsOnlyViewableInTPP;

	public GMGFlashpointManager m_FlashPointmanager;

	private static ActorIdentIterator myStaticActorIdentIterator = new ActorIdentIterator(0u);

	public List<Spawner> m_ReplaceCharacters = new List<Spawner>();

	public List<GameObject> m_OnlyActiveDuringSetpiece = new List<GameObject>();

	[HideInInspector]
	public SetPieceModule SPModule;

	public GuidRef m_CallOnCompletion;

	private bool SetPieceTriggered;

	private InterfaceableObject mHUDObject;

	private GameObject mOriginalParent;

	private bool m_bHasSetPieceArea;

	private Actor mTriggeringActor;

	private int mActorMaskAtStart;

	private List<GameObject> mReplacementWeapons = new List<GameObject>();

	private bool mortal;

	public bool startAutomatically;

	public GameObject[] ObjectActors;

	public int CurrentStatementIndex;

	public bool Mortal
	{
		get
		{
			return mortal;
		}
		set
		{
			mortal = value;
		}
	}

	private void Awake()
	{
		if (SPModule == null)
		{
			if (SetPiece != null)
			{
				SetPieceVisualiser setPieceVisualiser = SetPieceVisualiser.Instance();
				if (setPieceVisualiser == null)
				{
					SetPieceModule setPieceModule = SetPiece.GetComponent(typeof(SetPieceModule)) as SetPieceModule;
					if (setPieceModule != null && setPieceModule.UsePlaygroundMeshes && !m_IsCutscene)
					{
						SetPiece.gameObject.SetActive(false);
					}
				}
				mortal = false;
				if (m_IsCutscene)
				{
					SPModule = SetPiece.GetComponent<SetPieceModule>();
				}
				else
				{
					SPModule = Object.Instantiate(SetPiece.GetComponent<SetPieceModule>()) as SetPieceModule;
					SPModule.gameObject.SetActive(true);
					SPModule.LinkToSetPieceLogic(base.transform);
				}
				if (mHUDObject == null)
				{
					mHUDObject = GetComponent<InterfaceableObject>();
				}
			}
			else
			{
				mortal = true;
			}
		}
		WasSuccessful = false;
		if (startAutomatically)
		{
			StartCoroutine(PlaySetPieceWhenActorsReady());
		}
		if (SPModule != null)
		{
			SPModule.m_CallOnCompletion = m_CallOnCompletion;
		}
	}

	private void Update()
	{
		CurrentStatementIndex = ((!(SPModule != null) || SPModule.Sequence == null) ? (-1) : SPModule.Sequence.GetCurrentStatement());
	}

	private IEnumerator PlaySetPieceWhenActorsReady()
	{
		bool done = false;
		while (!done)
		{
			done = true;
			for (int i = 0; i < ObjectActors.GetLength(0); i++)
			{
				if (!(ObjectActors[i] != null))
				{
					continue;
				}
				Spawner s = ObjectActors[i].GetComponent<Spawner>();
				if (s != null)
				{
					if (s.spawned != null)
					{
						ObjectActors[i] = s.spawned;
					}
					else
					{
						done = false;
					}
				}
			}
			if (done)
			{
				PlaySetPiece();
				break;
			}
			yield return new WaitForEndOfFrame();
		}
	}

	public void ReceiveSignal(string sig)
	{
		if (SPModule != null)
		{
			SPModule.ReceiveSignal(sig);
		}
	}

	public void SetActor_IndexOnlyCharacters(int index, Actor act)
	{
		SPModule.SetPlayerActor_IndexOnlyCharacters(index, act);
	}

	public void SetObject(int index, GameObject theObject)
	{
		SPModule.SetObjectActor(index, theObject);
	}

	public void SetOnlyActiveDuringSetpiece(List<GameObject> activeObjs)
	{
		m_OnlyActiveDuringSetpiece = activeObjs;
	}

	public int GetNumActorsRequired()
	{
		int num = 0;
		if (ObjectActors != null)
		{
			for (int i = 0; i < ObjectActors.GetLength(0); i++)
			{
				if (ObjectActors[i] != null && (ObjectActors[i].GetComponent<BaseCharacter>() != null || ObjectActors[i].GetComponent<Spawner>() != null))
				{
					num++;
				}
			}
		}
		return SPModule.GetNumActorsRequired() - num;
	}

	public int GetNumActorsInvolved()
	{
		return SPModule.GetNumActorsRequired();
	}

	public Transform GetCharacterStartNode(int ActorIndex)
	{
		return SPModule.GetCharacterStartNode(ActorIndex);
	}

	public bool HasActorFinished(int index)
	{
		return SPModule.HasActorFinished(index);
	}

	public bool GunAway(int index)
	{
		return SPModule.GunAway(index);
	}

	public bool GunOut(int index)
	{
		return SPModule.GunOut(index);
	}

	public bool HasFinished()
	{
		return SPModule.HasFinished();
	}

	public void PlaceSetPiece(Transform target)
	{
		PlaceSetPiece(target.position, target.rotation);
	}

	public void PlaceSetPiece(Vector3 pos, Quaternion rot)
	{
		Transform worldReferenceNode = SPModule.WorldReferenceNode;
		Vector3 vector = worldReferenceNode.localPosition * -1f;
		Quaternion quaternion = WorldHelper.UfM_Maybe_Rotation(rot) * Quaternion.Inverse(WorldHelper.UfM_Maybe_Rotation(worldReferenceNode.localRotation));
		base.transform.position = pos + quaternion * vector;
		base.transform.rotation = quaternion;
	}

	public void SetModule(SetPieceModule setPieceMod)
	{
		if (SPModule != null)
		{
			Object.DestroyImmediate(SPModule.gameObject);
		}
		SPModule = Object.Instantiate(setPieceMod) as SetPieceModule;
		SPModule.LinkToSetPieceLogic(base.transform);
	}

	public void ShowHUD(bool OnOff)
	{
		if (mHUDObject == null && mOriginalParent != null)
		{
			mHUDObject = mOriginalParent.GetComponent<InterfaceableObject>();
		}
		if (mHUDObject != null)
		{
			mHUDObject.ShowBlip(OnOff);
		}
	}

	public void Activate()
	{
		PlaySetPiece();
	}

	public void Deactivate()
	{
	}

	public void Pause()
	{
		SPModule.Pause();
	}

	public void EnableInteraction()
	{
		CMSetPiece component = base.gameObject.GetComponent<CMSetPiece>();
		if (component != null)
		{
			component.StartViewable = true;
			component.Activate();
		}
	}

	public void DisableInteraction()
	{
		CMSetPiece component = base.gameObject.GetComponent<CMSetPiece>();
		if (component != null)
		{
			component.Deactivate();
		}
	}

	public void PlaySetPiece()
	{
		if (SPModule == null)
		{
			Debug.Log("No set piece module to play!");
			return;
		}
		if (m_FlashPointmanager != null)
		{
			m_FlashPointmanager.C4SetPieceStart();
		}
		foreach (GameObject item in m_OnlyActiveDuringSetpiece)
		{
			if (item != null)
			{
				item.SetActive(true);
			}
		}
		SPModule.m_FinishedDelegate = SetPieceFinished;
		if (m_DealWithEnemies)
		{
			m_bHasSetPieceArea = false;
			SetPieceArea[] componentsInChildren = base.gameObject.GetComponentsInChildren<SetPieceArea>();
			if (componentsInChildren != null)
			{
				SetPieceArea[] array = componentsInChildren;
				foreach (SetPieceArea setPieceArea in array)
				{
					setPieceArea.DestroyActorsInSetPiece();
					m_bHasSetPieceArea = true;
				}
			}
			if (m_bHasSetPieceArea)
			{
				StopAllActors(true);
			}
		}
		if (ObjectActors != null)
		{
			for (int j = 0; j < ObjectActors.GetLength(0); j++)
			{
				if (!ObjectActors[j])
				{
					continue;
				}
				Spawner component = ObjectActors[j].GetComponent<Spawner>();
				if (component == null)
				{
					Actor component2 = ObjectActors[j].GetComponent<Actor>();
					if (component2 != null)
					{
						SPModule.SetPlayerActor_IndexAll(j, component2);
						component2.baseCharacter.EnableNavMesh(false);
					}
					else
					{
						SPModule.SetObjectActor(j, ObjectActors[j]);
					}
				}
			}
			GameObject firstScriptTriggerActor = GameplayController.Instance().FirstScriptTriggerActor;
			if (firstScriptTriggerActor != null)
			{
				for (int k = 0; k < m_ReplaceCharacters.Count; k++)
				{
					Spawner spawner = m_ReplaceCharacters[k];
					if (spawner != null && firstScriptTriggerActor == spawner.spawned)
					{
						Spawner value = m_ReplaceCharacters[0];
						m_ReplaceCharacters[0] = spawner;
						m_ReplaceCharacters[k] = value;
						break;
					}
				}
				if (SPModule.MainCharacterFPP)
				{
					m_ReplaceCharacters.Reverse();
				}
			}
			if (SPModule.IsACutscene())
			{
				if (SPModule.ClearActorsTasks)
				{
					ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask & GKM.AliveMask);
					Actor a;
					while (actorIdentIterator.NextActor(out a))
					{
						a.tasks.CancelTasksExcluding(typeof(TaskRoutine));
					}
				}
				GameController instance = GameController.Instance;
				GameplayController instance2 = GameplayController.instance;
				if (instance != null && instance2 != null)
				{
					if (instance.ClaymoreDroppingModeActive || instance.PlacementModeActive || instance2.SettingClaymore)
					{
						instance2.CancelAnyPlacement();
					}
					else if (instance.GrenadeThrowingModeActive)
					{
						instance.EndGrenadeThrowingMode();
					}
				}
			}
			int num = 0;
			foreach (Spawner replaceCharacter in m_ReplaceCharacters)
			{
				if (replaceCharacter != null)
				{
					ActorDescriptor spawn = replaceCharacter.Spawn;
					int soldierIndex = spawn.soldierIndex;
					if (soldierIndex >= 0)
					{
						ReplaceWith(soldierIndex, num, spawn);
						num++;
					}
				}
			}
		}
		if (SPModule != null && SPModule.IsInvulnerableDuring)
		{
			List<SPObjectReference> actorReferences = SPModule.ActorReferences;
			int num2 = ((actorReferences != null) ? actorReferences.Count : 0);
			for (int l = 0; l < num2; l++)
			{
				if (actorReferences[l] != null)
				{
					Actor actorRef = actorReferences[l].ActorRef;
					if (actorRef != null && actorRef.realCharacter != null && actorRef.behaviour.PlayerControlled)
					{
						actorRef.health.CacheAndSetInvulnerable(SPModule.IsInvulnerableDuring);
					}
				}
			}
		}
		if (SPModule != null && SPModule.WeaponLighting != null)
		{
			foreach (CutsceneLighting item2 in SPModule.WeaponLighting)
			{
				if (item2.AllowRuntimeUpdate)
				{
					item2.UpdateRenderers(false);
				}
			}
		}
		SPModule.Play();
	}

	public void SetOriginalParent(GameObject OriginalParent)
	{
		mOriginalParent = OriginalParent;
		if (mOriginalParent != null)
		{
			mHUDObject = mOriginalParent.GetComponent<CMSetPiece>();
		}
	}

	private void StopAllActors(bool enabled)
	{
		ActorIdentIterator actorIdentIterator = myStaticActorIdentIterator.ResetWithMask(GKM.ActorsInPlay);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a != null)
			{
				a.navAgent.updatePosition = !enabled;
			}
		}
	}

	private void SetPieceFinished()
	{
		if (m_DealWithEnemies && m_bHasSetPieceArea)
		{
			StopAllActors(false);
		}
		foreach (GameObject item in m_OnlyActiveDuringSetpiece)
		{
			if (item != null)
			{
				Animation[] componentsInChildren = item.GetComponentsInChildren<Animation>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					Resources.UnloadAsset(componentsInChildren[i].clip);
					componentsInChildren[i].clip = null;
				}
				item.transform.parent = null;
				Object.Destroy(item);
			}
		}
		foreach (GameObject mReplacementWeapon in mReplacementWeapons)
		{
			if (mReplacementWeapon != null)
			{
				Object.Destroy(mReplacementWeapon);
			}
		}
		int num = 0;
		if (SPModule != null)
		{
			if (SPModule.MainCharacterFPP)
			{
				SPModule.m_WarpTo.Reverse();
			}
			foreach (Transform item2 in SPModule.m_WarpTo)
			{
				if (!(item2 != null) || num >= m_ReplaceCharacters.Count)
				{
					continue;
				}
				Spawner spawner = m_ReplaceCharacters[num];
				if (!(spawner != null))
				{
					continue;
				}
				ActorWrapper componentInChildren = IncludeDisabled.GetComponentInChildren<ActorWrapper>(spawner.gameObject);
				if (componentInChildren != null)
				{
					Actor actor = componentInChildren.GetActor();
					if (actor != null)
					{
						DebugWarp.WarpActor(actor, item2.position, item2.rotation.eulerAngles);
						num++;
					}
				}
			}
			if (SPModule != null && SPModule.IsInvulnerableDuring)
			{
				List<SPObjectReference> actorReferences = SPModule.ActorReferences;
				int num2 = ((actorReferences != null) ? actorReferences.Count : 0);
				for (int j = 0; j < num2; j++)
				{
					if (actorReferences[j] != null)
					{
						Actor actorRef = actorReferences[j].ActorRef;
						if (actorRef != null && actorRef.realCharacter != null && actorRef.behaviour.PlayerControlled)
						{
							actorRef.health.RestoreInvulnerable();
						}
					}
				}
			}
		}
		if (!IsOnlyViewableInTPP || !(SPModule != null))
		{
			return;
		}
		List<SPObjectReference> actorReferences2 = SPModule.ActorReferences;
		int num3 = ((actorReferences2 != null) ? actorReferences2.Count : 0);
		for (int k = 0; k < num3; k++)
		{
			if (actorReferences2[k] == null)
			{
				continue;
			}
			Actor actorRef2 = actorReferences2[k].ActorRef;
			if (!(actorRef2 != null) || !(actorRef2.realCharacter != null))
			{
				continue;
			}
			actorRef2.realCharacter.SetSelectable(true, true, true);
			if (GKM.UnitCount(GKM.PlayerControlledMask) == 1)
			{
				GameplayController instance = GameplayController.instance;
				if (instance != null)
				{
					instance.AddToSelected(actorRef2);
				}
			}
		}
	}

	private void ReplaceWith(int soldierIndex, int replaceIndex, ActorDescriptor ad)
	{
		string theme = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		SoldierSettings soldierSettings = null;
		if (soldierIndex >= 4)
		{
			return;
		}
		soldierSettings = GameSettings.Instance.Soldiers[soldierIndex];
		GameObject modelForTheme = ad.Model.GetModelForTheme(theme);
		GameObject gameObject = Object.Instantiate(modelForTheme) as GameObject;
		SkinnedMeshRenderer sourceSkin = gameObject.GetComponentInChildren(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		if (SPModule.m_ReplaceCharacters.Count <= replaceIndex)
		{
			return;
		}
		SkinnedMeshRenderer skinnedMeshRenderer = SPModule.m_ReplaceCharacters[replaceIndex];
		if (!(skinnedMeshRenderer != null))
		{
			return;
		}
		Transform transform = null;
		transform = skinnedMeshRenderer.bones[0];
		while (transform.parent.name.Contains("Bip"))
		{
			transform = transform.parent;
		}
		if (!(transform != null))
		{
			return;
		}
		SceneNanny.CopySkinMesh(sourceSkin, skinnedMeshRenderer, transform.gameObject);
		skinnedMeshRenderer.quality = SkinQuality.Bone2;
		Object.Destroy(gameObject);
		if (SPModule.ReplaceCharacterGuns && soldierSettings != null)
		{
			GameObject gameObject2 = WeaponUtils.CreateThirdPersonModelForCutscene(transform.gameObject, soldierSettings.Weapon.Descriptor);
			if (gameObject2 != null)
			{
				mReplacementWeapons.Add(gameObject2);
			}
		}
	}
}
