public class LockerOverride : ContainerOverride
{
	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		Locker locker = cont.FindComponentOfType(typeof(Locker)) as Locker;
		if (!(locker != null))
		{
		}
	}
}
