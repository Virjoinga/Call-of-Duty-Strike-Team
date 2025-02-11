using UnityEngine;

public class ListContextMenu : ContextMenuBase
{
	protected override void PositionButtons()
	{
		float num = 0.1f;
		int num2 = mButtons.Length;
		float height = mButtons[0].Button.height;
		float num3 = mButtons[0].Button.width + num;
		float num4 = num3 * (float)num2;
		Camera guiCamera = GUISystem.Instance.m_guiCamera;
		Vector3 vector = guiCamera.ScreenToWorldPoint(new Vector3(base.OriginatePosition.x, base.OriginatePosition.y, 0f));
		Vector3 position = vector;
		Vector3 position2 = vector + new Vector3(0f - num4 / 2f, height * 2f, 0f);
		Vector3 position3 = vector + new Vector3(num4 / 2f, 0f, 0f);
		Vector3 vector2 = guiCamera.WorldToScreenPoint(position2);
		bool flag = false;
		Vector3 position4 = base.OriginatePosition;
		position4.z = 0f;
		if (vector2.x < 0f)
		{
			position4.x -= vector2.x;
			flag = true;
		}
		if (vector2.y > (float)Screen.height)
		{
			position4.y -= vector2.y - (float)Screen.height;
			flag = true;
		}
		vector2 = guiCamera.WorldToScreenPoint(position3);
		if (vector2.x > (float)Screen.width)
		{
			position4.x -= vector2.x - (float)Screen.width;
			flag = true;
		}
		if (flag)
		{
			vector = guiCamera.ScreenToWorldPoint(position4);
		}
		for (int i = 0; i < num2; i++)
		{
			mButtons[i].transform.localScale = Vector3.zero;
			mButtons[i].transform.position = position;
			iTweenExtensions.MoveTo(position: vector + new Vector3(0f - num4 / 2f + (float)i * num3 + num3 / 2f, height, -0.1f), go: mButtons[i].gameObject, time: TransitionOnTime, delay: 0f, easeType: EaseType.easeOutCubic);
			mButtons[i].gameObject.ScaleTo(Vector3.one, TransitionOnTime, 0f, EaseType.easeOutCubic);
		}
	}

	public override void TransitionOff()
	{
		if (mButtons.Length == 0)
		{
			DestroyButtons();
			return;
		}
		for (int i = 0; i < mButtons.Length; i++)
		{
			mButtons[i].gameObject.MoveTo(base.OriginatePosition, TransitionOffTime, 0f, EaseType.linear);
			mButtons[i].gameObject.ScaleTo(Vector3.zero, TransitionOffTime, 0f, EaseType.linear, "DestroyButtons", base.gameObject);
		}
	}
}
