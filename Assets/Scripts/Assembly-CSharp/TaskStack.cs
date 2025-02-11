using System.Collections.Generic;

public class TaskStack
{
	public List<Task> Stack;

	public List<Task> ParallelStack;

	public TaskStack()
	{
		Stack = new List<Task>();
		ParallelStack = new List<Task>();
	}

	public void Clear()
	{
		foreach (Task item in Stack)
		{
			item.Finish();
		}
		foreach (Task item2 in ParallelStack)
		{
			item2.Finish();
		}
		Stack.Clear();
		ParallelStack.Clear();
	}
}
