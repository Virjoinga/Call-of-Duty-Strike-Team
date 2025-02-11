public class Condition_DoorOpen : Condition
{
	public BuildingDoor Door;

	public override bool Value()
	{
		return Door.m_Interface.State == BuildingDoor.DoorSate.Open;
	}
}
