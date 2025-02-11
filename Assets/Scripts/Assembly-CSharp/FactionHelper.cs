public static class FactionHelper
{
	public enum Category
	{
		Player = 0,
		Enemy = 1,
		SoloEnemy = 2,
		Neutral = 3,
		Count = 4
	}

	public static bool AreEnemies(Category first, Category second)
	{
		return AreEnemiesEval(first, second) || AreEnemiesEval(second, first);
	}

	public static bool AreEnemies(Actor first, Category second)
	{
		return (GlobalKnowledgeManager.Instance().FactionHostileTo[(int)first.awareness.faction] & (uint)(1 << (int)second)) != 0;
	}

	private static bool AreEnemiesEval(Category first, Category second)
	{
		return (GlobalKnowledgeManager.Instance().FactionHostileTo[(int)first] & (uint)(1 << (int)second)) != 0;
	}

	public static bool WillSlowToAvoidBeingHeard(Category c)
	{
		return c == Category.Player;
	}

	public static bool WillCrouchToAvoidBeingSeen(Category c)
	{
		return c == Category.Player;
	}
}
