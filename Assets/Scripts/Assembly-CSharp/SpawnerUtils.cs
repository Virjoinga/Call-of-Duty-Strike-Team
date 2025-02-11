public static class SpawnerUtils
{
	public static void InitialiseSpawnedActor(Actor actor, AITetherPoint aiTetherPoint, BehaviourController.AlertState releaseAlerted, ActorWrapper preferredTarget)
	{
		if (aiTetherPoint != null && !actor.behaviour.PlayerControlled)
		{
			actor.tether.TetherToAITetherPoint(aiTetherPoint);
		}
		if (BehaviourController.IsActiveAlertState(releaseAlerted))
		{
			actor.behaviour.ResetAlerted = true;
		}
		actor.behaviour.alertState = releaseAlerted;
		actor.behaviour.nextAlertState = releaseAlerted;
		if (BehaviourController.IsActiveAlertState(releaseAlerted))
		{
			actor.behaviour.SetAlertPerceptionState();
		}
		if (preferredTarget != null)
		{
			actor.behaviour.PreferredTarget = preferredTarget;
		}
	}
}
