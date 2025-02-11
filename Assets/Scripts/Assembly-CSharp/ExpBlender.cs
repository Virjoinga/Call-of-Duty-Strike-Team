using UnityEngine;

public class ExpBlender
{
	private float tickTime;

	private float mDeltaScale = 1f;

	public void Reset()
	{
		tickTime = 0f;
		mDeltaScale = 1f;
	}

	public void Tick()
	{
		if (tickTime == 0f)
		{
			mDeltaScale = 1f;
		}
		else
		{
			mDeltaScale = (Time.time - tickTime) * 60f;
		}
		tickTime = Time.time;
	}

	public void ExpBlend(ref float fr, float to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = to * frac + fr * (1f - frac);
	}

	public float ExpBlend(float fr, float to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = to * frac + fr * (1f - frac);
		return fr;
	}

	public void CappedExpBlend(ref float fr, float to, float frac, float cap)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		cap *= mDeltaScale;
		float num = to * frac + fr * (1f - frac);
		if (num - fr > cap)
		{
			fr += cap;
		}
		else if (fr - num > cap)
		{
			fr -= cap;
		}
		else
		{
			fr = num;
		}
	}

	public float CappedExpBlend(float fr, float to, float frac, float cap)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		cap *= mDeltaScale;
		float num = to * frac + fr * (1f - frac);
		if (num - fr > cap)
		{
			fr += cap;
			return fr;
		}
		if (fr - num > cap)
		{
			fr -= cap;
			return fr;
		}
		return num;
	}

	public Vector3 ExpBlend(Vector3 fr, Vector3 to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = fr * (1f - frac) + to * frac;
		return fr;
	}

	public void ExpBlend(ref Vector3 fr, Vector3 to, float frac)
	{
		frac = mDeltaScale / (mDeltaScale + 1f / frac - 1f);
		fr = fr * (1f - frac) + to * frac;
	}
}
