using UnityEngine;

public class ObjectSelectionDebug : MonoBehaviour
{
	private void Update()
	{
		base.renderer.enabled = !GameController.Instance.IsFirstPerson;
	}
}
