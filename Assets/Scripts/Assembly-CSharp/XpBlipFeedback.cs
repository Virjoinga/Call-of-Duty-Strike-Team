using System.Collections;
using UnityEngine;

public class XpBlipFeedback : MonoBehaviour
{
	public SpriteText ScoreSprite;

	public SpriteText MsgSprite;

	private float timeout;

	public void Display(int score, string msg, GameObject target, float minTimeBetweenXPMessages)
	{
		if (StartCoroutine(DoDisplay(score, msg, target, minTimeBetweenXPMessages)) == null)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator DoDisplay(int score, string msg, GameObject target, float minTimeBetweenXPMessages)
	{
		timeout = minTimeBetweenXPMessages;
		string xpTextStr = Language.Get("S_RESULT_XP");
		ScoreSprite.Text = ((score != 0) ? ("+" + score + xpTextStr) : string.Empty);
		if (msg == string.Empty)
		{
			Object.Destroy(MsgSprite);
		}
		else
		{
			MsgSprite.Text = msg;
		}
		Vector3 cachePos = ScoreSprite.transform.position;
		ScoreSprite.transform.position = cachePos + new Vector3(0f, 0f, -1000f);
		yield return new WaitForEndOfFrame();
		ScoreSprite.transform.position = cachePos;
		ScoreSprite.gameObject.ScaleFrom(new Vector3(3f, 3f, 3f), 0.33f, 0f, EaseType.spring);
		ScoreSprite.gameObject.ColorFrom(new Color(1f, 1f, 1f, 0f), 0.1f, 0f);
		if (MsgSprite != null)
		{
			MsgSprite.gameObject.ScaleFrom(new Vector3(3f, 3f, 3f), 0.33f, 0f, EaseType.spring);
			MsgSprite.gameObject.ColorFrom(new Color(1f, 1f, 1f, 0f), 0.1f, 0f);
		}
		float startTime = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup - startTime < timeout)
		{
			yield return null;
		}
		Object.Destroy(base.gameObject);
	}
}
