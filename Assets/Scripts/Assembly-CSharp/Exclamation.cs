using UnityEngine;

public class Exclamation : BaseActorComponent
{
	private GameObject topBox;

	private GameObject bottomBox;

	public static bool ExclamationsEnabled;

	private static float closestToCentre = float.MaxValue;

	private static Exclamation lastClosest;

	private static Exclamation lastToShowMessage;

	private void Awake()
	{
		if (Application.isEditor)
		{
			ExclamationsEnabled = true;
		}
		topBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
		topBox.renderer.material.shader = Shader.Find("VertexLit");
		bottomBox = Object.Instantiate(topBox) as GameObject;
		Object.Destroy(topBox.GetComponent<BoxCollider>());
		Object.Destroy(bottomBox.GetComponent<BoxCollider>());
		topBox.transform.localScale = new Vector3(0.5f, 2f, 0.5f);
		bottomBox.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		bottomBox.transform.parent = base.transform;
		topBox.transform.parent = bottomBox.transform;
		topBox.transform.localPosition = new Vector3(0f, 4f, 0f);
		bottomBox.transform.localPosition = new Vector3(0f, 2.5f, 0f);
		Reset();
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (!bottomBox.activeSelf)
		{
			return;
		}
		Vector3 v = CameraManager.Instance.CurrentCamera.WorldToViewportPoint(bottomBox.transform.position);
		v -= new Vector3(0.5f, 0.5f, 0.5f);
		float sqrMagnitude = v.xy().sqrMagnitude;
		bottomBox.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		if (lastClosest == this)
		{
			closestToCentre = sqrMagnitude;
			if (sqrMagnitude < 0.02f)
			{
				if (lastToShowMessage == null && HUDMessenger.Instance != null)
				{
					HUDMessenger.Instance.ClearAllMessages();
					lastToShowMessage = this;
					HUDMessenger.Instance.PushMessage(topBox.name, false);
				}
				bottomBox.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f);
			}
			else
			{
				lastToShowMessage = null;
			}
		}
		else if (sqrMagnitude < closestToCentre)
		{
			closestToCentre = sqrMagnitude;
			lastClosest = this;
			lastToShowMessage = null;
		}
	}

	public void Error(string err)
	{
		if (ExclamationsEnabled)
		{
			bottomBox.SetActive(true);
			bottomBox.renderer.material.color = Color.red;
			topBox.renderer.material.color = Color.red;
			bottomBox.renderer.material.SetColor("_Emission", Color.red);
			topBox.renderer.material.SetColor("_Emission", Color.red);
			bottomBox.name = "ERROR:";
			if (!topBox.name.Contains(err))
			{
				GameObject obj = topBox;
				obj.name = obj.name + err + "!";
			}
		}
	}

	public void Warning(string err)
	{
		if (ExclamationsEnabled)
		{
			bottomBox.SetActive(true);
			if (bottomBox.renderer.material.color == Color.white)
			{
				bottomBox.renderer.material.color = Color.yellow;
				topBox.renderer.material.color = Color.yellow;
				bottomBox.renderer.material.SetColor("_Emission", Color.yellow);
				topBox.renderer.material.SetColor("_Emission", Color.yellow);
				bottomBox.name = "WARNING:";
			}
			if (!topBox.name.Contains(err))
			{
				GameObject obj = topBox;
				obj.name = obj.name + err + "!";
			}
		}
	}

	public void Reset()
	{
		bottomBox.SetActive(false);
		bottomBox.name = "!";
		topBox.name = "!";
		bottomBox.renderer.material.color = Color.white;
	}

	public void Clear(string err)
	{
		if (!ExclamationsEnabled)
		{
			return;
		}
		int num = topBox.name.IndexOf(err);
		if (num >= 0)
		{
			topBox.name = topBox.name.Remove(num, err.Length + 1);
			if (topBox.name.Length == 1)
			{
				Reset();
			}
		}
	}
}
