using System;
using System.Collections;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class TriggerFX : MonoBehaviour
{
	public GameObject pfx;

	public virtual void Triggerfx()
	{
		GameObject gameObject = null;
		gameObject = (GameObject)UnityEngine.Object.Instantiate(pfx, transform.position, transform.rotation);
		SetActive(gameObject.transform);
	}

	public virtual void SetActive(Transform t)
	{
		t.gameObject.SetActive(true);
		IEnumerator enumerator = UnityRuntimeServices.GetEnumerator(t);
		while (enumerator.MoveNext())
		{
			object obj = enumerator.Current;
			if (!(obj is Transform))
			{
				obj = RuntimeServices.Coerce(obj, typeof(Transform));
			}
			Transform newValue = (Transform)obj;
			SetActive(newValue);
			UnityRuntimeServices.Update(enumerator, newValue);
		}
	}

	public virtual void Update()
	{
		if (Input.GetButtonDown("Jump"))
		{
			Triggerfx();
		}
	}

	public virtual void Main()
	{
	}
}
