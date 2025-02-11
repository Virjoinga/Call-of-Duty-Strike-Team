using UnityEngine;

public class ResizeableButton : MonoBehaviour
{
	public AutoSpriteBase m_button;

	public AutoSpriteBase m_left;

	public AutoSpriteBase m_right;

	private void LateUpdate()
	{
		if (m_button != null)
		{
			float num = m_button.width / 2f;
			if (m_left != null)
			{
				m_left.transform.localPosition = new Vector3(0f - num + 1f, 0f, 0f);
			}
			if (m_right != null)
			{
				m_right.transform.localPosition = new Vector3(num - 1f, 0f, 0f);
			}
		}
	}
}
