using UnityEngine;

public class BedrockInfoDisplay : MonoBehaviour
{
	private void Start()
	{
		SpriteText component = base.gameObject.GetComponent<SpriteText>();
		if (component != null)
		{
			component.Hide(true);
		}
	}
}
