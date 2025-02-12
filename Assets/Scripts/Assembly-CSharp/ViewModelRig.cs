using System;
using System.Collections.Generic;
using UnityEngine;

public class ViewModelRig : MonoBehaviour
{
	private class WeaponRig
	{
		public GameObject root;

		public Transform camera;

		public CharacterLighting lighting;

		public float flashIntensity;

		public Transform muzzle;

		public GameObject muzzleEffect;

		public CasingEffect casingEffect;

		public IViewModelWeaponBlendTree blendTree;

		public ViewModelMovementBlendTree movementBlendTree;

		public Texture2D scopeTexture;
	}

	[Flags]
	public enum FireEffects
	{
		None = 0,
		MuzzleFlash = 1,
		Lighting = 2,
		CasingEject = 4,
		All = 7
	}

	public ViewModelData Data;

	private List<WeaponRig> mWeapons;

	private Dictionary<string, WeaponRig> mWeaponMappings;

	private Transform mCameraLocator;

	private Vector3 mCrosshairCentre;

	private Vector3 mCrosshairForward;

	private Vector3 mSoftLockPosition;

	private bool mSoftLockActive;

	private Texture2D mScopeTexture;

	private string mActiveId;

	private WeaponRig mActiveRig;

	private string mOverrideId;

	private AnimationClip mOverrideClip;

	private float mOverrideTime;

	private Vector3 mOverrideLocatorPosition;

	private Quaternion mOverrideLocatorRotation;

	private static ViewModelRig sm_Instance;

	public bool IsOverrideActive
	{
		get
		{
			return mOverrideId != null;
		}
	}

	public Vector3 Velocity { get; set; }

	public Vector3 ViewBob { get; private set; }

	public void SetOverride(string rigId, AnimationClip clip, float time, Vector3 locatorPosition, Quaternion locatorRotation, bool dontClearDepth)
	{
		mOverrideId = rigId;
		mOverrideClip = clip;
		mOverrideTime = time;
		mOverrideLocatorPosition = locatorPosition;
		mOverrideLocatorRotation = locatorRotation;
		CameraManager.Instance.SetViewModelCameraDepthClear(!dontClearDepth);
	}

	public void ClearOverride()
	{
		mOverrideId = null;
		CameraManager.Instance.SetViewModelCameraDepthClear(true);
	}

	public void SetSoftLockPosition(Vector3 position)
	{
		mSoftLockPosition = position;
		mSoftLockActive = true;
	}

	public void ClearSoftLock()
	{
		mSoftLockActive = false;
	}

	public Transform GetCameraLocator()
	{
		return mCameraLocator;
	}

	public Vector3 GetCrosshairCentre()
	{
		return mCrosshairCentre;
	}

	public Vector3 GetCrosshairForward()
	{
		return (!mSoftLockActive || !GameController.Instance.DirectFireToSoftLockPosition) ? mCrosshairForward : (mSoftLockPosition - mCrosshairCentre).normalized;
	}

	public Texture2D GetScopeTexture()
	{
		return mScopeTexture;
	}

	public Vector3 Fire(string id)
	{
		return Fire(id, FireEffects.All);
	}

	public Vector3 Fire(string id, FireEffects effects)
	{
		WeaponRig value;
		if (!mWeaponMappings.TryGetValue(id, out value) || value.muzzle == null)
		{
			return Vector3.zero;
		}
		if ((effects & FireEffects.MuzzleFlash) != 0)
		{
			WeaponUtils.CreateMuzzleFlash(value.muzzle, value.muzzleEffect);
		}
		if ((effects & FireEffects.CasingEject) != 0 && value.casingEffect != null)
		{
			value.casingEffect.Fire(true, Velocity);
		}
		if ((effects & FireEffects.Lighting) != 0)
		{
			value.lighting.Flash(value.muzzle, value.flashIntensity);
		}
		return value.muzzle.position;
	}

	public void UpdateForActor(Actor owner)
	{
		if (mWeapons == null || mWeapons.Count == 0)
		{
			return;
		}
		if (owner == null || owner.baseCharacter == null || !owner.CanSeeHands)
		{
			Activate(null);
			return;
		}
		mCrosshairCentre = owner.baseCharacter.FirstPersonCamera.transform.position;
		mCrosshairForward = owner.baseCharacter.FirstPersonCamera.transform.forward;
		if (!(owner != null))
		{
			return;
		}
		IWeapon activeWeapon = owner.weapon.ActiveWeapon;
		IWeaponADS weaponADS = WeaponUtils.GetWeaponADS(activeWeapon);
		IWeaponEquip weaponEquip = WeaponUtils.GetWeaponEquip(activeWeapon);
		string id = mOverrideId ?? activeWeapon.GetId();
		WeaponRig weaponRig = Activate(id);
		if (weaponRig == null)
		{
			return;
		}
		weaponRig.root.GetComponent<Animation>().Stop();
		mCameraLocator = ((mOverrideId == null) ? null : weaponRig.root.transform.FindInHierarchy("CameraAim"));
		if (mOverrideId == null)
		{
			AnimationState animationState = weaponRig.root.GetComponent<Animation>()["PutAway"];
			AnimationState animationState2 = weaponRig.root.GetComponent<Animation>()["TakeOut"];
			AnimationState animationState3 = null;
			float normalizedTime = 0f;
			if (weaponADS != null)
			{
				if (weaponADS.GetADSState() == ADSState.ADS)
				{
					animationState3 = weaponRig.root.GetComponent<Animation>()["SightsIdle"];
				}
				else if (weaponADS.GetADSState() == ADSState.Hips)
				{
					animationState3 = weaponRig.root.GetComponent<Animation>()["HipsIdle"];
				}
				normalizedTime = weaponADS.GetHipsToSightsBlendAmount();
			}
			AnimationState animationState4 = weaponRig.root.GetComponent<Animation>()["HipsToSights"];
			if (animationState != null)
			{
				animationState.layer = 1;
			}
			if (animationState2 != null)
			{
				animationState2.layer = 1;
			}
			if (animationState3 != null)
			{
				animationState3.layer = 0;
			}
			if (animationState4 != null)
			{
				animationState4.layer = 0;
				if (weaponEquip != null && (weaponEquip.IsPuttingAway() || weaponEquip.HasNoWeapon()) && animationState != null)
				{
					animationState.enabled = true;
					animationState.weight = 1f;
					animationState.speed = 0f;
					animationState.normalizedTime = weaponEquip.GetEquipedBlendAmount();
				}
				else if (weaponEquip != null && weaponEquip.IsTakingOut() && animationState2 != null)
				{
					animationState2.enabled = true;
					animationState2.weight = 1f;
					animationState2.speed = 0f;
					animationState2.normalizedTime = weaponEquip.GetEquipedBlendAmount();
				}
				else if (animationState3 == null)
				{
					animationState4.enabled = true;
					animationState4.weight = 1f;
					animationState4.speed = 0f;
					animationState4.normalizedTime = normalizedTime;
				}
				else
				{
					animationState3.enabled = true;
					animationState3.weight = 1f;
					animationState3.speed = 0f;
					animationState3.time = Time.time;
				}
			}
			if (weaponRig.blendTree != null)
			{
				weaponRig.blendTree.Update(activeWeapon);
			}
			if (weaponRig.movementBlendTree != null)
			{
				weaponRig.movementBlendTree.Update(activeWeapon);
				ViewBob = weaponRig.movementBlendTree.ViewBob;
			}
		}
		else
		{
			AnimationState animationState5 = weaponRig.root.GetComponent<Animation>().AddClipSafe(mOverrideClip, mOverrideClip.name);
			animationState5.wrapMode = WrapMode.ClampForever;
			animationState5.weight = 1f;
			animationState5.speed = 0f;
			animationState5.enabled = true;
			animationState5.time = mOverrideTime;
		}
		weaponRig.root.GetComponent<Animation>().Sample();
		if (mOverrideId != null)
		{
			weaponRig.root.transform.position = mOverrideLocatorPosition;
			weaponRig.root.transform.rotation = mOverrideLocatorRotation;
		}
		else if (weaponEquip == null || !weaponEquip.HasNoWeapon() || weaponEquip.IsPuttingAway())
		{
			TransformUtils.ModifyTransformToAlignPoints(weaponRig.root.transform, owner.realCharacter.FirstPersonCamera.transform, weaponRig.camera);
		}
		if (weaponADS != null && weaponADS.GetADSState() == ADSState.ADS && !activeWeapon.HasScope())
		{
			Transform transform = weaponRig.root.transform.FindInHierarchy("AimSights");
			if (transform != null)
			{
				mCrosshairForward = transform.position - owner.baseCharacter.FirstPersonCamera.Position;
			}
		}
		mScopeTexture = weaponRig.scopeTexture;
	}

	public void CreateRig(ViewModelData data)
	{
		if (mWeapons != null)
		{
			return;
		}
		mWeapons = new List<WeaponRig>();
		mWeaponMappings = new Dictionary<string, WeaponRig>();
		string text = ((!(MissionSetup.Instance != null)) ? null : MissionSetup.Instance.Theme);
		if (text != null)
		{
			foreach (ThemedMaterial themedMaterial in data.ThemedMaterials)
			{
				Texture2D texture2D = themedMaterial.GetTextureForTheme(text) as Texture2D;
				if (texture2D != null)
				{
					themedMaterial.BaseMaterial.mainTexture = texture2D;
				}
			}
		}
		Datapad datapad = base.gameObject.AddComponent<Datapad>();
		datapad.ScreenOn = true;
		WeaponRig weaponRig = null;
		WeaponRig weaponRig2 = null;
		WeaponRig weaponRig3 = null;
		WeaponRig weaponRig4 = null;
		WeaponRig weaponRig5 = null;
		WeaponRig weaponRig6 = null;
		WeaponRig weaponRig7 = null;
		WeaponRig weaponRig8 = null;
		WeaponRig weaponRig9 = null;
		WeaponRig weaponRig10 = null;
		WeaponRig weaponRig11 = null;
		WeaponRig weaponRig12 = null;
		WeaponRig weaponRig13 = null;
		WeaponRig weaponRig14 = null;
		WeaponRig weaponRig15 = null;
		WeaponRig weaponRig16 = null;
		WeaponRig weaponRig17 = null;
		WeaponRig weaponRig18 = null;
		WeaponRig weaponRig19 = null;
		WeaponRig weaponRig20 = null;
		WeaponRig weaponRig21 = null;
		WeaponRig weaponRig22 = null;
		weaponRig = CreateIndividualRig("Empty", new GameObject[1] { data.Arms }, null, null, null, null);
		mWeapons.Add(weaponRig);
		mWeaponMappings["Empty"] = weaponRig;
		EffectsList effects = EffectsController.Instance.Effects;
		if (data.Ammo != null)
		{
			weaponRig2 = CreateIndividualRig("Ammo", new GameObject[2] { data.Arms, data.Ammo }, data.AmmoCoreAnims, null, null, null);
			SetupAmmo(weaponRig2, data);
			mWeapons.Add(weaponRig2);
			mWeaponMappings["Ammo"] = weaponRig2;
		}
		if (data.AN94 != null)
		{
			weaponRig3 = CreateIndividualRig("AN94", new GameObject[2] { data.Arms, data.AN94 }, data.AN94CoreAnims, effects.AN94MuzzleFlash, effects.AN94CasingEject, null);
			SetupAN94(weaponRig3, data);
			mWeapons.Add(weaponRig3);
			mWeaponMappings["AN-94"] = weaponRig3;
		}
		if (data.XPR50 != null)
		{
			weaponRig4 = CreateIndividualRig("XPR-50", new GameObject[2] { data.Arms, data.XPR50 }, data.XPR50CoreAnims, effects.AS50MuzzleFlash, effects.AS50CasingEject, data.XPR50Scope);
			SetupXPR50(weaponRig4, data);
			mWeapons.Add(weaponRig4);
			mWeaponMappings["XPR-50"] = weaponRig4;
		}
		if (data.Ballista != null)
		{
			weaponRig5 = CreateIndividualRig("Ballista", new GameObject[2] { data.Arms, data.Ballista }, data.BallistaCoreAnims, effects.BallistaMuzzleFlash, effects.BallistaCasingEject, data.BallistaScope);
			SetupBallista(weaponRig5, data);
			mWeapons.Add(weaponRig5);
			mWeaponMappings["Ballista"] = weaponRig5;
		}
		if (data.Beretta != null)
		{
			weaponRig6 = CreateIndividualRig("Beretta", new GameObject[2] { data.Arms, data.Beretta }, data.BerettaCoreAnims, effects.BerettaMuzzleFlash, effects.BerettaCasingEject, null);
			SetupBeretta(weaponRig6, data);
			mWeapons.Add(weaponRig6);
			mWeaponMappings["Beretta 23R"] = weaponRig6;
		}
		if (data.C4 != null)
		{
			weaponRig7 = CreateIndividualRig("C4", new GameObject[3] { data.Arms, data.C4, data.C4Remote }, data.C4CoreAnims, null, null, null);
			SetupC4(weaponRig7, data);
			mWeapons.Add(weaponRig7);
			mWeaponMappings["C4"] = weaponRig7;
		}
		if (data.Claymore != null)
		{
			weaponRig8 = CreateIndividualRig("Claymore", new GameObject[2] { data.Arms, data.Claymore }, data.ClaymoreCoreAnims, null, null, null);
			SetupClaymore(weaponRig8, data);
			mWeapons.Add(weaponRig8);
			mWeaponMappings["Claymore"] = weaponRig8;
		}
		if (data.FragGrenade != null)
		{
			weaponRig9 = CreateIndividualRig("FragGrenade", new GameObject[2] { data.Arms, data.FragGrenade }, data.FragGrenadeCoreAnims, null, null, null);
			SetupGrenade(weaponRig9, data);
			mWeapons.Add(weaponRig9);
			mWeaponMappings["Frag Grenade"] = weaponRig9;
		}
		if (data.HAMR != null)
		{
			weaponRig10 = CreateIndividualRig("HAMR", new GameObject[2] { data.Arms, data.HAMR }, data.HAMRCoreAnims, effects.HAMRMuzzleFlash, effects.HAMRCasingEject, null);
			SetupHAMR(weaponRig10, data);
			mWeapons.Add(weaponRig10);
			mWeaponMappings["HAMR"] = weaponRig10;
		}
		if (data.Knife != null)
		{
			weaponRig11 = CreateIndividualRig("Knife", new GameObject[2] { data.Arms, data.Knife }, data.KnifeCoreAnims, null, null, null);
			SetupKnife(weaponRig11, data);
			mWeapons.Add(weaponRig11);
			mWeaponMappings["Knife"] = weaponRig11;
		}
		if (data.KS23 != null)
		{
			weaponRig12 = CreateIndividualRig("KS-23", new GameObject[2] { data.Arms, data.KS23 }, data.KS23CoreAnims, effects.KS23MuzzleFlash, effects.KS23CasingEject, null);
			SetupKS23(weaponRig12, data);
			mWeapons.Add(weaponRig12);
			mWeaponMappings["KS-23"] = weaponRig12;
		}
		if (data.KSG != null)
		{
			weaponRig13 = CreateIndividualRig("KSG", new GameObject[2] { data.Arms, data.KSG }, data.KSGCoreAnims, effects.KSGMuzzleFlash, effects.KSGCasingEject, null);
			SetupKSG(weaponRig13, data);
			mWeapons.Add(weaponRig13);
			mWeaponMappings["KSG"] = weaponRig13;
		}
		if (data.LSAT != null)
		{
			weaponRig14 = CreateIndividualRig("LSAT", new GameObject[2] { data.Arms, data.LSAT }, data.LSATCoreAnims, effects.LSATMuzzleFlash, effects.LSATCasingEject, null);
			SetupLSAT(weaponRig14, data);
			mWeapons.Add(weaponRig14);
			mWeaponMappings["LSAT"] = weaponRig14;
		}
		if (data.M1216 != null)
		{
			weaponRig15 = CreateIndividualRig("M1216", new GameObject[2] { data.Arms, data.M1216 }, data.M1216CoreAnims, effects.M1216MuzzleFlash, effects.M1216CasingEject, null);
			SetupM1216(weaponRig15, data);
			mWeapons.Add(weaponRig15);
			mWeaponMappings["M1216"] = weaponRig15;
		}
		if (data.M8A1 != null)
		{
			weaponRig16 = CreateIndividualRig("M8A1", new GameObject[2] { data.Arms, data.M8A1 }, data.M8A1CoreAnims, effects.M8A1MuzzleFlash, effects.M8A1CasingEject, null);
			SetupM8A1(weaponRig16, data);
			mWeapons.Add(weaponRig16);
			mWeaponMappings["M8A1"] = weaponRig16;
		}
		if (data.PDW != null)
		{
			weaponRig17 = CreateIndividualRig("PDW-57", new GameObject[2] { data.Arms, data.PDW }, data.PDWCoreAnims, effects.PDW57MuzzleFlash, effects.PDW57CasingEject, null);
			SetupPDW(weaponRig17, data);
			mWeapons.Add(weaponRig17);
			mWeaponMappings["PDW-57"] = weaponRig17;
		}
		if (data.QBBLSW != null)
		{
			weaponRig18 = CreateIndividualRig("QBBLSW", new GameObject[2] { data.Arms, data.QBBLSW }, data.QBBLSWCoreAnims, effects.QBBLSWMuzzleFlash, effects.QBBLSWMuzzleFlash, null);
			SetupQBBLSW(weaponRig18, data);
			mWeapons.Add(weaponRig18);
			mWeaponMappings["QBBLSW"] = weaponRig18;
		}
		if (data.Type25 != null)
		{
			weaponRig19 = CreateIndividualRig("Type25", new GameObject[2] { data.Arms, data.Type25 }, data.Type25CoreAnims, effects.Type25MuzzleFlash, effects.Type25CasingEject, null);
			SetupType25(weaponRig19, data);
			mWeapons.Add(weaponRig19);
			mWeaponMappings["Type25"] = weaponRig19;
		}
		if (data.Skorpion != null)
		{
			weaponRig20 = CreateIndividualRig("Skorpion EVO", new GameObject[2] { data.Arms, data.Skorpion }, data.SkorpionCoreAnims, effects.SkorpionEVOMuzzleFlash, effects.SkorpionEVOCasingEject, null);
			SetupSkorpion(weaponRig20, data);
			mWeapons.Add(weaponRig20);
			mWeaponMappings["Skorpion EVO"] = weaponRig20;
		}
		if (data.SVUAS != null)
		{
			weaponRig21 = CreateIndividualRig("SVU-AS", new GameObject[2] { data.Arms, data.SVUAS }, data.SVUASCoreAnims, effects.SVUASMuzzleFlash, effects.SVUASCasingEject, data.SVUASScope);
			SetupSVUAS(weaponRig21, data);
			mWeapons.Add(weaponRig21);
			mWeaponMappings["SVU-AS"] = weaponRig21;
		}
		if (data.Vektor != null)
		{
			weaponRig22 = CreateIndividualRig("Vektor K10", new GameObject[2] { data.Arms, data.Vektor }, data.VektorCoreAnims, effects.VektorK10MuzzleFlash, effects.VektorK10CasingEject, null);
			SetupVektor(weaponRig22, data);
			mWeapons.Add(weaponRig22);
			mWeaponMappings["Vektor K10"] = weaponRig22;
		}
		UpdateForActor(null);
		SetLayerRecursively(base.gameObject, LayerMask.NameToLayer("ViewModel"));
	}

	private WeaponRig CreateIndividualRig(string name, IEnumerable<GameObject> models, WeaponCoreAnims anims, GameObject muzzleEffect, GameObject casingEffect, Texture2D scopeTexture)
	{
		GameObject gameObject = CreateRoot(name);
		foreach (GameObject model in models)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(model) as GameObject;
			SkinnedMeshRenderer[] componentsInChildren = gameObject2.GetComponentsInChildren<SkinnedMeshRenderer>();
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.quality = SkinQuality.Bone2;
			}
			while (gameObject2.transform.childCount > 0)
			{
				Transform child = gameObject2.transform.GetChild(0);
				child.parent = gameObject.transform;
			}
			UnityEngine.Object.Destroy(gameObject2);
		}
		GameObject gameObject3 = new GameObject("ProbeAnchor");
		gameObject3.transform.parent = gameObject.transform.FindInHierarchy("Bip001 Spine3") ?? gameObject.transform;
		gameObject3.transform.localPosition = Vector3.zero;
		gameObject3.transform.localRotation = Quaternion.identity;
		Transform transform = gameObject.transform.FindInHierarchy("muzzle_flash") ?? gameObject.transform;
		GameObject gameObject4 = new GameObject("FlashAnchor");
		gameObject4.transform.parent = transform;
		gameObject4.transform.localPosition = Vector3.left;
		gameObject4.transform.localRotation = Quaternion.identity;
		CharacterLighting characterLighting = gameObject.AddComponent<CharacterLighting>();
		characterLighting.ProbeAnchor = gameObject3.transform;
		characterLighting.FlashAnchor = gameObject4.transform;
		characterLighting.Renderers = gameObject.GetComponentsInChildren<Renderer>();
		Renderer[] renderers = characterLighting.Renderers;
		foreach (Renderer renderer in renderers)
		{
			renderer.materials = renderer.materials;
		}
		Transform locator = gameObject.transform.FindInHierarchy("cartridge") ?? gameObject.transform;
		gameObject.transform.parent = base.transform;
		Transform transform2 = new GameObject("OffsetCamera").transform;
		transform2.parent = gameObject.transform.FindInHierarchy("CameraAim");
		Animation animation = gameObject.GetComponent<Animation>();
		if (anims != null)
		{
			animation.AddClipSafe(anims.HipsIdle, "HipsIdle");
			animation.AddClipSafe(anims.SightsIdle, "SightsIdle");
			animation.AddClipSafe(anims.TakeOut, "TakeOut");
			animation.AddClipSafe(anims.PutAway, "PutAway");
			animation.AddClipSafe(anims.Move, "Move");
			animation.AddClipSafe(anims.HipsToSights, "HipsToSights");
			SetWrapMode(animation, "HipsIdle", WrapMode.Loop);
			SetWrapMode(animation, "SightsIdle", WrapMode.Loop);
			SetWrapMode(animation, "TakeOut", WrapMode.ClampForever);
			SetWrapMode(animation, "PutAway", WrapMode.ClampForever);
			SetWrapMode(animation, "Move", WrapMode.Loop);
			SetWrapMode(animation, "HipsToSights", WrapMode.ClampForever);
			transform2.localPosition = anims.PositionOffset;
			transform2.localEulerAngles = anims.RotationOffset;
		}
		else
		{
			transform2.localPosition = Vector3.zero;
			transform2.localEulerAngles = Vector3.zero;
		}
		WeaponRig weaponRig = new WeaponRig();
		weaponRig.root = gameObject;
		weaponRig.camera = transform2;
		weaponRig.lighting = characterLighting;
		weaponRig.flashIntensity = 50f;
		weaponRig.muzzle = transform;
		weaponRig.muzzleEffect = muzzleEffect;
		weaponRig.casingEffect = WeaponUtils.CreateCasingEffect(locator, casingEffect);
		weaponRig.movementBlendTree = ((anims == null) ? null : new ViewModelMovementBlendTree(gameObject, anims.HipsLook, anims.SightsLook));
		weaponRig.scopeTexture = scopeTexture;
		Deactivate(weaponRig);
		return weaponRig;
	}

	private GameObject CreateRoot(string name)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.AddComponent<Animation>();
		return gameObject;
	}

	private WeaponRig Activate(string id)
	{
		if (id != mActiveId)
		{
			mActiveId = id;
			if (mActiveRig != null)
			{
				Deactivate(mActiveRig);
			}
			if (id == null || !mWeaponMappings.TryGetValue(id, out mActiveRig))
			{
				return null;
			}
		}
		return mActiveRig;
	}

	private void Deactivate(WeaponRig rig)
	{
		Vector3 position = new Vector3(0f, 5000f, 0f);
		rig.root.transform.position = position;
		rig.root.GetComponent<Animation>().enabled = false;
	}

	private void SetupAmmo(WeaponRig weapon, ViewModelData data)
	{
	}

	private void SetupAN94(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_AN94(weapon.root, data.AN94CoreAnims, data.AN94SpecificAnims);
	}

	private void SetupXPR50(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_XPR50(weapon.root, data.XPR50CoreAnims, data.XPR50SpecificAnims);
	}

	private void SetupBallista(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_Ballista(weapon.root, data.BallistaCoreAnims, data.BallistaSpecificAnims);
	}

	private void SetupBeretta(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_Beretta(weapon.root, data.BerettaCoreAnims, data.BerettaSpecificAnims);
	}

	private void SetupC4(WeaponRig weapon, ViewModelData data)
	{
	}

	private void SetupClaymore(WeaponRig weapon, ViewModelData data)
	{
	}

	private void SetupGrenade(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_Grenade(weapon.root, data.FragGrenadeCoreAnims, data.FragGrenadeSpecificAnims);
	}

	private void SetupHAMR(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_HAMR(weapon.root, data.HAMRCoreAnims, data.HAMRSpecificAnims);
	}

	private void SetupKnife(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_Knife(weapon.root, data.KnifeCoreAnims, data.KnifeSpecificAnims);
	}

	private void SetupKS23(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_KS23(weapon.root, data.KS23CoreAnims, data.KS23SpecificAnims);
	}

	private void SetupKSG(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_KSG(weapon.root, data.KSGCoreAnims, data.KSGSpecificAnims);
	}

	private void SetupLSAT(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_LSAT(weapon.root, data.LSATCoreAnims, data.LSATSpecificAnims);
	}

	private void SetupM1216(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_M1216(weapon.root, data.M1216CoreAnims, data.M1216SpecificAnims);
	}

	private void SetupM8A1(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_M8A1(weapon.root, data.M8A1CoreAnims, data.M8A1SpecificAnims);
	}

	private void SetupPDW(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_PDW(weapon.root, data.PDWCoreAnims, data.PDWSpecificAnims);
	}

	private void SetupQBBLSW(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_QBBLSW(weapon.root, data.QBBLSWCoreAnims, data.QBBLSWSpecificAnims);
	}

	private void SetupSkorpion(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_Skorpion(weapon.root, data.SkorpionCoreAnims, data.SkorpionSpecificAnims);
	}

	private void SetupType25(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_Type25(weapon.root, data.Type25CoreAnims, data.Type25SpecificAnims);
		weapon.flashIntensity = 20f;
	}

	private void SetupSVUAS(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_SVUAS(weapon.root, data.SVUASCoreAnims, data.SVUASSpecificAnims);
	}

	private void SetupVektor(WeaponRig weapon, ViewModelData data)
	{
		weapon.blendTree = new ViewModelBlendTree_Vektor(weapon.root, data.VektorCoreAnims, data.VektorSpecificAnims);
	}

	private void SetBlendMode(Animation animation, string name, AnimationBlendMode blendMode)
	{
		AnimationState animationState = animation[name];
		if (animationState != null)
		{
			animationState.blendMode = blendMode;
		}
	}

	private void SetWrapMode(Animation animation, string name, WrapMode wrapMode)
	{
		AnimationState animationState = animation[name];
		if (animationState != null)
		{
			animationState.wrapMode = wrapMode;
		}
	}

	private static void SetLayerRecursively(GameObject go, int layer)
	{
		go.layer = layer;
		foreach (Transform item in go.transform)
		{
			SetLayerRecursively(item.gameObject, layer);
		}
	}

	public static ViewModelRig Instance()
	{
		if (sm_Instance == null)
		{
			sm_Instance = new GameObject("ViewModelRig").AddComponent<ViewModelRig>();
		}
		return sm_Instance;
	}

	public static void SetupEventData(out float[,] eventList, AnimationEventData eventData, AnimationEventData eventDataTactical)
	{
		int num = 0;
		int num2 = 0;
		if ((bool)eventData)
		{
			num = eventData.Events.Length;
		}
		if ((bool)eventDataTactical)
		{
			num2 = eventDataTactical.Events.Length;
		}
		int num3 = ((num <= num2) ? num2 : num) + 1;
		eventList = new float[num3, 2];
		int num4 = 0;
		for (num4 = 0; num4 < num; num4++)
		{
			eventList[num4, 0] = eventData.Events[num4].Time;
		}
		for (; num4 < num3; num4++)
		{
			eventList[num4, 0] = -1f;
		}
		for (num4 = 0; num4 < num2; num4++)
		{
			eventList[num4, 1] = eventDataTactical.Events[num4].Time;
		}
		for (; num4 < num3; num4++)
		{
			eventList[num4, 1] = -1f;
		}
	}

	private void OnDestroy()
	{
		sm_Instance = null;
	}
}
