using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UIButton))]
public class BouncyButton : MonoBehaviour
{
	public enum ButtonType
	{
		Category = 0,
		Deployment = 1,
		Back = 2,
		Default = 3
	}

	public ButtonType m_ButtonType = ButtonType.Default;

	public GameObject m_bouncingObject;

	public float m_pulseTime;

	public Vector3 m_pulseAmount = new Vector3(1.1f, 1.1f, 1f);

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
		if (m_bouncingObject == null)
		{
			m_bouncingObject = base.gameObject;
		}
	}

	private void Start()
	{
		if (m_pulseTime > 0f)
		{
			m_bouncingObject.ScaleTo(m_pulseAmount, m_pulseTime / 2f, 0f, EaseType.easeInOutCubic, LoopType.pingPong);
		}
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

	private IEnumerator BounceButtonAndCallMethod()
	{
		CommonAnimations.AnimateButton(m_bouncingObject);
		switch (m_ButtonType)
		{
		case ButtonType.Category:
			MenuSFX.Instance.SelectToggle.Play2D();
			break;
		case ButtonType.Deployment:
			MenuSFX.Instance.SelectDeploy.Play2D();
			break;
		case ButtonType.Back:
			MenuSFX.Instance.SelectBack.Play2D();
			break;
		case ButtonType.Default:
			MenuSFX.Instance.SoftSelect.Play2D();
			break;
		}
		float time = Time.realtimeSinceStartup + 0.25f;
		while (time > Time.realtimeSinceStartup)
		{
			yield return null;
		}
		if (m_scriptWithMethodToInvoke != null)
		{
			m_scriptWithMethodToInvoke.Invoke(m_methodToInvoke, m_button.delay);
		}
	}

	private void OnButtonPress()
	{
		GameController.ContextMenuLogic.NastyHackToAbsorbBouncyButtonPress();
		StartCoroutine("BounceButtonAndCallMethod");
	}
}
