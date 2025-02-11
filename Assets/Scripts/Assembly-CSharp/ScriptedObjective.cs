public class ScriptedObjective : MissionObjective
{
	public override void Start()
	{
		base.Start();
		UpdateBlipTarget();
	}

	public override void Activate()
	{
		base.Activate();
	}

	public override void Deactivate()
	{
		Fail();
	}

	public void Suspend()
	{
		base.Deactivate();
	}

	public override void HideObjective()
	{
		base.HideObjective();
	}
}
