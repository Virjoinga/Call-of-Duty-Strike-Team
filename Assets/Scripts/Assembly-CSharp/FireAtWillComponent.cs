using UnityEngine;

public class FireAtWillComponent : BaseActorComponent
{
	private const float kSlowDownToShootDistSqr = 100f;

	private bool mEnabled;

	public bool Enabled
	{
		get
		{
			return mEnabled;
		}
		set
		{
			mEnabled = value;
		}
	}

	private void Start()
	{
		Reset();
	}

	private void Update()
	{
		if (!OptimisationManager.Update(OptType.FireAtWill, myActor) || !mEnabled || myActor.tasks.IsRunningTask(typeof(TaskShoot)) || myActor.baseCharacter.IsMortallyWounded() || (bool)myActor.realCharacter.Carried)
		{
			return;
		}
		myActor.weapon.ClearTarget();
		if (!myActor.behaviour.PlayerControlled && myActor.behaviour.InPassiveAlertState())
		{
			myActor.realCharacter.PickSomethingToAimAt(null);
			return;
		}
		float distanceSquared;
		Actor nearestEnemyInView = ((myActor.awareness.ChDefCharacterType != CharacterType.Human) ? myActor.awareness.GetNearestVisibleEnemy(out distanceSquared) : TargetScorer.GetBestTarget(myActor));
		nearestEnemyInView = myActor.behaviour.GetBestTarget(nearestEnemyInView);
		if (nearestEnemyInView != null && myActor.behaviour.PlayerControlled && nearestEnemyInView != myActor.behaviour.aimedShotTarget && nearestEnemyInView != myActor.behaviour.engagementTarget && (nearestEnemyInView.awareness.EnemiesIKnowAboutRecent & GKM.AliveMask) == 0)
		{
			nearestEnemyInView = null;
		}
		GameObject gameObject = myActor.realCharacter.PickSomethingToAimAt(nearestEnemyInView);
		Actor actor = null;
		if (gameObject != null)
		{
			actor = gameObject.GetComponent<Actor>();
		}
		if (actor != null && actor.awareness.ChDefCharacterType != CharacterType.SentryGun)
		{
			myActor.weapon.SetTarget(actor);
			AwarenessComponent awareness = actor.awareness;
			if ((awareness.ChDefCharacterType == CharacterType.Human || awareness.ChDefCharacterType == CharacterType.RiotShieldNPC) && (!awareness.isInCover || (awareness.coverBooked.noCoverAgainst & myActor.ident) != 0 || awareness.PoppedOutOfCover) && (actor.GetPosition() - myActor.GetPosition()).sqrMagnitude < 100f)
			{
				myActor.baseCharacter.SlowDown();
			}
		}
	}

	public void Reset()
	{
		mEnabled = true;
	}
}
