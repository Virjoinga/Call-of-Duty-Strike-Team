using UnityEngine;

public class BundleManagerProgress : MonoBehaviour
{
	private void Start()
	{
		if (!base.GetComponent<GUIText>())
		{
			Debug.Log("BundleManagerProgress needs a GUIText component!");
			base.enabled = false;
		}
		else
		{
			Vector2 pixelOffset = base.GetComponent<GUIText>().pixelOffset;
			pixelOffset.y = (float)Screen.height / 2f;
			base.GetComponent<GUIText>().pixelOffset = pixelOffset;
		}
	}

	private void Update()
	{
		if (BundleManager.Instance.IsReady())
		{
			base.GetComponent<GUIText>().text = "DLC bundles downloaded!";
			base.GetComponent<GUIText>().material.color = Color.green;
		}
		else
		{
			base.GetComponent<GUIText>().text = "DLC bundles downloading...";
			base.GetComponent<GUIText>().material.color = Color.red;
		}
	}
}
