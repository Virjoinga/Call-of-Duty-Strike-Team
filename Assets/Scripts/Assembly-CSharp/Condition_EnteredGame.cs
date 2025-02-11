using UnityEngine;

public class Condition_EnteredGame : Condition
{
	private bool mSpawned;

	public override bool Value()
	{
		return !mSpawned && GameController.Instance != null && GameController.Instance.GameplayStarted;
	}

	private void OnSpawned(GameObject spawned)
	{
		mSpawned = true;
	}

	public void Reset()
	{
		mSpawned = false;
	}
}
