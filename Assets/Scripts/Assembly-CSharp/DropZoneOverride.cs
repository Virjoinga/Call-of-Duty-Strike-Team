public class DropZoneOverride : ContainerOverride
{
	public DropZoneData m_OverrideData = new DropZoneData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		DropZoneArea dropZoneArea = cont.FindComponentOfType(typeof(DropZoneArea)) as DropZoneArea;
		if (dropZoneArea != null)
		{
			dropZoneArea.m_Interface = m_OverrideData;
		}
	}
}
