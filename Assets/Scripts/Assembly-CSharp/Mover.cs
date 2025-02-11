using UnityEngine;

public class Mover : MonoBehaviour
{
	public float fMoveSpeed = 0.03f;

	public float fMoveDist = 7f;

	public bool bLoop = true;

	private float counter;

	private void Start()
	{
		counter = fMoveDist;
	}

	private void Update()
	{
		Vector3 position = base.gameObject.transform.position;
		if (counter > 0f)
		{
			counter -= Mathf.Abs(fMoveSpeed);
			position += base.gameObject.transform.up * (0f - fMoveSpeed);
		}
		else if (bLoop)
		{
			counter = fMoveDist;
			base.gameObject.transform.Rotate(Vector3.forward * 180f);
		}
		base.gameObject.transform.position = position;
	}
}
