using UnityEngine;

public class HaloEffect : MonoBehaviour
{
	public enum HaloColour
	{
		Red = 0,
		Green = 1,
		Cyan = 2,
		White = 3,
		Orange = 4,
		MintGreen = 5,
		Yellowish = 6
	}

	public enum BlinkPattern
	{
		On = 0,
		Off = 1,
		BlinkSlow = 2,
		BlinkMedium = 3,
		BlinkFast = 4
	}

	public Transform AttachPoint;

	public GameObject HaloModel;

	public float Size = 0.3f;

	public HaloColour Colour;

	public BlinkPattern Pattern;

	public float TimeOffset;

	private float OnTime = 1f;

	private float OffTime = 1f;

	private float mBlinkTimer;

	private float CamOffset = 0.1f;

	private void Start()
	{
		if (AttachPoint == null)
		{
			AttachPoint = base.transform;
		}
		HaloModel = Object.Instantiate(HaloModel, Vector3.zero, new Quaternion(0f, 180f, 0f, 1f)) as GameObject;
		if (HaloModel != null)
		{
			HaloModel.GetComponent<Renderer>().enabled = true;
			HaloModel.transform.parent = AttachPoint.transform;
			HaloModel.transform.localPosition = Vector3.zero;
			HaloModel.transform.localScale = new Vector3(Size, Size, Size);
			mBlinkTimer = TimeOffset;
			SetColour(Colour);
			SetBlinkPattern(Pattern);
		}
		else
		{
			TBFAssert.DoAssert(false);
			base.enabled = false;
		}
	}

	public void SetColour(HaloColour col)
	{
		Colour = col;
		switch (Colour)
		{
		case HaloColour.Red:
			HaloModel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 0f, 0f, 1f));
			break;
		case HaloColour.Green:
			HaloModel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0f, 1f, 0f, 1f));
			break;
		case HaloColour.Cyan:
			HaloModel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0f, 0.8f, 1f, 1f));
			break;
		default:
			HaloModel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f));
			break;
		case HaloColour.Orange:
			HaloModel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.88f, 0.6f, 0.31f, 1f));
			break;
		case HaloColour.MintGreen:
			HaloModel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(0.5f, 0.79f, 0.69f, 1f));
			break;
		case HaloColour.Yellowish:
			HaloModel.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, 1f));
			break;
		}
	}

	public void SetBlinkPattern(BlinkPattern pattern)
	{
		Pattern = pattern;
		HaloModel.GetComponent<Renderer>().enabled = true;
		switch (Pattern)
		{
		case BlinkPattern.On:
			break;
		case BlinkPattern.Off:
			HaloModel.GetComponent<Renderer>().enabled = false;
			break;
		case BlinkPattern.BlinkSlow:
			SetBlinkSpeed(1f);
			break;
		case BlinkPattern.BlinkMedium:
			SetBlinkSpeed(0.25f);
			break;
		case BlinkPattern.BlinkFast:
			SetBlinkSpeed(0.1f);
			break;
		}
	}

	public void SetBlinkSpeed(float speed)
	{
		OnTime = (OffTime = speed);
	}

	public void SetBlinkSpeed(float onTime, float offTime)
	{
		OnTime = onTime;
		OffTime = offTime;
	}

	private void LateUpdate()
	{
		switch (Pattern)
		{
		case BlinkPattern.On:
			UpdateHalo();
			break;
		case BlinkPattern.Off:
			break;
		case BlinkPattern.BlinkSlow:
		case BlinkPattern.BlinkMedium:
		case BlinkPattern.BlinkFast:
		{
			mBlinkTimer += Time.deltaTime;
			float num = OnTime + OffTime;
			while (mBlinkTimer > num)
			{
				mBlinkTimer -= num;
			}
			if (mBlinkTimer > OnTime)
			{
				HaloModel.GetComponent<Renderer>().enabled = false;
				break;
			}
			HaloModel.GetComponent<Renderer>().enabled = true;
			UpdateHalo();
			break;
		}
		}
	}

	private void UpdateHalo()
	{
		Transform transform = CameraManager.Instance.CurrentCamera.transform;
		HaloModel.transform.rotation = transform.rotation;
		HaloModel.transform.localPosition = Vector3.zero;
		HaloModel.transform.position -= HaloModel.transform.forward * CamOffset;
	}

	private void OnDrawGizmos()
	{
		Vector3 position = base.transform.position;
		if (AttachPoint != null)
		{
			position = AttachPoint.position;
		}
		Gizmos.color = Color.white;
		switch (Colour)
		{
		case HaloColour.Red:
			Gizmos.color = Color.red;
			break;
		case HaloColour.Green:
			Gizmos.color = Color.green;
			break;
		case HaloColour.Cyan:
			Gizmos.color = new Color(0f, 0.8f, 1f);
			break;
		case HaloColour.White:
			Gizmos.color = Color.white;
			break;
		default:
			Gizmos.color = Color.white;
			break;
		}
		Gizmos.DrawIcon(position, "halo_gizmo");
	}
}
