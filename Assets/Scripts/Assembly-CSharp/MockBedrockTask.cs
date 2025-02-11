using UnityEngine;

public class MockBedrockTask : BedrockTask
{
	private static short CurrentTaskHandle;

	public bool ShouldTimeout;

	public float InitTime = 0.5f;

	public float PendingTime = 0.5f;

	private float _taskStartTime;

	public override int ErrorCode
	{
		get
		{
			return -1;
		}
	}

	public object Result { get; set; }

	public MockBedrockTask()
		: base(CurrentTaskHandle++)
	{
		_taskStartTime = Time.realtimeSinceStartup;
		base.Status = Bedrock.brTaskStatus.BR_TASK_INIT;
	}

	public override void UpdateStatus()
	{
		float num = Time.realtimeSinceStartup - _taskStartTime;
		if (num < InitTime)
		{
			base.Status = Bedrock.brTaskStatus.BR_TASK_INIT;
		}
		else if (num < InitTime + PendingTime)
		{
			base.Status = Bedrock.brTaskStatus.BR_TASK_PENDING;
		}
		else if (!ShouldTimeout)
		{
			base.Status = Bedrock.brTaskStatus.BR_TASK_SUCCESS;
		}
	}

	public override void Dispose()
	{
		base.Status = Bedrock.brTaskStatus.BR_TASK_NOT_IN_USE;
	}

	public override string ToString()
	{
		return "[MOCK: " + base.ToString() + ",ShouldTimeout=" + ShouldTimeout + "]";
	}
}
