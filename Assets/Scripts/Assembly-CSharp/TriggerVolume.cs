using UnityEngine;

public class TriggerVolume : MonoBehaviour
{
	public TriggerVolumeData m_Interface;

	private bool bTriggered;

	private void Start()
	{
		bTriggered = false;
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((!bTriggered || !m_Interface.OneShot) && !m_Interface.Locked && IsValidMonitorObject(other.gameObject))
		{
			GameplayController.Instance().FirstScriptTriggerActor = other.gameObject;
			OnEnter();
			bTriggered = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if ((!bTriggered || !m_Interface.OneShot) && !m_Interface.Locked && IsValidMonitorObject(other.gameObject))
		{
			OnLeave();
			bTriggered = true;
		}
	}

	public void Activate()
	{
		OnEnter();
	}

	public void Deactivate()
	{
		OnLeave();
	}

	public void Disable()
	{
		base.gameObject.SetActive(false);
	}

	public void Enable()
	{
		base.gameObject.SetActive(true);
	}

	public void OnEnter()
	{
		if (m_Interface.NotifyOnEnter == null)
		{
			return;
		}
		string message = string.Empty;
		string text = string.Empty;
		int num = 0;
		foreach (GameObject item in m_Interface.NotifyOnEnter)
		{
			if (num < m_Interface.OptionalFunctionToCallOnEnter.Count)
			{
				message = m_Interface.OptionalFunctionToCallOnEnter[num];
			}
			if (num < m_Interface.OptionalStringParamToPassOnEnter.Count)
			{
				text = m_Interface.OptionalStringParamToPassOnEnter[num];
			}
			if (text != string.Empty)
			{
				Container.SendMessageWithParam(item, message, text, base.gameObject);
			}
			else
			{
				Container.SendMessage(item, message, base.gameObject);
			}
			num++;
		}
	}

	public void OnLeave()
	{
		string message = string.Empty;
		string text = string.Empty;
		int num = 0;
		foreach (GameObject item in m_Interface.NotifyOnLeave)
		{
			if (!(item == null))
			{
				if (num < m_Interface.OptionalFunctionToCallOnLeave.Count)
				{
					message = m_Interface.OptionalFunctionToCallOnLeave[num];
				}
				if (num < m_Interface.OptionalStringParamToPassOnLeave.Count)
				{
					text = m_Interface.OptionalStringParamToPassOnLeave[num];
				}
				if (text != string.Empty)
				{
					Container.SendMessageWithParam(item, message, text, base.gameObject);
				}
				else
				{
					Container.SendMessage(item, message, base.gameObject);
				}
				num++;
			}
		}
	}

	private bool IsValidMonitorObject(GameObject check)
	{
		Entity component = check.GetComponent<Entity>();
		if (m_Interface.AnyPlayer)
		{
			return component != null && (component.Type == "Player 1" || component.Type == "Player 2" || component.Type == "Player 3" || component.Type == "Player 4");
		}
		return component != null && component.Type == m_Interface.EntityType;
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.collider as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.green.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}
}
