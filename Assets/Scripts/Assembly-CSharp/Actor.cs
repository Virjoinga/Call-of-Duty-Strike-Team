using UnityEngine;

public class Actor : MonoBehaviour
{
	public uint ident;

	public int quickIndex;

	public bool IsHidden;

	private bool onScreen;

	public AuditoryAwarenessComponent ears;

	public AwarenessComponent awareness;

	public BaseCharacter baseCharacter;

	public BehaviourController behaviour;

	public AIGunHandler aiGunHandler;

	public FireAtWillComponent fireAtWill;

	public RealCharacter realCharacter;

	public RulesSystemInterface rules;

	public GrenadeThrowerComponent grenadeThrower;

	public PoseModuleSharedData poseModuleSharedData;

	public SentryHackingComponent sentryHacker;

	public SpeechComponent speech;

	public PlayerWeapon weapon;

	public HandGestureModule gestures;

	public Tether tether;

	public AnimDirector animDirector;

	public NavMeshAgent navAgent;

	public TaskManager tasks;

	public HealthComponent health;

	public FirstThirdPersonWidget firstThirdPersonWidget;

	public CapsuleCollider Picker;

	public FPFootstepPlayer FootstepPlayer;

	public int SoldierIndex;

	public bool CanSeeHands = true;

	public GameObject model;

	public bool OnScreen
	{
		get
		{
			return onScreen;
		}
		set
		{
			onScreen = value;
			if (animDirector != null)
			{
				animDirector.OnScreen = value;
			}
		}
	}

	public bool InitiatingGameFail { get; set; }

	public bool MysteryBoxInvulnerability { get; set; }

	public PoseModuleSharedData Pose
	{
		get
		{
			return poseModuleSharedData;
		}
	}

	private void Awake()
	{
		tether = base.gameObject.AddComponent<Tether>();
		tether.myActor = this;
	}

	private void OnDestroy()
	{
		Object.Destroy(realCharacter.myActor.model.gameObject);
		if (realCharacter.Ragdoll != null)
		{
			Object.Destroy(realCharacter.Ragdoll.gameObject);
		}
		if (ident != 0 && GlobalKnowledgeManager.Instance() != null)
		{
			GlobalKnowledgeManager.Instance().RemoveActor(this);
		}
		animDirector = null;
		ears = null;
		awareness = null;
		baseCharacter = null;
		behaviour = null;
		aiGunHandler = null;
		fireAtWill = null;
		realCharacter = null;
		rules = null;
		grenadeThrower = null;
		poseModuleSharedData = null;
		sentryHacker = null;
		speech = null;
		weapon = null;
		gestures = null;
		tether = null;
	}

	public void Connect(BaseActorComponent bac)
	{
		bac.myActor = this;
		ears = (bac as AuditoryAwarenessComponent) ?? ears;
		awareness = (bac as AwarenessComponent) ?? awareness;
		baseCharacter = (bac as BaseCharacter) ?? baseCharacter;
		behaviour = (bac as BehaviourController) ?? behaviour;
		aiGunHandler = (bac as AIGunHandler) ?? aiGunHandler;
		fireAtWill = (bac as FireAtWillComponent) ?? fireAtWill;
		realCharacter = (bac as RealCharacter) ?? realCharacter;
		rules = (bac as RulesSystemInterface) ?? rules;
		grenadeThrower = (bac as GrenadeThrowerComponent) ?? grenadeThrower;
		sentryHacker = (bac as SentryHackingComponent) ?? sentryHacker;
		speech = (bac as SpeechComponent) ?? speech;
		poseModuleSharedData = (bac as PoseModuleSharedData) ?? poseModuleSharedData;
		weapon = (bac as PlayerWeapon) ?? weapon;
		gestures = (bac as HandGestureModule) ?? gestures;
	}

	public void Command(string com)
	{
		if (baseCharacter != null)
		{
			baseCharacter.Command(com);
		}
		if (poseModuleSharedData != null)
		{
			poseModuleSharedData.Command(com);
		}
		if (tasks != null)
		{
			tasks.Command(com);
		}
	}

	public Vector3 GetPosition()
	{
		if (awareness != null)
		{
			return awareness.cachedTrans.position;
		}
		return Vector3.zero;
	}

	public void SetPosition(Vector3 newPos)
	{
		if (base.transform.position != newPos)
		{
			base.transform.position = newPos;
		}
	}

	public void ShowHide(bool Show)
	{
		if (Show != IsHidden)
		{
			return;
		}
		if (model != null)
		{
			Renderer[] componentsInChildren = model.GetComponentsInChildren<Renderer>(true);
			Renderer[] array = componentsInChildren;
			foreach (Renderer renderer in array)
			{
				if (renderer != null)
				{
					renderer.enabled = Show;
				}
			}
		}
		IsHidden = !Show;
	}
}
