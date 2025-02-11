using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPieceModule : MonoBehaviour
{
	public delegate void FinishedDelegate();

	public const float kBigFOVValue = 150f;

	public const float kDefaultFOVValue = 55f;

	public const float kTransitionSpeed = 1.25f;

	public const float kNearClipOverrideValue = 0.04f;

	public const int kMaxActors = 8;

	public const int kShortTextWidth = 70;

	public const int kMediumTextWidth = 96;

	public const int kListWidth = 110;

	public const int kAnimationWidth = 160;

	public const float kCrossFadeTime = 0.25f;

	public static GUIStyle DefaultTextStyle;

	public static GUIStyle DefaultButtStyle;

	public static GUIStyle DefaultBoxStyle;

	public List<GameObject> m_PlaygroundActors = new List<GameObject>();

	public List<SkinnedMeshRenderer> m_ReplaceCharacters = new List<SkinnedMeshRenderer>();

	public List<Transform> m_WarpTo = new List<Transform>();

	private Transform WorldOffset;

	public List<SPLinkInfo> LinkInfo;

	public SPSequence Sequence;

	public GameObject ReferenceMesh;

	public Transform WorldReferenceNode;

	public float PausedTime;

	public CameraBase TheCamera;

	public bool Activated;

	public bool UsePlaygroundMeshes;

	public bool IsCutscene;

	public bool ClearActorsTasks = true;

	public bool AddStartTransition = true;

	public bool AddEndTransition = true;

	public bool MaintainCameraMode = true;

	public bool MainCharacterFPP;

	public bool ReplaceCharacterGuns = true;

	public bool BlendToFPP;

	public bool IsInvulnerableDuring;

	public Spawner FirstPersonSpawner;

	public SpawnController SpawnerController;

	public ActionComponent.ActionType ActionType = ActionComponent.ActionType.Ignore;

	public List<SPObjectReference> ActorReferences;

	public List<SPObjectReference> NodeReferences;

	public List<SPObjectReference> WorldObjects;

	public List<CutsceneLighting> WeaponLighting = new List<CutsceneLighting>();

	public static List<string> ObjectTypeNames;

	public static List<string> ActorNames;

	public static List<string> NodeNames;

	public static List<string> ConditionalNames;

	public static List<string> BlendNames;

	public static List<string> FadeGroupNames;

	[NonSerialized]
	public bool OpenLocalVersion;

	public GameObject m_CallOnCompletion;

	public List<float> CameraCutTimes = new List<float>();

	public FinishedDelegate m_FinishedDelegate;

	private bool mIsInPlayground;

	private List<float> mAnimSpeeds = new List<float>();

	private int mCurrentCutIndex;

	private bool mJustCut;

	private float mPreviousAnimSpeed = 1f;

	private bool mFirstPersonWhenStarted;

	private Transform mFirstPersonTransform;

	public Transform FirstPersonTransform
	{
		set
		{
			mFirstPersonTransform = value;
		}
	}

	public SetPieceModule()
	{
		LinkInfo = new List<SPLinkInfo>();
		ActorReferences = new List<SPObjectReference>();
	}

	public bool IsACutscene()
	{
		if (UsePlaygroundMeshes || IsCutscene)
		{
			return true;
		}
		return false;
	}

	public void ReceiveSignal(string sig)
	{
		Sequence.ReceiveSignal(sig);
	}

	public int GetNumActorsRequired()
	{
		int num = 0;
		foreach (SPLinkInfo item in LinkInfo)
		{
			if (item.Type == SPLinkInfo.SPLinkType.ActorToTriggerPoint)
			{
				num++;
			}
		}
		return num;
	}

	public void SetPlayerActor_IndexOnlyCharacters(int Index, Actor act)
	{
		int num = 0;
		int actorIndex = 0;
		foreach (SPLinkInfo item in LinkInfo)
		{
			if (item.Type == SPLinkInfo.SPLinkType.ActorToTriggerPoint)
			{
				actorIndex = num;
				Index--;
				if (Index < 0)
				{
					break;
				}
			}
			num++;
		}
		SetPlayerActor_IndexAll(actorIndex, act);
	}

	public void SetPlayerActor_IndexAll(int actorIndex, Actor actorRef)
	{
		SPObjectReference sPObjectReference = ActorReferences[actorIndex];
		sPObjectReference.ObjectTransform = actorRef.poseModuleSharedData.offAxisTrans;
		sPObjectReference.SetAnimDirector(actorRef.animDirector);
		sPObjectReference.ActorRef = actorRef;
	}

	public void SetObjectActor(int Index, GameObject theObject)
	{
		SPObjectReference sPObjectReference = ActorReferences[Index];
		sPObjectReference.ObjectTransform = theObject.transform;
		sPObjectReference.SetActorAnimation(theObject.animation);
		sPObjectReference.ActorRef = null;
	}

	public void LinkToSetPieceLogic(Transform trans)
	{
		base.transform.parent = trans;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
	}

	public Transform GetCharacterStartNode(int ActorIndex)
	{
		int num = 0;
		foreach (SPLinkInfo item in LinkInfo)
		{
			if (item.Type == SPLinkInfo.SPLinkType.ActorToTriggerPoint)
			{
				if (num == ActorIndex)
				{
					return NodeReferences[item.NodeIndex].ObjectTransform;
				}
				num++;
			}
		}
		return null;
	}

	public bool HasActorFinished(int Index)
	{
		return ActorReferences[Index].HasFinished();
	}

	public bool GunAway(int Index)
	{
		foreach (SPLinkInfo item in LinkInfo)
		{
			if (item.Type == SPLinkInfo.SPLinkType.ActorToTriggerPoint && item.ActorIndex == Index)
			{
				return item.RefObject.GunAway;
			}
		}
		return false;
	}

	public bool GunOut(int index)
	{
		foreach (SPLinkInfo item in LinkInfo)
		{
			if (item.Type == SPLinkInfo.SPLinkType.ActorToTriggerPoint && item.ActorIndex == index)
			{
				return item.RefObject.GunAway && item.RefObject.GunOut;
			}
		}
		return false;
	}

	public bool HasFinished()
	{
		return Sequence.HasFinished();
	}

	public void FixUpPlaygroundActors()
	{
	}

	public void Play()
	{
		if (Activated)
		{
			Stop();
		}
		InteractionsManager.Instance.RegisterAction(base.gameObject, ActionType);
		if (IsACutscene())
		{
			CommonHudController instance = CommonHudController.Instance;
			if (instance != null && instance.MissionDialogueQueue != null)
			{
				instance.MissionDialogueQueue.ClearDialogueQueue(false);
			}
		}
		foreach (SPObjectReference actorReference in ActorReferences)
		{
			if (actorReference == null)
			{
				continue;
			}
			if (mIsInPlayground)
			{
				actorReference.PlaygroundRestore();
			}
			actorReference.linkTransform = GetCharacterStartNode(actorReference.Index);
			if (!(actorReference.ActorRef != null))
			{
				continue;
			}
			actorReference.ActorRef.baseCharacter.IsInASetPiece = true;
			if (GunAway(actorReference.Index))
			{
				IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(actorReference.ActorRef.weapon.ActiveWeapon);
				if (weaponEquip != null && !weaponEquip.IsPuttingAway() && !weaponEquip.IsTakingOut())
				{
					weaponEquip.PutAway(1f);
				}
			}
			IWeapon activeWeapon = actorReference.ActorRef.weapon.ActiveWeapon;
			if (activeWeapon != null)
			{
				activeWeapon.CancelReload();
			}
		}
		if ((bool)TheCamera)
		{
			if ((bool)CameraManager.Instance)
			{
				CameraController playCameraController = CameraManager.Instance.PlayCameraController;
				if (IsACutscene())
				{
					bool flag = AddStartTransition;
					mCurrentCutIndex = 0;
					GameController instance2 = GameController.Instance;
					if (instance2 != null)
					{
						mFirstPersonWhenStarted = instance2.IsFirstPerson;
						if (mFirstPersonWhenStarted)
						{
							instance2.ExitFirstPerson(AddStartTransition);
							flag = false;
						}
					}
					if (playCameraController != null)
					{
						Camera component = TheCamera.gameObject.GetComponent<Camera>();
						if (component != null)
						{
							if (component.fieldOfView >= 150f)
							{
								component.fieldOfView = 55f;
							}
							TheCamera.Fov = component.fieldOfView;
							TheCamera.NearClip = 0.04f;
						}
						playCameraController.ForcedCutTo(TheCamera);
						if (flag)
						{
							InteractionsManager.Instance.TransitionFromSolid(1.25f);
						}
						InputManager instance3 = InputManager.Instance;
						if ((bool)instance3)
						{
							instance3.SetForCutscene();
						}
					}
				}
			}
			else
			{
				Camera component2 = TheCamera.gameObject.GetComponent<Camera>();
				if (component2 != null)
				{
					Camera.SetupCurrent(component2);
					Camera.main.fieldOfView = component2.fieldOfView;
					Camera.main.nearClipPlane = component2.nearClipPlane;
				}
			}
		}
		PausedTime = 0f;
		Sequence.Activate(this);
		Activated = true;
	}

	public void Pause()
	{
		int num = 0;
		foreach (SPObjectReference actorReference in ActorReferences)
		{
			if (actorReference.CurrentAnim != null && actorReference.ActorAnimation != null)
			{
				if (Activated)
				{
					if (actorReference.ActorAnimation.IsPlaying(actorReference.CurrentAnim.name))
					{
						mAnimSpeeds.Add(actorReference.ActorAnimation.animation[actorReference.CurrentAnim.name].speed);
						actorReference.ActorAnimation.animation[actorReference.CurrentAnim.name].speed = 0f;
					}
					else
					{
						mAnimSpeeds.Add(0f);
					}
				}
				else if (num < mAnimSpeeds.Count)
				{
					actorReference.ActorAnimation.animation[actorReference.CurrentAnim.name].speed = mAnimSpeeds[num];
				}
				else
				{
					Debug.LogWarning("Array out of range - are you trying to unpause before pausing?");
				}
			}
			num++;
		}
		Activated = !Activated;
		if (Activated)
		{
			mAnimSpeeds.Clear();
		}
	}

	public void Stop()
	{
		foreach (SPObjectReference actorReference in ActorReferences)
		{
			if (actorReference.CurrentAnim != null && actorReference.ActorAnimation != null)
			{
				actorReference.ActorAnimation.Stop(actorReference.CurrentAnim.name);
			}
		}
		Sequence.Deactivate();
		Finished();
	}

	public void Skip()
	{
		Sequence.Skip(this);
		foreach (SPObjectReference actorReference in ActorReferences)
		{
			actorReference.SkipCurrentAnim();
		}
		Sequence.Deactivate();
		Finished();
	}

	public void PostSkip()
	{
		Sequence.PostSkip();
	}

	public bool IsPlaying()
	{
		return Activated;
	}

	public float GetCurrentSequenceTime()
	{
		return Sequence.GetCurrentTimer();
	}

	public string GetLastAction()
	{
		return Sequence.GetLastAction();
	}

	public int GetCurrentStatement()
	{
		return Sequence.GetCurrentStatement();
	}

	public void InsertAction(SPAction.SPActionType type)
	{
		Sequence.InsertAction(type);
	}

	private void Awake()
	{
		mIsInPlayground = false;
		SetPieceVisualiser setPieceVisualiser = SetPieceVisualiser.Instance();
		if (setPieceVisualiser != null)
		{
			mIsInPlayground = true;
		}
		BuildNameArrays();
		ActorReferences.Clear();
		if (mIsInPlayground || UsePlaygroundMeshes)
		{
			foreach (GameObject playgroundActor in m_PlaygroundActors)
			{
				AddPlaygroundActor(playgroundActor, -1);
			}
		}
		else
		{
			for (int i = 0; i < 8; i++)
			{
				SPObjectReference sPObjectReference = new SPObjectReference(SPObjectReference.SPObjectType.PlayerCharacter);
				sPObjectReference.Position = ReferenceMesh.transform.position;
				ActorReferences.Add(sPObjectReference);
			}
			if (ReferenceMesh != null)
			{
				Transform transform = base.transform;
				for (int j = 0; j < base.transform.childCount; j++)
				{
					Transform child = transform.GetChild(j);
					if (ReferenceMesh.gameObject.name == child.name)
					{
						child.gameObject.SetActive(false);
					}
					if (WorldReferenceNode.gameObject.name == child.name)
					{
						child.gameObject.SetActive(false);
					}
					foreach (GameObject playgroundActor2 in m_PlaygroundActors)
					{
						if (playgroundActor2 != null && playgroundActor2.gameObject.name == child.name)
						{
							child.gameObject.SetActive(false);
						}
					}
				}
			}
		}
		foreach (SPLinkInfo item in LinkInfo)
		{
			if (item.UsePlaygroundMesh)
			{
				AddPlaygroundActor(m_PlaygroundActors[item.ActorIndex], item.ActorIndex);
			}
		}
		foreach (SPStatement statement in Sequence.Statements)
		{
			foreach (SPCondition condition in statement.Conditions)
			{
				if (condition.Actor != null)
				{
					int index = condition.Actor.Index;
					if (index < ActorReferences.Count)
					{
						condition.Actor = ActorReferences[index];
						condition.Actor.Index = index;
					}
				}
			}
			foreach (SPAction action in statement.Actions)
			{
				int index2 = action.Target.Index;
				if (index2 < NodeReferences.Count)
				{
					action.Reset();
					action.Target = NodeReferences[index2];
					action.Target.Index = index2;
				}
				int index3 = action.Actor.Index;
				if (index3 < ActorReferences.Count)
				{
					action.Actor = ActorReferences[index3];
					action.Actor.Index = index3;
				}
				int index4 = action.Prop.Index;
				if (index4 < ActorReferences.Count)
				{
					action.Prop = ActorReferences[index4];
					action.Prop.Index = index4;
				}
			}
		}
	}

	private void OnDestroy()
	{
		Debug.Log("Cleaning up " + base.name);
		m_PlaygroundActors.Clear();
		m_ReplaceCharacters.Clear();
		m_WarpTo.Clear();
		ReferenceMesh = null;
		ActorReferences.Clear();
		NodeReferences.Clear();
		WorldObjects.Clear();
		WeaponLighting.Clear();
	}

	private void Update()
	{
		if (Activated)
		{
			if (!Sequence.Update())
			{
				Finished();
			}
		}
		else
		{
			PausedTime += Time.deltaTime;
		}
		if (!IsACutscene())
		{
			return;
		}
		if (mCurrentCutIndex < CameraCutTimes.Count && ReferenceMesh != null)
		{
			AnimationClip clip = ReferenceMesh.animation.clip;
			AnimationState animationState = ReferenceMesh.animation[clip.name];
			if (!(animationState != null))
			{
				return;
			}
			float time = animationState.time;
			if (time >= CameraCutTimes[mCurrentCutIndex] - 0.05f)
			{
				if (mJustCut)
				{
					mCurrentCutIndex++;
					animationState.speed = mPreviousAnimSpeed;
					mJustCut = false;
				}
				else
				{
					animationState.time += 0.1f;
					mPreviousAnimSpeed = animationState.speed;
					animationState.speed = 0f;
					mJustCut = true;
				}
			}
		}
		else if (ReferenceMesh == null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void Refresh()
	{
		BuildNameArrays();
		if (ReferenceMesh != null)
		{
			ReferenceMesh.transform.localPosition = Vector3.zero;
			ReferenceMesh.transform.localRotation = Quaternion.identity;
		}
	}

	public void FixUpSoundTargets()
	{
		if (!(ReferenceMesh != null))
		{
			return;
		}
		foreach (SPStatement statement in Sequence.Statements)
		{
			foreach (SPAction action in statement.Actions)
			{
				if ((action.Type != SPAction.SPActionType.PlaySound && action.Type != SPAction.SPActionType.FadeInSound && action.Type != SPAction.SPActionType.FadeOutSound && action.Type != SPAction.SPActionType.FadeSound) || action.Target == null || action.Target.Index >= NodeNames.Count || string.Compare(NodeNames[action.Target.Index], action.Target.Name) == 0)
				{
					continue;
				}
				Debug.Log("Sound target " + action.Target.Name + " is not the same as the index set in the play sound:" + NodeNames[action.Target.Index]);
				Transform transform = ReferenceMesh.transform;
				for (int i = 0; i < transform.childCount; i++)
				{
					Transform child = transform.GetChild(i);
					if (string.Compare(child.gameObject.name, action.Target.Name) != 0)
					{
						continue;
					}
					for (int j = 0; j < NodeNames.Count; j++)
					{
						if (string.Compare(NodeNames[j], action.Target.Name) == 0)
						{
							action.Target.Index = j;
							break;
						}
					}
				}
			}
		}
	}

	public bool IsOutro()
	{
		if (Sequence != null)
		{
			foreach (SPStatement statement in Sequence.Statements)
			{
				foreach (SPCondition condition in statement.Conditions)
				{
					if (condition.Type == SPCondition.SPConditionType.OutroEnd)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	private void BuildLinkInfo()
	{
	}

	private void BuildReferenceObjectList()
	{
	}

	private void BuildNameArrays()
	{
		if (ObjectTypeNames == null)
		{
			ObjectTypeNames = new List<string>();
		}
		ObjectTypeNames.Clear();
		for (SPObjectReference.SPObjectType sPObjectType = SPObjectReference.SPObjectType.Invalid; sPObjectType < SPObjectReference.SPObjectType.NumObjectTypes; sPObjectType++)
		{
			ObjectTypeNames.Add(sPObjectType.ToString());
		}
		if (ActorNames == null)
		{
			ActorNames = new List<string>();
		}
		ActorNames.Clear();
		for (int i = 0; i < 8; i++)
		{
			ActorNames.Add("Actor" + i);
		}
		ActorNames.Add("All Actors");
		if (NodeNames == null)
		{
			NodeNames = new List<string>();
		}
		NodeNames.Clear();
		if (NodeReferences == null)
		{
			NodeReferences = new List<SPObjectReference>();
		}
		NodeReferences.Clear();
		NodeNames.Add("Invalid");
		SPObjectReference item = new SPObjectReference(SPObjectReference.SPObjectType.Node);
		NodeReferences.Add(item);
		if (ReferenceMesh != null)
		{
			Transform transform = ReferenceMesh.transform;
			for (int j = 0; j < transform.childCount; j++)
			{
				Transform child = transform.GetChild(j);
				NodeNames.Add(child.name);
				item = new SPObjectReference(SPObjectReference.SPObjectType.Node);
				item.ObjectTransform = child;
				NodeReferences.Add(item);
			}
		}
		if (ConditionalNames == null)
		{
			ConditionalNames = new List<string>();
		}
		ConditionalNames.Clear();
		for (SPAction.SPConditionType sPConditionType = SPAction.SPConditionType.Invalid; sPConditionType < SPAction.SPConditionType.NumConditions; sPConditionType++)
		{
			ConditionalNames.Add(sPConditionType.ToString());
		}
		if (BlendNames == null)
		{
			BlendNames = new List<string>();
		}
		BlendNames.Clear();
		for (RawAnimation.AnimBlendType animBlendType = RawAnimation.AnimBlendType.kLinearCrossFade; animBlendType < RawAnimation.AnimBlendType.kSharpSharp; animBlendType++)
		{
			BlendNames.Add(animBlendType.ToString());
		}
		if (FadeGroupNames == null)
		{
			FadeGroupNames = new List<string>();
		}
		FadeGroupNames.Clear();
		int length = Enum.GetValues(typeof(SoundFXData.VolumeGroup)).Length;
		for (int k = 0; k < length; k++)
		{
			FadeGroupNames.Add(((SoundFXData.VolumeGroup)k).ToString());
		}
	}

	private void AddPlaygroundActor(GameObject GameObj, int Index)
	{
		if (GameObj != null)
		{
			SPObjectReference sPObjectReference = new SPObjectReference(SPObjectReference.SPObjectType.PlayerCharacter);
			sPObjectReference.SetActorAnimation(GameObj.animation);
			sPObjectReference.ObjectTransform = GameObj.transform;
			sPObjectReference.Position = ReferenceMesh.transform.position;
			if (Index == -1)
			{
				ActorReferences.Add(sPObjectReference);
			}
			else
			{
				ActorReferences[Index] = sPObjectReference;
			}
			sPObjectReference.PlaygroundSnapshot();
		}
		else
		{
			SPObjectReference item = new SPObjectReference(SPObjectReference.SPObjectType.Invalid);
			ActorReferences.Add(item);
		}
	}

	private void Finished()
	{
		bool flag = AddEndTransition;
		bool flag2 = false;
		if (IsACutscene())
		{
			if (FirstPersonSpawner != null)
			{
				ActorWrapper componentInChildren = IncludeDisabled.GetComponentInChildren<ActorWrapper>(FirstPersonSpawner.gameObject);
				if (componentInChildren != null)
				{
					if (SpawnerController != null)
					{
						SpawnerController.Activate();
					}
					StartCoroutine(WaitForSpawner());
					flag2 = true;
					flag = false;
				}
			}
			else if (MaintainCameraMode)
			{
				GameController instance = GameController.Instance;
				if (instance != null && mFirstPersonWhenStarted)
				{
					instance.SwitchToLastFirstPerson(AddEndTransition);
					SetFPPRotation(mFirstPersonTransform);
					flag = false;
				}
			}
			if (flag)
			{
				InteractionsManager.Instance.TransitionFromSolid(1.25f);
			}
		}
		if (!flag2)
		{
			FinishComplete();
		}
		Activated = false;
	}

	private IEnumerator WaitForSpawner()
	{
		if (!(FirstPersonSpawner != null))
		{
			yield break;
		}
		Actor act;
		while (true)
		{
			GameObject go = FirstPersonSpawner.spawned;
			if (go != null)
			{
				act = IncludeDisabled.GetComponentInChildren<Actor>(go);
				if (act != null)
				{
					break;
				}
			}
			yield return null;
		}
		GameController gc = GameController.Instance;
		if (gc != null)
		{
			gc.SwitchToFirstPerson(act, AddEndTransition);
			if (TheCamera != null)
			{
				InputSettings.FirstPersonFieldOfView = TheCamera.Fov;
			}
			act.OnScreen = false;
			act.Pose.PostModuleUpdate();
			if (mFirstPersonTransform != null)
			{
				SetFPPRotation(mFirstPersonTransform);
			}
			else
			{
				SetFPPRotation(FirstPersonSpawner.transform);
			}
			if (BlendToFPP)
			{
				CameraController cc = CameraManager.Instance.PlayCameraController;
				if (cc != null)
				{
					CameraBase cam = act.realCharacter.FirstPersonCamera;
					if (cam != null)
					{
						GameController.Instance.SuppressHud(true);
						cc.ForcedBlendTo(cam, 1f);
						yield return new WaitForSeconds(1f);
					}
				}
			}
		}
		FinishComplete();
	}

	private void FinishComplete()
	{
		foreach (SPObjectReference actorReference in ActorReferences)
		{
			if (actorReference == null || !(actorReference.ActorRef != null))
			{
				continue;
			}
			actorReference.ActorRef.baseCharacter.IsInASetPiece = false;
			if (GunOut(actorReference.Index))
			{
				IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(actorReference.ActorRef.weapon.ActiveWeapon);
				if (weaponEquip != null && !weaponEquip.IsPuttingAway() && !weaponEquip.IsTakingOut())
				{
					weaponEquip.TakeOut(1f);
				}
			}
		}
		if (m_CallOnCompletion != null)
		{
			Container.SendMessage(m_CallOnCompletion, "Activate");
		}
		if (m_FinishedDelegate != null)
		{
			m_FinishedDelegate();
		}
		InteractionsManager.Instance.FinishedAction(base.gameObject, null);
	}

	private void SetFPPRotation(Transform tranny)
	{
		GameController instance = GameController.Instance;
		if (instance != null && tranny != null)
		{
			instance.mFirstPersonActor.realCharacter.FirstPersonCamera.Angles = new Vector3(0f, tranny.eulerAngles.y, 0f);
		}
	}
}
