using UnityEngine;

public class SniperScope : MonoBehaviour
{
	public Material ScopeMaterial;

	public void OnPostRender()
	{
		if (GameController.Instance != null && GameController.Instance.AimimgDownScopeThisFrame)
		{
			ScopeMaterial.mainTexture = CameraManager.Instance.SniperTexture;
			ScopeMaterial.SetTexture("_ScopeTex", ViewModelRig.Instance().GetScopeTexture());
		}
	}
}
