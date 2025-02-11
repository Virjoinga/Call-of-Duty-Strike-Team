using System;
using UnityEngine;

public class Waypoint : ScriptableObject
{
	[Flags]
	public enum Flavour
	{
		Default = 0,
		Cover = 1,
		Breach = 2
	}

	public Vector2 Position;

	public Flavour Configuration;

	public Vector2 Facing;

	public Waypoint()
	{
		Configuration = Flavour.Default;
		Facing = new Vector2(0f, 1f);
	}
}
