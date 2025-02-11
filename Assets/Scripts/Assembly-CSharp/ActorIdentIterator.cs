using System.Collections.Generic;

public class ActorIdentIterator
{
	private uint mask;

	private int current;

	public uint Mask
	{
		get
		{
			return mask;
		}
	}

	public ActorIdentIterator(uint m)
	{
		mask = m;
		current = 0;
	}

	public static List<Actor> AsList(uint m)
	{
		List<Actor> list = new List<Actor>();
		uint num = 1u;
		int num2 = 0;
		for (num2 = 0; num2 < 32; num2++)
		{
			if ((m & num) != 0)
			{
				list.Add(GlobalKnowledgeManager.Instance().GetActorFromIndex(num2));
			}
			num += num;
		}
		return list;
	}

	public void Reset()
	{
		current = 0;
	}

	public ActorIdentIterator ResetWithMask(uint m)
	{
		mask = m;
		current = 0;
		return this;
	}

	public Actor NextActor()
	{
		uint num = (uint)(1 << current);
		while ((mask & num) == 0)
		{
			current++;
			num <<= 1;
			if (current > 32)
			{
				return null;
			}
		}
		current++;
		return GlobalKnowledgeManager.Instance().GetActorFromIndex(current - 1);
	}

	public bool NextActor(out Actor a)
	{
		a = null;
		if (current >= 32)
		{
			return false;
		}
		if (mask == 0)
		{
			current = 32;
			return false;
		}
		uint num = (uint)(1 << current);
		while ((mask & num) == 0)
		{
			current++;
			num <<= 1;
			if (current > 32)
			{
				return false;
			}
		}
		current++;
		a = GlobalKnowledgeManager.Instance().GetActorFromIndex(current - 1);
		return true;
	}
}
