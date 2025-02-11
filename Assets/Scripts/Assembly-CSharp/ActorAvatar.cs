using UnityEngine;

public class ActorAvatar : MonoBehaviour
{
	public GameObject ActorObj;

	public SpriteText Text;

	private Actor mActor;

	private Material mAvatarMaterial;

	public static Material CreateAvatarTexture(Texture2D tex)
	{
		Material material = new Material(Shader.Find("Corona/Unlit"));
		material.name = "Avatar-Default";
		material.mainTexture = tex;
		return material;
	}

	private void Start()
	{
		int nextClosestScoreFriendId = WaveStats.Instance.GetNextClosestScoreFriendId(false);
		if (nextClosestScoreFriendId != -1)
		{
			Texture2D friendAvatar = WaveStats.Instance.GetFriendAvatar(nextClosestScoreFriendId);
			if (friendAvatar != null)
			{
				mAvatarMaterial = CreateAvatarTexture(friendAvatar);
				base.renderer.material = mAvatarMaterial;
			}
			Text.renderer.enabled = true;
			Text.Text = WaveStats.Instance.GetFriendName(nextClosestScoreFriendId);
		}
		else
		{
			Cleanup();
		}
	}

	public void Update()
	{
		if (mActor != null)
		{
			if (!mActor.baseCharacter.IsDead())
			{
				base.transform.position = mActor.gameObject.transform.position + new Vector3(0f, 2.3f, 0f);
				if ((bool)GameController.Instance.mFirstPersonActor)
				{
					Vector3 position = GameController.Instance.mFirstPersonActor.gameObject.transform.position;
					position.y = base.transform.position.y;
					Vector3 forward = position - base.transform.position;
					base.transform.rotation = Quaternion.LookRotation(forward) * Quaternion.Euler(0f, 90f, 0f);
				}
			}
			else
			{
				Cleanup();
			}
		}
		else
		{
			mActor = ActorObj.GetComponent<Actor>();
		}
	}

	public void Cleanup()
	{
		base.gameObject.SetActive(false);
		Object.DestroyImmediate(this);
	}
}
