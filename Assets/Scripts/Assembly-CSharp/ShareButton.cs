using UnityEngine;

public class ShareButton : MonoBehaviour
{
	public PackedSprite Icon;

	private void Start()
	{
		if (Icon != null)
		{
			Icon.SetFrame(0, 1);
		}
	}

	private void Update()
	{
	}
}
