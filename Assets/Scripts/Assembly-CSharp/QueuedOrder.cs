using System.Collections.Generic;

public abstract class QueuedOrder
{
	protected Actor target;

	protected bool boolParam;

	protected Actor issueTo;

	private static List<QueuedOrder> queuedOrders = new List<QueuedOrder>();

	protected QueuedOrder()
	{
		queuedOrders.Add(this);
	}

	public abstract void Execute();

	public abstract bool RequiresStealth();

	public static Actor OrderedTarget(Actor a)
	{
		foreach (QueuedOrder queuedOrder in queuedOrders)
		{
			if (queuedOrder.issueTo == a)
			{
				return queuedOrder.target;
			}
		}
		return null;
	}

	public static bool AnyStealthyOrdersFor(Actor a)
	{
		foreach (QueuedOrder queuedOrder in queuedOrders)
		{
			if (queuedOrder.issueTo == a && queuedOrder.RequiresStealth())
			{
				return true;
			}
		}
		return false;
	}

	public static void DestroyOrders(Actor rc)
	{
		int num = 0;
		foreach (QueuedOrder queuedOrder in queuedOrders)
		{
			if (queuedOrder.issueTo == rc)
			{
				queuedOrders.RemoveAt(num);
				break;
			}
			num++;
		}
	}

	public static void ExecuteOrders()
	{
		TaskManager.OpenSyncBracket();
		foreach (QueuedOrder queuedOrder in queuedOrders)
		{
			queuedOrder.Execute();
		}
		TaskManager.CloseSyncBracket();
		queuedOrders.Clear();
	}

	public static void ExecuteOrders(Actor a)
	{
		int num = 0;
		foreach (QueuedOrder queuedOrder in queuedOrders)
		{
			if (queuedOrder.issueTo == a)
			{
				queuedOrder.Execute();
				queuedOrders.RemoveAt(num);
				break;
			}
			num++;
		}
	}

	public static void ClearOrders()
	{
		queuedOrders.Clear();
	}

	public static bool OrdersPending()
	{
		return queuedOrders.Count > 0;
	}
}
