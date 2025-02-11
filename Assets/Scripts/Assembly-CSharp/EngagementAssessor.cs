public class EngagementAssessor
{
	private Actor mOwner;

	private Actor mTarget;

	private float mStartHealthTarget;

	private float mStartHealthOwner;

	public Actor Target
	{
		get
		{
			return mTarget;
		}
	}

	public EngagementAssessor(Actor owner, Actor target)
	{
		mOwner = owner;
		mTarget = target;
		Reset();
	}

	public bool IsWinning()
	{
		float health = mTarget.health.Health;
		float num = mStartHealthTarget - health;
		float health2 = mOwner.health.Health;
		float num2 = mStartHealthOwner - health2;
		if (num2 > num)
		{
			return false;
		}
		return true;
	}

	public void Reset()
	{
		mStartHealthTarget = mTarget.health.Health;
		mStartHealthOwner = mOwner.health.Health;
	}
}
