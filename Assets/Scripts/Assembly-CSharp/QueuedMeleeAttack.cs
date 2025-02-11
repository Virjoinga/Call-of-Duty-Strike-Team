public class QueuedMeleeAttack : QueuedOrder
{
	public QueuedMeleeAttack(Actor i, Actor t)
	{
		issueTo = i;
		target = t;
	}

	public override void Execute()
	{
		OrdersHelper.ExecuteMeleeAttack(issueTo, target);
	}

	public override bool RequiresStealth()
	{
		return true;
	}
}
