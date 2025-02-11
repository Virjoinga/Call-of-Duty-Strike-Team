using System.Collections.Generic;
using UnityEngine;

public static class ActorAccessor
{
	public enum ActorTypes
	{
		AllAI = 0,
		PlayerEnemy = 1,
		PlayerFriendly = 2,
		Player = 3,
		FriendlyHuman = 4
	}

	private static ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	public static List<Actor> GetActorsOfType(ActorTypes type, Collider col, bool useOutsideCol, string tag)
	{
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(0u);
		switch (type)
		{
		case ActorTypes.AllAI:
			actorIdentIterator.ResetWithMask(GKM.AIMask);
			break;
		case ActorTypes.PlayerEnemy:
			actorIdentIterator.ResetWithMask(GKM.EnemiesMask(0));
			break;
		case ActorTypes.PlayerFriendly:
			actorIdentIterator.ResetWithMask(GKM.FriendsMask(0) & ~GKM.PlayerControlledMask);
			break;
		case ActorTypes.Player:
			actorIdentIterator.ResetWithMask(GKM.PlayerControlledMask);
			break;
		case ActorTypes.FriendlyHuman:
			actorIdentIterator.ResetWithMask(GKM.FriendsMask(0) & GKM.CharacterTypeMask(CharacterType.Human) & ~GKM.PlayerControlledMask);
			break;
		}
		Actor a;
		if (col != null && actorIdentIterator.Mask != 0)
		{
			uint num = 0u;
			while (actorIdentIterator.NextActor(out a))
			{
				Collider componentInChildren = a.gameObject.GetComponentInChildren<Collider>();
				if (componentInChildren != null)
				{
					if (col.bounds.Intersects(componentInChildren.bounds))
					{
						if (!useOutsideCol)
						{
							num |= a.ident;
						}
					}
					else if (useOutsideCol)
					{
						num |= a.ident;
					}
				}
				else if (col.bounds.Contains(a.transform.position))
				{
					if (!useOutsideCol)
					{
						num |= a.ident;
					}
				}
				else if (useOutsideCol)
				{
					num |= a.ident;
				}
			}
			actorIdentIterator.ResetWithMask(num);
		}
		if (tag != string.Empty && actorIdentIterator.Mask != 0)
		{
			uint num = 0u;
			while (actorIdentIterator.NextActor(out a))
			{
				Entity component = a.gameObject.GetComponent<Entity>();
				if (component != null && component.Type == tag)
				{
					num |= a.ident;
				}
			}
			actorIdentIterator.ResetWithMask(num);
		}
		return ActorIdentIterator.AsList(actorIdentIterator.Mask);
	}

	public static List<FakeGunmanOverride> GetFakeActorsOfType(GameObject gameObject, ActorTypes type, Collider col)
	{
		List<FakeGunmanOverride> list = new List<FakeGunmanOverride>();
		GameObject gameObject2 = gameObject;
		while (gameObject2.transform.parent != null)
		{
			gameObject2 = gameObject2.transform.parent.gameObject;
		}
		FakeGunmanOverride[] componentsInChildren = gameObject2.GetComponentsInChildren<FakeGunmanOverride>();
		foreach (FakeGunmanOverride fakeGunmanOverride in componentsInChildren)
		{
			if (col != null)
			{
				if (col.bounds.Contains(fakeGunmanOverride.transform.position))
				{
					list.Add(fakeGunmanOverride);
				}
			}
			else
			{
				list.Add(fakeGunmanOverride);
			}
		}
		return list;
	}
}
