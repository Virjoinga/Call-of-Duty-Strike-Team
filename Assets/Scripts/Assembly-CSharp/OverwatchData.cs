using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OverwatchData
{
	public bool TimeoutOverwatch = true;

	public float DurationSeconds = 10f;

	public float TargetSpread = 1f;

	public float CharacterMovementSpeedMultiplier = 0.5f;

	public float RocketReloadTime = 3f;

	public List<Transform> Waypoints;

	public Transform FocusPoint;

	public GameObject Origin;

	public float DistanceFromCentre = 200f;

	public float Height = 100f;

	public float RotateSpeed = 5f;

	public float TranslateSpeed = 2f;

	public float PanningSensitivity = 10f;

	public float FovDefault = 45f;

	public float MaxLookRadius = 30f;

	public float TargetRecentreRate = 0.1f;

	public GameObject ObjectToCallOnComplete;

	public string FunctionToCallOnComplete;

	public List<GameObject> GroupObjectToCall;

	public List<string> GroupFunctionToCall;
}
