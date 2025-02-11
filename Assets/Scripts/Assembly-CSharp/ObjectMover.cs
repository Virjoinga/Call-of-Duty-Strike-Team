using System.Collections;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
	public Vector3 EndPosition;

	public float TimeToMove = 5f;

	public bool AwaitMessage = true;

	private Vector3 startPosition;

	private bool moveActive;

	private void Start()
	{
		startPosition = base.gameObject.transform.localPosition;
		if (!AwaitMessage)
		{
			StartCoroutine(MoveObject(startPosition, EndPosition, TimeToMove));
		}
	}

	private void Update()
	{
	}

	public void OnEnter()
	{
		StartCoroutine(MoveObject(startPosition, EndPosition, TimeToMove));
	}

	public void OnLeave()
	{
		StartCoroutine(MoveObject(EndPosition, startPosition, TimeToMove));
	}

	private IEnumerator MoveObject(Vector3 startPos, Vector3 endPos, float time)
	{
		if (!moveActive)
		{
			float i = 0f;
			float rate = 1f / time;
			moveActive = true;
			while (i < 1f)
			{
				i += Time.deltaTime * rate;
				base.gameObject.transform.localPosition = Vector3.Lerp(startPos, endPos, i);
				yield return null;
			}
			moveActive = false;
		}
	}

	public bool IsMoving()
	{
		return moveActive;
	}
}
