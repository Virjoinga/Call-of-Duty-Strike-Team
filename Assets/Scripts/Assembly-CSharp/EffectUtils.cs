using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EffectUtils
{
	public static IEnumerator FadeAndDestroyAfter(GameObject model, GameObject destroyObject, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		Renderer[] renderers = model.GetComponentsInChildren<Renderer>();
		ICollection<Material> fadingMaterials = EffectsController.Instance.Fade(renderers);
		float fade = 1f;
		while (fade > 0f)
		{
			fade -= TimeManager.DeltaTime;
			foreach (Material material in fadingMaterials)
			{
				material.SetFloat("_Opacity", fade);
			}
			yield return null;
		}
		Object.Destroy(destroyObject);
	}
}
