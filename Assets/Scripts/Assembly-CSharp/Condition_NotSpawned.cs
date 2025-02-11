using UnityEngine;

public class Condition_NotSpawned : Condition
{
	private bool mSpawned;

	private void Awake()
	{
		mSpawned = false;
	}

	private void OnSpawned(GameObject spawned)
	{
		mSpawned = true;
	}

	public override bool Value()
	{
		return !mSpawned;
	}

	public void Reset()
	{
		mSpawned = false;
	}
}
