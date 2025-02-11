using System.Collections;
using UnityEngine;

public class MetalGateSwing : MonoBehaviour
{
	private enum State
	{
		Open = 0,
		Closed = 1,
		InUse = 2
	}

	[HideInInspector]
	public MetalGateSwingData m_interface;

	public GameObject gate;

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
		float amountToRotate2 = m_interface.degreesToOpen;
		float rotationDirection = Mathf.Sign(amountToRotate2);
		amountToRotate2 = Mathf.Abs(amountToRotate2);
		while (amountToRotate2 > 0f)
		{
			float maximumRotationThisFrame = m_interface.degreesPerSecond * Time.deltaTime;
			float actualRotationThisFrame = Mathf.Min(maximumRotationThisFrame, amountToRotate2);
			amountToRotate2 -= actualRotationThisFrame;
			gate.transform.RotateAround(gate.transform.position, gate.transform.forward, rotationDirection * actualRotationThisFrame);
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
		float amountToRotate2 = m_interface.degreesToOpen;
		float rotationDirection = Mathf.Sign(amountToRotate2);
		amountToRotate2 = Mathf.Abs(amountToRotate2);
		while (amountToRotate2 > 0f)
		{
			float maximumRotationThisFrame = m_interface.degreesPerSecond * Time.deltaTime;
			float actualRotationThisFrame = Mathf.Min(maximumRotationThisFrame, amountToRotate2);
			amountToRotate2 -= actualRotationThisFrame;
			gate.transform.RotateAround(gate.transform.position, gate.transform.forward, (0f - rotationDirection) * actualRotationThisFrame);
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
