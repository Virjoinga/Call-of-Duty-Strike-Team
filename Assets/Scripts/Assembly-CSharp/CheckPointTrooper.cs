using System;

[Serializable]
public class CheckPointTrooper
{
	public int TrooperID;

	public string TrooperName;

	public Actor ActorRef;

	public CheckPointTrooper(int id, string name, Actor actor)
	{
		TrooperID = id;
		TrooperName = name;
		ActorRef = actor;
	}

	public override string ToString()
	{
		return "_ID:" + TrooperID + "_Name:" + TrooperName;
	}
}
