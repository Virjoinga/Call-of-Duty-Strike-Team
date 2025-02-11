using UnityEngine;

public class RPGOverride : ContainerOverride
{
	public RPGOverrideData m_RPGOverrideData = new RPGOverrideData();

	public SpawnerData m_SpawnerOverrideData = new SpawnerData();

	public OnDeadData m_OnDeadOverrideData = new OnDeadData();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		m_SpawnerOverrideData.ResolveGuidLinks();
		Spawner spawner = cont.FindComponentOfType(typeof(Spawner)) as Spawner;
		if (spawner != null)
		{
			spawner.m_Interface = m_SpawnerOverrideData;
			m_SpawnerOverrideData.CopyContainerData(spawner);
		}
		TargetSwitchComponent targetSwitchComponent = cont.FindComponentOfType(typeof(TargetSwitchComponent)) as TargetSwitchComponent;
		if (targetSwitchComponent != null)
		{
			m_RPGOverrideData.TargetSwitcher = targetSwitchComponent;
		}
		RPGDescriptor rPGDescriptor = cont.FindComponentOfType(typeof(RPGDescriptor)) as RPGDescriptor;
		if (rPGDescriptor != null)
		{
			rPGDescriptor.m_Interface = m_RPGOverrideData;
		}
		OnEnemyDead_SendMessage onEnemyDead_SendMessage = cont.FindComponentOfType(typeof(OnEnemyDead_SendMessage)) as OnEnemyDead_SendMessage;
		if (onEnemyDead_SendMessage != null)
		{
			onEnemyDead_SendMessage.m_Interface = m_OnDeadOverrideData;
		}
		EnemyFromDoorOverride enemyFromDoorOverride = cont.FindComponentOfType(typeof(EnemyFromDoorOverride)) as EnemyFromDoorOverride;
		if (enemyFromDoorOverride != null)
		{
			enemyFromDoorOverride.m_OverrideData.Speed = 5f;
			enemyFromDoorOverride.m_OverrideData.TimeToWalkOut = 0.8f;
		}
	}

	public override void CreateAssociatedDefaultObjects(Container cont)
	{
		ContainerOverride containerOverride = cont.gameObject.GetComponent(typeof(ContainerOverride)) as ContainerOverride;
		m_SpawnerOverrideData.EventsList = containerOverride.CreateNewBagObject("Script/Character/Behaviours/Character Event Listeners", "Event Listeners");
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		Spawner componentInChildren = gameObj.GetComponentInChildren<Spawner>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	public override void SendOverrideMessageWithParam(GameObject gameObj, string methodName, GameObject param)
	{
		base.SendOverrideMessageWithParam(gameObj, methodName, param);
		Spawner componentInChildren = gameObj.GetComponentInChildren<Spawner>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName, param);
		}
	}
}
