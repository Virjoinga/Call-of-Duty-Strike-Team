using UnityEngine;

public class ScriptVariable : MonoBehaviour
{
	public string Name;

	private int Count;

	public int Value
	{
		get
		{
			return Count;
		}
	}

	private void Start()
	{
		Count = 0;
	}

	private void Activate()
	{
		Count++;
		LogSequenceDebug();
	}

	private void Deactivate()
	{
		Count--;
		LogSequenceDebug();
	}

	private void LogSequenceDebug()
	{
	}
}
