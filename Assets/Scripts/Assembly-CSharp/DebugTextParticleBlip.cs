using UnityEngine;

public class DebugTextParticleBlip : MonoBehaviour
{
	private SpriteText mSpriteText;

	private string mText = "DBG";

	private float mLifeTime;

	private Transform mFollowThis;

	private float mLocalY;

	private void Start()
	{
		mSpriteText = GetComponent<SpriteText>();
		SetText(mText);
		mLifeTime = 2f;
	}

	public void SetText(string text)
	{
		mText = text;
		if (mSpriteText != null)
		{
			mSpriteText.Text = mText;
		}
	}

	public void SetParent(Transform t)
	{
		mFollowThis = t;
		if (mFollowThis != null)
		{
			base.transform.position = mFollowThis.position;
		}
	}

	private void Update()
	{
		if (mFollowThis != null)
		{
			base.transform.position = mFollowThis.position;
		}
		mLifeTime -= Time.deltaTime;
		Vector3 localPosition = base.transform.localPosition;
		mLocalY += Time.deltaTime * 2f;
		localPosition.y = mLocalY;
		base.transform.localPosition = localPosition;
		if (mLifeTime <= 0f)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
