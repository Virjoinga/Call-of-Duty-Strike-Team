using System.Collections.Generic;
using UnityEngine;

public class NewsTicker : MonoBehaviour
{
	private const int NUM_TICKER_STRINGS = 7;

	public SpriteText TickerEntryPrefab;

	public SimpleSprite Background;

	public float TickerSpeed = 1f;

	public float EntrySpacing = 10f;

	private List<SpriteText> mEntrys;

	private float mTickerOffset;

	private void Start()
	{
		Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width * 0.5f, 0f, 0f));
		if (Background != null)
		{
			float num = CommonHelper.CalculatePixelSizeInWorldSpace(base.transform);
			int width = Background.GetComponent<Renderer>().material.mainTexture.width;
			int height = Background.GetComponent<Renderer>().material.mainTexture.height;
			Background.Setup((float)Screen.width * num, (float)height * num * 0.5f, new Vector2((float)width * 0.5f, height), new Vector2(1f, height));
			position.y += Background.height * 0.3f;
			Background.transform.position = position;
		}
		base.transform.position = position;
		RefreshEntryList();
	}

	private void Update()
	{
		mTickerOffset -= Time.deltaTime * TickerSpeed;
		bool flag = true;
		bool flag2 = false;
		float num = mTickerOffset;
		for (int i = 0; i < mEntrys.Count; i++)
		{
			SpriteText spriteText = mEntrys[i];
			Vector3 position = base.transform.position;
			position.x += num;
			position.y += spriteText.BaseHeight * 0.2f;
			position.z = -1f;
			spriteText.gameObject.transform.position = position;
			if (flag && Camera.main != null)
			{
				flag = false;
				float orthographicSize = Camera.main.orthographicSize;
				if (mTickerOffset + spriteText.TotalWidth + EntrySpacing < 0f - orthographicSize)
				{
					flag2 = true;
				}
			}
			else if (Camera.main == null)
			{
				Debug.Log("WARNING: News ticker trying to update with no camera!");
			}
			num += spriteText.TotalWidth;
			num += EntrySpacing;
		}
		if (flag2)
		{
			SpriteText spriteText2 = mEntrys[0];
			mEntrys.Remove(spriteText2);
			mEntrys.Add(spriteText2);
			mTickerOffset += spriteText2.TotalWidth + EntrySpacing;
		}
	}

	public void RefreshEntryList()
	{
		if (mEntrys != null)
		{
			for (int i = 0; i < mEntrys.Count; i++)
			{
				Object.Destroy(mEntrys[i].gameObject);
			}
		}
		mEntrys = new List<SpriteText>();
		mTickerOffset = 0f;
		string format = Language.Get("S_XP_REMAINING_TICKER_MSG");
		int level = 0;
		int prestigeLevel = 0;
		float percent = 0f;
		int xpToNextLevel = 0;
		XPManager.Instance.ConvertXPToLevel(StatsHelper.PlayerXP(), out level, out prestigeLevel, out xpToNextLevel, out percent);
		format = string.Format(format, xpToNextLevel);
		for (int j = 1; j < 7; j++)
		{
			if (j + 1 != 5)
			{
				AddEntry(Language.Get("S_NEWS_TICKER_" + (j + 1).ToString("D2")).ToUpper());
				if ((mEntrys.Count + 1) % 3 == 0)
				{
					AddEntry(format.ToUpper());
				}
			}
		}
	}

	private void AddEntry(string text)
	{
		GameObject gameObject = Object.Instantiate(TickerEntryPrefab.gameObject) as GameObject;
		gameObject.transform.parent = base.transform;
		Vector3 position = new Vector3(100f, 0f, 0f);
		gameObject.transform.position = position;
		SpriteText component = gameObject.GetComponent<SpriteText>();
		component.Text = text;
		mEntrys.Add(component);
	}
}
