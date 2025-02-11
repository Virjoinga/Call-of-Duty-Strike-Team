using System;
using System.Collections.Generic;
using UnityEngine;

public class HackableObjectClaymore : HackableObject
{
	public SetPieceModule SetPiece;

	public SetPieceModule SetPiece_FPP;

	public HitBoxDescriptor HitBoxRig;

	private float mRange = 0.25f;

	private float BeamHeight;

	private float mDamageRadius = 7f;

	public float DetonationTime;

	private float mTriggerTime;

	private bool mTriggered;

	private float mDetonationDistance = 4f;

	public GameObject ClaymoreModel;

	private GameObject mModel;

	private LaserScanLineEffect mLaserEffect;

	private FactionHelper.Category mPreviousFaction = FactionHelper.Category.Neutral;

	private Vector3 mInternalForward;

	private Actor mActorWhoPlacedThis;

	private bool mHasFinished;

	public static List<HackableObjectClaymore> GlobalPoolCache = new List<HackableObjectClaymore>();

	private HealthComponent mHealth;

	public FactionHelper.Category TeamFaction { get; private set; }

	private void Awake()
	{
		GlobalPoolCache.Add(this);
		ShouldUseConsultant = true;
		SetupHitBox();
	}

	private void OnDestroy()
	{
		if (mHealth != null)
		{
			mHealth.OnHealthEmpty -= OnHealthEmpty;
		}
		GlobalPoolCache.Remove(this);
	}

	protected override void Consult()
	{
		if (base.FullyHacked && !mHasFinished)
		{
			mHasFinished = true;
			TeamFaction = FactionHelper.Category.Player;
			SetFactionColor();
		}
	}

	public override void Update()
	{
		base.Update();
		if (!mTriggered)
		{
			return;
		}
		mTriggerTime += Time.deltaTime;
		if (mTriggerTime >= DetonationTime)
		{
			if (!base.FullyHacked)
			{
				FailHackAttempt(true, true);
			}
			Vector3 position = base.transform.position;
			position = mModel.transform.position + mInternalForward * (mDetonationDistance * 0.5f);
			Vector3 collision;
			if (!WorldHelper.IsClearTrace(base.transform.position, position, out collision))
			{
				position = collision - mInternalForward * 0.1f;
			}
			bool ignoreFriendlies = !(ActStructure.Instance != null) || ActStructure.Instance.CurrentMissionMode != DifficultyMode.Veteran;
			RulesSystem.DoAreaOfEffectDamage(position, mDamageRadius, 1f, (!(mActorWhoPlacedThis != null)) ? null : mActorWhoPlacedThis.gameObject, ExplosionManager.ExplosionType.Claymore, "Claymore", ignoreFriendlies);
			BroadcastNoise();
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void PlaceClaymore(GameObject model, Actor placingActor, Vector3 position, Vector3 forward, GameObject parentToStore)
	{
		if (!(placingActor == null))
		{
			mModel = null;
			mActorWhoPlacedThis = placingActor;
			InstantiateModel();
			SetupModelInstance(position, forward, mActorWhoPlacedThis.awareness.faction);
			InterfaceableObject componentInChildren = GetComponentInChildren<InterfaceableObject>();
			if (componentInChildren != null)
			{
				componentInChildren.Deactivate();
			}
		}
	}

	public void PlaceEnemyClaymore(Actor placingActor, Vector3 position, Vector3 forward)
	{
		mActorWhoPlacedThis = placingActor;
		InstantiateModel();
		SetupModelInstance(position, forward, FactionHelper.Category.Enemy);
	}

	public void OnTriggerStay(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (component == null || (component.realCharacter != null && component.realCharacter.IsDead()) || !FactionHelper.AreEnemies(component, TeamFaction) || component.tasks.IsRunningTask<TaskSetPiece>() || (mModel.transform.position - component.transform.position).magnitude > mDetonationDistance)
		{
			return;
		}
		Vector3 rhs = component.transform.position - mModel.transform.position;
		rhs.Normalize();
		float num = Vector3.Dot(mInternalForward, rhs);
		if (num < mRange)
		{
			return;
		}
		Vector3 position = mModel.transform.position;
		Vector3 position2 = component.transform.position;
		RaycastHit hitInfo;
		if (Physics.Linecast(position, position2, out hitInfo, 1 << LayerMask.NameToLayer("Default")))
		{
			float magnitude = (position - position2).magnitude;
			if (hitInfo.distance != magnitude)
			{
				return;
			}
		}
		Trigger();
	}

	private void BroadcastNoise()
	{
		ExplosionManager.BroadcastNoise(base.transform.position, mActorWhoPlacedThis);
	}

	private void InstantiateModel()
	{
		if (mModel == null && ClaymoreModel != null)
		{
			mModel = UnityEngine.Object.Instantiate(ClaymoreModel) as GameObject;
			if (mModel != null)
			{
				mModel.transform.parent = base.transform;
				Transform transform = new GameObject("ProbeAnchor").transform;
				transform.parent = mModel.transform;
				transform.localPosition = 0.5f * Vector3.up;
				StaticLighting staticLighting = mModel.AddComponent<StaticLighting>();
				staticLighting.GroupRoots = new Transform[1] { mModel.transform };
				staticLighting.ProbeAnchor = transform;
			}
		}
	}

	private void SetupModelInstance(Vector3 position, Vector3 forward, FactionHelper.Category faction)
	{
		base.transform.position = position;
		SetFacing(forward);
		if (mModel != null)
		{
			Transform transform = mModel.transform;
			transform.position = position;
			transform.localRotation = Quaternion.identity;
			transform.right = mInternalForward;
		}
		mPreviousFaction = FactionHelper.Category.Neutral;
		TeamFaction = faction;
		CreateScanEffect();
		SetFactionColor();
	}

	private void SetFactionColor()
	{
		if (TeamFaction != mPreviousFaction && !(mLaserEffect == null))
		{
			mPreviousFaction = TeamFaction;
			if (mPreviousFaction == FactionHelper.Category.Player)
			{
				mLaserEffect.SetLaserColour(ColourChart.FriendlyBlip);
			}
			else
			{
				mLaserEffect.SetLaserColour(ColourChart.EnemyBlip);
			}
		}
	}

	private void CreateScanEffect()
	{
		if (!(mLaserEffect != null))
		{
			float num = Mathf.Acos(mRange);
			float f = num * 0.5f;
			float num2 = Mathf.Cos(f);
			float num3 = Mathf.Sin(f);
			Vector3 vector = new Vector3(mInternalForward.x * num2 - mInternalForward.z * num3, 0f, mInternalForward.x * num3 + mInternalForward.z * num2);
			vector *= mDetonationDistance;
			GameObject gameObject = new GameObject();
			if (gameObject != null)
			{
				gameObject.name = "ScanLineEffectObject";
				gameObject.transform.parent = mModel.transform;
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.forward = mInternalForward;
			}
			mLaserEffect = (LaserScanLineEffect)gameObject.AddComponent(typeof(LaserScanLineEffect));
			if (mLaserEffect != null)
			{
				mLaserEffect.Setup(vector.x, mRange, new Vector3(0f, 0.25f, 0f), false);
				mLaserEffect.AllowInFirstPerson(true);
			}
		}
	}

	public void SetFacing(Vector3 facing)
	{
		mInternalForward = facing;
		if (mModel != null)
		{
			mModel.transform.right = mInternalForward;
		}
	}

	public bool CanBeDefusedBy(Actor actor)
	{
		return FactionHelper.AreEnemies(actor, TeamFaction);
	}

	public void Trigger()
	{
		mTriggerTime = 0f;
		mTriggered = true;
	}

	private void SetupHitBox()
	{
		if (HitBoxRig == null)
		{
			return;
		}
		mHealth = base.gameObject.AddComponent<HealthComponent>();
		mHealth.OnHealthEmpty += OnHealthEmpty;
		GameObject gameObject = base.transform.gameObject;
		List<HitLocation> list = new List<HitLocation>();
		foreach (HitBoxDescriptor.HitBox hitBox in HitBoxRig.HitBoxes)
		{
			HitLocation hitLocation = HitBoxUtils.CreateHitLocation(gameObject, hitBox);
			hitLocation.transform.parent = gameObject.transform;
			hitLocation.Owner = gameObject;
			hitLocation.Health = mHealth;
			list.Add(hitLocation);
		}
		foreach (HitLocation item in list)
		{
			Rigidbody rigidbody = item.gameObject.GetComponent<Rigidbody>() ?? item.gameObject.AddComponent<Rigidbody>();
			rigidbody.isKinematic = true;
			rigidbody.freezeRotation = true;
			rigidbody.mass = item.Mass;
		}
	}

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		Trigger();
	}
}
