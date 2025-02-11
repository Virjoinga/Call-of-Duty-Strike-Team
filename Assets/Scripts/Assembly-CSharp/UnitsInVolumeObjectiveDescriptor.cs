using UnityEngine;

public class UnitsInVolumeObjectiveDescriptor : ObjectiveDescriptor
{
	public string ObjectiveLabel;

	public bool IsPrimaryObjective;

	private Collider TriggerVolume;

	public UnitsInVolumeObjective.TriggerCheck Check;

	public int SpecificNumber = 1;

	public ActorWrapper[] Actors;

	public override void CreateObjective()
	{
		UnitsInVolumeObjective unitsInVolumeObjective = new UnitsInVolumeObjective();
		unitsInVolumeObjective.m_Interface.ObjectiveLabel = ObjectiveLabel;
		unitsInVolumeObjective.m_Interface.IsPrimaryObjective = IsPrimaryObjective;
		unitsInVolumeObjective.m_UnitsVolInterface.Check = Check;
		unitsInVolumeObjective.m_UnitsVolInterface.SpecificNumber = SpecificNumber;
		unitsInVolumeObjective.Actors.Equals(Actors);
		Register(unitsInVolumeObjective);
	}
}
