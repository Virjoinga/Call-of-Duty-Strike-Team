using System.Collections.Generic;
using UnityEngine;

public class TaskDead : Task
{
	private enum State
	{
		Start = 0,
		BeingDead = 1,
		FadeOut = 2
	}

	private const int mDeadTaskLimit = 5;

	private const float mFadeOutTime = 2f;

	private const float kFixedDeathStayTime = 10f;

	private static List<TaskDead> mDeadEnemies = new List<TaskDead>();

	private static List<TaskDead> mPrunableDeadEnemies = new List<TaskDead>();

	private float mAlpha;

	private ICollection<Material> mFadeMaterials;

	public float TimeOfDeath;

	public float FixedDeathStayTimeEnd;

	private State mState;

	public TaskDead(TaskManager owner, TaskManager.Priority priority, Config flags, GameObject killer)
		: base(owner, priority, flags)
	{
		mActor.realCharacter.IsDeadForReal = true;
		TimeOfDeath = Time.time;
		FixedDeathStayTimeEnd = TimeOfDeath + 10f;
		mState = State.Start;
		owner.CancelTasksExcluding(typeof(TaskDead));
		mAlpha = 1f;
		if (!WorldHelper.IsPlayerControlledActor(mActor))
		{
			mDeadEnemies.Add(this);
			if (mActor.awareness.ChDefCharacterType != CharacterType.SecurityCamera && mActor.awareness.ChDefCharacterType != CharacterType.SentryGun)
			{
				mPrunableDeadEnemies.Add(this);
			}
		}
		PruneTheDead();
		WaypointMarkerManager.Instance.RemoveMarker(base.Owner.gameObject);
		mActor.realCharacter.IsAimingDownSights = false;
		if (mActor.realCharacter.SnapTarget != null)
		{
			mActor.realCharacter.SnapTarget.gameObject.SetActive(false);
		}
		if (mActor == GameController.Instance.mFirstPersonActor && !ActStructure.Instance.CurrentMissionIsSpecOps() && !GameController.Instance.IsLockedToFirstPerson)
		{
			GameController.Instance.ExitFirstPerson();
		}
		mActor.awareness.canLook = false;
		if (mActor.Picker != null)
		{
			ActorSelectUtils.EnableActorSelectCollider(mActor, false);
		}
	}

	public static void ClearTheDeadBetweenSections()
	{
		mPrunableDeadEnemies.Clear();
	}

	~TaskDead()
	{
		if (mDeadEnemies.Contains(this))
		{
			mDeadEnemies.Remove(this);
		}
		if (mPrunableDeadEnemies.Contains(this))
		{
			mPrunableDeadEnemies.Remove(this);
		}
	}

	public override void Destroy()
	{
		if (mDeadEnemies.Contains(this))
		{
			mDeadEnemies.Remove(this);
		}
		if (mPrunableDeadEnemies.Contains(this))
		{
			mPrunableDeadEnemies.Remove(this);
		}
	}

	private void PruneTheDead()
	{
		while (mPrunableDeadEnemies.Count >= 5)
		{
			float num = 0f;
			TaskDead taskDead = null;
			foreach (TaskDead mPrunableDeadEnemy in mPrunableDeadEnemies)
			{
				if (mPrunableDeadEnemy != null && !(mPrunableDeadEnemy.mActor == null) && !(Time.time < mPrunableDeadEnemy.FixedDeathStayTimeEnd) && !mPrunableDeadEnemy.mActor.realCharacter.IsBeingCarried && !mPrunableDeadEnemy.mActor.realCharacter.WasLastActorDropped)
				{
					bool flag = false;
					Vector3 vector = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(mPrunableDeadEnemy.Owner.transform.position);
					if (vector.x > 0f && vector.x < 1f && vector.y > 0f && vector.y < 1f && vector.z > 0f)
					{
						flag = true;
					}
					float num2 = 0f;
					num2 += ((!flag) ? 100f : 0f);
					num2 += Time.time - mPrunableDeadEnemy.TimeOfDeath;
					num2 += (CameraManager.Instance.CurrentCamera.transform.position.xz() - mPrunableDeadEnemy.mOwner.transform.position.xz()).sqrMagnitude;
					num2 += ((!mPrunableDeadEnemy.mActor.gameObject.activeSelf) ? 10000f : 0f);
					if (num2 > num)
					{
						num = num2;
						taskDead = mPrunableDeadEnemy;
					}
				}
			}
			if (taskDead == null)
			{
				foreach (TaskDead mPrunableDeadEnemy2 in mPrunableDeadEnemies)
				{
					if (mPrunableDeadEnemy2 == null || mPrunableDeadEnemy2.mActor == null || mPrunableDeadEnemy2.mActor.realCharacter.WasLastActorDropped)
					{
						continue;
					}
					taskDead = mPrunableDeadEnemy2;
					break;
				}
			}
			if (taskDead == null)
			{
				taskDead = mPrunableDeadEnemies[0];
			}
			PruneDeadEnemy(taskDead);
		}
	}

	private void PruneDeadEnemy(TaskDead deadEnemy)
	{
		if (deadEnemy == null)
		{
			return;
		}
		mPrunableDeadEnemies.Remove(deadEnemy);
		mDeadEnemies.Remove(deadEnemy);
		deadEnemy.mState = State.FadeOut;
		if (deadEnemy.mActor != null)
		{
			InterfaceableObject componentInChildren = deadEnemy.mActor.GetComponentInChildren<InterfaceableObject>();
			if (componentInChildren != null && componentInChildren.AssociatedObject == deadEnemy.mActor.gameObject)
			{
				componentInChildren.SuppressSound = true;
				componentInChildren.CancelMenu();
				componentInChildren.enabled = false;
			}
			if (deadEnemy.mActor.model != null)
			{
				deadEnemy.mFadeMaterials = EffectsController.Instance.Fade(deadEnemy.mActor.model.GetComponentsInChildren<SkinnedMeshRenderer>());
			}
		}
	}

	public override void Update()
	{
		if (mActor != null && mActor.weapon != null && !(mActor.weapon.ActiveWeapon is Weapon_Minigun))
		{
			mActor.weapon.SetTrigger(false);
		}
		switch (mState)
		{
		case State.Start:
		{
			AuditoryAwarenessComponent component = mOwner.GetComponent<AuditoryAwarenessComponent>();
			if (component != null)
			{
				component.CanBeHeard = false;
			}
			FireAtWillComponent component2 = mOwner.GetComponent<FireAtWillComponent>();
			if (component2 != null)
			{
				component2.Enabled = false;
			}
			mState = State.BeingDead;
			break;
		}
		case State.FadeOut:
			if (mFadeMaterials != null)
			{
				foreach (Material mFadeMaterial in mFadeMaterials)
				{
					mFadeMaterial.SetFloat("_Opacity", mAlpha);
				}
			}
			mAlpha -= Time.deltaTime / 2f;
			if (mAlpha <= 0f)
			{
				Object.Destroy(mActor.model.gameObject);
				if (mActor.realCharacter.Ragdoll != null)
				{
					Object.Destroy(mActor.realCharacter.Ragdoll.gameObject);
				}
				if (mActor.realCharacter.SimpleHitBounds != null)
				{
					Object.Destroy(mActor.realCharacter.SimpleHitBounds.gameObject);
				}
				if (mActor.realCharacter.mNavigationSetPiece != null)
				{
					Object.Destroy(mActor.realCharacter.mNavigationSetPiece.gameObject);
				}
				Object.Destroy(base.Owner.gameObject);
			}
			break;
		case State.BeingDead:
			break;
		}
	}

	public override bool HasFinished()
	{
		if (mState == State.Start)
		{
			return false;
		}
		return false;
	}

	public override void Finish()
	{
		ActorSelectUtils.EnableActorSelectCollider(mActor, true);
		ActorSelectUtils.UpdateActorSelectCollider(mActor, ActorSelectUtils.NormalColliderSettings[(int)mActor.awareness.faction]);
		if (mActor.health.Health <= 0f)
		{
			Debug.LogWarning(string.Format("{0} is having his Dead Task cancelled but is still dead. Could lead to oddness.", mActor.name));
		}
	}
}
