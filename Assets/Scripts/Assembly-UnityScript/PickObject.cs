using System;
using Boo.Lang.Runtime;
using UnityEngine;

[Serializable]
public class PickObject : MonoBehaviour
{
	public TextMesh textField;

	public virtual void OnEnable()
	{
		FingerGestures.OnFingerDown += FingerGestures_OnFingerDown;
	}

	public virtual void OnDisable()
	{
		FingerGestures.OnFingerDown -= FingerGestures_OnFingerDown;
	}

	public virtual void FingerGestures_OnFingerDown(int fingerIndex, Vector2 fingerPos)
	{
		GameObject gameObject = PickObject(fingerPos);
		if ((bool)gameObject)
		{
			DisplayText("You pressed " + gameObject.name);
		}
		else
		{
			DisplayText("You didn't pressed any object");
		}
	}

	public virtual void DisplayText(object text)
	{
		if ((bool)textField)
		{
			TextMesh textMesh = textField;
			object obj = text;
			if (!(obj is string))
			{
				obj = RuntimeServices.Coerce(obj, typeof(string));
			}
			textMesh.text = (string)obj;
		}
		else
		{
			Debug.Log(text);
		}
	}

	public virtual GameObject PickObject(Vector2 screenPos)
	{
		Ray ray = Camera.main.ScreenPointToRay(screenPos);
		RaycastHit hitInfo = default(RaycastHit);
		return (!Physics.Raycast(ray, out hitInfo)) ? null : hitInfo.collider.gameObject;
	}

	public virtual void Main()
	{
	}
}
