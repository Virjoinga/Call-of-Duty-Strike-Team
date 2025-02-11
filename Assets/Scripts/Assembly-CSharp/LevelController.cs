using UnityEngine;

public class LevelController : MonoBehaviour
{
	private void Awake()
	{
		base.gameObject.AddComponent<PerFramePrep>();
		base.gameObject.AddComponent<CinematicHelper>();
		base.gameObject.AddComponent<GlobalKnowledgeManager>();
		base.gameObject.AddComponent<AuditoryAwarenessManager>();
		base.gameObject.AddComponent<CoverPointManager>();
		base.gameObject.AddComponent<TargetWrapperManager>();
		base.gameObject.AddComponent<ProjectileManager>();
		base.gameObject.AddComponent<InputManager>();
		base.gameObject.AddComponent<PlayerSquadManager>();
		base.gameObject.AddComponent<NavigationZoneManager>();
		base.gameObject.AddComponent<TetheringManager>();
		base.gameObject.AddComponent<MaterialShaderHelp>();
		base.gameObject.AddComponent<TimeManager>();
		base.gameObject.AddComponent<DropZoneController>();
		base.gameObject.AddComponent<DeadBodyManager>();
		base.gameObject.AddComponent<NavGateManager>();
		base.gameObject.AddComponent<FixedGunManager>();
		base.gameObject.AddComponent<FirstPersonPenaliser>();
	}
}
