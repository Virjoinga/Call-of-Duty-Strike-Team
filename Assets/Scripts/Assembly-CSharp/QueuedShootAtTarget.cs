public class QueuedShootAtTarget : QueuedOrder
{
	public QueuedShootAtTarget(Actor i, Actor t, bool isSuppression)
	{
		issueTo = i;
		target = t;
		boolParam = isSuppression;
	}

	public override void Execute()
	{
		if (GameplayController.Instance().CanAimedShot(issueTo, target))
		{
			OrdersHelper.ExecuteShootAtTarget(issueTo, target, boolParam, true);
		}
	}

	public override bool RequiresStealth()
	{
		return false;
	}
}
