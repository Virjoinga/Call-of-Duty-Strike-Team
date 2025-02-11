using System.Collections.Generic;
using UnityEngine;

public class ActorDescriptor : ScriptableObject
{
	public const float SHORT_LOOK_DISTANCE = 10f;

	public int soldierIndex;

	public string Name = "Actor";

	public CharacterType ChctrType = CharacterType.Human;

	public ContextMenuOptionType CMRules = ContextMenuOptionType.Everything;

	public bool PlayerControlled;

	public FactionHelper.Category Faction;

	public ThemedVocalDescriptor Nationality;

	public float HealthMultiplier = 1f;

	public bool Invulnerable;

	public bool Rechargeable = true;

	public float StealthSpeed = 1.5f;

	public float WalkSpeed = 2f;

	public float SaunterSpeed = 1.2f;

	public float RunSpeed = 5f;

	public WeaponDescriptor DefaultPrimaryWeapon;

	public WeaponDescriptor DefaultSecondaryWeapon;

	public MinigunDescriptor SentryGunWeapon;

	public ThemedModelDescriptor Model;

	public Material ModelOverWatchMaterial;

	public ThemedModelDescriptor[] Attachments;

	public HudBlipIcon HudMarker;

	public bool BlipVisible = true;

	public AnimLibrary AnimationLibrary;

	private float mNavBaseOffset;

	public bool UseParentTransform = true;

	public MoveAimDescriptor MoveAimDescription;

	public NavigationSetPieceLogic NavigationSetPiece;

	public bool HasLaserSight = true;

	public float BroadcastPerceptionRange = 20f;

	public float IdealMaxBurstFireTimeMin = 0.5f;

	public float IdealMaxBurstFireTimeMax = 2f;

	public float TimeToPopUp = 0.5f;

	public bool LineOfSightEnabled = true;

	public float LineOfSightFOV = 120f;

	public float LineOfSightLookDistance = 10f;

	public Vector3 LineOfSightOffset = new Vector3(0f, 1.5f, 0f);

	public float Range = 120f;

	public float FiringRange = 100f;

	public float LineOfSightPeripheralFOV;

	public float LineOfSightSideProximityRange;

	public float LineOfSightRearProximityRange;

	public bool AuditoryAwarenessEnabled = true;

	public float AuditoryAwarenessRange = 20f;

	public HitBoxDescriptor HitBoxRig;

	public List<ScriptableObject> AdditionalDetails = new List<ScriptableObject>();

	public float NavBaseOffset
	{
		get
		{
			return mNavBaseOffset;
		}
	}

	public float ExtraHealthPoints { get; set; }

	public bool ForceKeepAlive { get; set; }

	public bool InvulernableToExplosions { get; set; }
}
