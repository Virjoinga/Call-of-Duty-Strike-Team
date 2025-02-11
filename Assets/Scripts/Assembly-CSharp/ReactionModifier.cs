using System;

[Serializable]
public class ReactionModifier
{
	public enum RandomPreset
	{
		FallForward = 0,
		ShotInChestFront = 1,
		Grenade = 2,
		LargeBlast = 3,
		None = 4
	}

	public bool ApplyReactionOnDeath;

	public RandomPreset RandomReaction = RandomPreset.None;

	public float ImpactHeight = 3f;

	public float ForwardModifier = 1f;

	public float RightModifier;

	public float Upforce = 2f;

	public float ImpactForce = 30f;
}
