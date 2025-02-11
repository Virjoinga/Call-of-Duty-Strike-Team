public class Condition_Alive : Condition
{
	public HealthComponent Target;

	private bool mCondition;

	private void Start()
	{
		mCondition = !Target.HealthEmpty;
		Target.OnHealthEmpty += delegate
		{
			mCondition = false;
		};
	}

	public override bool Value()
	{
		return mCondition;
	}
}
