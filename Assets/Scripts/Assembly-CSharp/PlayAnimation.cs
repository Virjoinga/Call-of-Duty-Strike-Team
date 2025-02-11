using UnityEngine;

public class PlayAnimation : MonoBehaviour
{
	public Animation Player;

	public AnimationClip Clip;

	private void Awake()
	{
		if (Player.GetClip(Clip.name) == null)
		{
			Player.AddClip(Clip, Clip.name);
		}
	}

	private void Start()
	{
		Player.Play(Clip.name);
	}
}
