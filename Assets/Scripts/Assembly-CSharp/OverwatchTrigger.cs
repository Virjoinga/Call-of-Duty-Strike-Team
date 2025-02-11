using UnityEngine;

public class OverwatchTrigger : MonoBehaviour
{
	public OverwatchData m_Interface;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Activate()
	{
		if (!(CommonHudController.Instance == null))
		{
			GameController instance = GameController.Instance;
			OverwatchController overwatchController = instance.GetOverwatchController();
			overwatchController.mOnFinishObj = m_Interface.ObjectToCallOnComplete;
			overwatchController.mOnFinishMsg = m_Interface.FunctionToCallOnComplete;
			overwatchController.mGroupObjectToCall = m_Interface.GroupObjectToCall;
			overwatchController.mGroupFunctionToCall = m_Interface.GroupFunctionToCall;
			overwatchController.TimeoutOverwatch = m_Interface.TimeoutOverwatch;
			overwatchController.Duration = m_Interface.DurationSeconds;
			overwatchController.TargetSpread = m_Interface.TargetSpread;
			overwatchController.CharacterMovementSpeedMultiplier = m_Interface.CharacterMovementSpeedMultiplier;
			overwatchController.RocketReloadTime = m_Interface.RocketReloadTime;
			overwatchController.Origin = m_Interface.Origin;
			if (m_Interface.Waypoints != null && m_Interface.Waypoints.Count > 0)
			{
				overwatchController.OverwatchLC.Waypoints = m_Interface.Waypoints;
			}
			overwatchController.OverwatchLC.FocusPoint = m_Interface.FocusPoint;
			overwatchController.OverwatchLC.DistanceFromCentre = m_Interface.DistanceFromCentre;
			overwatchController.OverwatchLC.Height = m_Interface.Height;
			overwatchController.OverwatchLC.RotateSpeed = m_Interface.RotateSpeed;
			overwatchController.OverwatchLC.TranslateSpeed = m_Interface.TranslateSpeed;
			overwatchController.OverwatchLC.PanningSensitivity = m_Interface.PanningSensitivity;
			overwatchController.OverwatchLC.FovDefault = m_Interface.FovDefault;
			overwatchController.OverwatchLC.MaxLookRadius = m_Interface.MaxLookRadius;
			overwatchController.OverwatchLC.TargetRecentreRate = m_Interface.TargetRecentreRate;
			overwatchController.Active = true;
			SoundManager.Instance.ActivateOverwatchSFX();
			MusicManager.Instance.PlayHighDramaThemeMusic();
			instance.SwitchToOverwatch();
			StrategyHudController.Instance.Look.gameObject.SetActive(true);
		}
	}

	public void Deactivate()
	{
		SoundManager.Instance.DeactivateOverwatchSFX(false);
		GameController instance = GameController.Instance;
		OverwatchController overwatchController = instance.GetOverwatchController();
		StrategyHudController.Instance.Look.gameObject.SetActive(false);
		overwatchController.Duration = 0f;
	}

	public void OnDestroy()
	{
		SoundManager.Instance.DeactivateOverwatchSFX(true);
	}
}
