public class Condition_Dead : Condition
{
	public HealthComponent Target;

	private bool mCondition;

	private void Start()
	{
		mCondition = Target.HealthEmpty;
		Target.OnHealthEmpty += delegate
		{
			mCondition = true;
		};
	}

	public override bool Value()
	{
		return mCondition;
	}
}
