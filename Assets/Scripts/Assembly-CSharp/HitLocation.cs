using UnityEngine;

public class HitLocation : MonoBehaviour
{
	public GameObject Owner;

	public string Location;

	public Transform Bone;

	public float DamageMultiplier;

	public Actor Actor;

	public HealthComponent Health;

	public float Mass;

	public string LocationName;

	public float[] SiblingForceFraction;
}
