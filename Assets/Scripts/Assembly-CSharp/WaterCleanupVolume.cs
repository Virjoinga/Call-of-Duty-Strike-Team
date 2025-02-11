using UnityEngine;

public class WaterCleanupVolume : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		Actor componentInChildren = other.gameObject.GetComponentInChildren<Actor>();
		if (componentInChildren != null && componentInChildren.realCharacter.IsDead())
		{
			InterfaceableObject componentInChildren2 = componentInChildren.GetComponentInChildren<InterfaceableObject>();
			if (componentInChildren2 != null && componentInChildren2.AssociatedObject == componentInChildren.gameObject)
			{
				componentInChildren2.CancelMenu();
				componentInChildren2.enabled = false;
			}
			Object.Destroy(componentInChildren.model.gameObject);
			if (componentInChildren.realCharacter.Ragdoll != null)
			{
				Object.Destroy(componentInChildren.realCharacter.Ragdoll.gameObject);
			}
			if (componentInChildren.realCharacter.SimpleHitBounds != null)
			{
				Object.Destroy(componentInChildren.realCharacter.SimpleHitBounds.gameObject);
			}
			Object.Destroy(componentInChildren.gameObject);
		}
	}

	public void OnDrawGizmos()
	{
		BoxCollider componentInChildren = GetComponentInChildren<BoxCollider>();
		if (componentInChildren != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.red.Alpha(0.25f);
			Gizmos.DrawCube(componentInChildren.center, componentInChildren.size);
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(componentInChildren.center, componentInChildren.size);
		}
	}
}
