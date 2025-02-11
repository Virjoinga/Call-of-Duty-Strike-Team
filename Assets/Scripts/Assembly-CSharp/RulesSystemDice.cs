using UnityEngine;

public static class RulesSystemDice
{
	public static bool Roll(int threshold, int size)
	{
		int num = Random.Range(0, size) + 1;
		if (num <= threshold)
		{
			RulesSystem.Log(string.Format("   Roll threshold={0}. Rolled a {1} on a d{2}. Success!", threshold, num, size));
			return true;
		}
		RulesSystem.Log(string.Format("   Roll threshold={0}. Rolled a {1} on a d{2}. Fail.", threshold, num, size));
		return false;
	}

	public static float Delta()
	{
		return Random.Range(0f, 1f);
	}
}
