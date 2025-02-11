using System.Collections.Generic;
using UnityEngine;

public class SetPieceBagProcess : BaseBagProcess
{
	public bool m_AutoWeaponLighting = true;

	public List<GameObject> m_DisableAtStart;

	public List<GameObject> m_OnlyActiveDuringSetpiece;

	public List<ReplaceObject> m_Replacements;

	public List<AttachObject> m_Attachments;

	public List<LightingGroup> m_LightingGroups;

	public List<DynamicCollision> m_DynamicCollision;

	public List<CutsceneEffectsInstance> m_Effects;

	public float CameraCutDetection = 10f;

	private GameObject mReplacementFixupObj;

	private List<RedirectBone> mRedirectList;

	private void Start()
	{
	}

	public override void ApplyProcess(GameObject obj)
	{
		if (!BagManager.Instance.IsInBag())
		{
			Debug.Log("Replacing objects in Cutscene: " + obj.name);
			ReplaceObjects();
			AttachObjects();
			SetSkinQuality();
		}
		CutsceneLighting[] componentsInChildren = obj.GetComponentsInChildren<CutsceneLighting>();
		foreach (CutsceneLighting obj2 in componentsInChildren)
		{
			Object.DestroyImmediate(obj2);
		}
		CutsceneEffectsPlayer[] componentsInChildren2 = obj.GetComponentsInChildren<CutsceneEffectsPlayer>();
		foreach (CutsceneEffectsPlayer obj3 in componentsInChildren2)
		{
			Object.DestroyImmediate(obj3);
		}
		AutoDetectDynamicCollision();
		ApplyLightingGroups(obj);
		ApplyDynamicCollision();
		ApplyCutsceneEffects(obj);
		DisableObjectsAtStart();
		SetPieceModule[] componentsInChildren3 = base.gameObject.GetComponentsInChildren<SetPieceModule>();
		foreach (SetPieceModule setPieceModule in componentsInChildren3)
		{
			if (!setPieceModule.UsePlaygroundMeshes || !(setPieceModule != null))
			{
				continue;
			}
			foreach (GameObject playgroundActor in setPieceModule.m_PlaygroundActors)
			{
				if (playgroundActor != null)
				{
					Animation component = playgroundActor.GetComponent<Animation>();
					if (component != null)
					{
						component.cullingType = AnimationCullingType.AlwaysAnimate;
					}
				}
			}
		}
		Camera[] componentsInChildren4 = GetComponentsInChildren<Camera>(true);
		foreach (Camera camera in componentsInChildren4)
		{
			if (camera.fieldOfView > 150f)
			{
				camera.fieldOfView = 55f;
			}
		}
		ExtractCameraCutData();
		SetPieceModule[] componentsInChildren5 = base.gameObject.GetComponentsInChildren<SetPieceModule>(true);
		foreach (SetPieceModule setPieceModule2 in componentsInChildren5)
		{
			if (setPieceModule2.IsOutro())
			{
				setPieceModule2.MaintainCameraMode = false;
			}
		}
	}

	public void TestReplaceAttach()
	{
	}

	private void AutoDetectDynamicCollision()
	{
		BagObject component = base.gameObject.GetComponent<BagObject>();
		if (!(component != null))
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		component.GetObjectsWithTag("_dyncol", list, true);
		foreach (GameObject item in list)
		{
			DynamicCollision dynamicCollision = new DynamicCollision();
			dynamicCollision.Root = item.transform;
			m_DynamicCollision.Add(dynamicCollision);
		}
	}

	private void DisableObjectsAtStart()
	{
		if (m_DisableAtStart != null)
		{
			foreach (GameObject item in m_DisableAtStart)
			{
				if (item != null)
				{
					item.SetActive(false);
				}
			}
		}
		if (1 == 0)
		{
			return;
		}
		if (m_OnlyActiveDuringSetpiece != null)
		{
			foreach (GameObject item2 in m_OnlyActiveDuringSetpiece)
			{
				if (item2 != null)
				{
					item2.SetActive(false);
				}
			}
		}
		SetPieceLogic[] componentsInChildren = base.gameObject.GetComponentsInChildren<SetPieceLogic>(true);
		foreach (SetPieceLogic setPieceLogic in componentsInChildren)
		{
			setPieceLogic.SetOnlyActiveDuringSetpiece(m_OnlyActiveDuringSetpiece);
		}
	}

	private void ApplyLightingGroups(GameObject obj)
	{
		if (m_LightingGroups != null)
		{
			foreach (LightingGroup lightingGroup in m_LightingGroups)
			{
				CutsceneLighting cutsceneLighting = obj.AddComponent<CutsceneLighting>();
				cutsceneLighting.ProbeAnchor = lightingGroup.Anchor;
				cutsceneLighting.GroupRoots = lightingGroup.Roots;
				cutsceneLighting.Override = FindLightingOverride(lightingGroup.Override);
			}
		}
		if (!m_AutoWeaponLighting)
		{
			return;
		}
		SetPieceModule componentInChildren = IncludeDisabled.GetComponentInChildren<SetPieceModule>(obj);
		if (!(componentInChildren != null))
		{
			return;
		}
		componentInChildren.WeaponLighting.Clear();
		foreach (SkinnedMeshRenderer replaceCharacter in componentInChildren.m_ReplaceCharacters)
		{
			if (replaceCharacter != null)
			{
				Transform rootBone = replaceCharacter.rootBone;
				if (rootBone != null)
				{
					CutsceneLighting cutsceneLighting2 = obj.AddComponent<CutsceneLighting>();
					cutsceneLighting2.ProbeAnchor = FindPelvis(rootBone);
					Transform[] groupRoots = new Transform[1] { rootBone };
					cutsceneLighting2.GroupRoots = groupRoots;
					cutsceneLighting2.AllowRuntimeUpdate = true;
					componentInChildren.WeaponLighting.Add(cutsceneLighting2);
				}
				else
				{
					Debug.Log("Unable to find root bone in replacement character");
				}
			}
		}
	}

	private Transform FindPelvis(Transform trans)
	{
		Transform[] componentsInChildren = trans.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform.name.Contains("Pelvis"))
			{
				return transform;
			}
		}
		Debug.Log("Unable to find pelvis bone in replacement character");
		return null;
	}

	private Object FindLightingOverride(Object o)
	{
		if (o == null)
		{
			Debug.Log("Invalid lighting override in cutscene");
			return null;
		}
		if (o is ILightingOverride)
		{
			return o;
		}
		Component[] array = new Component[0];
		if (o is GameObject)
		{
			array = (o as GameObject).GetComponents<Component>();
		}
		else if (o is Transform)
		{
			array = (o as Transform).GetComponents<Component>();
		}
		Component[] array2 = array;
		foreach (Component component in array2)
		{
			if (component is ILightingOverride)
			{
				return component;
			}
		}
		return null;
	}

	private void ApplyDynamicCollision()
	{
		if (m_DynamicCollision == null)
		{
			return;
		}
		foreach (DynamicCollision item in m_DynamicCollision)
		{
			if (item == null || !(item.Root != null))
			{
				continue;
			}
			Rigidbody rigidbody = item.Root.gameObject.GetComponent<Rigidbody>() ?? item.Root.gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			GameObject gameObject = item.Root.gameObject;
			if (gameObject != null)
			{
				gameObject.layer = 0;
				if (gameObject.GetComponent<UncertaintyTag>() == null)
				{
					gameObject.AddComponent<UncertaintyTag>();
				}
			}
		}
	}

	private void ApplyCutsceneEffects(GameObject obj)
	{
		if (m_Effects == null)
		{
			return;
		}
		foreach (CutsceneEffectsInstance effect in m_Effects)
		{
			CutsceneEffectsPlayer cutsceneEffectsPlayer = obj.AddComponent<CutsceneEffectsPlayer>();
			cutsceneEffectsPlayer.AnimationPlayer = effect.AnimationPlayer;
			cutsceneEffectsPlayer.Effects = effect.Effects;
		}
	}

	private void ReplaceObjects()
	{
		if (m_Replacements == null)
		{
			return;
		}
		foreach (ReplaceObject replacement in m_Replacements)
		{
			if (replacement.m_PreviousReplacement != null)
			{
				Object.DestroyImmediate(replacement.m_PreviousReplacement);
			}
			if (!(replacement.m_Source != null) || !(replacement.m_Replace != null))
			{
				continue;
			}
			PreReplaceFixUp(replacement.m_Replace);
			if (replacement.m_TargetBones != null)
			{
				GameObject gameObject = null;
				GameObject gameObject2 = null;
				SkinnedMeshRenderer componentInChildren = IncludeDisabled.GetComponentInChildren<SkinnedMeshRenderer>(replacement.m_Source);
				SkinnedMeshRenderer component = replacement.m_Replace.GetComponent<SkinnedMeshRenderer>();
				if (componentInChildren != null && component != null)
				{
					Debug.Log(string.Concat("Converting Skinned mesh on ", base.gameObject.name, ". Replacing ", replacement.m_Replace, " with ", replacement.m_Source));
					replacement.m_ReplacedWith = (replacement.m_PreviousReplacement = CopySMRenderer(componentInChildren, component, replacement.m_TargetBones));
				}
				if (gameObject != null)
				{
					Object.DestroyImmediate(gameObject);
				}
			}
			else
			{
				replacement.m_ReplacedWith = TransformMerge(replacement.m_Source, replacement.m_Replace, true);
			}
			PostReplaceFixUp(replacement.m_ReplacedWith);
		}
	}

	private void AttachObjects()
	{
		if (m_Attachments == null)
		{
			return;
		}
		foreach (AttachObject attachment in m_Attachments)
		{
			if (attachment.m_PreviousAttachment != null)
			{
				Object.DestroyImmediate(attachment.m_PreviousAttachment);
			}
			if (attachment.m_Source != null && attachment.m_AttachTo != null)
			{
				attachment.m_Source.transform.parent = attachment.m_AttachTo.transform;
			}
		}
	}

	private GameObject CopySMRenderer(SkinnedMeshRenderer skinRenderer, SkinnedMeshRenderer replaceSkin, GameObject targetBones)
	{
		if (replaceSkin == null || targetBones == null || skinRenderer == null)
		{
			Debug.Log(string.Concat("Invalid objects in skin copy - skin: ", skinRenderer, " replaceSkin:", replaceSkin, " bones: ", targetBones));
		}
		GameObject gameObject = new GameObject(replaceSkin.name);
		SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)gameObject.AddComponent(typeof(SkinnedMeshRenderer));
		skinnedMeshRenderer.sharedMesh = skinRenderer.sharedMesh;
		skinnedMeshRenderer.sharedMaterials = skinRenderer.sharedMaterials;
		Transform[] array = new Transform[skinRenderer.bones.Length];
		Transform transform = skinRenderer.rootBone.transform;
		Transform transform2 = targetBones.transform;
		Component[] componentsInChildren = transform.GetComponentsInChildren(typeof(Transform), true);
		foreach (Component component in componentsInChildren)
		{
			if (component != null && component.name.Contains("Nub"))
			{
				Object.DestroyImmediate(component.gameObject);
			}
		}
		Component[] componentsInChildren2 = transform.GetComponentsInChildren(typeof(Transform), true);
		Component[] componentsInChildren3 = transform2.GetComponentsInChildren(typeof(Transform), true);
		List<RedirectBone> list = new List<RedirectBone>();
		int num = 0;
		Component[] array2 = componentsInChildren3;
		for (int j = 0; j < array2.Length; j++)
		{
			Transform transform3 = (Transform)array2[j];
			if (num >= componentsInChildren2.Length)
			{
				continue;
			}
			Transform transform4 = componentsInChildren2[num] as Transform;
			if (!(transform4 != null))
			{
				continue;
			}
			string text = BaseBagProcess.TrimNumbersAtEnd(transform4.name);
			string text2 = BaseBagProcess.TrimNumbersAtEnd(transform3.name);
			if (text == text2)
			{
				list.Add(new RedirectBone(transform4, transform3));
				Debug.Log("Redirect - src bone : " + transform4.name + " dst bone : " + transform3.name);
				num++;
				continue;
			}
			for (int k = 1; k <= 3; k++)
			{
				int num2 = num + k;
				if (num2 < componentsInChildren2.Length)
				{
					Transform transform5 = componentsInChildren2[num2] as Transform;
					string text3 = BaseBagProcess.TrimNumbersAtEnd(transform5.name);
					if (text3 == text2)
					{
						list.Add(new RedirectBone(transform5, transform3));
						Debug.Log("Forward Redirect - src bone : " + transform4.name + " dst bone : " + transform3.name);
						num += k + 1;
						break;
					}
				}
			}
		}
		for (int l = 0; l < skinRenderer.bones.Length; l++)
		{
			Transform bone = skinRenderer.bones[l];
			RedirectBone redirectBone = list.Find((RedirectBone obj) => obj.srcTrans == bone);
			if (redirectBone != null)
			{
				array[l] = redirectBone.dstTrans;
				continue;
			}
			if (bone == null)
			{
				Debug.Log("Bone is null! Index = " + l);
				continue;
			}
			Debug.Log("Bone not found: " + bone.name + " on skin " + replaceSkin);
		}
		skinnedMeshRenderer.bones = array;
		gameObject.transform.parent = replaceSkin.transform.parent;
		gameObject.transform.localPosition = replaceSkin.transform.localPosition;
		gameObject.transform.localRotation = replaceSkin.transform.localRotation;
		gameObject.transform.localScale = replaceSkin.transform.localScale;
		skinnedMeshRenderer.localBounds = replaceSkin.localBounds;
		skinnedMeshRenderer.updateWhenOffscreen = true;
		Object.DestroyImmediate(replaceSkin.gameObject);
		return gameObject;
	}

	private void PreReplaceFixUp(GameObject replaceObj)
	{
		mReplacementFixupObj = new GameObject("Replacement marker");
		int num = m_DisableAtStart.IndexOf(replaceObj);
		if (num >= 0)
		{
			m_DisableAtStart[num] = mReplacementFixupObj;
		}
		num = m_OnlyActiveDuringSetpiece.IndexOf(replaceObj);
		if (num >= 0)
		{
			m_OnlyActiveDuringSetpiece[num] = mReplacementFixupObj;
		}
		foreach (LightingGroup lightingGroup in m_LightingGroups)
		{
			if (lightingGroup.Anchor == replaceObj.transform)
			{
				lightingGroup.Anchor = mReplacementFixupObj.transform;
			}
			if (lightingGroup.Roots == null)
			{
				continue;
			}
			for (int i = 0; i < lightingGroup.Roots.Length; i++)
			{
				if (lightingGroup.Roots[i] == replaceObj.transform)
				{
					lightingGroup.Roots[i] = mReplacementFixupObj.transform;
				}
			}
		}
	}

	private void PostReplaceFixUp(GameObject newObject)
	{
		int num = m_DisableAtStart.IndexOf(mReplacementFixupObj);
		if (num >= 0)
		{
			m_DisableAtStart[num] = newObject;
		}
		num = m_OnlyActiveDuringSetpiece.IndexOf(mReplacementFixupObj);
		if (num >= 0)
		{
			m_OnlyActiveDuringSetpiece[num] = newObject;
		}
		foreach (LightingGroup lightingGroup in m_LightingGroups)
		{
			if (!(newObject != null))
			{
				continue;
			}
			if (lightingGroup.Anchor == mReplacementFixupObj.transform)
			{
				lightingGroup.Anchor = newObject.transform;
			}
			if (lightingGroup.Roots == null)
			{
				continue;
			}
			for (int i = 0; i < lightingGroup.Roots.Length; i++)
			{
				if (lightingGroup.Roots[i] == mReplacementFixupObj.transform)
				{
					lightingGroup.Roots[i] = newObject.transform;
				}
			}
		}
		Object.DestroyImmediate(mReplacementFixupObj);
	}

	private void ExtractCameraCutData()
	{
	}

	private void SetSkinQuality()
	{
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(SkinnedMeshRenderer));
		for (int i = 0; i < array.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = (SkinnedMeshRenderer)array[i];
			skinnedMeshRenderer.quality = SkinQuality.Bone2;
		}
	}

	public override void ReplacedTransform(Transform newTrans, Transform oldTrans)
	{
		foreach (LightingGroup lightingGroup in m_LightingGroups)
		{
			if (lightingGroup.Anchor == oldTrans)
			{
				lightingGroup.Anchor = newTrans;
			}
			if (lightingGroup.Roots == null)
			{
				continue;
			}
			for (int i = 0; i < lightingGroup.Roots.Length; i++)
			{
				if (lightingGroup.Roots[i] == oldTrans)
				{
					lightingGroup.Roots[i] = newTrans;
				}
			}
		}
		for (int j = 0; j < m_OnlyActiveDuringSetpiece.Count; j++)
		{
			GameObject gameObject = m_OnlyActiveDuringSetpiece[j];
			if (gameObject != null && gameObject.transform == oldTrans)
			{
				m_OnlyActiveDuringSetpiece[j] = newTrans.gameObject;
			}
		}
		for (int k = 0; k < m_DisableAtStart.Count; k++)
		{
			GameObject gameObject2 = m_DisableAtStart[k];
			if (gameObject2 != null && gameObject2.transform == oldTrans)
			{
				m_DisableAtStart[k] = newTrans.gameObject;
			}
		}
	}

	private void ForceGunSwap()
	{
		SetPieceModule componentInChildren = base.gameObject.GetComponentInChildren<SetPieceModule>();
		if (!(componentInChildren != null))
		{
			return;
		}
		foreach (SkinnedMeshRenderer replaceCharacter in componentInChildren.m_ReplaceCharacters)
		{
			if (replaceCharacter == null || (bool)replaceCharacter.bones[0])
			{
				continue;
			}
			Transform transform = null;
			transform = replaceCharacter.bones[0];
			while (transform.parent.name.Contains("Bip"))
			{
				transform = transform.parent;
			}
			if (transform != null)
			{
				SoldierSettings soldierSettings = GameSettings.Instance.Soldiers[0];
				if (soldierSettings != null)
				{
					WeaponUtils.CreateThirdPersonModelForCutscene(transform.gameObject, soldierSettings.Weapon.Descriptor);
				}
			}
		}
	}
}
