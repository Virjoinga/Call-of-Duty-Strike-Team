using UnityEngine;

public class PlaceClaymore : MonoBehaviour
{
	public HackableObjectClaymore PlacedExplosive;

	private void Start()
	{
		GameObject gameObject = Object.Instantiate(ExplosionManager.Instance.Claymore) as GameObject;
		if (!(gameObject == null))
		{
			gameObject.transform.position = base.transform.position;
			PlacedExplosive = gameObject.GetComponentInChildren<HackableObjectClaymore>();
			if (PlacedExplosive != null)
			{
				PlacedExplosive.PlaceEnemyClaymore(null, base.transform.position, base.transform.forward);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Vector3 vector = base.transform.position + new Vector3(0f, 0.25f, 0f);
		Vector3 vector2 = vector + base.transform.forward * 1f;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(vector, vector2);
		Quaternion quaternion = Quaternion.AngleAxis(45f, Vector3.up);
		Vector3 to = vector2 - quaternion * base.transform.forward * 0.3f;
		Gizmos.DrawLine(vector2, to);
		Quaternion quaternion2 = Quaternion.AngleAxis(-45f, Vector3.up);
		Vector3 to2 = vector2 - quaternion2 * base.transform.forward * 0.3f;
		Gizmos.DrawLine(vector2, to2);
	}
}
