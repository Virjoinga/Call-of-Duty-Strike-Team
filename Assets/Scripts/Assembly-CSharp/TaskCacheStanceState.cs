using System.Collections.Generic;

public class TaskCacheStanceState : Task
{
	private struct CachedStance
	{
		public Actor mActor;

		public bool mWasCrouched;

		public bool mWasInFPP;

		public bool mIgnoreFPPCache;
	}

	private List<CachedStance> mCache;

	public TaskCacheStanceState(TaskManager owner, TaskManager.Priority priority, Config flags, Actor actorInvolved)
		: base(owner, priority, flags)
	{
		Init(new List<Actor>(1) { actorInvolved });
	}

	public TaskCacheStanceState(TaskManager owner, TaskManager.Priority priority, Config flags, List<Actor> actorsInvolved)
		: base(owner, priority, flags)
	{
		Init(actorsInvolved);
	}

	private void Init(List<Actor> actorsInvolved)
	{
		if (actorsInvolved == null)
		{
			return;
		}
		mCache = new List<CachedStance>();
		foreach (Actor item2 in actorsInvolved)
		{
			if (item2 == null || item2.realCharacter.IsMortallyWounded() || item2.realCharacter.IsDead())
			{
				continue;
			}
			CachedStance item = default(CachedStance);
			item.mActor = item2;
			item.mWasCrouched = item2.realCharacter.IsCrouching();
			if (GameController.Instance != null)
			{
				Actor mFirstPersonActor = GameController.Instance.mFirstPersonActor;
				if (GameController.Instance.IsFirstPerson)
				{
					item.mWasInFPP = mFirstPersonActor == GameController.Instance.mFirstPersonActor;
				}
			}
			mCache.Add(item);
		}
	}

	public override void Update()
	{
	}

	public override bool HasFinished()
	{
		return true;
	}

	public override void Finish()
	{
		foreach (CachedStance item in mCache)
		{
			bool flag = item.mActor.realCharacter.IsCrouching();
			if (item.mWasCrouched && !flag)
			{
				item.mActor.realCharacter.Crouch();
			}
			else if (!item.mWasCrouched && flag)
			{
				item.mActor.realCharacter.Stand();
			}
			CommonHudController.Instance.ForceCrouchButtonIcon(item.mWasCrouched);
			if (!item.mIgnoreFPPCache)
			{
				bool isFirstPerson = GameController.Instance.IsFirstPerson;
				if (item.mWasInFPP && !isFirstPerson && GameplayController.instance.IsSelected(item.mActor))
				{
					GameController.Instance.SwitchToFirstPerson(item.mActor, true);
				}
				else if (!item.mWasInFPP && isFirstPerson && item.mActor == GameController.Instance.mFirstPersonActor)
				{
					GameController.Instance.ExitFirstPerson();
				}
			}
			item.mActor.realCharacter.IsAimingDownSights = false;
		}
		base.Finish();
	}

	public void ClearFppCache()
	{
		int count = mCache.Count;
		for (int i = 0; i < count; i++)
		{
			CachedStance value = mCache[i];
			value.mWasInFPP = false;
			mCache[i] = value;
		}
	}

	public void IgnoreFPPCacheCheck()
	{
		int count = mCache.Count;
		for (int i = 0; i < count; i++)
		{
			CachedStance value = mCache[i];
			value.mIgnoreFPPCache = true;
			mCache[i] = value;
		}
	}
}
