using UnityEngine;

public class CommonAnimations : MonoBehaviour
{
	public static void AnimateButton(GameObject Button)
	{
		Button.PunchScale(new Vector3(0.15f, 0.15f, 0f), 0.5f, 0f);
	}
}
