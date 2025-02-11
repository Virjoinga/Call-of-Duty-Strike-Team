using UnityEngine;

public class UnitsNotInVolumeObjectiveDescriptor : ObjectiveDescriptor
{
	public string ObjectiveLabel;

	public bool IsPrimaryObjective;

	public Collider TriggerVolume;

	public UnitsInVolumeObjective.TriggerCheck Check;

	public int SpecificNumber = 1;

	public ActorWrapper[] Actors;

	public override void CreateObjective()
	{
		UnitsNotInVolumeObjective unitsNotInVolumeObjective = new UnitsNotInVolumeObjective();
		unitsNotInVolumeObjective.m_Interface.ObjectiveLabel = ObjectiveLabel;
		unitsNotInVolumeObjective.m_Interface.IsPrimaryObjective = IsPrimaryObjective;
		unitsNotInVolumeObjective.m_UnitsVolInterface.TriggerVolume = TriggerVolume;
		unitsNotInVolumeObjective.m_UnitsVolInterface.Check = Check;
		unitsNotInVolumeObjective.m_UnitsVolInterface.SpecificNumber = SpecificNumber;
		unitsNotInVolumeObjective.m_UnitsVolInterface.Actors.Equals(Actors);
		Register(unitsNotInVolumeObjective);
	}
}
