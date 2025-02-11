public class ConditionalSequenceOverride : ContainerOverride
{
	public ConditionalSequenceData m_OverrideData = new ConditionalSequenceData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		ConditionalSequence component = GetComponent<ConditionalSequence>();
		if (component != null)
		{
			component.m_Interface = m_OverrideData;
			m_OverrideData.CopyContainerData(component);
		}
	}
}
