using System;

public class TechPurchaseEventArgs : EventArgs
{
	public DropZoneArea Area { get; private set; }

	public DropZoneObject Obj { get; private set; }

	public TechPurchaseEventArgs(DropZoneArea area, DropZoneObject obj)
	{
		Area = area;
		Obj = obj;
	}
}
