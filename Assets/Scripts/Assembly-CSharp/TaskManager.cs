using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public enum Priority
	{
		REACTIVE = 0,
		IMMEDIATE = 1,
		LONG_TERM = 2,
		DEFAULT = 3
	}

	public enum TaskResult
	{
		None = 0,
		Complete = 1,
		Aborted = 2
	}

	public struct TaskFeedback
	{
		public TaskResult mResult;

		public string mDetails;

		public TaskFeedback(TaskResult r, string d)
		{
			mResult = r;
			mDetails = d;
		}
	}

	public delegate void VisualiseTaskHandler();

	private const int kMaximumSafeStateThrashCount = 50;

	public ActorIdentIterator mVolatileIterator = new ActorIdentIterator(0u);

	public TaskStack Reactive = new TaskStack();

	public TaskStack Immediate = new TaskStack();

	public TaskStack LongTerm = new TaskStack();

	private Task mCachedTask;

	private static TaskSynchroniser dummyTaskSynchroniser = new TaskSynchroniser(false);

	private static TaskSynchroniser taskSyncInProgress = null;

	private int framesTillNextUpdate = -1;

	private float timeTillNextUpdate;

	protected TaskFeedback mLastFeedback;

	private Task mTaskProcessedLastUpdate;

	private List<TaskChain> mChainedTasks;

	private Task updatingTask;

	public static TaskSynchroniser TaskSyncInProgress
	{
		get
		{
			return taskSyncInProgress;
		}
	}

	public bool RunningTaskDeniesPlayerInput
	{
		get
		{
			Task runningTask = GetRunningTask();
			if (runningTask != null)
			{
				return (runningTask.ConfigFlags & Task.Config.DenyPlayerInput) != 0;
			}
			return false;
		}
	}

	public TaskFeedback lastFeedback
	{
		get
		{
			return mLastFeedback;
		}
	}

	public Task UpdatingTask
	{
		get
		{
			return updatingTask;
		}
	}

	public event VisualiseTaskHandler VisualiseTasks;

	public static void OpenSyncBracket()
	{
		TBFAssert.DoAssert(taskSyncInProgress == dummyTaskSynchroniser);
		taskSyncInProgress = new TaskSynchroniser(true);
	}

	public static void CloseSyncBracket()
	{
		TBFAssert.DoAssert(taskSyncInProgress != dummyTaskSynchroniser);
		taskSyncInProgress = dummyTaskSynchroniser;
	}

	public void ScheduleNextUpdateNoLaterThan(int frameCount)
	{
		if (framesTillNextUpdate > frameCount || framesTillNextUpdate == -1)
		{
			timeTillNextUpdate = 0f;
			framesTillNextUpdate = frameCount;
		}
	}

	public void ScheduleNextUpdateNoLaterThan(float time)
	{
		if ((timeTillNextUpdate <= Time.time || timeTillNextUpdate > time) && framesTillNextUpdate == -1)
		{
			timeTillNextUpdate = time;
		}
	}

	public void ScheduleUpdateNextFrame()
	{
		framesTillNextUpdate = 0;
		timeTillNextUpdate = 0f;
	}

	private void ResetUpdateSchedule()
	{
		framesTillNextUpdate = -1;
		timeTillNextUpdate = 0f;
	}

	private void Awake()
	{
		mChainedTasks = new List<TaskChain>();
		Reset();
		taskSyncInProgress = dummyTaskSynchroniser;
	}

	private bool SkipUpdatingTasksThisFrame()
	{
		if (timeTillNextUpdate > Time.time)
		{
			return true;
		}
		if (framesTillNextUpdate > 0)
		{
			framesTillNextUpdate--;
			return true;
		}
		ResetUpdateSchedule();
		return false;
	}

	private void Update()
	{
		if (SkipUpdatingTasksThisFrame())
		{
			if (this.VisualiseTasks != null)
			{
				this.VisualiseTasks();
			}
			return;
		}
		ScheduleUpdateNextFrame();
		if (!UpdateTaskStack(Reactive) && !UpdateTaskStack(Immediate))
		{
			UpdateTaskStack(LongTerm);
		}
		if (this.VisualiseTasks != null)
		{
			this.VisualiseTasks();
		}
	}

	private void LateUpdate()
	{
		mCachedTask = null;
	}

	private void OnDestroy()
	{
		DestroyAllTasksOnStack(Reactive.Stack);
		DestroyAllTasksOnStack(Immediate.Stack);
		DestroyAllTasksOnStack(LongTerm.Stack);
	}

	public void CancelAllTasks()
	{
		CancelTasksExcluding(typeof(DefinitelyNotATask));
	}

	private void Reset()
	{
		mTaskProcessedLastUpdate = null;
		Reactive.Clear();
		Immediate.Clear();
		LongTerm.Clear();
	}

	public bool IsRunningTask(Type taskType)
	{
		return GetRunningTaskOnStack(Reactive.Stack, taskType) != null || GetRunningTaskOnStack(Immediate.Stack, taskType) != null || GetRunningTaskOnStack(LongTerm.Stack, taskType) != null;
	}

	public bool IsRunningTask<T>() where T : Task
	{
		return IsRunningTask(typeof(T));
	}

	public Task GetRunningTask(Type taskType)
	{
		Task runningTaskOnStack = GetRunningTaskOnStack(Reactive.Stack, taskType);
		if (runningTaskOnStack != null)
		{
			return runningTaskOnStack;
		}
		runningTaskOnStack = GetRunningTaskOnStack(Immediate.Stack, taskType);
		if (runningTaskOnStack != null)
		{
			return runningTaskOnStack;
		}
		return GetRunningTaskOnStack(LongTerm.Stack, taskType);
	}

	public T GetRunningTask<T>() where T : Task
	{
		return (mCachedTask == null) ? (GetRunningTask(typeof(T)) as T) : (mCachedTask as T);
	}

	public Task GetRunningTask()
	{
		Task runningTaskOnStack = GetRunningTaskOnStack(Reactive.Stack);
		if (runningTaskOnStack != null)
		{
			mCachedTask = runningTaskOnStack;
			return runningTaskOnStack;
		}
		runningTaskOnStack = GetRunningTaskOnStack(Immediate.Stack);
		if (runningTaskOnStack != null)
		{
			mCachedTask = runningTaskOnStack;
			return runningTaskOnStack;
		}
		return mCachedTask = GetRunningTaskOnStack(LongTerm.Stack);
	}

	public void CancelTasks(Type taskType)
	{
		CancelTasksOnStack(Reactive.Stack, taskType);
		CancelTasksOnStack(Immediate.Stack, taskType);
		CancelTasksOnStack(LongTerm.Stack, taskType);
	}

	public void CancelTasksOtherThanThisOne(Type taskType, Task toExclude)
	{
		CancelTasksOnStackOtherThanThisOne(Reactive.Stack, taskType, toExclude);
		CancelTasksOnStackOtherThanThisOne(Immediate.Stack, taskType, toExclude);
		CancelTasksOnStackOtherThanThisOne(LongTerm.Stack, taskType, toExclude);
	}

	public void CancelTasksOfEqualOrHigherPriority(Type taskType, Priority priority)
	{
		CancelTasksOnStack(Reactive.Stack, taskType);
		if (priority > Priority.REACTIVE)
		{
			CancelTasksOnStack(Immediate.Stack, taskType);
		}
		if (priority > Priority.IMMEDIATE)
		{
			CancelTasksOnStack(LongTerm.Stack, taskType);
		}
	}

	public void CancelTasks(Type taskType, Priority priority)
	{
		if (priority == Priority.REACTIVE)
		{
			CancelTasksOnStack(Reactive.Stack, taskType);
		}
		if (priority <= Priority.IMMEDIATE)
		{
			CancelTasksOnStack(Immediate.Stack, taskType);
		}
		CancelTasksOnStack(LongTerm.Stack, taskType);
	}

	public void CancelTasks<T>() where T : Task
	{
		CancelTasks(typeof(T));
	}

	public void CancelTasksOtherThanThisOne<T>(Task toExclude) where T : Task
	{
		CancelTasksOtherThanThisOne(typeof(T), toExclude);
	}

	public void CancelTasksExcluding(Type taskType)
	{
		CancelTasksOnStackExcluding(Reactive.Stack, taskType);
		CancelTasksOnStackExcluding(Immediate.Stack, taskType);
		CancelTasksOnStackExcluding(LongTerm.Stack, taskType);
	}

	public void CancelTasksExcluding(Type[] taskTypes)
	{
		CancelTasksOnStackExcluding(Reactive.Stack, taskTypes);
		CancelTasksOnStackExcluding(Immediate.Stack, taskTypes);
		CancelTasksOnStackExcluding(LongTerm.Stack, taskTypes);
	}

	public void CancelTasksExcluding<T>() where T : Task
	{
		CancelTasksExcluding(typeof(T));
	}

	public void RegisterChainedTask(Task chainTask, Task parentTask)
	{
		TaskChain item = new TaskChain(chainTask, parentTask);
		mChainedTasks.Add(item);
	}

	private bool UpdateTaskStack(TaskStack taskStack)
	{
		for (int num = taskStack.ParallelStack.Count - 1; num >= 0; num--)
		{
			Task task = (updatingTask = taskStack.ParallelStack[num]);
			task.Update();
			updatingTask = null;
			if (task.HasFinished())
			{
				taskStack.ParallelStack.RemoveAt(num);
				FinishTask(task);
			}
		}
		bool flag = false;
		while (!flag && taskStack.Stack.Count > 0)
		{
			Task task2 = taskStack.Stack[taskStack.Stack.Count - 1];
			if (task2.HasFinished())
			{
				bool flag2 = false;
				int num2 = taskStack.Stack.Count - 1;
				while (!flag2 && num2 >= 0)
				{
					if (taskStack.Stack[num2] == task2)
					{
						taskStack.Stack.RemoveAt(num2);
						flag2 = true;
					}
					num2--;
				}
				TBFAssert.DoAssert(flag2, "Everything you thought you knew is wrong");
				FinishTask(task2);
				ReplaceWithChainedTask(task2, taskStack);
				continue;
			}
			if (mTaskProcessedLastUpdate != task2)
			{
				if (mTaskProcessedLastUpdate != null)
				{
					mTaskProcessedLastUpdate.OnSleep();
					if (HasStackChanged(taskStack.Stack, task2))
					{
						ScheduleUpdateNextFrame();
						return flag;
					}
				}
				task2.OnResume();
				if (HasStackChanged(taskStack.Stack, task2))
				{
					ScheduleUpdateNextFrame();
					return flag;
				}
			}
			updatingTask = task2;
			task2.Update();
			mLastFeedback.mResult = TaskResult.None;
			flag = true;
			updatingTask = null;
			mTaskProcessedLastUpdate = task2;
		}
		return flag;
	}

	private bool HasStackChanged(List<Task> taskStack, Task expectedRunningTask)
	{
		if (taskStack == null || taskStack.Count == 0 || expectedRunningTask == null)
		{
			return false;
		}
		Task task = taskStack[taskStack.Count - 1];
		if (task != expectedRunningTask)
		{
			mTaskProcessedLastUpdate = null;
			return true;
		}
		return false;
	}

	private Task GetRunningTaskOnStack(List<Task> taskStack, Type taskType)
	{
		for (int num = taskStack.Count - 1; num >= 0; num--)
		{
			Task task = taskStack[num];
			if (task.GetType() == taskType)
			{
				return task;
			}
		}
		return null;
	}

	private Task GetRunningTaskOnStack(List<Task> taskStack)
	{
		if (taskStack.Count >= 1)
		{
			return taskStack[taskStack.Count - 1];
		}
		return null;
	}

	private void DestroyAllTasksOnStack(List<Task> taskStack)
	{
		for (int num = taskStack.Count - 1; num >= 0; num--)
		{
			Task task = taskStack[num];
			taskStack.RemoveAt(num);
			task.mTaskSync.TaskFinished(task);
			task.Destroy();
		}
	}

	private void CancelTasksOnStack(List<Task> taskStack, Type taskType)
	{
		int num = 0;
		while (CancelTasksOnStack_Internal(taskStack, taskType) && num < 50)
		{
			num++;
		}
		TBFAssert.DoAssert(num < 50, "CancelTasksOnStack: Task System is state-thrashing!");
	}

	private bool CancelTasksOnStack_Internal(List<Task> taskStack, Type taskType)
	{
		for (int num = taskStack.Count - 1; num >= 0; num--)
		{
			Task task = taskStack[num];
			if (task.GetType() == taskType)
			{
				taskStack.RemoveAt(num);
				FinishTask(task);
				return true;
			}
		}
		return false;
	}

	private void CancelTasksOnStackOtherThanThisOne(List<Task> taskStack, Type taskType, Task toExclude)
	{
		int num = 0;
		while (CancelTasksOnStackOtherThanThisOne_Internal(taskStack, taskType, toExclude) && num < 10)
		{
			num++;
		}
		TBFAssert.DoAssert(num < 10, "CancelTasksOnStackOtherThanThisOne: Task System is state-thrashing!");
	}

	private bool CancelTasksOnStackOtherThanThisOne_Internal(List<Task> taskStack, Type taskType, Task toExclude)
	{
		for (int num = taskStack.Count - 1; num >= 0; num--)
		{
			Task task = taskStack[num];
			if (task.GetType() == taskType && task != toExclude)
			{
				taskStack.RemoveAt(num);
				FinishTask(task);
				return true;
			}
		}
		return false;
	}

	private void CancelTasksOnStackExcluding(List<Task> taskStack, Type taskType)
	{
		int num = 0;
		while (CancelTasksOnStackExcluding_Internal(taskStack, taskType) && num < 50)
		{
			num++;
		}
		TBFAssert.DoAssert(num < 50, "CancelTasksOnStackExcluding: Task System is state-thrashing!");
	}

	private bool CancelTasksOnStackExcluding_Internal(List<Task> taskStack, Type taskType)
	{
		for (int num = taskStack.Count - 1; num >= 0; num--)
		{
			Task task = taskStack[num];
			if (task.GetType() != taskType)
			{
				taskStack.RemoveAt(num);
				FinishTask(task);
				return true;
			}
		}
		return false;
	}

	private void CancelTasksOnStackExcluding(List<Task> taskStack, Type[] taskTypes)
	{
		int num = 0;
		while (CancelTasksOnStackExcluding_Internal(taskStack, taskTypes) && num < 50)
		{
			num++;
		}
		TBFAssert.DoAssert(num < 50, "CancelTasksOnStackExcluding[]: Task System is state-thrashing!");
	}

	private bool CancelTasksOnStackExcluding_Internal(List<Task> taskStack, Type[] taskTypes)
	{
		for (int num = taskStack.Count - 1; num >= 0; num--)
		{
			Task task = taskStack[num];
			bool flag = false;
			for (int i = 0; i < taskTypes.GetLength(0); i++)
			{
				if (task.GetType() == taskTypes[i])
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				taskStack.RemoveAt(num);
				FinishTask(task);
				return true;
			}
		}
		return false;
	}

	private void ReplaceWithChainedTask(Task completedTask, TaskStack stack)
	{
		for (int num = mChainedTasks.Count - 1; num >= 0; num--)
		{
			TaskChain taskChain = mChainedTasks[num];
			if (taskChain.Parent == completedTask)
			{
				stack.Stack.Add(taskChain.ChainedTask);
				mChainedTasks.RemoveAt(num);
				break;
			}
		}
	}

	public void Feedback(TaskResult r, string d)
	{
		mLastFeedback.mResult = r;
		mLastFeedback.mDetails = d;
	}

	public void Command(string com)
	{
		Command(Reactive, com);
		Command(Immediate, com);
		Command(LongTerm, com);
	}

	public void Command(TaskStack taskStack, string com)
	{
		for (int num = taskStack.ParallelStack.Count - 1; num >= 0; num--)
		{
			Task task = taskStack.ParallelStack[num];
			task.Command(com);
		}
		for (int num2 = taskStack.Stack.Count - 1; num2 >= 0; num2--)
		{
			taskStack.Stack[num2].Command(com);
		}
	}

	private void FinishTask(Task task)
	{
		InteractionsManager.Instance.FinishedAction(base.gameObject, task);
		task.Finish();
		task.mTaskSync.TaskFinished(task);
		if (task.BroadcastOnCompletion != null)
		{
			task.BroadcastOnCompletion.SendMessages();
			task.BroadcastOnCompletion.ActivateTurnonables();
			task.BroadcastOnCompletion.DeactivateTurnoffables();
		}
		if (mTaskProcessedLastUpdate == task)
		{
			mTaskProcessedLastUpdate = null;
		}
	}

	public void StackMe(Task t)
	{
		t.mTaskSync = taskSyncInProgress;
		taskSyncInProgress.AddTask(t);
		ScheduleUpdateNextFrame();
		if ((t.ConfigFlags & Task.Config.Chain) == 0)
		{
			if ((t.ConfigFlags & Task.Config.Parallel) != 0)
			{
				switch (t.Priority)
				{
				case Priority.REACTIVE:
					Reactive.ParallelStack.Add(t);
					break;
				case Priority.IMMEDIATE:
					Immediate.ParallelStack.Add(t);
					break;
				case Priority.LONG_TERM:
					LongTerm.ParallelStack.Add(t);
					break;
				default:
					throw new Exception("Invalid Task Priority (Parallel) - " + t.Priority);
				}
			}
			else
			{
				switch (t.Priority)
				{
				case Priority.REACTIVE:
					Reactive.Stack.Add(t);
					break;
				case Priority.IMMEDIATE:
					Immediate.Stack.Add(t);
					break;
				case Priority.LONG_TERM:
					LongTerm.Stack.Add(t);
					break;
				default:
					throw new Exception("Invalid Task Priority - " + t.Priority);
				}
			}
		}
		InteractionsManager.Instance.RegisterAction(base.gameObject, t);
	}
}
