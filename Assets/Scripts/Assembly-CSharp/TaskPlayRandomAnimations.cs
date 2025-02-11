using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskPlayRandomAnimations : Task
{
	[Flags]
	private enum Flags
	{
		Default = 0,
		Invalidated = 1
	}

	private enum State
	{
		Active = 0,
		CyclesCompleted = 1
	}

	private Flags mFlags;

	private State mState;

	private List<PlayAnimationDescriptor> mPrioristisedDescriptorList;

	private int mCyclesRemaining;

	public TaskPlayRandomAnimations(TaskManager owner, TaskManager.Priority priority, Config flags, List<RandomAnimationDescriptor> randomAnimationDescriptorList, int cycles)
		: base(owner, priority, flags)
	{
		mFlags = Flags.Default;
		mState = State.Active;
		mCyclesRemaining = cycles;
		BuildPrioristisedRandomDescriptorList(randomAnimationDescriptorList);
	}

	public override void Update()
	{
		if (mState != 0 || base.Owner.IsRunningTask(typeof(TaskPlayAnimation)))
		{
			return;
		}
		TaskPlayAnimation taskPlayAnimation = CreateAnimTaskUsingRandomDescriptor();
		TBFAssert.DoAssert(taskPlayAnimation != null, string.Format("Failed to create Animation Task for {0}", mActor.realCharacter.name));
		taskPlayAnimation.PartOfSequence = true;
		if (mCyclesRemaining != -1)
		{
			mCyclesRemaining--;
			if (mCyclesRemaining <= 0)
			{
				taskPlayAnimation.PartOfSequence = false;
				mState = State.CyclesCompleted;
			}
		}
	}

	public override bool HasFinished()
	{
		if ((base.ConfigFlags & Config.AbortOnAlert) != 0 && mActor.behaviour.alertState == BehaviourController.AlertState.Reacting)
		{
			return true;
		}
		if (CheckConfigFlagsFinished())
		{
			return true;
		}
		if ((mFlags & Flags.Invalidated) != 0)
		{
			return true;
		}
		return mState == State.CyclesCompleted;
	}

	private void BuildPrioristisedRandomDescriptorList(List<RandomAnimationDescriptor> randomAnimationDescriptorList)
	{
		mPrioristisedDescriptorList = new List<PlayAnimationDescriptor>();
		foreach (RandomAnimationDescriptor randomAnimationDescriptor in randomAnimationDescriptorList)
		{
			for (int i = 0; i < randomAnimationDescriptor.Chance; i++)
			{
				mPrioristisedDescriptorList.Add(randomAnimationDescriptor.TaskDescriptor);
			}
		}
	}

	private TaskPlayAnimation CreateAnimTaskUsingRandomDescriptor()
	{
		int index = UnityEngine.Random.Range(0, mPrioristisedDescriptorList.Count);
		return mPrioristisedDescriptorList[index].CreateTask(base.Owner, base.Priority, base.ConfigFlags) as TaskPlayAnimation;
	}
}
