using UnityEngine;

public class AwarenessZoneOverride : ContainerOverride
{
	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		AwarenessZonelet[] componentsInChildren = GetComponentsInChildren<AwarenessZonelet>();
		AwarenessZone componentInChildren = GetComponentInChildren<AwarenessZone>();
		int layer = LayerMask.NameToLayer("Triggers");
		if (!(componentInChildren != null))
		{
			return;
		}
		if (componentsInChildren.Length > 0)
		{
			AwarenessZonelet[] array = componentsInChildren;
			foreach (AwarenessZonelet awarenessZonelet in array)
			{
				awarenessZonelet.gameObject.layer = layer;
				awarenessZonelet.parent = componentInChildren;
			}
			base.transform.localScale = new Vector3(1f, 1f, 1f);
			componentInChildren.children = componentsInChildren;
			BoxCollider component = componentInChildren.GetComponent<BoxCollider>();
			if (component != null)
			{
				if (Application.isPlaying)
				{
					Object.Destroy(component);
				}
				else
				{
					Object.DestroyImmediate(component);
				}
			}
		}
		else
		{
			componentInChildren.children = null;
			BoxCollider component = componentInChildren.GetComponent<BoxCollider>();
			if (component == null)
			{
				component = componentInChildren.gameObject.AddComponent<BoxCollider>();
			}
			component.isTrigger = true;
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		AwarenessZone componentInChildren = gameObj.GetComponentInChildren<AwarenessZone>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}
}
