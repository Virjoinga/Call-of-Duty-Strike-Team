using UnityEngine;

public class TaskDescriptorOverride : ContainerOverride
{
	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		TaskDescriptor[] componentsInChildren = GetComponentsInChildren<TaskDescriptor>();
		TaskDescriptor[] array = componentsInChildren;
		foreach (TaskDescriptor taskDescriptor in array)
		{
			taskDescriptor.ResolveGuidLinks();
		}
		HackDescriptor[] componentsInChildren2 = GetComponentsInChildren<HackDescriptor>(true);
		HackDescriptor[] array2 = componentsInChildren2;
		foreach (HackDescriptor hackDescriptor in array2)
		{
			if (hackDescriptor.Obj == null)
			{
				GameObject theObject = hackDescriptor.ObjRef.theObject;
				if (theObject != null)
				{
					hackDescriptor.Obj = theObject.GetComponentInChildren<HackableObject>();
				}
			}
		}
	}
}
