using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicHelper : MonoBehaviour
{
	private enum State
	{
		Off = 0,
		Begin = 1,
		On = 2,
		End = 3
	}

	private static State state;

	private static bool Skippable = true;

	private static bool UpdateHUD = true;

	private static bool AutoSwitchToFPP = true;

	private static bool AllowTransitions = true;

	private static bool AllowFPPAnims;

	private static bool MaintainSelection = true;

	private static bool ForceHUDOff;

	private List<PlayerInfo> mPlayerInfoCache = new List<PlayerInfo>();

	private static CinematicHelper instance;

	public static bool IsInCinematic
	{
		get
		{
			return state != State.Off;
		}
	}

	public static bool AllowFirstPersonAnims
	{
		get
		{
			return AllowFPPAnims;
		}
	}

	public static CinematicHelper Instance()
	{
		return instance;
	}

	public static void Begin(bool skippable, bool updateHUD, bool allowFPPAnims)
	{
		Skippable = skippable;
		UpdateHUD = updateHUD;
		AllowFPPAnims = allowFPPAnims;
		if (state == State.Off)
		{
			state = State.Begin;
			Instance()._Begin();
		}
		if (state == State.End)
		{
			state = State.On;
		}
	}

	public static void End(bool updateHUD, bool autoSwitchToFPP, bool allowTransitions, bool maintainSelection, bool forceHUDOff)
	{
		if (state == State.On)
		{
			state = State.End;
		}
		if (state == State.Begin)
		{
			state = State.Off;
		}
		UpdateHUD = updateHUD;
		AutoSwitchToFPP = autoSwitchToFPP;
		AllowTransitions = allowTransitions;
		MaintainSelection = maintainSelection;
		ForceHUDOff = forceHUDOff;
	}

	private void OnDestroy()
	{
		state = State.Off;
		instance = null;
	}

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (state == State.Begin)
		{
			_Begin();
		}
		if (state == State.End)
		{
			StartCoroutine(_End());
		}
	}

	private void _Begin()
	{
		CommonHudController.Instance.ClearContextMenu();
		WaypointMarkerManager.Instance.DisableRendering();
		if (UpdateHUD)
		{
			HudBlipIcon.SwitchOffForCutscene();
			GameController.Instance.SuppressHud(true);
		}
		BlackBarsController.Instance.SetBlackBars(true, Skippable);
		InputManager.Instance.SetForCutscene();
		SoundManager.Instance.SetVolumeGroup(SoundFXData.VolumeGroup.Cutscene, 1f);
		SoundManager.Instance.SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 0.5f);
		if ((bool)GameController.Instance)
		{
			Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
			if (mFirstPersonActor != null && mFirstPersonActor.realCharacter != null)
			{
				mFirstPersonActor.realCharacter.IsAimingDownSights = false;
			}
		}
		if (CommonHudController.Instance.TriggerPressed)
		{
			CommonHudController.Instance.SetTriggerPressed(false);
		}
		if (UpdateHUD)
		{
			GameController.Instance.HideHUDElements();
		}
		SetPlayersForCutscene(true);
		AmmoDropManager.Instance.ShowHideClips(false);
		state = State.On;
		SoundManager.Instance.CinematicStarted();
	}

	private IEnumerator _End()
	{
		state = State.Off;
		BlackBarsController.Instance.SetBlackBars(false, Skippable);
		SetPlayersForCutscene(false);
		yield return new WaitForSeconds(1f);
		AmmoDropManager.Instance.ShowHideClips(true);
		InputManager.Instance.SetForGamplay();
		if (UpdateHUD)
		{
			GameController.Instance.SuppressHud(ForceHUDOff);
			HudBlipIcon.SwitchOnAfterCutscene();
		}
		SoundManager.Instance.SetVolumeGroup(SoundFXData.VolumeGroup.Sfx, 1f);
		GameController game = GameController.Instance;
		MissionSetup setup = MissionSetup.Instance;
		if (AutoSwitchToFPP && game != null && setup != null && setup.DefaultCameraIsFirstPerson && game.GetSquadLeader != null && game.GetSquadLeader.realCharacter.CanGoFirstPerson())
		{
			game.SwitchToFirstPerson(game.GetSquadLeader, AllowTransitions);
		}
		WaypointMarkerManager.Instance.EnableRendering();
	}

	private void SetPlayersForCutscene(bool intoCutscene)
	{
		if (intoCutscene)
		{
			mPlayerInfoCache.Clear();
		}
		GameplayController gameplayController = GameplayController.Instance();
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask & GKM.AliveMask);
		int num = 0;
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (intoCutscene)
			{
				PlayerInfo playerInfo = new PlayerInfo();
				playerInfo.Invulnerable = a.health.Invulnerable;
				playerInfo.InvulnerableToExplosions = a.health.InvulnerableToExplosions;
				if (gameplayController != null)
				{
					playerInfo.Selected = gameplayController.IsSelected(a);
					playerInfo.Selectable = false;
					if (a.realCharacter != null)
					{
						playerInfo.Selectable = a.realCharacter.Selectable;
					}
				}
				mPlayerInfoCache.Add(playerInfo);
				CharacterPropertyModifier.SetPlayerControl(a, base.gameObject, false);
				a.health.Invulnerable = true;
				a.health.InvulnerableToExplosions = true;
			}
			else if (mPlayerInfoCache != null && num < mPlayerInfoCache.Count)
			{
				PlayerInfo playerInfo2 = mPlayerInfoCache[num];
				a.health.Invulnerable = playerInfo2.Invulnerable;
				a.health.InvulnerableToExplosions = playerInfo2.InvulnerableToExplosions;
				if (MaintainSelection)
				{
					CharacterPropertyModifier.SetPlayerControl(a, base.gameObject, true);
					if (playerInfo2.Selectable && gameplayController != null)
					{
						if (playerInfo2.Selected)
						{
							gameplayController.AddToSelected(a);
						}
						else
						{
							gameplayController.RemoveFromSelected(a);
						}
					}
				}
			}
			num++;
		}
		GameController gameController = GameController.Instance;
		if (gameplayController != null && gameController != null && gameplayController.IsSelectionEmpty() && gameController.IsFirstPerson)
		{
			gameplayController.AddToSelected(gameController.mFirstPersonActor);
		}
	}
}
