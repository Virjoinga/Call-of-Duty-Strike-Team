using System;

[Serializable]
public class SPLinkInfo
{
	public enum SPLinkType
	{
		Invalid = 0,
		ActorToTriggerPoint = 1,
		WorldReference = 2,
		WorldObject = 3,
		NumLinkTypes = 4
	}

	private const int kMediumTextWidth = 150;

	private const int kShortTextWidth = 32;

	private const int kGunAwayWidth = 75;

	private const int kGunOutWidth = 70;

	private const int kRequiredWidth = 60;

	private const int kUsePlaygroundMeshWidth = 110;

	private const int kCheckBoxWidth = 16;

	public SPLinkType Type;

	public SPObjectReference RefObject;

	public int NodeIndex;

	public int ActorIndex;

	public int FPSActorIndex;

	public int WorldReferenceIndex;

	public bool Required;

	public bool UsePlaygroundMesh;

	public SPLinkInfo(SPLinkType type)
	{
		Type = type;
		NodeIndex = 0;
		ActorIndex = 0;
		FPSActorIndex = 0;
		Required = true;
		WorldReferenceIndex = 0;
		Required = false;
		switch (Type)
		{
		case SPLinkType.ActorToTriggerPoint:
			RefObject = new SPObjectReference(SPObjectReference.SPObjectType.PlayerCharacter);
			RefObject.Name = "PlayerCharacter";
			Required = true;
			RefObject.GunAway = false;
			RefObject.GunOut = true;
			break;
		case SPLinkType.WorldReference:
			RefObject = new SPObjectReference(SPObjectReference.SPObjectType.Node);
			RefObject.Name = "World Reference";
			break;
		case SPLinkType.WorldObject:
			RefObject = new SPObjectReference(SPObjectReference.SPObjectType.Node);
			RefObject.Name = "World Object";
			break;
		}
	}
}
