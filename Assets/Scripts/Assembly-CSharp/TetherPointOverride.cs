public class TetherPointOverride : ContainerOverride
{
	public AITetherPointData m_OverrideData = new AITetherPointData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		AITetherPoint aITetherPoint = cont.FindComponentOfType(typeof(AITetherPoint)) as AITetherPoint;
		if (aITetherPoint != null)
		{
			aITetherPoint.m_Interface = m_OverrideData;
		}
	}
}
