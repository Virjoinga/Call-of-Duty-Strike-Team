using UnityEngine;

public class SpecularSource : MonoBehaviour
{
	public static Vector4 specdir;

	private void Start()
	{
		Apply();
	}

	private void GrabLightVectors()
	{
		specdir = base.transform.forward * -1f;
	}

	public void Apply()
	{
		GrabLightVectors();
		Shader.SetGlobalVector("_SpecDir", specdir);
	}
}
