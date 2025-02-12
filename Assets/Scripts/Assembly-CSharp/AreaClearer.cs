using UnityEngine;

public class AreaClearer : MonoBehaviour
{
	private const float kTinySizeToStartWith = 0.05f;

	private float mRadius;

	private float mEndTime;

	private UnityEngine.AI.NavMeshAgent mNavAgent;

	private float mTime;

	private float mDuration;

	private bool mForceLastFrame = true;

	private Vector3 mStartPosition;

	private void Start()
	{
		mNavAgent = base.gameObject.AddComponent<UnityEngine.AI.NavMeshAgent>();
		mNavAgent.radius = 0.05f;
		mTime = 0f;
		mStartPosition = base.transform.position;
	}

	public void Initialise(float radius, float duration)
	{
		mRadius = radius;
		mDuration = duration;
	}

	private void Update()
	{
		base.transform.position = mStartPosition + Quaternion.Euler(0f, Random.Range(0f, 360f), 0f) * new Vector3(0f, 0f, 0.05f);
		mTime += Time.deltaTime;
		mNavAgent.radius = Mathf.Lerp(0.05f, mRadius, Mathf.Clamp01(mTime / mDuration));
		if (mTime > mDuration)
		{
			if (mForceLastFrame)
			{
				mForceLastFrame = false;
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
