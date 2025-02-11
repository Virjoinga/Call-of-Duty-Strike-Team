using System;
using UnityEngine;

public class Projectile
{
	public enum ProjectileType
	{
		Grenade = 0,
		RPG = 1
	}

	public static void BroadcastImpact(MonoBehaviour projectile, float damageRadiusSq, ProjectileType type)
	{
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.AliveMask & GKM.CharacterTypeMask(CharacterType.Human));
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (a.behaviour.PlayerControlled || a.health == null || a.health.InvulnerableToExplosions || a.realCharacter.IsSniper || a.realCharacter.IsWindowLookout)
			{
				continue;
			}
			float sqrMagnitude = (projectile.transform.position - a.GetPosition()).sqrMagnitude;
			if (!(sqrMagnitude < damageRadiusSq))
			{
				continue;
			}
			Vector3 position = projectile.transform.position;
			Vector3 traceEnd = a.GetPosition() + Vector3.up;
			Vector3 collision;
			if (WorldHelper.IsClearTrace(position, traceEnd, out collision) && type == ProjectileType.Grenade)
			{
				Grenade grenade = projectile as Grenade;
				if (grenade != null)
				{
					new TaskReactToGrenade(a.tasks, TaskManager.Priority.IMMEDIATE, Task.Config.ClearAllCurrentType, grenade);
				}
			}
		}
	}

	public static void BroadcastExplosion(MonoBehaviour projectile, Actor owner)
	{
		if (owner == null)
		{
			return;
		}
		ActorIdentIterator actorIdentIterator = new ActorIdentIterator(GKM.FriendsMask(owner) & ~owner.ident & GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			Vector3 vector = projectile.transform.position - a.GetPosition();
			float num = Vector3.Dot(a.awareness.LookDirection.normalized, vector.normalized);
			float num2 = Mathf.Cos(a.awareness.FoV * ((float)Math.PI / 180f));
			if (num > num2 && vector.sqrMagnitude < a.awareness.visionRangeSqr)
			{
				a.awareness.BecomeAware(owner, projectile.transform.position);
			}
		}
		actorIdentIterator = new ActorIdentIterator(GKM.CharacterTypeMask(CharacterType.SecurityCamera));
		while (actorIdentIterator.NextActor(out a))
		{
			TaskSecurityCamera taskSecurityCamera = (TaskSecurityCamera)a.tasks.GetRunningTask(typeof(TaskSecurityCamera));
			if (taskSecurityCamera != null && a.awareness.InFOV(projectile.transform.position) == InFOVResult.Yes)
			{
				taskSecurityCamera.SoundAlarm(owner, projectile.transform.position);
			}
		}
	}
}
