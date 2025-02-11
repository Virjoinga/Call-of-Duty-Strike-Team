using System;

public class DataSetCreator : SequenceCreator
{
	public Type[] DataTypes = new Type[3]
	{
		typeof(SpawnerDoorDataCommand),
		typeof(SpawnerDoorCoordinatorDataCommand),
		typeof(SendMessageOnRecievedCountDataCommand)
	};

	public override Type[] GetTypes()
	{
		return DataTypes;
	}
}
