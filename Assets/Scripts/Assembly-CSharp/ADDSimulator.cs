using UnityEngine;

public class ADDSimulator : MonoBehaviour
{
	public GameObject victim;

	public Vector3 pos1 = new Vector3(10f, 0f, 0f);

	public Vector3 pos2 = new Vector3(-10f, 0f, 0f);

	private void Start()
	{
	}

	private void Update()
	{
		if (victim != null)
		{
			RealCharacter component = victim.GetComponent<RealCharacter>();
			if (component != null)
			{
				component.forceTarget = base.gameObject;
			}
			if (((int)Time.time & 3) < 2)
			{
				base.transform.position = victim.transform.position + pos1;
			}
			else
			{
				base.transform.position = victim.transform.position + pos2;
			}
		}
	}
}
