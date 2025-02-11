using UnityEngine;

public class LevelHiresTextureLoader : MonoBehaviour
{
	public LevelHiresTextureData Data;

	private void Awake()
	{
		if (ShouldUseHiresTextures())
		{
			Debug.Log("Loading Hires textures");
			if (Data != null && Data.Materials != null && Data.TextureResources != null)
			{
				for (int i = 0; i < Data.Materials.Length; i++)
				{
					string text = Data.TextureResources[i];
					Texture2D texture2D = ((!string.IsNullOrEmpty(text)) ? (Resources.Load(text, typeof(Texture2D)) as Texture2D) : null);
					if (texture2D != null)
					{
						Material material = Data.Materials[i];
						Texture mainTexture = material.mainTexture;
						if (mainTexture != texture2D)
						{
							material.mainTexture = texture2D;
							Resources.UnloadAsset(mainTexture);
						}
					}
				}
			}
		}
		else
		{
			Debug.Log("Not Loading Hires textures");
		}
		Object.Destroy(base.gameObject);
	}

	private bool ShouldUseHiresTextures()
	{
		return TBFUtils.IsHighMemoryAndroidDevice();
	}
}
