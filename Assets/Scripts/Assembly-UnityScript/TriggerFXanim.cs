using System;
using UnityEngine;

[Serializable]
public class TriggerFXanim : MonoBehaviour
{
	public GameObject pfx;

	public virtual void Triggerfx()
	{
		GameObject gameObject = null;
		gameObject = (GameObject)UnityEngine.Object.Instantiate(pfx, transform.position, transform.rotation);
	}

	public virtual void Main()
	{
	}
}
