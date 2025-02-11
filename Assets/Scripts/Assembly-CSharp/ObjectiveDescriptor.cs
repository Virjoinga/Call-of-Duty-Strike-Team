using UnityEngine;

public abstract class ObjectiveDescriptor : MonoBehaviour
{
	private MissionObjective mObjective;

	public MissionObjective Objective
	{
		get
		{
			return mObjective;
		}
	}

	public abstract void CreateObjective();

	protected void Register(MissionObjective objective)
	{
	}
}
