using UnityEngine;

public class PlacedC4Override : ContainerOverride
{
	public PlacedC4Data m_OverrideData = new PlacedC4Data();

	public override void SetupOverride(Container cont)
	{
		ApplyOverride(cont);
	}

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		PlacedC4 placedC = cont.FindComponentOfType(typeof(PlacedC4)) as PlacedC4;
		if (!(placedC != null))
		{
			return;
		}
		placedC.mInterface = m_OverrideData;
		if (!placedC.mInterface.m_NotifyFlashpointManager)
		{
			return;
		}
		SetPieceLogic setPieceLogic = cont.FindComponentOfType(typeof(SetPieceLogic)) as SetPieceLogic;
		if (setPieceLogic != null)
		{
			Object[] array = IncludeDisabled.FindSceneObjectsOfType(typeof(GMGFlashpointManager));
			int num = 0;
			if (num < array.Length)
			{
				GMGFlashpointManager flashPointmanager = (GMGFlashpointManager)array[num];
				setPieceLogic.m_FlashPointmanager = flashPointmanager;
			}
		}
	}

	public override void SendOverrideMessage(GameObject gameObj, string methodName)
	{
		base.SendOverrideMessage(gameObj, methodName);
		PlacedC4 componentInChildren = gameObj.GetComponentInChildren<PlacedC4>();
		if (componentInChildren != null)
		{
			componentInChildren.SendMessage(methodName);
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = base.transform.position + new Vector3(0f, 0.25f, 0f) + base.transform.forward * -0.1f;
		Vector3 from = vector + base.transform.forward * -1.1f;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(from, vector);
		Quaternion quaternion = Quaternion.AngleAxis(45f, Vector3.up);
		Vector3 to = vector - quaternion * base.transform.forward * 0.3f;
		Gizmos.DrawLine(vector, to);
		Quaternion quaternion2 = Quaternion.AngleAxis(-45f, Vector3.up);
		Vector3 to2 = vector - quaternion2 * base.transform.forward * 0.3f;
		Gizmos.DrawLine(vector, to2);
	}
}
