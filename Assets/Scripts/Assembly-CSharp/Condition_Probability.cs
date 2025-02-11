using UnityEngine;

public class Condition_Probability : Condition
{
	public float Probability;

	public override bool Value()
	{
		return Random.value < Probability;
	}
}
