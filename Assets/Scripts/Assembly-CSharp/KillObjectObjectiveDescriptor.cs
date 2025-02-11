using System.Collections.Generic;

public class KillObjectObjectiveDescriptor : ObjectiveDescriptor
{
	public string ObjectiveLabel;

	public bool IsPrimaryObjective;

	public List<KillObjectMonitor> Monitors;

	public override void CreateObjective()
	{
		KillObjectObjective killObjectObjective = new KillObjectObjective();
		killObjectObjective.m_Interface.ObjectiveLabel = ObjectiveLabel;
		killObjectObjective.m_Interface.IsPrimaryObjective = IsPrimaryObjective;
		killObjectObjective.Monitors = new List<KillObjectMonitor>(Monitors);
		Register(killObjectObjective);
	}
}
