using UnityEngine;

public class FirstPersonPenaliser : MonoBehaviour
{
	public enum EventEnum
	{
		FirstPersonHeadshot = 0,
		FirstPersonShotEnemy = 1,
		FirstPersonNearMiss = 2,
		ThirdPersonSuppression = 3,
		ThirdPersonMultiSuppression = 4,
		ThirdPersonStealthKill = 5,
		ThirdPersonAimedShot = 6,
		Count = 7
	}

	private static FirstPersonPenaliser instance;

	private float penalty;

	private Actor fpAimedAt;

	private Actor lastAimedAt;

	private float startedAimingAtTime;

	private static bool mEnabled = true;

	public float Penalty
	{
		get
		{
			return penalty;
		}
	}

	private void Awake()
	{
		instance = this;
		penalty = 0f;
		mEnabled = true;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static void EventOccurred(EventEnum e)
	{
		if (mEnabled)
		{
			instance.penalty = Mathf.Clamp(instance.penalty + GlobalBalanceTweaks.eventCosts[(int)e], 0f, 1f);
		}
	}

	private void Update()
	{
		fpAimedAt = null;
		if (GameController.Instance.FirstPersonTarget != null && GameController.Instance.mFirstPersonActor != null && GameController.Instance.mFirstPersonActor.awareness.IsEnemy(GameController.Instance.FirstPersonTarget))
		{
			fpAimedAt = GameController.Instance.FirstPersonTarget;
			if (lastAimedAt != fpAimedAt)
			{
				lastAimedAt = fpAimedAt;
				startedAimingAtTime = Time.time;
			}
		}
		else if (Time.time > startedAimingAtTime + 2f)
		{
			startedAimingAtTime = 0f;
			lastAimedAt = null;
		}
	}

	public static bool ShouldDodge(Actor a)
	{
		if (instance.fpAimedAt == a)
		{
			float num = Time.time - instance.startedAimingAtTime;
			if (num > Mathf.Lerp(GlobalBalanceTweaks.kMaximumDodgeTime, GlobalBalanceTweaks.kMinimumDodgeTime, instance.penalty))
			{
				return true;
			}
		}
		return false;
	}

	public static float ApplyAccuracyBonus(float acc)
	{
		return Mathf.Min(0.95f, acc + GlobalBalanceTweaks.kBaseAccuracyModifier + instance.penalty * GlobalBalanceTweaks.kPenaltyToAccuracyScale);
	}

	public static Actor PickTarget(Actor a, Actor target)
	{
		if (target != null && GameController.Instance.IsFirstPerson && GameController.Instance.mFirstPersonActor.awareness.IsEnemy(a) && (a.awareness.EnemiesIKnowAbout & GameController.Instance.mFirstPersonActor.ident) != 0 && !a.awareness.Obstructed(GameController.Instance.mFirstPersonActor))
		{
			float sqrMagnitude = (a.GetPosition() - target.GetPosition()).sqrMagnitude;
			float sqrMagnitude2 = (a.GetPosition() - GameController.Instance.mFirstPersonActor.GetPosition()).sqrMagnitude;
			if ((1f - instance.penalty * 0.4f) * sqrMagnitude2 < sqrMagnitude)
			{
				return GameController.Instance.mFirstPersonActor;
			}
		}
		return target;
	}

	public static void EnablePenalty(bool val)
	{
		if (val && !mEnabled)
		{
			instance.penalty = 0f;
		}
		mEnabled = val;
	}
}
