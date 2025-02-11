using System;
using System.Collections.Generic;
using UnityEngine;

public class KillObjectObjective : MissionObjective
{
	public List<KillObjectMonitor> Monitors;

	public override void Start()
	{
		base.Start();
		foreach (KillObjectMonitor monitor in Monitors)
		{
			TBFAssert.DoAssert(monitor != null, "KillObjectObjective - Null hole in Monitors list");
			TBFAssert.DoAssert(monitor.Actor != null, "KillObjectObjective - Null ActorWrapper in Monitors list");
			monitor.Actor.TargetActorChanged += Setup;
			base.gameObject.transform.position = monitor.Actor.gameObject.transform.position;
			UpdateBlipTarget();
		}
	}

	private void Setup(object sender)
	{
		GameObject gameObject = sender as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		Actor component = gameObject.GetComponent<Actor>();
		TBFAssert.DoAssert(component != null, "KillObjectObjective - Not monitoring an Actor");
		foreach (KillObjectMonitor monitor in Monitors)
		{
			if (component != monitor.Actor.GetActor() || !(monitor.Target == null))
			{
				continue;
			}
			monitor.Target = component.health;
			if (monitor.Target != null)
			{
				monitor.Target.OnHealthEmpty += OnHealthEmpty;
			}
			break;
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (KillObjectMonitor monitor in Monitors)
		{
			if (monitor.Actor != null)
			{
				monitor.Actor.TargetActorChanged -= Setup;
			}
			if (monitor.Target != null)
			{
				monitor.Target.OnHealthEmpty -= OnHealthEmpty;
			}
		}
	}

	private void OnHealthEmpty(object sender, EventArgs args)
	{
		bool flag = true;
		foreach (KillObjectMonitor monitor in Monitors)
		{
			if (monitor.Target != null && !monitor.Target.HealthEmpty)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			Pass();
		}
	}
}
