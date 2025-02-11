using System.Collections.Generic;

public static class OrdersQueryInterface
{
	public class OrderStack
	{
		public List<ContextMenuIcons> Orders;

		public OrderStack()
		{
			Orders = new List<ContextMenuIcons>();
		}
	}

	public static OrderStack GetCurrentOrders(Actor unit)
	{
		OrderStack orderStack = new OrderStack();
		GetCurrentOrdersFromTaskStack(orderStack, unit.tasks.LongTerm.Stack);
		GetCurrentOrdersFromTaskStack(orderStack, unit.tasks.Immediate.Stack);
		GetCurrentOrdersFromTaskStack(orderStack, unit.tasks.Reactive.Stack);
		GetNotifcationFromAlliedOrders(orderStack, unit);
		return orderStack;
	}

	private static void GetCurrentOrdersFromTaskStack(OrderStack orderStack, List<Task> taskStack)
	{
		for (int i = 0; i < taskStack.Count; i++)
		{
			CheckAndAddOrder(orderStack, taskStack[i]);
		}
	}

	private static void CheckAndAddOrder(OrderStack orderStack, Task task)
	{
		if ((task.ConfigFlags & Task.Config.IssuedByPlayerRequest) == 0)
		{
			return;
		}
		if (task.GetType() == typeof(TaskBreach))
		{
			orderStack.Orders.Add(ContextMenuIcons.Breach);
		}
		else if (task.GetType() == typeof(TaskCarry))
		{
			TaskCarry taskCarry = (TaskCarry)task;
			if (taskCarry.IsDropping())
			{
				orderStack.Orders.Add(ContextMenuIcons.DropBody);
			}
			else
			{
				orderStack.Orders.Add(ContextMenuIcons.PickupBody);
			}
		}
		else if (task.GetType() == typeof(TaskExit))
		{
			orderStack.Orders.Add(ContextMenuIcons.ExitHide);
		}
		else if (task.GetType() == typeof(TaskThrowGrenade))
		{
			orderStack.Orders.Add(ContextMenuIcons.Grenade);
		}
		else if (task.GetType() == typeof(TaskHack))
		{
			orderStack.Orders.Add(ContextMenuIcons.Hack);
		}
		else if (task.GetType() == typeof(TaskEnter))
		{
			orderStack.Orders.Add(ContextMenuIcons.Hide);
		}
		else if (task.GetType() == typeof(TaskStealthKill))
		{
			orderStack.Orders.Add(ContextMenuIcons.Melee);
		}
		else if (task.GetType() == typeof(TaskOpen))
		{
			orderStack.Orders.Add(ContextMenuIcons.OpenDoor);
		}
		else if (task.GetType() == typeof(TaskPeekaboo))
		{
			orderStack.Orders.Add(ContextMenuIcons.Peek);
		}
		else if (task.GetType() == typeof(TaskShoot))
		{
			orderStack.Orders.Add(ContextMenuIcons.Shoot);
		}
		else if (task.GetType() == typeof(TaskMoveToCover))
		{
			orderStack.Orders.Add(ContextMenuIcons.TakeCover);
		}
		else if (task.GetType() == typeof(TaskClose))
		{
			orderStack.Orders.Add(ContextMenuIcons.CloseDoor);
		}
		else if (task.GetType() == typeof(TaskSetPiece))
		{
			orderStack.Orders.Add(ContextMenuIcons.TwoManMantle);
		}
		else
		{
			orderStack.Orders.Add(ContextMenuIcons.Halt);
		}
	}

	private static void GetNotifcationFromAlliedOrders(OrderStack orderStack, Actor unit)
	{
		bool flag = false;
		bool flag2 = false;
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.PlayerControlledMask & GKM.AliveMask & ~unit.ident);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			TaskDefend runningTask = a.tasks.GetRunningTask<TaskDefend>();
			if (runningTask != null && runningTask.Target == unit)
			{
				flag = true;
			}
			TaskFollow runningTask2 = a.tasks.GetRunningTask<TaskFollow>();
			if (runningTask2 != null && runningTask2.Target == unit)
			{
				flag2 = true;
			}
			if (flag && flag2)
			{
				break;
			}
		}
		if (flag)
		{
			orderStack.Orders.Add(ContextMenuIcons.FirstPerson);
		}
		if (flag2)
		{
			orderStack.Orders.Add(ContextMenuIcons.ExitFirstPerson);
		}
	}
}
