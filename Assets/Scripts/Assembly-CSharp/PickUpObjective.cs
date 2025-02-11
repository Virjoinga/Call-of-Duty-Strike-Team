using System.Collections.Generic;

public class PickUpObjective : MissionObjective
{
	public List<PickUpObject> PickUpsToCollect = new List<PickUpObject>();

	public override void Start()
	{
		base.Start();
		foreach (PickUpObject item in PickUpsToCollect)
		{
			item.AddEventHandler(OnPickUp);
		}
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		foreach (PickUpObject item in PickUpsToCollect)
		{
			item.RemoveEventHandler(OnPickUp);
		}
	}

	private void OnPickUp(object sender)
	{
		PickUpsToCollect.Remove(sender as PickUpObject);
		if (PickUpsToCollect.Count == 0)
		{
			Pass();
		}
	}
}
