using UnityEngine;

public class DoorZoneDescriptor : ScriptableObject
{
	public SetPieceModule SP_OpenTowardsLeaveAjar;

	public SetPieceModule SP_OpenAwayLeaveAjar;

	public SetPieceModule SP_OpenTowardsAndClose;

	public SetPieceModule SP_OpenAwayAndClose;

	public SetPieceModule SP_WalkThroughAndCloseTowards;

	public SetPieceModule SP_WalkThroughAndCloseAway;

	public SetPieceModule SP_FlingOpenTowards;

	public SetPieceModule SP_BurstThrough;

	public SetPieceModule PlayerOpenTowardsNormal;

	public SetPieceModule PlayerOpenTowardsNormal_FPP;

	public SetPieceModule PlayerOpenAwayNormal;

	public SetPieceModule PlayerOpenAwayNormal_FPP;

	public SetPieceModule PlayerOpenTowardsStealth;

	public SetPieceModule PlayerOpenAwayStealth;

	public Vector2 delay_OpenTowardsLeaveAjar;

	public Vector2 delay_OpenAwayLeaveAjar;

	public Vector2 delay_OpenTowardsAndClose;

	public Vector2 delay_OpenAwayAndClose;

	public Vector2 delay_WalkThroughAndCloseTowards;

	public Vector2 delay_WalkThroughAndCloseAway;

	public Vector2 delay_FlingOpenTowards;

	public Vector2 delay_BurstThrough;

	public Vector2 delay_PlayerOpenTowardsNormal;

	public Vector2 delay_PlayerOpenAwayNormal;

	public Vector2 delay_PlayerOpenTowardsStealth;

	public Vector2 delay_PlayerOpenAwayStealth;

	public Vector2 offset_OpenTowardsLeaveAjar;

	public Vector2 offset_OpenAwayLeaveAjar;

	public Vector2 offset_OpenTowardsAndClose;

	public Vector2 offset_OpenAwayAndClose;

	public Vector2 offset_WalkThroughAndCloseTowards;

	public Vector2 offset_WalkThroughAndCloseAway;

	public Vector2 offset_FlingOpenTowards;

	public Vector2 offset_BurstThrough;

	public Vector2 offset_PlayerOpenTowardsNormal;

	public Vector2 offset_PlayerOpenAwayNormal;

	public Vector2 offset_PlayerOpenTowardsStealth;

	public Vector2 offset_PlayerOpenAwayStealth;

	public Vector2 delay_WalkThroughTowardsLeaveAjar;

	public Vector2 delay_WalkThroughAwayLeaveAjar;
}
