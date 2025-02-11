using UnityEngine;

public class UncertaintyTag : MonoBehaviour
{
	private enum LastOp
	{
		None = 0,
		ForceOn = 1,
		ForceOff = 2,
		Restore = 3
	}

	public bool[] wasEnabled;

	private static LastOp lastOp;

	private static void SaveState()
	{
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(UncertaintyTag));
		foreach (Object @object in array)
		{
			if (@object as UncertaintyTag != null)
			{
				((UncertaintyTag)@object)._SaveState();
			}
		}
	}

	public static void Reset()
	{
		lastOp = LastOp.None;
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(UncertaintyTag));
		foreach (Object @object in array)
		{
			if (@object as UncertaintyTag != null)
			{
				((UncertaintyTag)@object).wasEnabled = null;
			}
		}
	}

	public static void ForceOn()
	{
		if (lastOp == LastOp.None)
		{
			SaveState();
		}
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(UncertaintyTag));
		foreach (Object @object in array)
		{
			if (@object as UncertaintyTag != null)
			{
				((UncertaintyTag)@object)._ForceOn();
			}
		}
		lastOp = LastOp.ForceOn;
	}

	public static void ForceOff()
	{
		if (lastOp == LastOp.None)
		{
			SaveState();
		}
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(UncertaintyTag));
		foreach (Object @object in array)
		{
			if (@object as UncertaintyTag != null)
			{
				((UncertaintyTag)@object)._ForceOff();
			}
		}
		lastOp = LastOp.ForceOff;
	}

	public static void Restore()
	{
		if (lastOp == LastOp.None)
		{
			return;
		}
		Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(UncertaintyTag));
		foreach (Object @object in array)
		{
			if (@object as UncertaintyTag != null)
			{
				((UncertaintyTag)@object)._Restore();
			}
		}
		lastOp = LastOp.Restore;
	}

	private void _SaveState()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(Collider), true);
		if (componentsInChildren != null)
		{
			wasEnabled = new bool[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				wasEnabled[i] = ((Collider)componentsInChildren[i]).enabled;
			}
		}
	}

	private void _ForceOff()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(Collider), true);
		if (componentsInChildren != null && wasEnabled != null && wasEnabled.Length == componentsInChildren.Length)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((Collider)componentsInChildren[i]).enabled = false;
			}
		}
	}

	private void _ForceOn()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(Collider), true);
		if (componentsInChildren != null && wasEnabled != null && wasEnabled.Length == componentsInChildren.Length)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((Collider)componentsInChildren[i]).enabled = true;
			}
		}
	}

	private void _Restore()
	{
		Component[] componentsInChildren = GetComponentsInChildren(typeof(Collider), true);
		if (componentsInChildren != null && wasEnabled != null && wasEnabled.Length == componentsInChildren.Length)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				((Collider)componentsInChildren[i]).enabled = wasEnabled[i];
			}
		}
	}
}
