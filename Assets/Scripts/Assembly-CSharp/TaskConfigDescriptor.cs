using System;

[Serializable]
public class TaskConfigDescriptor
{
	public bool ClearAllCurrentType;

	public bool AbortOnAlert;

	public bool AbortOnVisibleEnemy;

	public bool AbortOnUnobstructedEnemy;

	public bool AbortOnUnobstructedTarget;

	public bool AbortOnAllEnemiesDead;

	public bool AbortWhenInRangeOfTarget;

	public bool AbortIfSpotted;

	public bool ClearCurrentTypeEqualOrHigher;

	public Task.Config GetAsFlags()
	{
		Task.Config config = Task.Config.Default;
		if (ClearAllCurrentType)
		{
			config |= Task.Config.ClearAllCurrentType;
		}
		if (AbortOnAlert)
		{
			config |= Task.Config.AbortOnAlert;
		}
		if (AbortOnVisibleEnemy)
		{
			config |= Task.Config.AbortOnVisibleEnemy;
		}
		if (AbortOnUnobstructedEnemy)
		{
			config |= Task.Config.AbortOnUnobstructedEnemy;
		}
		if (AbortOnUnobstructedTarget)
		{
			config |= Task.Config.AbortOnUnobstructedTarget;
		}
		if (AbortOnAllEnemiesDead)
		{
			config |= Task.Config.AbortOnAllEnemiesDead;
		}
		if (AbortWhenInRangeOfTarget)
		{
			config |= Task.Config.AbortWhenInRangeOfTarget;
		}
		if (AbortIfSpotted)
		{
			config |= Task.Config.AbortIfSpotted;
		}
		if (ClearCurrentTypeEqualOrHigher)
		{
			config |= Task.Config.ClearCurrentTypeEqualOrHigher;
		}
		return config;
	}
}
