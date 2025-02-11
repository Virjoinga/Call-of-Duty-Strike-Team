using System;

[Serializable]
public class ActionTest
{
	public string Name;

	public float Duration;

	private int Handle;

	public void SetHandle(int inHandle)
	{
		Handle = inHandle;
	}

	public int GetHandle()
	{
		return Handle;
	}
}
