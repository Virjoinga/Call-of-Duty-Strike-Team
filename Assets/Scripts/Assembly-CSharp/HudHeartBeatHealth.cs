using UnityEngine;

public class HudHeartBeatHealth : MonoBehaviour
{
	public UIProgressBar[] BeatBars;

	private int mMaxBeats;

	private float mBeatProgress;

	private int mFirstBeatId;

	private HealthComponent mHealth;

	private float mPreviousHealthLevel;

	private void Start()
	{
		mMaxBeats = BeatBars.Length;
		SetupForNewUnit();
	}

	private void SetBarForHeath(UIProgressBar bar, float health)
	{
		int animFrameForHealth = ColourChart.GetAnimFrameForHealth(health);
		bar.SetFrame(0, animFrameForHealth);
		bar.GetComponent<Renderer>().material.color = Color.white;
		if (mPreviousHealthLevel != (float)animFrameForHealth)
		{
			SoundManager.Instance.ForceHealthFilterOntoExistingSFX((int)mPreviousHealthLevel);
		}
		mPreviousHealthLevel = animFrameForHealth;
	}

	private void Update()
	{
		int num = Mathf.FloorToInt(mBeatProgress);
		mBeatProgress += Time.deltaTime * 2f;
		int num2 = Mathf.FloorToInt(mBeatProgress);
		if (num != num2)
		{
			mFirstBeatId++;
			if (mFirstBeatId >= mMaxBeats)
			{
				mFirstBeatId -= mMaxBeats;
			}
			mBeatProgress -= 1f;
			if ((bool)mHealth)
			{
				SetBarForHeath(BeatBars[mFirstBeatId], mHealth.Health01);
			}
			else
			{
				SetBarForHeath(BeatBars[mFirstBeatId], 1f);
			}
		}
		BeatBars[mFirstBeatId].Value = mBeatProgress;
		int num3 = mFirstBeatId + 1;
		if (num3 >= mMaxBeats)
		{
			num3 = 0;
		}
		Color color = BeatBars[num3].GetComponent<Renderer>().material.color;
		color.a = 1f - mBeatProgress * 4f;
		color.a = Mathf.Clamp01(color.a);
		BeatBars[num3].GetComponent<Renderer>().material.color = color;
	}

	public void SetupForNewUnit(Actor actor)
	{
		if (actor != null)
		{
			mHealth = actor.health;
			UIProgressBar[] beatBars = BeatBars;
			foreach (UIProgressBar uIProgressBar in beatBars)
			{
				SetBarForHeath(uIProgressBar, mHealth.Health01);
				uIProgressBar.Value = 0f;
			}
		}
		mBeatProgress = 0f;
		mFirstBeatId = 0;
	}

	public void SetupForNewUnit()
	{
		Actor actor = null;
		if ((bool)GameController.Instance)
		{
			actor = GameController.Instance.mFirstPersonActor;
			SetupForNewUnit(actor);
		}
	}
}
