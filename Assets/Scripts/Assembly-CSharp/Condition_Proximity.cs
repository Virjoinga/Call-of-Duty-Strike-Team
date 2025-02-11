using UnityEngine;

public class Condition_Proximity : Condition
{
	public Transform TargetA;

	public Transform TargetB;

	public float Range;

	public override bool Value()
	{
		return TargetA != null && TargetB != null && (TargetA.position - TargetB.position).sqrMagnitude < Range * Range;
	}

	private void OnDrawGizmosSelected()
	{
		if (TargetA != null && TargetB != null)
		{
			Gizmos.color = Color.red;
			Vector3 normalized = (TargetB.position - TargetA.position).normalized;
			Vector3 vector = TargetA.position + Range * normalized;
			Gizmos.color = Color.green;
			Gizmos.DrawLine(TargetA.position, vector);
			Gizmos.DrawWireSphere(vector, 0.1f);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(vector, TargetB.position);
		}
	}
}
