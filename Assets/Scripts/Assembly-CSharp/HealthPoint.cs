using UnityEngine;

public class HealthPoint : MonoBehaviour
{
	public float RegenerationRate = 8f;

	public float RadiusOfEffect = 2f;

	private ActorIdentIterator myActorIdentIterator = new ActorIdentIterator(0u);

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		float num = RadiusOfEffect * RadiusOfEffect;
		ActorIdentIterator actorIdentIterator = myActorIdentIterator.ResetWithMask(GKM.AliveMask);
		Actor a;
		while (actorIdentIterator.NextActor(out a))
		{
			if (!(a.health.Health >= a.health.HealthMax))
			{
				float sqrMagnitude = (new Vector2(base.transform.position.x, base.transform.position.z) - a.realCharacter.GetPositionGproj()).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					float amount = RegenerationRate * Time.deltaTime;
					a.health.ModifyHealth(base.gameObject, amount, "Healing", a.GetPosition() - base.transform.position, false);
				}
			}
		}
	}
}
