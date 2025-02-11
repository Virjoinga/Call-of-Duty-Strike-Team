using System;
using UnityEngine;

public class RadialMenu : ContextMenuBase
{
	public float MenuRadius = 5f;

	protected override void PositionButtons()
	{
		float num = 5f;
		int num2 = mButtons.Length;
		float num3 = (float)Math.PI * 2f / (float)num2;
		float num4 = 0f;
		for (int i = 0; i < num2; i++)
		{
			mButtons[i].transform.localScale = Vector3.zero;
			mButtons[i].transform.position = base.transform.position;
			Vector3 vector = new Vector3((0f - num) * Mathf.Sin(num4), num * Mathf.Cos(num4), 0f);
			mButtons[i].gameObject.MoveTo(base.transform.position + vector, TransitionOnTime, 0f, EaseType.easeOutCubic);
			mButtons[i].gameObject.ScaleTo(Vector3.one, TransitionOnTime, 0f, EaseType.easeOutCubic);
			num4 += num3;
		}
	}

	public override void TransitionOff()
	{
		for (int i = 0; i < mButtons.Length; i++)
		{
			mButtons[i].gameObject.MoveTo(base.transform.position, TransitionOffTime, 0f, EaseType.linear);
			mButtons[i].gameObject.ScaleTo(Vector3.zero, TransitionOffTime, 0f, EaseType.linear, "DestroyButtons", base.gameObject);
		}
	}
}
