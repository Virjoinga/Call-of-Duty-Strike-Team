public class KeepObjectAliveObjectiveDescriptor : ObjectiveDescriptor
{
	public string ObjectiveLabel;

	public bool IsPrimaryObjective;

	public ActorWrapper Actor;

	public override void CreateObjective()
	{
		KeepObjectAliveObjective keepObjectAliveObjective = new KeepObjectAliveObjective();
		keepObjectAliveObjective.m_Interface.ObjectiveLabel = ObjectiveLabel;
		keepObjectAliveObjective.m_Interface.IsPrimaryObjective = IsPrimaryObjective;
		keepObjectAliveObjective.Actor = Actor;
		Register(keepObjectAliveObjective);
	}
}
