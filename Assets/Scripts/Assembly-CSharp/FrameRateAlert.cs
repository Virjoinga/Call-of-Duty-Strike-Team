using UnityEngine;

public class FrameRateAlert : MonoBehaviour
{
	public static bool DoFrameRateAlert;

	public float UpdateInterval = 1f;

	private float mAccum;

	private int mFrames;

	private float mTimeLeft;

	public float AlertNumber = 15f;

	private Material mMatRef;

	private void Awake()
	{
		if (!DoFrameRateAlert)
		{
			Object.Destroy(this);
		}
	}

	private void Update()
	{
		mTimeLeft -= Time.deltaTime;
		mAccum += Time.timeScale / Time.deltaTime;
		mFrames++;
		if (!((double)mTimeLeft <= 0.0))
		{
			return;
		}
		float num = mAccum / (float)mFrames;
		mTimeLeft = UpdateInterval;
		mAccum = 0f;
		mFrames = 0;
		if (mMatRef == null)
		{
			GameObject gameObject = GameObject.Find("PauseIcon");
			if (gameObject != null)
			{
				mMatRef = gameObject.GetComponent<Renderer>().sharedMaterial;
			}
		}
		if (num < AlertNumber)
		{
			if (mMatRef != null)
			{
				mMatRef.SetColor("_Color", Color.red);
			}
		}
		else if (num < AlertNumber + 5f)
		{
			if (mMatRef != null)
			{
				mMatRef.SetColor("_Color", Color.yellow);
			}
		}
		else if (mMatRef != null)
		{
			mMatRef.SetColor("_Color", Color.white);
		}
	}
}
