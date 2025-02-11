using System;
using UnityEngine;

public class ForceOnDeathTriggerOverride : ContainerOverride
{
	private Vector3 directionOfForce = Vector3.zero;

	public float angle;

	public float forceAmount = 100f;

	public override void ApplyOverride(Container cont)
	{
		base.ApplyOverride(cont);
		ForceOnDeathTriggerVolume forceOnDeathTriggerVolume = cont.FindComponentOfType(typeof(ForceOnDeathTriggerVolume)) as ForceOnDeathTriggerVolume;
		if (forceOnDeathTriggerVolume != null)
		{
			forceOnDeathTriggerVolume.Angle = angle;
			forceOnDeathTriggerVolume.ForceAmount = forceAmount;
		}
	}

	public void OnDrawGizmos()
	{
		BoxCollider componentInChildren = GetComponentInChildren<BoxCollider>();
		if (componentInChildren != null)
		{
			float f = angle * ((float)Math.PI / 180f);
			directionOfForce = new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			Gizmos.color = Color.black;
			Vector3 vector = directionOfForce;
			vector.Normalize();
			Vector3 vector2 = base.transform.position + vector * 3f;
			Gizmos.DrawLine(base.transform.position, vector2);
			float num = Vector3.Angle(vector, base.transform.forward);
			float y = vector2.x - base.transform.position.x;
			float x = vector2.z - base.transform.position.z;
			num = Mathf.Atan2(y, x) * 180f / (float)Math.PI;
			if (num < 0f)
			{
				num += 360f;
			}
			Quaternion quaternion = Quaternion.AngleAxis(num + 30f, Vector3.up);
			Vector3 from = vector2 - quaternion * Vector3.forward * 0.8f;
			Gizmos.DrawLine(from, vector2);
			Quaternion quaternion2 = Quaternion.AngleAxis(num - 30f, Vector3.up);
			Vector3 to = vector2 - quaternion2 * Vector3.forward * 0.8f;
			Gizmos.DrawLine(vector2, to);
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.magenta.Alpha(0.25f);
			Gizmos.DrawCube(componentInChildren.center, componentInChildren.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(componentInChildren.center, componentInChildren.size);
		}
	}
}
