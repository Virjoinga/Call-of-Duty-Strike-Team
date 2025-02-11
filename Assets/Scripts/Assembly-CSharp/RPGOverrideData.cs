using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RPGOverrideData
{
	public float WaitInbetweenShots = 7f;

	public float Inaccuracy = 6f;

	public float AccuracyIncrease = 1f;

	public float DamageMultiplier = 6f;

	public List<RPGMoveCommand> MoveCommands = new List<RPGMoveCommand>();

	public bool LoopMove;

	public GameObject Target;

	public TargetSwitchComponent TargetSwitcher;

	public GameObject SpawnTarget;

	public float TimeToWaitAfterSpawn = 1.5f;

	public bool AimAtGround = true;
}
