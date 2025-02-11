public class CMDropZoneObject : InterfaceableObject
{
	protected override void PopulateMenuItems()
	{
		if (GameplayController.Instance().AnySelectedAllowedCMOption(ContextMenuOptionType.Carry))
		{
			AddCallableMethod("S_CMPickUpObject", ContextMenuIcons.PickupBody);
		}
	}

	public void S_CMPickUpObject()
	{
		DropZoneObject component = base.gameObject.GetComponent<DropZoneObject>();
		CommonHudController.Instance.ClearContextMenu();
	}
}
