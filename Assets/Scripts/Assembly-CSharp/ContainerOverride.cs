using UnityEngine;

public class ContainerOverride : MonoBehaviour
{
	public virtual void SetupOverride(Container cont)
	{
	}

	public virtual void ApplyOverride(Container cont)
	{
	}

	public virtual void HookUpVisibleConnections(Container cont)
	{
	}

	public virtual void SendOverrideMessage(GameObject gameObj, string methodName)
	{
	}

	public virtual void SendOverrideMessageWithParam(GameObject gameObj, string methodName, string param)
	{
	}

	public virtual void SendOverrideMessageWithParam(GameObject gameObj, string methodName, GameObject param)
	{
	}

	public virtual void CreateAssociatedDefaultObjects(Container cont)
	{
	}

	public GameObject CreateNewBagObject(string refID, string name)
	{
		if (refID != string.Empty)
		{
		}
		return null;
	}
}
