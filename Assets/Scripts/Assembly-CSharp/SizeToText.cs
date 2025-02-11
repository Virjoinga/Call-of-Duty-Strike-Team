using UnityEngine;

public class SizeToText : MonoBehaviour
{
	public SpriteText m_Text;

	public bool m_resetScale = true;

	private AutoSpriteBase m_Sprite;

	private bool m_IsNumeric;

	private void Awake()
	{
		if (m_resetScale)
		{
			base.transform.localScale = Vector3.one;
		}
	}

	private void Start()
	{
		m_Sprite = base.gameObject.GetComponent<AutoSpriteBase>();
		if (m_Text != null)
		{
			m_IsNumeric = false;
			int i;
			for (i = 0; i < m_Text.text.Length && m_Text.text[i] == ' '; i++)
			{
			}
			if (i < m_Text.text.Length && m_Text.text[i] >= '\0' && m_Text.text[i] <= '9')
			{
				m_IsNumeric = true;
			}
		}
	}

	private void OnEnable()
	{
		Update();
	}

	private void Update()
	{
		if ((bool)m_Text)
		{
			float num = (m_IsNumeric ? (m_Text.GetWidth("0") * (float)m_Text.text.Length) : m_Text.GetWidth(m_Text.Text));
			if (m_Sprite != null && m_Sprite.width != num)
			{
				m_Sprite.SetSize(num, m_Sprite.height);
				m_Sprite.pixelPerfect = false;
			}
		}
	}
}
