using UnityEngine;

public class SectionTrigger : MonoBehaviour
{
	public enum CommandType
	{
		DoNothing = 0,
		LoadAndActivateNextSection = 1
	}

	public SectionTriggerData m_Interface;

	private bool m_LoadedTriggered;

	private bool m_ActivationTriggered;

	private void OnLoadSection()
	{
		m_ActivationTriggered = false;
		OnActivateSection();
	}

	private void OnActivateSection()
	{
		if (!m_ActivationTriggered)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.OnMissionPassed(this, 0f);
			}
			m_ActivationTriggered = true;
		}
	}

	private void LoadNextSection()
	{
		if (!m_LoadedTriggered)
		{
			SectionManager sectionManager = SectionManager.GetSectionManager();
			if (sectionManager != null)
			{
				sectionManager.LoadNextSection(-1);
			}
			if (GameController.Instance != null)
			{
				GameController.Instance.SuppressHud(true);
				CommonHudController.Instance.ClearContextMenu();
			}
			m_LoadedTriggered = true;
		}
		ActivateSection();
	}

	private void ActivateSection()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.SuppressHud(false);
		}
	}

	private void Activate()
	{
		if (m_Interface.m_commandType == CommandType.LoadAndActivateNextSection)
		{
			OnActivateSection();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Actor component = other.gameObject.GetComponent<Actor>();
		if (component != null && component.behaviour.PlayerControlled)
		{
			Activate();
		}
	}
}
