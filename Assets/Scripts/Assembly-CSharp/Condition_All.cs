using System.Collections.Generic;

public class Condition_All : Condition
{
	public List<Condition> Conditions;

	private bool mCondition;

	public override bool Value()
	{
		return Conditions.TrueForAll((Condition condition) => condition.Value());
	}
}
