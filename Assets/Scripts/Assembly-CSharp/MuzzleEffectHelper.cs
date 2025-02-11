using System.Collections;
using UnityEngine;

public class MuzzleEffectHelper : MonoBehaviour
{
	private ParticleSystem mPS;

	private void Awake()
	{
		mPS = GetComponentInChildren<ParticleSystem>();
	}

	private void OnEnable()
	{
		if (mPS != null)
		{
			mPS.Play();
			StartCoroutine(AutoTurnOff(mPS.duration));
		}
	}

	private IEnumerator AutoTurnOff(float duration)
	{
		yield return new WaitForSeconds(duration);
		mPS.Stop();
		base.gameObject.SetActive(false);
		base.gameObject.transform.parent = null;
	}
}
