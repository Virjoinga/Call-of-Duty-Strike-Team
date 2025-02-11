using System.Collections;
using UnityEngine;

public class LoadingScreenImage : MonoBehaviour
{
	public SpriteText MissionName;

	private Material mMaterial;

	private Texture2D mTexture;

	private void Start()
	{
		mTexture = null;
		mMaterial = Resources.Load("LoadingImages/NullLoadingImage", typeof(Material)) as Material;
		if (!(mMaterial != null))
		{
			return;
		}
		mTexture = Resources.Load("LoadingImages/" + SceneLoader.SceneName, typeof(Texture2D)) as Texture2D;
		if (mTexture == null)
		{
			mTexture = Resources.Load("LoadingImages/" + SectionLoader.SceneName, typeof(Texture2D)) as Texture2D;
		}
		if (mTexture != null)
		{
			if (TBFUtils.IsRetinaHdDevice())
			{
				base.transform.localScale = new Vector3(2f, 2f, 2f);
			}
			mMaterial.mainTexture = mTexture;
			base.renderer.material = mMaterial;
			StartCoroutine(SetMissionName());
		}
		else
		{
			Object.Destroy(base.gameObject);
		}
	}

	private IEnumerator SetMissionName()
	{
		if (MissionName != null && ActStructure.Instance != null)
		{
			if (ActStructure.Instance.CurrentMissionID != MissionListings.eMissionID.MI_MAX)
			{
				MissionName.Text = AutoLocalize.Get(MissionListings.Instance.Mission(ActStructure.Instance.CurrentMissionID).Sections[ActStructure.Instance.CurrentSection].Name);
			}
			else
			{
				MissionName.Text = "MISSION NAME HERE";
			}
		}
		yield return null;
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		if (mMaterial != null)
		{
			Resources.UnloadAsset(mMaterial);
		}
		if (mTexture != null)
		{
			Resources.UnloadAsset(mTexture);
			mTexture = null;
		}
	}
}
