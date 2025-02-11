using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour
{
	public HitBoxDescriptor HitBoxRig;

	public GameObject Model;

	public float Health = 5000f;

	public TankRoutine Routine;

	public HudBlipIcon HudMarker;

	public static List<Tank> GlobalPoolCache = new List<Tank>();

	private static float MAX_SPEED = 3f;

	private CharacterLighting mLighting;

	private SnapTarget mSnapTarget;

	private NavMeshAgent mNavAgent;

	private HealthComponent mHealth;

	private float mExplosionTimer;

	private HudBlipIcon mHudMarker;

	private void Awake()
	{
		GlobalPoolCache.Add(this);
		SetupHitBox();
		SetupLighting();
		SetupSnapTarget();
		SetupNavMeshAgent();
		SetupBlip();
	}

	private void Start()
	{
		mExplosionTimer = 8f;
	}

	private void OnDestroy()
	{
		if (mSnapTarget != null)
		{
			Object.Destroy(mSnapTarget);
		}
		GlobalPoolCache.Remove(this);
	}

	private void Update()
	{
		if (mHealth.HealthEmpty)
		{
			if (Random.Range(0, 2) == 1)
			{
				ExplosionManager.Instance.StartExplosion(base.transform.position + Random.insideUnitSphere * 5f, 50f);
			}
			mExplosionTimer -= Time.deltaTime;
			if (mExplosionTimer <= 0f)
			{
				base.gameObject.SetActive(false);
				if (mSnapTarget != null)
				{
					Object.Destroy(mSnapTarget);
				}
			}
		}
		else
		{
			Move();
		}
	}

	private void LateUpdate()
	{
		mLighting.UpdateMaterials(true);
	}

	private void SetupHitBox()
	{
		if (HitBoxRig == null)
		{
			return;
		}
		SetupHealth();
		List<HitLocation> list = new List<HitLocation>();
		foreach (HitBoxDescriptor.HitBox hitBox in HitBoxRig.HitBoxes)
		{
			HitLocation hitLocation = HitBoxUtils.CreateHitLocation(Model, hitBox);
			hitLocation.transform.parent = Model.transform;
			hitLocation.Owner = Model;
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

	private void SetupHealth()
	{
		mHealth = base.gameObject.AddComponent<HealthComponent>();
		mHealth.Initialise(0f, Health, Health);
	}

	private void SetupLighting()
	{
		CharacterLighting characterLighting = Model.AddComponent<CharacterLighting>();
		characterLighting.Renderers = Model.GetComponentsInChildren<Renderer>();
		characterLighting.ProbeAnchor = Model.transform;
		mLighting = characterLighting;
	}

	private void SetupSnapTarget()
	{
		mSnapTarget = ActorGenerator.CreateStandardSnapTarget(base.transform);
		mSnapTarget.SnapPositionOverride = () => base.transform.position + Vector3.up;
	}

	private void SetupNavMeshAgent()
	{
		NavMeshAgent navMeshAgent = base.gameObject.AddComponent<NavMeshAgent>();
		navMeshAgent.angularSpeed = 90f;
		navMeshAgent.autoTraverseOffMeshLink = false;
		navMeshAgent.autoRepath = false;
		navMeshAgent.radius = 5f;
		navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
		navMeshAgent.avoidancePriority = 0;
		navMeshAgent.speed = MAX_SPEED;
		int num = 0;
		num |= 1 << (NavMesh.GetNavMeshLayerFromName("Default") & 0x1F);
		num |= 1 << (NavMesh.GetNavMeshLayerFromName("EnemyOnly") & 0x1F);
		navMeshAgent.walkableMask = num;
		mNavAgent = navMeshAgent;
	}

	private void Move()
	{
		if (Routine != null && Routine.MovingToTarget)
		{
			bool flag = mNavAgent.hasPath;
			if (flag)
			{
				flag = (mNavAgent.destination - Routine.WaypointTargetPosition).sqrMagnitude <= 1f;
			}
			if (!flag)
			{
				mNavAgent.destination = Routine.WaypointTargetPosition;
			}
		}
		float num = Vector3.Dot((mNavAgent.steeringTarget - base.transform.position).normalized, base.transform.forward);
		num += 1f;
		num *= 0.5f;
		mNavAgent.speed = num * MAX_SPEED;
	}

	private void SetupBlip()
	{
		if (!(HudMarker == null))
		{
			mHudMarker = SceneNanny.Instantiate(HudMarker) as HudBlipIcon;
			mHudMarker.Target = base.transform;
			mHudMarker.Visible = true;
		}
	}
}
