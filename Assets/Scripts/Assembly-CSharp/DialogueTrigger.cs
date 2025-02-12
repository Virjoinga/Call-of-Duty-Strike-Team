using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	private enum State
	{
		Init = 0,
		Queue = 1,
		Repeat = 2,
		Destory = 3
	}

	public DialogueRequestData m_Interface = new DialogueRequestData();

	private DialogueManager diagManager;

	private float mDelayed;

	private int mQueueIndex;

	private int mActivateCallsWhilstRepeating;

	private State mState;

	private void Start()
	{
		Transform parent = base.transform;
		Transform transform = null;
		while (parent.parent != null && transform == null)
		{
			if (parent.name == "Dialogue")
			{
				transform = parent;
			}
			else
			{
				parent = parent.parent;
			}
		}
		if (transform != null)
		{
			Transform transform2 = transform.Find("Dialogue Manager");
			if (transform2 != null)
			{
				diagManager = transform2.gameObject.GetComponentInChildren<DialogueManager>();
			}
		}
		if (diagManager == null)
		{
			Debug.LogError("Unable to find dialogue manager!");
		}
		mState = State.Init;
	}

	private void Update()
	{
		switch (mState)
		{
		case State.Queue:
			mDelayed -= Time.deltaTime;
			if (mDelayed <= 0f)
			{
				PlayNextQueuedItem();
			}
			break;
		case State.Destory:
			Object.Destroy(this);
			break;
		case State.Repeat:
			break;
		}
	}

	public void Activate()
	{
		if (diagManager == null)
		{
			Start();
		}
		if (mState == State.Init)
		{
			if (m_Interface.Queue == null || m_Interface.Queue.Length == 0)
			{
				if (m_Interface.RepeatBehaviour)
				{
					diagManager.StartRepeatingDialogue(m_Interface.Text);
					mState = State.Repeat;
				}
				else
				{
					diagManager.PlayDialogue(m_Interface.Text);
					if (m_Interface.OneShot)
					{
						mState = State.Destory;
					}
				}
			}
			else
			{
				mQueueIndex = -1;
				PlayNextQueuedItem();
				mState = State.Queue;
			}
			if (m_Interface.OnQueueEmptyCallbackScript != null && m_Interface.OnQueueEmptyCallbackMethod != string.Empty)
			{
				CommonHudController.Instance.MissionDialogueQueue.SetOnQueueFinishedCallback(m_Interface.OnQueueEmptyCallbackScript, m_Interface.OnQueueEmptyCallbackMethod);
			}
			mActivateCallsWhilstRepeating = 0;
			return;
		}
		mActivateCallsWhilstRepeating++;
		if (mActivateCallsWhilstRepeating >= m_Interface.RepeatUntilActivateCallsReceived)
		{
			diagManager.FinishRepeatingDialogue(m_Interface.Text);
			if (m_Interface.OneShot)
			{
				mState = State.Destory;
			}
		}
	}

	public void Deactivate()
	{
		if (m_Interface.RepeatBehaviour)
		{
			diagManager.FinishRepeatingDialogue(m_Interface.Text);
			if (m_Interface.OneShot)
			{
				mState = State.Destory;
			}
			else
			{
				mState = State.Init;
			}
		}
		if (m_Interface.OneShot)
		{
			mState = State.Destory;
		}
	}

	public void OnDisable()
	{
		if (m_Interface.RepeatBehaviour)
		{
			if (diagManager != null)
			{
				diagManager.FinishRepeatingDialogue(m_Interface.Text);
			}
			if (m_Interface.OneShot)
			{
				mState = State.Destory;
			}
			else
			{
				mState = State.Init;
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (m_Interface.UseTrigger && IsValidMonitorObject(other.gameObject) && mState == State.Init)
		{
			Activate();
		}
	}

	private bool IsValidMonitorObject(GameObject check)
	{
		Entity component = check.GetComponent<Entity>();
		return component != null && (component.Type == "Player 1" || component.Type == "Player 2" || component.Type == "Player 3" || component.Type == "Player 4");
	}

	public void OnDrawGizmos()
	{
		BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
		if (boxCollider != null)
		{
			Gizmos.matrix = base.transform.localToWorldMatrix;
			Gizmos.color = Color.cyan.Alpha(0.25f);
			Gizmos.DrawCube(boxCollider.center, boxCollider.size);
			Gizmos.color = Color.black;
			Gizmos.DrawWireCube(boxCollider.center, boxCollider.size);
		}
	}

	private void PlayNextQueuedItem()
	{
		mQueueIndex++;
		bool flag = mQueueIndex + 1 >= m_Interface.Queue.Length;
		if (flag && m_Interface.RepeatBehaviour)
		{
			diagManager.StartRepeatingDialogue(m_Interface.Queue[mQueueIndex].Text);
			mState = State.Repeat;
		}
		else
		{
			diagManager.PlayDialogue(m_Interface.Queue[mQueueIndex].Text);
			if (flag)
			{
				mState = State.Destory;
			}
		}
		mDelayed = m_Interface.Queue[mQueueIndex].Delay;
	}
}
