using UnityEngine;

internal class SampleWindow
{
	private Vector2[] mSamples;

	private int mNextSampleIndex;

	public SampleWindow(int samples)
	{
		mSamples = new Vector2[samples];
	}

	public void Reset()
	{
		for (int i = 0; i < mSamples.Length; i++)
		{
			mSamples[i] = Vector2.zero;
		}
	}

	public void AddSample(Vector2 sample)
	{
		mSamples[mNextSampleIndex] = sample;
		mNextSampleIndex++;
		if (mNextSampleIndex == mSamples.Length)
		{
			mNextSampleIndex = 0;
		}
	}

	public Vector2 GetAverageValue()
	{
		Vector2 zero = Vector2.zero;
		for (int i = 0; i < mSamples.Length; i++)
		{
			zero += mSamples[i];
		}
		return zero / mSamples.Length;
	}
}
