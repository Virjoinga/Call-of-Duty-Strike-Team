using System.Collections.Generic;
using UnityEngine;

internal class OrderedEventList<T>
{
	public List<OrderedEvent<T>> mList;

	public OrderedEventList()
	{
		mList = new List<OrderedEvent<T>>();
	}

	private OrderedEvent<T> Find(T func)
	{
		foreach (OrderedEvent<T> m in mList)
		{
			if (func.Equals(m.mFunc))
			{
				return m;
			}
		}
		return null;
	}

	public void AddEvent(int priority, T func)
	{
		OrderedEvent<T> orderedEvent = Find(func);
		if (orderedEvent != null)
		{
			Debug.LogWarning("event already added, just changing priority - you should fix this");
			orderedEvent.mPriotiry = priority;
		}
		else
		{
			mList.Add(new OrderedEvent<T>(priority, func));
		}
		mList.Sort(delegate(OrderedEvent<T> x, OrderedEvent<T> y)
		{
			if (x.mPriotiry > y.mPriotiry)
			{
				return 1;
			}
			return (x.mPriotiry != y.mPriotiry) ? (-1) : 0;
		});
	}

	public void RemoveEvent(T func)
	{
		OrderedEvent<T> orderedEvent = Find(func);
		if (orderedEvent != null)
		{
			mList.Remove(orderedEvent);
		}
		else
		{
			TBFAssert.DoAssert(false, string.Concat(orderedEvent.mFunc, " not in list to remove!"));
		}
	}
}
