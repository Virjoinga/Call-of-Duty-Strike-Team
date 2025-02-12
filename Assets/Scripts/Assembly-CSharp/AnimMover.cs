using UnityEngine;

public class AnimMover : MonoBehaviour
{
	public float fMoveSpeed = 0.03f;

	public float fMoveDist = 7f;

	public bool bLoop = true;

	private float counter;

	private void Start()
	{
		base.GetComponent<Animation>().wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().Stop();
		counter = fMoveDist;
	}

	private void Update()
	{
		Vector3 position = base.gameObject.transform.position;
		if (counter > 0f)
		{
			float num = fMoveSpeed * Time.deltaTime * 50f;
			counter -= Mathf.Abs(num);
			position += base.gameObject.transform.forward * num;
			base.GetComponent<Animation>().CrossFade("Walk_GunOut");
		}
		else if (bLoop)
		{
			if (Random.value * 300f < 1f)
			{
				counter = fMoveDist;
				base.gameObject.transform.Rotate(Vector3.up * 180f);
			}
			else
			{
				base.GetComponent<Animation>().CrossFade("IdleLookAround");
			}
		}
		else
		{
			base.GetComponent<Animation>().CrossFade("IdleLookAround");
		}
		base.gameObject.transform.position = position;
	}
}
