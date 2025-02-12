using UnityEngine;

public class ObjectSelectionDebug : MonoBehaviour
{
	private void Update()
	{
		base.GetComponent<Renderer>().enabled = !GameController.Instance.IsFirstPerson;
	}
}
