using UnityEngine;

public class MaterialShaderHelp : MonoBehaviour
{
	public void Awake()
	{
	}

	public void CheckShaders()
	{
		Shader shader = Shader.Find("Diffuse");
		Shader shader2 = Shader.Find("Bumped Specular");
		Shader shader3 = Shader.Find("Specular");
		Shader shader4 = Shader.Find("Corona/Lightmap/Base");
		Shader shader5 = Shader.Find("Corona/Lightmap/[Spec]");
		Object[] array = Object.FindObjectsOfType(typeof(MeshRenderer));
		for (int i = 0; i < array.Length; i++)
		{
			MeshRenderer meshRenderer = (MeshRenderer)array[i];
			if (meshRenderer.renderer.material.shader == shader)
			{
				Debug.LogWarning("Slow shader found " + shader.ToString() + ", please convert to " + shader4.ToString());
			}
			if (meshRenderer.renderer.material.shader == shader3)
			{
				Debug.LogWarning("Slow shader found " + shader3.ToString() + ", please convert to " + shader5.ToString());
			}
			if (meshRenderer.renderer.material.shader == shader2)
			{
				Debug.LogWarning("Slow shader found " + shader2.ToString() + ", please convert to " + shader5.ToString());
			}
		}
	}
}
