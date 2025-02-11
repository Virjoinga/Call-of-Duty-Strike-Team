using UnityEngine;

public class SetAnimationCulling : MonoBehaviour
{
	public Animation[] Players;

	public AnimationCullingType CullingType;

	private void Awake()
	{
		Animation[] players = Players;
		foreach (Animation animation in players)
		{
			animation.cullingType = CullingType;
			if (animation.enabled)
			{
				animation.enabled = false;
				animation.enabled = true;
			}
		}
		Object.Destroy(this);
	}
}
