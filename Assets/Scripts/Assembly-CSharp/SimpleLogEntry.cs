using UnityEngine;

[RequireComponent(typeof(SpriteText))]
public class SimpleLogEntry : MonoBehaviour
{
	public float TimeOut = 2f;

	public float FadeOutDuration = 0.5f;

	private SpriteText mSpriteText;

	private Color mColour;

	private MonoBehaviour mScriptToCallOnRemove;

	private string mMethodToCallOnRemove;

	public void SetText(string text)
	{
		GetSpriteText();
		mSpriteText.Text = text;
	}

	public void SetRemovedCallback(MonoBehaviour script, string method)
	{
		mScriptToCallOnRemove = script;
		mMethodToCallOnRemove = method;
	}

	private void Start()
	{
		GetSpriteText();
	}

	private void GetSpriteText()
	{
		if (mSpriteText == null)
		{
			mSpriteText = base.gameObject.GetComponent<SpriteText>();
			mColour = mSpriteText.color;
		}
	}

	private void Update()
	{
		if (TimeOut == 0f)
		{
			return;
		}
		if (TimeOut > 0f)
		{
			TimeOut -= Time.deltaTime;
			if (TimeOut <= 0f)
			{
				TimeOut = -0.001f;
			}
			return;
		}
		TimeOut -= Time.deltaTime;
		if (TimeOut < 0f - FadeOutDuration)
		{
			RemoveMe();
			return;
		}
		Color color = mColour;
		color.a = mColour.a * (1f - TimeOut / (0f - FadeOutDuration));
		mSpriteText.SetColor(color);
	}

	private void RemoveMe()
	{
		if (base.transform.childCount > 0)
		{
			Transform child = base.transform.GetChild(0);
			if (child != null)
			{
				child.parent = base.transform.parent;
				child.SendMessage("OnReparent");
			}
		}
		if (mScriptToCallOnRemove != null && mMethodToCallOnRemove != null)
		{
			mScriptToCallOnRemove.Invoke(mMethodToCallOnRemove, 0f);
		}
		Object.Destroy(base.gameObject);
	}

	public void OnReparent()
	{
		Transform parent = base.transform.parent;
		if (parent != null)
		{
			base.transform.position = parent.position - new Vector3(0f, mSpriteText.BaseHeight, 0f);
		}
		TellChildrenToUpdateFromParentPosition();
	}

	private void TellChildrenToUpdateFromParentPosition()
	{
		if (base.transform.childCount > 0)
		{
			Transform child = base.transform.GetChild(0);
			if (child != null)
			{
				child.SendMessage("OnReparent");
			}
		}
	}
}
