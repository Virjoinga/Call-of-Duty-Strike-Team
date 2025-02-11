using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class AnimateButtonOnInvoke : MonoBehaviour
{
	public enum AnimType
	{
		PunchLeft = 0,
		PunchRight = 1,
		DropColourAndAlpha = 2,
		FlashYellow = 3
	}

	public AnimType AnimationType;

	public GameObject AnimateObject;

	private UIButton m_button;

	private MonoBehaviour m_scriptWithMethodToInvoke;

	private string m_methodToInvoke = string.Empty;

	private void Awake()
	{
		m_button = base.gameObject.GetComponentInChildren<UIButton>();
		if (m_button != null)
		{
			ReplaceButtonScript();
		}
		if (AnimateObject == null)
		{
			AnimateObject = base.gameObject;
		}
	}

	private void Start()
	{
	}

	public void ReplaceButtonScript()
	{
		if (m_button != null)
		{
			m_scriptWithMethodToInvoke = m_button.scriptWithMethodToInvoke;
			m_methodToInvoke = m_button.methodToInvoke;
			m_button.scriptWithMethodToInvoke = this;
			m_button.methodToInvoke = "OnButtonPress";
		}
	}

	public void ActivateButtonPress()
	{
		OnButtonPress();
	}

	private void OnButtonPress()
	{
		float time = 0.2f;
		iTween component = AnimateObject.GetComponent<iTween>();
		if (component == null)
		{
			switch (AnimationType)
			{
			case AnimType.PunchRight:
				AnimateObject.PunchPosition(new Vector3(1f, 0f, 0f), time, 0f);
				break;
			case AnimType.PunchLeft:
				AnimateObject.PunchPosition(new Vector3(-1f, 0f, 0f), time, 0f);
				break;
			case AnimType.DropColourAndAlpha:
				AnimateObject.ColorFrom(new Color(0.5f, 0.5f, 0.5f, 0.5f), time, 0f);
				break;
			case AnimType.FlashYellow:
				AnimateObject.ColorFrom(ColourChart.HudButtonPress, time, 0f);
				break;
			}
		}
		PlaySoundFx component2 = base.gameObject.GetComponent<PlaySoundFx>();
		if (component2 != null)
		{
			component2.Play();
		}
		else
		{
			InterfaceSFX.Instance.GeneralButtonPress.Play2D();
		}
		if (m_scriptWithMethodToInvoke != null)
		{
			m_scriptWithMethodToInvoke.Invoke(m_methodToInvoke, m_button.delay);
		}
	}
}
