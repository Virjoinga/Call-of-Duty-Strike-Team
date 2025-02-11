using UnityEngine;

public class BundleManagerProgress : MonoBehaviour
{
	private void Start()
	{
		if (!base.guiText)
		{
			Debug.Log("BundleManagerProgress needs a GUIText component!");
			base.enabled = false;
		}
		else
		{
			Vector2 pixelOffset = base.guiText.pixelOffset;
			pixelOffset.y = (float)Screen.height / 2f;
			base.guiText.pixelOffset = pixelOffset;
		}
	}

	private void Update()
	{
		if (BundleManager.Instance.IsReady())
		{
			base.guiText.text = "DLC bundles downloaded!";
			base.guiText.material.color = Color.green;
		}
		else
		{
			base.guiText.text = "DLC bundles downloading...";
			base.guiText.material.color = Color.red;
		}
	}
}
