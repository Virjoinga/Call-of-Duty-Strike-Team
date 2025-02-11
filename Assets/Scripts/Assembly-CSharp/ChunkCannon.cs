using UnityEngine;

public class ChunkCannon : MonoBehaviour
{
	public GameObject[] Models;

	public int ChunksPerFire = 1;

	public void Fire()
	{
		if (Models == null || Models.Length < 1)
		{
			return;
		}
		for (int i = 0; i < ChunksPerFire; i++)
		{
			GameObject gameObject = null;
			GameObject gameObject2 = Models[Random.Range(0, Models.Length)];
			if (gameObject2 != null)
			{
				gameObject = Object.Instantiate(gameObject2) as GameObject;
				gameObject.SetActive(true);
				gameObject.transform.ParentAndZeroLocalPositionAndRotation(base.transform);
				CutsceneLighting cutsceneLighting = gameObject.AddComponent<CutsceneLighting>();
				cutsceneLighting.ProbeAnchor = gameObject.transform;
				cutsceneLighting.GroupRoots = new Transform[1] { gameObject.transform };
				Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = 10f;
				rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
				rigidbody.AddForce(60f * (base.transform.forward + 0.3f * Random.insideUnitSphere), ForceMode.Impulse);
				rigidbody.AddTorque(100f * Random.insideUnitSphere, ForceMode.Impulse);
				StartCoroutine(EffectUtils.FadeAndDestroyAfter(gameObject, gameObject, 2f));
			}
		}
	}
}
