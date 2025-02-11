using System.Collections;
using UnityEngine;

public class GateSlide : MonoBehaviour
{
	private enum State
	{
		Open = 0,
		Closed = 1,
		InUse = 2
	}

	[HideInInspector]
	public GateSlideData m_interface;

	public GameObject leftDoor;

	public GameObject rightDoor;

	public GameObject NavGate;

	[HideInInspector]
	public NavGate navGate;

	private State mState = State.Closed;

	private void Start()
	{
		if (NavGate != null)
		{
			navGate = NavGate.GetComponentInChildren<NavGate>();
		}
		if (navGate != null)
		{
			navGate.CloseNavGate();
		}
	}

	private void Update()
	{
	}

	public void Activate()
	{
		if (mState == State.Closed)
		{
			StartCoroutine(OpenGate());
		}
	}

	public void Deactivate()
	{
		if (mState == State.Open)
		{
			StartCoroutine(CloseGate());
		}
	}

	private IEnumerator OpenGate()
	{
		mState = State.InUse;
		float timer = m_interface.timeToOpen;
		while (timer > 0f)
		{
			leftDoor.transform.position += Time.deltaTime * (m_interface.openSpeed * leftDoor.transform.right);
			rightDoor.transform.position -= Time.deltaTime * (m_interface.openSpeed * leftDoor.transform.right);
			timer -= Time.deltaTime;
			SetPieceSFX.Instance.ArcGateSlide.Play(base.gameObject);
			yield return null;
		}
		if (navGate != null)
		{
			navGate.OpenNavGate();
		}
		mState = State.Open;
		yield return null;
	}

	private IEnumerator CloseGate()
	{
		mState = State.InUse;
		float timer = m_interface.timeToOpen;
		while (timer > 0f)
		{
			leftDoor.transform.position -= Time.deltaTime * (m_interface.openSpeed * leftDoor.transform.right);
			rightDoor.transform.position += Time.deltaTime * (m_interface.openSpeed * leftDoor.transform.right);
			timer -= Time.deltaTime;
			yield return null;
		}
		if (navGate != null)
		{
			navGate.CloseNavGate();
		}
		mState = State.Closed;
		yield return null;
	}
}
