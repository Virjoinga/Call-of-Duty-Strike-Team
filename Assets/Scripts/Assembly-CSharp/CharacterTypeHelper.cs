public static class CharacterTypeHelper
{
	public static bool IsSpecialCharacter(CharacterType type)
	{
		return type == CharacterType.RiotShieldNPC || type == CharacterType.RPG;
	}

	public static bool RequiresFireAtWillComponent(CharacterType type)
	{
		return type != CharacterType.AutonomousGroundRobot && !IsSpecialCharacter(type);
	}

	public static bool RequiresGrenadeThrowerComponent(CharacterType type)
	{
		return type != CharacterType.AutonomousGroundRobot;
	}

	public static bool RequiresSentryHackingComponent(CharacterType type)
	{
		return type != CharacterType.AutonomousGroundRobot;
	}

	public static bool RequiresShootReloadActionHandles(CharacterType type)
	{
		return type != CharacterType.AutonomousGroundRobot && !IsSpecialCharacter(type);
	}

	public static bool IsAllowedToOpenDoors(CharacterType type)
	{
		return type != CharacterType.AutonomousGroundRobot && !IsSpecialCharacter(type);
	}

	public static bool IsAimDirectionLockedToFacing(CharacterType type)
	{
		return type == CharacterType.AutonomousGroundRobot || IsSpecialCharacter(type);
	}

	public static bool DoPosePostModuleUpdate(CharacterType type)
	{
		return type != CharacterType.AutonomousGroundRobot;
	}

	public static bool ReleaseTriggerEveryFrame(CharacterType type)
	{
		return type != CharacterType.SentryGun && !IsSpecialCharacter(type);
	}

	public static bool CanRagdoll(CharacterType type)
	{
		return type == CharacterType.Human || IsSpecialCharacter(type);
	}

	public static bool CanMantle(CharacterType type)
	{
		return type == CharacterType.Human;
	}

	public static bool CanFlinch(CharacterType type)
	{
		return type == CharacterType.Human;
	}
}
