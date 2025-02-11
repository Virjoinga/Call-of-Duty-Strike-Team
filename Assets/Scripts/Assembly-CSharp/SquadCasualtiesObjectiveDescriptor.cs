public class SquadCasualtiesObjectiveDescriptor : ObjectiveDescriptor
{
	public string ObjectiveLabel;

	public bool IsPrimaryObjective;

	public int MaxPlayerCasualties = 3;

	public override void CreateObjective()
	{
		SquadCasualtiesObjective squadCasualtiesObjective = new SquadCasualtiesObjective();
		squadCasualtiesObjective.m_Interface.ObjectiveLabel = ObjectiveLabel;
		squadCasualtiesObjective.m_Interface.IsPrimaryObjective = IsPrimaryObjective;
		squadCasualtiesObjective.MaxPlayerCasualties = MaxPlayerCasualties;
		Register(squadCasualtiesObjective);
	}
}
