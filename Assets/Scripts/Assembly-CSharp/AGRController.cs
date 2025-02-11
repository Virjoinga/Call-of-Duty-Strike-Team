using System.Collections.Generic;
using UnityEngine;

public class AGRController : MonoBehaviour
{
	public ActorWrapper AGR;

	public float DiffuseTime = 10f;

	private List<Transform> mBombs;

	private Actor mRealAGR;

	private bool mDiffusing;

	private void Start()
	{
		mBombs = new List<Transform>();
	}

	private void Update()
	{
		if (mRealAGR == null)
		{
			mRealAGR = AGR.GetActor();
		}
		if (mBombs.Count == 0)
		{
			return;
		}
		Transform transform = mBombs[0];
		float sqrMagnitude = (transform.position - mRealAGR.GetPosition()).sqrMagnitude;
		if (mDiffusing && !mRealAGR.tasks.IsRunningTask<TaskStopAndStare>())
		{
			mDiffusing = false;
			mBombs.RemoveAt(0);
		}
		else if (!mDiffusing && !mRealAGR.tasks.IsRunningTask<TaskRouteTo>() && !mRealAGR.tasks.IsRunningTask<TaskMoveTo>())
		{
			if (sqrMagnitude < 1f)
			{
				StartDiffusing(transform);
				return;
			}
			InheritableMovementParams inheritableMovementParams = new InheritableMovementParams(transform.position);
			inheritableMovementParams.FinalLookDirection = transform.forward;
			new TaskRouteTo(mRealAGR.tasks, TaskManager.Priority.LONG_TERM, Task.Config.ClearAllCurrentType, inheritableMovementParams);
		}
	}

	public void RegisterBombDiffusalRequest(Transform bomb)
	{
		mBombs.Add(bomb);
	}

	private void StartDiffusing(Transform bomb)
	{
		mDiffusing = true;
	}
}
