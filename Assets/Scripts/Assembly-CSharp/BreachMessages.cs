using System;

[Serializable]
public class BreachMessages
{
	public ObjectMessage[] SequenceStarted = new ObjectMessage[0];

	public ObjectMessage[] BreachStarted = new ObjectMessage[0];

	public ObjectMessage[] AllEnemiesDead = new ObjectMessage[0];

	public ObjectMessage[] AllEnemiesDeadDuringSlowMotion = new ObjectMessage[0];

	public ObjectMessage[] SlowMotionFinished = new ObjectMessage[0];
}
